<template>
  <div class="profile-page">
    <div class="profile-header">
      <div class="header-content">
        <h1 class="page-title">Личный профиль</h1>
        <p class="page-subtitle">Управление вашими данными и бронированиями</p>
      </div>
    </div>

    <div class="profile-layout">
      <div class="profile-main">
        <div class="profile-stats-bar">
          <div class="stats-left">
            <span class="stats-text">
              <strong>{{ authStore.user?.email }}</strong>
            </span>
            <span class="stats-role" :class="profileData.role">
              {{ profileData.role === 'admin' ? 'Администратор' : 'Пользователь' }}
            </span>
          </div>
          <div class="stats-right">
            <span class="stats-reservations">
              Бронирований: <strong>{{ reservations.length }}</strong>
            </span>
          </div>
        </div>

        <!-- Навигация по вкладкам -->
        <div class="profile-tabs">
          <button 
            class="profile-tab" 
            :class="{ 'active': activeTab === 'personal' }"
            @click="activeTab = 'personal'"
          >
            Личные данные
          </button>
          <button 
            class="profile-tab" 
            :class="{ 'active': activeTab === 'reservations' }"
            @click="activeTab = 'reservations'"
          >
            Мои бронирования
            <span v-if="pendingReservationsCount > 0" class="tab-badge">
              {{ pendingReservationsCount }}
            </span>
          </button>
          <button 
            class="profile-tab" 
            :class="{ 'active': activeTab === 'security' }"
            @click="activeTab = 'security'"
          >
            Безопасность
          </button>
        </div>

        <!-- Контент вкладок -->
        <div class="tab-content">
          <!-- Вкладка личных данных -->
          <div v-if="activeTab === 'personal'" class="tab-pane">
            <div class="profile-section">
              <div class="section-header">
                <h3 class="section-title">Основная информация</h3>
                <p class="section-subtitle">Для сохранения изменений требуется ввести текущий пароль</p>
              </div>

              <form @submit.prevent="handleSubmit" class="profile-form">
                <div class="form-grid">
                  <div class="form-group">
                    <label class="form-label">
                      Имя пользователя
                    </label>
                    <input 
                      v-model="formData.username" 
                      type="text"
                      class="form-input"
                      placeholder="Ваш никнейм"
                    />
                    <div class="form-hint">
                      Отображается в публичном профиле
                    </div>
                  </div>

                  <div class="form-group">
                    <label class="form-label">
                      Email адрес
                      <span class="required-mark">*</span>
                    </label>
                    <input 
                      v-model="formData.email" 
                      type="email" 
                      class="form-input"
                      :class="{ 'error': errors.email }"
                      placeholder="example@email.com"
                      required
                    />
                    <div v-if="errors.email" class="error-message">
                      {{ errors.email }}
                    </div>
                  </div>
                </div>

                <div class="form-grid">
                  <div class="form-group">
                    <label class="form-label">Имя</label>
                    <input 
                      v-model="formData.firstName" 
                      type="text"
                      class="form-input"
                      placeholder="Ваше имя"
                    />
                  </div>

                  <div class="form-group">
                    <label class="form-label">Фамилия</label>
                    <input 
                      v-model="formData.lastName" 
                      type="text"
                      class="form-input"
                      placeholder="Ваша фамилия"
                    />
                  </div>
                </div>

                <div class="form-group">
                  <label class="form-label">Телефон</label>
                  <input 
                    v-model="formData.phone" 
                    type="tel"
                    class="form-input"
                    placeholder="+7 (XXX) XXX-XX-XX"
                  />
                </div>

                <!-- Добавлено поле для текущего пароля -->
                <div class="form-group">
                  <label class="form-label">
                    Текущий пароль (обязательно для сохранения)
                    <span class="required-mark">*</span>
                  </label>
                  <div class="password-input-wrapper">
                    <input 
                      v-model="formData.currentPassword" 
                      :type="showCurrentPasswordInPersonal ? 'text' : 'password'" 
                      class="form-input"
                      :class="{ 'error': errors.currentPassword }"
                      placeholder="Введите ваш текущий пароль"
                      required
                    />
                    <button 
                      type="button"
                      class="password-toggle"
                      @click="showCurrentPasswordInPersonal = !showCurrentPasswordInPersonal"
                    >
                      <svg v-if="!showCurrentPasswordInPersonal" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M12 5C5 5 1 12 1 12C1 12 5 19 12 19C19 19 23 12 23 12C23 12 19 5 12 5Z" stroke-linecap="round" stroke-linejoin="round"/>
                        <circle cx="12" cy="12" r="4" stroke-linecap="round" stroke-linejoin="round"/>
                      </svg>
                      <svg v-else width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M3 3L21 21M10 10.5V13.5M14 10.5V13.5M10.5 10.5L13.5 13.5M10.5 13.5L13.5 10.5M6 10.5V13.5M18 10.5V13.5M1 12C1 12 5 5 12 5C15 5 17 6 19 7M23 12C23 12 20 17 15 19C14 19.3 13 19.5 12 19.5" stroke-linecap="round" stroke-linejoin="round"/>
                      </svg>
                    </button>
                  </div>
                  <div v-if="errors.currentPassword" class="error-message">
                    {{ errors.currentPassword }}
                  </div>
                  <div class="form-hint">
                    Требуется для подтверждения вашей личности при сохранении изменений
                  </div>
                </div>

                <div class="form-actions">
                  <button 
                    type="button" 
                    @click="resetForm"
                    class="btn-secondary"
                    :disabled="loading"
                  >
                    Сбросить
                  </button>
                  
                  <button 
                    type="submit" 
                    :disabled="loading"
                    class="btn-primary"
                  >
                    <span v-if="loading" class="loading-spinner"></span>
                    Сохранить изменения
                  </button>
                </div>
              </form>
            </div>
          </div>

          <!-- Вкладка бронирований -->
          <div v-if="activeTab === 'reservations'" class="tab-pane">
            <div class="reservations-section">
              <div class="section-header">
                <h3 class="section-title">Мои бронирования</h3>
                <p class="section-subtitle">История и активные брони</p>
              </div>

              <div v-if="loadingReservations" class="loading-state">
                <div class="loader"></div>
                <p>Загружаем ваши бронирования...</p>
              </div>

              <div v-else-if="reservations.length === 0" class="empty-state">
                <div class="empty-icon">
                  <svg width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
                    <path d="M16 3H1V20H23V8M16 3L23 8M16 3V8H23M5 14H10M5 17H19" stroke-linecap="round" stroke-linejoin="round"/>
                  </svg>
                </div>
                <h3>Бронирований пока нет</h3>
                <p>Начните с просмотра автомобилей в каталоге</p>
                <button 
                  class="btn-primary"
                  @click="router.push('/')"
                >
                  Перейти в каталог
                </button>
              </div>

              <div v-else class="reservations-list">
                <!-- Фильтры бронирований -->
                <div class="reservation-filters">
                  <div class="filter-group">
                    <label class="filter-label">Сортировка:</label>
                    <select v-model="reservationSort" class="filter-select">
                      <option value="newest">Сначала новые</option>
                      <option value="oldest">Сначала старые</option>
                    </select>
                  </div>
                </div>

                <!-- Список бронирований -->
                <div class="reservation-items">
                  <div 
                    v-for="reservation in filteredReservations" 
                    :key="reservation.id"
                    class="reservation-card"
                    :class="`status-${reservation.status}`"
                  >
                    <div class="reservation-header">
                      <div class="reservation-id">
                        Бронирование #{{ reservation.id }}
                      </div>
                      <div class="reservation-status" :class="reservation.status">
                        {{ getStatusText(reservation.status) }}
                      </div>
                    </div>

                    <div class="reservation-content">
                      <div class="reservation-car">
                        <div class="car-image" v-if="reservation.car?.photoUrl">
                          <img 
                            :src="reservation.car.photoUrl" 
                            :alt="`${reservation.car.brandName} ${reservation.car.modelName}`"
                            @error="handleCarImageError"
                          />
                        </div>
                        <div class="car-info">
                          <h4 class="car-title">
                            {{ reservation.car?.brandName }} {{ reservation.car?.modelName }}
                          </h4>
                          <div class="car-details">
                            <span class="car-year">{{ reservation.car?.year }} год</span>
                            <span class="car-color">{{ reservation.car?.color }}</span>
                            <span class="car-price">{{ formatPrice(reservation.car?.price) }} ₽</span>
                          </div>
                        </div>
                      </div>

                      <div class="reservation-details">
                        <div class="detail-row">
                          <div class="detail-item">
                            <span class="detail-label">Даты аренды:</span>
                            <span class="detail-value">
                              {{ formatDate(reservation.startDate) }} – {{ formatDate(reservation.endDate) }}
                            </span>
                          </div>
                          <div class="detail-item">
                            <span class="detail-label">Дней:</span>
                            <span class="detail-value">
                              {{ getDaysCount(reservation.startDate, reservation.endDate) }}
                            </span>
                          </div>
                        </div>
                        <div class="detail-row">
                          <div class="detail-item">
                            <span class="detail-label">Общая стоимость:</span>
                            <span class="detail-value total-price">
                              {{ formatPrice(reservation.totalPrice) }} ₽
                            </span>
                          </div>
                          <div class="detail-item">
                            <span class="detail-label">Дата создания:</span>
                            <span class="detail-value">
                              {{ formatDateTime(reservation.createdAt) }}
                            </span>
                          </div>
                        </div>
                        <div v-if="reservation.comment" class="reservation-comment">
                          <span class="comment-label">Комментарий:</span>
                          <p class="comment-text">{{ reservation.comment }}</p>
                        </div>
                      </div>
                    </div>

                    <div class="reservation-actions">
                      <button 
                        v-if="canCancelReservation(reservation)"
                        @click="cancelReservation(reservation.id)"
                        class="btn-danger"
                        :disabled="cancellingReservationId === reservation.id"
                      >
                        <span v-if="cancellingReservationId === reservation.id" class="loading-spinner-small"></span>
                        Отменить бронирование
                      </button>
                      <button 
                        v-if="reservation.carId"
                        @click="viewCarDetails(reservation.carId)"
                        class="btn-secondary"
                      >
                        Посмотреть автомобиль
                      </button>
                    </div>
                  </div>
                </div>

                <!-- Статистика -->
                <div class="reservations-stats">
                  <div class="stat-card">
                    <div class="stat-value">{{ formatPrice(totalSpent) }}</div>
                    <div class="stat-label">Всего потрачено</div>
                  </div>
                  <div class="stat-card">
                    <div class="stat-value">{{ activeReservationsCount }}</div>
                    <div class="stat-label">Активных броней</div>
                  </div>
                  <div class="stat-card">
                    <div class="stat-value">{{ completedReservationsCount }}</div>
                    <div class="stat-label">Завершено</div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Вкладка безопасности -->
          <div v-if="activeTab === 'security'" class="tab-pane">
            <div class="security-section">
              <div class="section-header">
                <h3 class="section-title">Безопасность</h3>
                <p class="section-subtitle">Изменение пароля</p>
              </div>

              <form @submit.prevent="handlePasswordChange" class="security-form">
                <div class="form-group">
                  <label class="form-label">
                    Текущий пароль
                    <span class="required-mark">*</span>
                  </label>
                  <div class="password-input-wrapper">
                    <input 
                      v-model="passwordData.currentPassword" 
                      :type="showCurrentPassword ? 'text' : 'password'" 
                      class="form-input"
                      :class="{ 'error': passwordErrors.currentPassword }"
                      placeholder="Введите текущий пароль"
                      required
                    />
                    <button 
                      type="button"
                      class="password-toggle"
                      @click="showCurrentPassword = !showCurrentPassword"
                    >
                      <svg v-if="!showCurrentPassword" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M12 5C5 5 1 12 1 12C1 12 5 19 12 19C19 19 23 12 23 12C23 12 19 5 12 5Z" stroke-linecap="round" stroke-linejoin="round"/>
                        <circle cx="12" cy="12" r="4" stroke-linecap="round" stroke-linejoin="round"/>
                      </svg>
                      <svg v-else width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M3 3L21 21M10 10.5V13.5M14 10.5V13.5M10.5 10.5L13.5 13.5M10.5 13.5L13.5 10.5M6 10.5V13.5M18 10.5V13.5M1 12C1 12 5 5 12 5C15 5 17 6 19 7M23 12C23 12 20 17 15 19C14 19.3 13 19.5 12 19.5" stroke-linecap="round" stroke-linejoin="round"/>
                      </svg>
                    </button>
                  </div>
                  <div v-if="passwordErrors.currentPassword" class="error-message">
                    {{ passwordErrors.currentPassword }}
                  </div>
                </div>

                <div class="form-grid">
                  <div class="form-group">
                    <label class="form-label">
                      Новый пароль
                      <span class="required-mark">*</span>
                    </label>
                    <div class="password-input-wrapper">
                      <input 
                        v-model="passwordData.newPassword" 
                        :type="showNewPassword ? 'text' : 'password'" 
                        class="form-input"
                        :class="{ 'error': passwordErrors.newPassword }"
                        placeholder="Минимум 6 символов"
                        required
                      />
                      <button 
                        type="button"
                        class="password-toggle"
                        @click="showNewPassword = !showNewPassword"
                      >
                        <svg v-if="!showNewPassword" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                          <path d="M12 5C5 5 1 12 1 12C1 12 5 19 12 19C19 19 23 12 23 12C23 12 19 5 12 5Z" stroke-linecap="round" stroke-linejoin="round"/>
                          <circle cx="12" cy="12" r="4" stroke-linecap="round" stroke-linejoin="round"/>
                        </svg>
                        <svg v-else width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                          <path d="M3 3L21 21M10 10.5V13.5M14 10.5V13.5M10.5 10.5L13.5 13.5M10.5 13.5L13.5 10.5M6 10.5V13.5M18 10.5V13.5M1 12C1 12 5 5 12 5C15 5 17 6 19 7M23 12C23 12 20 17 15 19C14 19.3 13 19.5 12 19.5" stroke-linecap="round" stroke-linejoin="round"/>
                        </svg>
                      </button>
                    </div>
                    <div v-if="passwordErrors.newPassword" class="error-message">
                      {{ passwordErrors.newPassword }}
                    </div>
                  </div>

                  <div class="form-group">
                    <label class="form-label">
                      Подтверждение пароля
                      <span class="required-mark">*</span>
                    </label>
                    <div class="password-input-wrapper">
                      <input 
                        v-model="passwordData.confirmPassword" 
                        :type="showConfirmPassword ? 'text' : 'password'" 
                        class="form-input"
                        :class="{ 'error': passwordErrors.confirmPassword }"
                        placeholder="Повторите новый пароль"
                        required
                      />
                      <button 
                        type="button"
                        class="password-toggle"
                        @click="showConfirmPassword = !showConfirmPassword"
                      >
                        <svg v-if="!showConfirmPassword" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                          <path d="M12 5C5 5 1 12 1 12C1 12 5 19 12 19C19 19 23 12 23 12C23 12 19 5 12 5Z" stroke-linecap="round" stroke-linejoin="round"/>
                          <circle cx="12" cy="12" r="4" stroke-linecap="round" stroke-linejoin="round"/>
                        </svg>
                        <svg v-else width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                          <path d="M3 3L21 21M10 10.5V13.5M14 10.5V13.5M10.5 10.5L13.5 13.5M10.5 13.5L13.5 10.5M6 10.5V13.5M18 10.5V13.5M1 12C1 12 5 5 12 5C15 5 17 6 19 7M23 12C23 12 20 17 15 19C14 19.3 13 19.5 12 19.5" stroke-linecap="round" stroke-linejoin="round"/>
                        </svg>
                      </button>
                    </div>
                    <div v-if="passwordErrors.confirmPassword" class="error-message">
                      {{ passwordErrors.confirmPassword }}
                    </div>
                  </div>
                </div>

                <div class="password-strength" v-if="passwordData.newPassword">
                  <div class="strength-label">Надежность пароля:</div>
                  <div class="strength-bar" :class="passwordStrengthClass">
                    <div class="strength-fill" :style="{ width: passwordStrength + '%' }"></div>
                  </div>
                  <div class="strength-text">{{ passwordStrengthText }}</div>
                </div>

                <div class="form-actions">
                  <button 
                    type="submit" 
                    :disabled="changingPassword"
                    class="btn-primary"
                  >
                    <span v-if="changingPassword" class="loading-spinner"></span>
                    Изменить пароль
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>

      <div class="profile-sidebar">
        <!-- Краткая информация -->
        <div class="sidebar-section">
          <div class="sidebar-header">
            <h4 class="sidebar-title">Ваш профиль</h4>
          </div>
          <div class="user-summary">
            <div class="user-summary-info">
              <h4>{{ displayName }}</h4>
              <p class="user-email">{{ displayEmail }}</p>
              <p class="user-join-date" v-if="profileData.createdAt">
                Участник с {{ formatDate(profileData.createdAt) }}
              </p>
            </div>
          </div>
        </div>

        <!-- Быстрые действия -->
        <div class="sidebar-section">
          <div class="sidebar-header">
            <h4 class="sidebar-title">Быстрые действия</h4>
          </div>
          <div class="quick-actions">
            <button class="quick-action-btn" @click="router.push('/')">
              Каталог авто
            </button>
            <button 
              class="quick-action-btn" 
              @click="router.push('/admin')" 
              v-if="authStore.isAdmin"
            >
              Админ-панель
            </button>
            <button 
              class="quick-action-btn" 
              @click="activeTab = 'reservations'"
              v-if="pendingReservationsCount > 0"
            >
              Мои брони ({{ pendingReservationsCount }})
            </button>
            <button class="quick-action-btn text-danger" @click="handleLogout">
              ↪ Выйти
            </button>
          </div>
        </div>

        <!-- Информация об аккаунте -->
        <div class="sidebar-section">
          <div class="sidebar-header">
            <h4 class="sidebar-title">Информация об аккаунте</h4>
          </div>
          <div class="account-info">
            <div class="info-item">
              <span class="info-label">ID пользователя:</span>
              <span class="info-value">{{ profileData.id || 'Н/Д' }}</span>
            </div>
            <div class="info-item">
              <span class="info-label">Роль:</span>
              <span class="info-value badge" :class="profileData.role">
                {{ profileData.role === 'admin' ? 'Администратор' : 'Пользователь' }}
              </span>
            </div>
            <div class="info-item" v-if="profileData.createdAt">
              <span class="info-label">Дата регистрации:</span>
              <span class="info-value">{{ formatDate(profileData.createdAt) }}</span>
            </div>
            <div class="info-item" v-if="profileData.updatedAt">
              <span class="info-label">Последнее обновление:</span>
              <span class="info-value">{{ formatDateTime(profileData.updatedAt) }}</span>
            </div>
            <div class="info-item" v-else>
              <span class="info-label">Последнее обновление:</span>
              <span class="info-value">Никогда</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { authApi } from '@/api/auth'
import { reservationsApi } from '@/api/reservations' 


const router = useRouter()
const authStore = useAuthStore()

console.log('=== ProfilePage Debug Info ===')
console.log('authStore:', authStore)
console.log('authStore.user:', authStore.user)
console.log('authStore.isAdmin:', authStore.isAdmin)
console.log('=======================')

// Состояние вкладок
const activeTab = ref('personal')

// Данные профиля
const profileData = ref({
  id: null,
  email: '',
  role: '',
  createdAt: null,
  updatedAt: null
})

// Форма личных данных
const formData = reactive({
  email: '',
  username: '',
  firstName: '',
  lastName: '',
  phone: '',
  currentPassword: ''
})

const originalFormData = ref({})
const errors = reactive({})
const loading = ref(false)
const showCurrentPasswordInPersonal = ref(false)

// Бронирования
const reservations = ref([])
const loadingReservations = ref(false)
const reservationSort = ref('newest')
const cancellingReservationId = ref(null)

// Фильтрация бронирований
const filteredReservations = computed(() => {
  let filtered = [...reservations.value]

  // Сортировка
  switch (reservationSort.value) {
    case 'newest':
      filtered.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt))
      break
    case 'oldest':
      filtered.sort((a, b) => new Date(a.createdAt) - new Date(b.createdAt))
      break
  }

  return filtered
})

// Статистика бронирований
const pendingReservationsCount = computed(() => {
  return reservations.value.filter(r => r.status === 'pending').length
})

const activeReservationsCount = computed(() => {
  return reservations.value.filter(r => 
    r.status === 'pending' || r.status === 'confirmed'
  ).length
})

const completedReservationsCount = computed(() => {
  return reservations.value.filter(r => r.status === 'completed').length
})

const displayName = computed(() => {
  const user = authStore.user || {}
  const firstName = user.firstName || formData.firstName || ''
  const lastName = user.lastName || formData.lastName || ''
  const username = user.username || formData.username || ''
  
  if (firstName || lastName) {
    return `${firstName} ${lastName}`.trim()
  } else if (username) {
    return username
  }
  return 'Пользователь'
})

const displayEmail = computed(() => {
  return authStore.user?.email || formData.email || ''
})

const totalSpent = computed(() => {
  const completedReservations = reservations.value.filter(r => 
    r.status === 'completed' || r.status === 'confirmed'
  )
  return completedReservations.reduce((sum, r) => sum + r.totalPrice, 0)
})

// Смена пароля
const showCurrentPassword = ref(false)
const showNewPassword = ref(false)
const showConfirmPassword = ref(false)
const changingPassword = ref(false)

const passwordData = reactive({
  currentPassword: '',
  newPassword: '',
  confirmPassword: ''
})

const passwordErrors = reactive({})

const passwordStrength = computed(() => {
  if (!passwordData.newPassword) return 0
  
  let strength = 0
  const password = passwordData.newPassword
  
  // Длина пароля
  if (password.length >= 8) strength += 25
  if (password.length >= 12) strength += 15
  
  // Наличие разных символов
  if (/[A-Z]/.test(password)) strength += 20
  if (/[0-9]/.test(password)) strength += 20
  if (/[^A-Za-z0-9]/.test(password)) strength += 20
  
  return Math.min(strength, 100)
})

const passwordStrengthClass = computed(() => {
  if (passwordStrength.value < 40) return 'weak'
  if (passwordStrength.value < 70) return 'medium'
  return 'strong'
})

const passwordStrengthText = computed(() => {
  if (passwordStrength.value < 40) return 'Слабый'
  if (passwordStrength.value < 70) return 'Средний'
  return 'Сильный'
})

const loadProfile = async () => {
  try {
    const response = await authApi.getProfile()
    const data = response.data
    console.log('Данные профиля с сервера:', data)
    
    // Обновляем profileData с учетом null значений
    profileData.value = {
      id: data.id,
      email: data.email || '',
      role: data.role || 'user',
      createdAt: data.createdAt || null,
      updatedAt: data.updatedAt || null
    }
    
    // ВАЖНО: Используем данные из authStore.user как приоритетные
    // потому что они уже могут содержать обновленные значения
    const storedUser = authStore.user || {}
    
    // Заполняем форму: сначала берем из хранилища, если есть, иначе с сервера
    formData.email = storedUser.email || data.email || ''
    formData.username = storedUser.username !== undefined ? storedUser.username : (data.username || '')
    formData.firstName = storedUser.firstName !== undefined ? storedUser.firstName : (data.firstName || '')
    formData.lastName = storedUser.lastName !== undefined ? storedUser.lastName : (data.lastName || '')
    formData.phone = storedUser.phone !== undefined ? storedUser.phone : (data.phone || '')
    // currentPassword не заполняем из профиля
    
    // Если в хранилище нет данных (например, при первой загрузке), 
    // обновляем хранилище данными с сервера
    if (!storedUser.username && !storedUser.firstName && !storedUser.lastName && !storedUser.phone) {
      const userUpdate = {
        email: data.email || '',
        username: data.username || '',
        firstName: data.firstName || '',
        lastName: data.lastName || '',
        phone: data.phone || '',
        role: data.role || 'user'
      }
      
      // Обновляем хранилище
      if (typeof authStore.updateUser === 'function') {
        authStore.updateUser(userUpdate)
      } else {
        authStore.user = {
          ...(authStore.user || {}),
          ...userUpdate
        }
        localStorage.setItem('user', JSON.stringify(authStore.user))
      }
    }
    
    // Сохраняем оригинальные данные
    originalFormData.value = { 
      email: formData.email,
      username: formData.username,
      firstName: formData.firstName,
      lastName: formData.lastName,
      phone: formData.phone
    }
    
    console.log('Форма после загрузки:', formData)
    console.log('authStore.user после загрузки:', authStore.user)
    
  } catch (error) {
    console.error('Ошибка загрузки профиля:', error)
  }
}

const loadReservations = async () => {
  loadingReservations.value = true
  try {
    const response = await reservationsApi.getUserReservations()
    reservations.value = response.data || []
  } catch (error) {
    console.error('Ошибка загрузки бронирований:', error)
  } finally {
    loadingReservations.value = false
  }
}

const handleSubmit = async () => {
  if (!validateForm()) return
  
  loading.value = true
  
  try {
    const updateData = {
      email: formData.email,
      username: formData.username || '',
      firstName: formData.firstName || '',
      lastName: formData.lastName || '',
      phone: formData.phone || '',
      currentPassword: formData.currentPassword
    }
    
    console.log('Отправка данных профиля на сервер:', updateData)
    
    await authApi.updateProfile(updateData)
    
    // Обновляем данные в хранилище
    const updatedUserData = {
      ...authStore.user,
      ...updateData
    }
    
    // Удаляем currentPassword из объекта пользователя
    delete updatedUserData.currentPassword
    
    // Обновляем хранилище
    if (typeof authStore.updateUser === 'function') {
      authStore.updateUser(updatedUserData)
    } else {
      authStore.user = updatedUserData
      localStorage.setItem('user', JSON.stringify(updatedUserData))
    }
    
    // Обновляем formData и originalFormData
    const { currentPassword, ...formDataWithoutPassword } = formData
    originalFormData.value = { ...formDataWithoutPassword }
    
    // Очищаем поле пароля
    formData.currentPassword = ''
    
    // НЕ вызываем loadProfile() - данные уже обновлены локально
    console.log('Данные обновлены локально. authStore.user:', authStore.user)
    
    alert('Профиль успешно обновлен!')
    
  } catch (error) {
    console.error('Ошибка обновления профиля:', error)
    // ... обработка ошибок
  } finally {
    loading.value = false
  }
}

const handlePasswordChange = async () => {
  if (!validatePasswordForm()) return
  
  changingPassword.value = true
  
  try {
    const passwordUpdateData = {
      currentPassword: passwordData.currentPassword,
      newPassword: passwordData.newPassword,
      confirmNewPassword: passwordData.confirmPassword
    }
    
    console.log('Отправка данных для смены пароля:', passwordUpdateData)
    
    const response = await authApi.updateProfile(passwordUpdateData)
    
    // Очищаем форму
    passwordData.currentPassword = ''
    passwordData.newPassword = ''
    passwordData.confirmPassword = ''
    
    alert('Пароль успешно изменен!')
    
  } catch (error) {
    console.error('Ошибка смены пароля:', error)
    if (error.response?.data?.message) {
      alert(error.response.data.message)
    } else {
      alert('Ошибка при смене пароля. Проверьте текущий пароль и попробуйте снова.')
    }
  } finally {
    changingPassword.value = false
  }
}

const cancelReservation = async (reservationId) => {
  const reservation = reservations.value.find(r => r.id === reservationId)
  if (!reservation) return
  
  // Проверяем возможность отмены
  if (!canCancelReservation(reservation)) {
    alert('Отмена возможна не позднее чем за 24 часа до начала бронирования')
    return
  }
  
  if (!confirm(`Вы уверены, что хотите отменить бронирование #${reservationId}?`)) {
    return
  }
  
  cancellingReservationId.value = reservationId
  
  try {
    await reservationsApi.cancelReservation(reservationId)
    
    // Обновляем статус в локальном списке
    const index = reservations.value.findIndex(r => r.id === reservationId)
    if (index !== -1) {
      reservations.value[index].status = 'cancelled'
    }
    
    alert('Бронирование успешно отменено!')
  } catch (error) {
    console.error('Ошибка отмены бронирования:', error)
    if (error.response?.data?.message) {
      alert(error.response.data.message)
    } else {
      alert('Ошибка отмены бронирования')
    }
  } finally {
    cancellingReservationId.value = null
  }
}

// Валидация
const validateForm = () => {
  let isValid = true
  
  Object.keys(errors).forEach(key => errors[key] = '')
  
  // Email
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  if (!formData.email) {
    errors.email = 'Email обязателен'
    isValid = false
  } else if (!emailRegex.test(formData.email)) {
    errors.email = 'Введите корректный email адрес'
    isValid = false
  }
  
  // Текущий пароль (обязателен для сохранения изменений)
  if (!formData.currentPassword) {
    errors.currentPassword = 'Текущий пароль обязателен для сохранения изменений'
    isValid = false
  } else if (formData.currentPassword.length < 6) {
    errors.currentPassword = 'Пароль должен содержать минимум 6 символов'
    isValid = false
  }
  
  return isValid
}

const validatePasswordForm = () => {
  let isValid = true
  
  Object.keys(passwordErrors).forEach(key => passwordErrors[key] = '')
  
  // Текущий пароль
  if (!passwordData.currentPassword) {
    passwordErrors.currentPassword = 'Введите текущий пароль'
    isValid = false
  }
  
  // Новый пароль
  if (!passwordData.newPassword) {
    passwordErrors.newPassword = 'Введите новый пароль'
    isValid = false
  } else if (passwordData.newPassword.length < 6) {
    passwordErrors.newPassword = 'Пароль должен содержать минимум 6 символов'
    isValid = false
  }
  
  // Подтверждение пароля
  if (!passwordData.confirmPassword) {
    passwordErrors.confirmPassword = 'Подтвердите новый пароль'
    isValid = false
  } else if (passwordData.newPassword !== passwordData.confirmPassword) {
    passwordErrors.confirmPassword = 'Пароли не совпадают'
    isValid = false
  }
  
  return isValid
}

// Утилиты
const formatDate = (dateString) => {
  if (!dateString) return ''
  const date = new Date(dateString)
  return date.toLocaleDateString('ru-RU', {
    day: 'numeric',
    month: 'long',
    year: 'numeric'
  })
}

const formatDateTime = (dateString) => {
  if (!dateString) return ''
  const date = new Date(dateString)
  return date.toLocaleDateString('ru-RU', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const formatPrice = (price) => {
  return new Intl.NumberFormat('ru-RU').format(price || 0)
}

const getDaysCount = (startDate, endDate) => {
  const start = new Date(startDate)
  const end = new Date(endDate)
  const diffTime = Math.abs(end - start)
  return Math.ceil(diffTime / (1000 * 60 * 60 * 24))
}

const getStatusText = (status) => {
  const statusMap = {
    pending: 'Ожидание',
    confirmed: 'Подтверждено',
    cancelled: 'Отменено',
    completed: 'Завершено'
  }
  return statusMap[status] || status
}

const canCancelReservation = (reservation) => {
  if (reservation.status !== 'pending' && reservation.status !== 'confirmed') {
    return false
  }
  
  // Проверяем, что до начала бронирования осталось больше 24 часов
  const startDate = new Date(reservation.startDate)
  const now = new Date()
  const hoursUntilStart = (startDate - now) / (1000 * 60 * 60)
  
  return hoursUntilStart > 24
}

const resetForm = () => {
  // Восстанавливаем оригинальные значения (кроме пароля)
  Object.keys(originalFormData.value).forEach(key => {
    if (formData.hasOwnProperty(key)) {
      formData[key] = originalFormData.value[key] || ''
    }
  })
  formData.currentPassword = ''
}

const handleCarImageError = (event) => {
  event.target.src = 'https://via.placeholder.com/300x200?text=Car'
}

const viewCarDetails = (carId) => {
  if (carId) {
    router.push(`/car/${carId}`)
  }
}

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
}

// Инициализация
// Инициализация
onMounted(() => {
  // Сначала проверяем, есть ли данные в authStore.user
  console.log('Инициализация профиля. authStore.user:', authStore.user)
  
  // Если в authStore.user уже есть данные (из localStorage), используем их
  if (authStore.user && (authStore.user.username || authStore.user.firstName || authStore.user.lastName || authStore.user.phone)) {
    console.log('Использую данные из authStore.user для формы')
    
    // Заполняем форму из authStore.user
    formData.email = authStore.user.email || ''
    formData.username = authStore.user.username || ''
    formData.firstName = authStore.user.firstName || ''
    formData.lastName = authStore.user.lastName || ''
    formData.phone = authStore.user.phone || ''
    
    originalFormData.value = { 
      email: formData.email,
      username: formData.username,
      firstName: formData.firstName,
      lastName: formData.lastName,
      phone: formData.phone
    }
    
    // profileData тоже обновляем из authStore
    profileData.value = {
      id: authStore.user.id,
      email: authStore.user.email,
      role: authStore.user.role,
      createdAt: authStore.user.createdAt,
      updatedAt: authStore.user.updatedAt
    }
  }
  
  // Затем загружаем свежие данные с сервера
  loadProfile()
  loadReservations()
})
</script>

<style scoped>
/* Стили остаются такими же, как в предыдущем коде */
.profile-page {
  min-height: 100vh;
  margin-top: -23px;
}

.profile-header {
  background: linear-gradient(135deg, var(--tts-primary) 0%, var(--tts-primary-dark) 100%);
  color: white;
  padding: 2rem 1.5rem;
  position: relative;
  border-radius: 0 0 var(--tts-radius-lg) var(--tts-radius-lg);
  margin-bottom: 2rem;
}

.header-content {
  max-width: 1440px;
  margin: 0 auto;
}

.page-title {
  font-size: 2rem;
  font-weight: 700;
  color: white;
  margin-bottom: 0.5rem;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
}

.page-subtitle {
  color: rgba(255, 255, 255, 0.95);
  font-size: 1rem;
}

.profile-layout {
  max-width: 1440px;
  margin: 0 auto;
  padding: 0 1.5rem 2rem;
  display: grid;
  grid-template-columns: 1fr 350px;
  gap: 2rem;
}

@media (max-width: 1024px) {
  .profile-layout {
    grid-template-columns: 1fr;
  }
}

.profile-stats-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  padding: 1rem 1.5rem;
  background: var(--tts-white);
  border-radius: var(--tts-radius-lg);
  box-shadow: var(--tts-shadow);
  border: 1px solid var(--tts-gray-200);
}

.stats-left, .stats-right {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.stats-text {
  color: var(--tts-gray-700);
  font-size: 0.875rem;
}

.stats-text strong {
  color: var(--tts-gray-900);
  font-weight: 600;
}

.stats-reservations {
  color: var(--tts-gray-600);
  font-size: 0.875rem;
}

.stats-reservations strong {
  color: var(--tts-primary);
}

.stats-role {
  padding: 0.25rem 0.75rem;
  border-radius: var(--tts-radius);
  font-size: 0.75rem;
  font-weight: 500;
}

.stats-role.admin {
  background: rgba(26, 86, 219, 0.1);
  color: var(--tts-primary);
}

.stats-role.user {
  background: rgba(107, 114, 128, 0.1);
  color: var(--tts-gray-700);
}

.profile-tabs {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1.5rem;
  padding: 0.5rem;
  background: var(--tts-white);
  border-radius: var(--tts-radius-lg);
  box-shadow: var(--tts-shadow);
  border: 1px solid var(--tts-gray-200);
}

.profile-tab {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  padding: 0.75rem 1rem;
  background: none;
  border: none;
  border-radius: var(--tts-radius);
  font-size: 0.875rem;
  font-weight: 500;
  color: var(--tts-gray-700);
  cursor: pointer;
  transition: all 0.2s;
  position: relative;
}

.profile-tab:hover {
  background: var(--tts-gray-100);
}

.profile-tab.active {
  background: var(--tts-primary);
  color: white;
}

.tab-badge {
  position: absolute;
  top: -4px;
  right: -4px;
  background: var(--tts-error);
  color: white;
  font-size: 0.75rem;
  padding: 0.125rem 0.375rem;
  border-radius: 10px;
  min-width: 16px;
  text-align: center;
}

.tab-content {
  margin-top: 1rem;
}

.tab-pane {
  animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

.profile-section,
.reservations-section,
.security-section {
  background: var(--tts-white);
  border-radius: var(--tts-radius-lg);
  padding: 2rem;
  box-shadow: var(--tts-shadow-md);
  border: 1px solid var(--tts-gray-200);
}

.section-header {
  margin-bottom: 2rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid var(--tts-gray-200);
}

.section-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--tts-gray-800);
  margin-bottom: 0.5rem;
}

.section-subtitle {
  color: var(--tts-gray-600);
  font-size: 0.875rem;
}

.profile-form,
.security-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.5rem;
}

@media (max-width: 768px) {
  .form-grid {
    grid-template-columns: 1fr;
  }
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-label {
  font-size: 0.875rem;
  font-weight: 500;
  color: var(--tts-gray-700);
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.required-mark {
  color: var(--tts-error);
}

.form-input {
  padding: 0.75rem;
  border: 1px solid var(--tts-gray-300);
  border-radius: var(--tts-radius);
  font-size: 0.875rem;
  color: var(--tts-gray-800);
  background: var(--tts-white);
  transition: all 0.2s;
  width: 100%;
}

.form-input:focus {
  outline: none;
  border-color: var(--tts-primary);
  box-shadow: 0 0 0 3px rgba(26, 86, 219, 0.1);
}

.form-input.error {
  border-color: var(--tts-error);
}

.form-hint {
  font-size: 0.75rem;
  color: var(--tts-gray-500);
  margin-top: 0.25rem;
}

.error-message {
  font-size: 0.75rem;
  color: var(--tts-error);
  margin-top: 0.25rem;
}

.password-input-wrapper {
  position: relative;
  width: 100%;
}

.password-toggle {
  position: absolute;
  right: 12px;
  top: 50%;
  transform: translateY(-50%);
  background: none;
  border: none;
  cursor: pointer;
  color: var(--tts-gray-500);
  padding: 4px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.password-toggle:hover {
  color: var(--tts-primary);
}

.form-input {
  padding-right: 45px;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 2rem;
  padding-top: 1.5rem;
  border-top: 1px solid var(--tts-gray-200);
}

.btn-primary,
.btn-secondary,
.btn-danger,
.btn-outline {
  padding: 0.75rem 2rem;
  border: none;
  border-radius: var(--tts-radius);
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-primary {
  background: var(--tts-primary);
  color: var(--tts-white);
}

.btn-primary:hover:not(:disabled) {
  background: var(--tts-primary-dark);
}

.btn-primary:disabled {
  background: var(--tts-gray-400);
  cursor: not-allowed;
}

.btn-secondary {
  background: var(--tts-gray-100);
  color: var(--tts-gray-700);
  border: 1px solid var(--tts-gray-300);
}

.btn-secondary:hover {
  background: var(--tts-gray-200);
}

.btn-danger {
  background: var(--tts-error);
  color: white;
}

.btn-danger:hover:not(:disabled) {
  background: #dc2626;
}

.btn-danger:disabled {
  background: var(--tts-gray-400);
  cursor: not-allowed;
}

.btn-outline {
  background: transparent;
  color: var(--tts-primary);
  border: 1px solid var(--tts-primary);
}

.btn-outline:hover {
  background: rgba(26, 86, 219, 0.1);
}

.loading-spinner {
  width: 16px;
  height: 16px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
  margin-right: 0.5rem;
}

.loading-spinner-small {
  width: 14px;
  height: 14px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
  margin-right: 0.5rem;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.reservation-filters {
  display: flex;
  gap: 1rem;
  margin-bottom: 1.5rem;
  padding: 1rem;
  background: var(--tts-gray-50);
  border-radius: var(--tts-radius);
}

.filter-group {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.filter-label {
  font-size: 0.875rem;
  color: var(--tts-gray-700);
}

.filter-select {
  padding: 0.5rem 0.75rem;
  border: 1px solid var(--tts-gray-300);
  border-radius: var(--tts-radius);
  font-size: 0.875rem;
  background: var(--tts-white);
  color: var(--tts-gray-700);
  min-width: 150px;
}

.reservation-items {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.reservation-card {
  background: var(--tts-white);
  border-radius: var(--tts-radius-lg);
  border: 1px solid var(--tts-gray-200);
  overflow: hidden;
  transition: all 0.3s;
}

.reservation-card:hover {
  box-shadow: var(--tts-shadow-lg);
  transform: translateY(-2px);
}

.reservation-card.status-pending {
  border-left: 4px solid var(--tts-warning);
}

.reservation-card.status-confirmed {
  border-left: 4px solid var(--tts-success);
}

.reservation-card.status-cancelled {
  border-left: 4px solid var(--tts-error);
}

.reservation-card.status-completed {
  border-left: 4px solid var(--tts-primary);
}

.reservation-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 1.5rem;
  background: var(--tts-gray-50);
  border-bottom: 1px solid var(--tts-gray-200);
}

.reservation-id {
  font-weight: 600;
  color: var(--tts-gray-800);
}

.reservation-status {
  padding: 0.25rem 0.75rem;
  border-radius: var(--tts-radius);
  font-size: 0.75rem;
  font-weight: 500;
}

.reservation-status.pending {
  background: rgba(245, 158, 11, 0.1);
  color: var(--tts-warning);
}

.reservation-status.confirmed {
  background: rgba(34, 197, 94, 0.1);
  color: var(--tts-success);
}

.reservation-status.cancelled {
  background: rgba(239, 68, 68, 0.1);
  color: var(--tts-error);
}

.reservation-status.completed {
  background: rgba(26, 86, 219, 0.1);
  color: var(--tts-primary);
}

.reservation-content {
  padding: 1.5rem;
  display: grid;
  grid-template-columns: 1fr 2fr;
  gap: 1.5rem;
}

@media (max-width: 768px) {
  .reservation-content {
    grid-template-columns: 1fr;
  }
}

.reservation-car {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.car-image {
  width: 100%;
  aspect-ratio: 16/9;
  border-radius: var(--tts-radius);
  overflow: hidden;
  background: var(--tts-gray-100);
}

.car-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.car-info {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.car-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: var(--tts-gray-800);
  margin: 0;
}

.car-details {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
  font-size: 0.875rem;
  color: var(--tts-gray-600);
}

.car-year,
.car-color,
.car-price {
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.car-price {
  font-weight: 600;
  color: var(--tts-primary);
}

.reservation-details {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.detail-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

@media (max-width: 640px) {
  .detail-row {
    grid-template-columns: 1fr;
  }
}

.detail-item {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.detail-label {
  font-size: 0.75rem;
  color: var(--tts-gray-500);
}

.detail-value {
  font-size: 0.875rem;
  color: var(--tts-gray-800);
  font-weight: 500;
}

.total-price {
  font-size: 1.125rem;
  color: var(--tts-primary);
  font-weight: 700;
}

.reservation-comment {
  padding: 1rem;
  background: var(--tts-gray-50);
  border-radius: var(--tts-radius);
  margin-top: 0.5rem;
}

.comment-label {
  font-size: 0.75rem;
  color: var(--tts-gray-500);
  display: block;
  margin-bottom: 0.5rem;
}

.comment-text {
  font-size: 0.875rem;
  color: var(--tts-gray-700);
  margin: 0;
  line-height: 1.5;
}

.reservation-actions {
  display: flex;
  gap: 1rem;
  padding: 1rem 1.5rem;
  border-top: 1px solid var(--tts-gray-200);
  background: var(--tts-gray-50);
}

@media (max-width: 640px) {
  .reservation-actions {
    flex-direction: column;
  }
}

.reservations-stats {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 1rem;
  margin-top: 2rem;
}

.stat-card {
  background: var(--tts-white);
  border: 1px solid var(--tts-gray-200);
  border-radius: var(--tts-radius-lg);
  padding: 1.5rem;
  text-align: center;
  transition: all 0.2s;
}

.stat-card:hover {
  box-shadow: var(--tts-shadow);
}

.stat-value {
  font-size: 2rem;
  font-weight: 700;
  color: var(--tts-primary);
  margin-bottom: 0.5rem;
}

.stat-label {
  font-size: 0.875rem;
  color: var(--tts-gray-600);
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  text-align: center;
  background: var(--tts-white);
  border-radius: var(--tts-radius-lg);
  border: 1px solid var(--tts-gray-200);
}

.loader {
  width: 48px;
  height: 48px;
  border: 3px solid var(--tts-gray-200);
  border-top-color: var(--tts-primary);
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 1rem;
}

.empty-state {
  text-align: center;
  padding: 4rem 2rem;
  background: var(--tts-white);
  border-radius: var(--tts-radius-lg);
  border: 1px solid var(--tts-gray-200);
}

.empty-icon {
  margin-bottom: 1.5rem;
  color: var(--tts-gray-400);
}

.empty-icon svg {
  width: 64px;
  height: 64px;
}

.empty-state h3 {
  margin-bottom: 0.75rem;
  color: var(--tts-gray-800);
  font-size: 1.25rem;
  font-weight: 600;
}

.empty-state p {
  color: var(--tts-gray-600);
  margin-bottom: 1.5rem;
  font-size: 0.875rem;
}

.password-strength {
  margin-top: 1rem;
}

.strength-label {
  font-size: 0.875rem;
  color: var(--tts-gray-700);
  margin-bottom: 0.5rem;
}

.strength-bar {
  height: 6px;
  background: var(--tts-gray-200);
  border-radius: 3px;
  overflow: hidden;
  margin-bottom: 0.5rem;
}

.strength-fill {
  height: 100%;
  transition: width 0.3s ease;
}

.strength-bar.weak .strength-fill {
  background: var(--tts-error);
}

.strength-bar.medium .strength-fill {
  background: var(--tts-warning);
}

.strength-bar.strong .strength-fill {
  background: var(--tts-success);
}

.strength-text {
  font-size: 0.75rem;
  font-weight: 500;
}

.strength-bar.weak .strength-text {
  color: var(--tts-error);
}

.strength-bar.medium .strength-text {
  color: var(--tts-warning);
}

.strength-bar.strong .strength-text {
  color: var(--tts-success);
}

.profile-sidebar {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.sidebar-section {
  background: var(--tts-white);
  border-radius: var(--tts-radius-lg);
  padding: 1.5rem;
  box-shadow: var(--tts-shadow-md);
  border: 1px solid var(--tts-gray-200);
}

.sidebar-header {
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid var(--tts-gray-200);
}

.sidebar-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: var(--tts-gray-800);
}

.user-summary {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1.5rem;
}

.user-summary-info {
  text-align: center;
  width: 100%;
}

.user-summary-info h4 {
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--tts-gray-800);
  margin: 0 0 0.5rem 0;
}

.user-email {
  color: var(--tts-gray-600);
  font-size: 0.875rem;
  margin-bottom: 0.5rem;
}

.user-join-date {
  color: var(--tts-gray-500);
  font-size: 0.75rem;
}

.quick-actions {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.quick-action-btn {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
  padding: 0.75rem 1rem;
  background: var(--tts-gray-50);
  border: 1px solid var(--tts-gray-200);
  border-radius: var(--tts-radius);
  font-size: 0.875rem;
  color: var(--tts-gray-700);
  cursor: pointer;
  transition: all 0.2s;
}

.quick-action-btn:hover {
  background: var(--tts-gray-100);
  border-color: var(--tts-gray-300);
}

.quick-action-btn.text-danger {
  color: var(--tts-error);
}

.quick-action-btn.text-danger:hover {
  background: rgba(239, 68, 68, 0.1);
  border-color: var(--tts-error);
}

.account-info {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.info-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.5rem 0;
  border-bottom: 1px solid var(--tts-gray-100);
}

.info-item:last-child {
  border-bottom: none;
}

.info-label {
  font-size: 0.875rem;
  color: var(--tts-gray-600);
}

.info-value {
  font-size: 0.875rem;
  font-weight: 500;
  color: var(--tts-gray-800);
}

.badge {
  padding: 0.25rem 0.75rem;
  border-radius: var(--tts-radius);
  font-size: 0.75rem;
  font-weight: 500;
}

.badge.admin {
  background: rgba(26, 86, 219, 0.1);
  color: var(--tts-primary);
}

.badge.user {
  background: rgba(107, 114, 128, 0.1);
  color: var(--tts-gray-700);
}

@media (max-width: 768px) {
  .profile-header {
    padding: 1.5rem 1rem;
  }
  
  .page-title {
    font-size: 1.75rem;
  }
  
  .profile-layout {
    padding: 0 1rem 1.5rem;
  }
  
  .profile-section,
  .reservations-section,
  .security-section {
    padding: 1.5rem;
  }
  
  .profile-tabs {
    flex-direction: column;
  }
}
</style>