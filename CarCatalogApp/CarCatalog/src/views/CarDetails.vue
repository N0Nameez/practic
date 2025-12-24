<!-- В CarDetail.vue изменения минимальны, только удалили процент -->
<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { carsApi } from '@/api/cars'
import { useAuthStore } from '@/stores/auth'
import ReservationModal from '@/components/ReservationModal.vue'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()

const car = ref(null)
const loading = ref(true)
const error = ref('')
const showReservationModal = ref(false)

const loadCar = async () => {
  loading.value = true
  error.value = ''
  
  try {
    const response = await carsApi.getCar(route.params.id)
    car.value = response.data
    console.log(car.value)
  } catch (err) {
    console.error('Ошибка загрузки автомобиля:', err)
    error.value = 'Не удалось загрузить информацию об автомобиле'
  } finally {
    loading.value = false
  }
}

const getStatusText = (status) => {
  const statusMap = {
    available: 'В наличии',
    sold: 'Продано',
    reserved: 'Забронировано'
  }
  return statusMap[status] || status
}

const formatDate = (dateString) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('ru-RU', {
    day: 'numeric',
    month: 'long',
    year: 'numeric'
  })
}

const editCar = () => {
  if (car.value) {
    router.push({
      path: `/admin/cars/${car.value.id}/edit`,
      query: { from: 'car' }
    })
  }
}

const deleteCar = async () => {
  if (!confirm('Вы уверены, что хотите удалить этот автомобиль?')) {
    return
  }
  
  try {
    await carsApi.deleteCar(car.value.id)
    router.push('/')
  } catch (err) {
    console.error('Ошибка удаления автомобиля:', err)
    alert('Не удалось удалить автомобиль')
  }
}

const openReservationModal = () => {
  if (!authStore.isAuthenticated) {
    router.push('/login')
    return
  }
  
  if (car.value.status !== 'available') {
    alert('Этот автомобиль недоступен для бронирования')
    return
  }
  
  showReservationModal.value = true
}

const handleReservationCreated = (reservation) => {
  // Обновляем статус автомобиля
  if (car.value) {
    car.value.status = 'reserved'
  }
}

const formatPrice = (price) => {
  return new Intl.NumberFormat('ru-RU').format(price)
}

const formatMileage = (mileage) => {
  return new Intl.NumberFormat('ru-RU').format(mileage)
}

onMounted(() => {
  loadCar()
})
</script>

<template>
  <div class="car-details-page">
    <div v-if="loading" class="loading-state">
      <div class="loader"></div>
      <p>Загрузка автомобиля...</p>
    </div>
    
    <div v-else-if="error" class="error-state">
      <div class="error-icon">
        <svg width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
          <path d="M12 8V12M12 16H12.01M22 12C22 17.5228 17.5228 22 12 22C6.47715 22 2 17.5228 2 12C2 6.47715 6.47715 2 12 2C17.5228 2 22 6.47715 22 12Z" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
      </div>
      <h3>{{ error }}</h3>
      <button @click="$router.push('/')" class="back-btn">Вернуться в каталог</button>
    </div>
    
    <div v-else-if="car" class="car-details-container">
      <div class="breadcrumbs">
        <button @click="$router.push('/')" class="breadcrumb-link">Каталог</button>
        <span class="breadcrumb-separator">/</span>
        <span class="breadcrumb-current">{{ car.brandName }} {{ car.modelName }}</span>
      </div>
      
      <div class="car-details-card">
        <div class="car-gallery">
          <img 
            :src="car.photoUrl || 'https://images.unsplash.com/photo-1549399542-7e3f8b79c341?w=800&auto=format&fit=crop'"
            :alt="car.brandName + ' ' + car.modelName"
            class="main-image"
            @error="e => e.target.src = 'https://images.unsplash.com/photo-1553440569-bcc63803a83d?w=800&auto=format&fit=crop'"
          />
          <div class="car-status" :class="car.status">
            {{ getStatusText(car.status) }}
          </div>
        </div>
        
        <div class="car-info">
          <div class="car-header">
            <h1 class="car-title">{{ car.brandName }} {{ car.modelName }}</h1>
            <div class="car-year">{{ car.year }} год</div>
          </div>
          
          <div class="price-section">
            <div class="price-value">{{ formatPrice(car.price) }} ₽</div>
            <div class="price-label">Стоимость</div>
          </div>
          
          <div class="specifications">
            <h3 class="section-title">Характеристики</h3>
            <div class="specs-grid">
              <div class="spec-item">
                <div class="spec-label">Цвет</div>
                <div class="spec-value">{{ car.color }}</div>
              </div>
              <div class="spec-item">
                <div class="spec-label">Пробег</div>
                <div class="spec-value">{{ formatMileage(car.mileage) }} км</div>
              </div>
              <div class="spec-item">
                <div class="spec-label">VIN номер</div>
                <div class="spec-value">{{ car.vin || 'Не указан' }}</div>
              </div>
              <div class="spec-item">
                <div class="spec-label">Дата добавления</div>
                <div class="spec-value">{{ formatDate(car.createdAt) }}</div>
              </div>
            </div>
          </div>
          
          <div v-if="car.description" class="description-section">
            <h3 class="section-title">Описание</h3>
            <div class="description-content">
              {{ car.description }}
            </div>
          </div>
          
          <div class="reservation-info">
            <h3 class="section-title">Условия бронирования</h3>
            <div class="reservation-conditions">
              <div class="condition-item">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                  <circle cx="12" cy="12" r="10" stroke-width="2"/>
                  <path d="M12 6V12L16 14" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>
                <span>Бронирование начинается сегодня</span>
              </div>
              <div class="condition-item">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                  <circle cx="12" cy="12" r="10" stroke-width="2" />
                  <path d="M12 6V12L16 14" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
                </svg>
                <span>Минимальный срок бронирования - 1 день (с сегодня до завтра)</span>
              </div>
              <div class="condition-item">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                  <circle cx="12" cy="12" r="10" stroke-width="2"/>
                  <path d="M12 6V12L16 14" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>
                <span>Максимальный срок бронирования - 7 дней</span>
              </div>
              <div class="condition-item">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor">
                  <circle cx="12" cy="12" r="10" stroke-width="2"/>
                  <path d="M12 6V12L16 14" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>
                <span>Стоимость бронирования - 1% от цены автомобиля в день</span>
              </div>
            </div>
          </div>
          
          <div class="action-buttons">
            <button 
              v-if="authStore.isAdmin" 
              @click="editCar"
              class="action-btn edit-btn"
            >
              Редактировать
            </button>
            
            <button 
              v-if="authStore.isAdmin" 
              @click="deleteCar"
              class="action-btn delete-btn"
            >
              Удалить
            </button>
            
            <button 
              v-if="car.status === 'available'" 
              @click="openReservationModal"
              class="action-btn reserve-btn"
              :disabled="!authStore.isAuthenticated"
            >
              Забронировать
            </button>
          </div>
          
          <div v-if="!authStore.isAuthenticated && car.status === 'available'" class="auth-notice">
            Для бронирования автомобиля требуется войти в систему
          </div>
          
          <div v-if="car.status === 'reserved'" class="reserved-notice">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor">
              <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
            <span>Этот автомобиль уже забронирован</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Модальное окно бронирования -->
    <ReservationModal
      v-if="car"
      :car="car"
      :show-modal="showReservationModal"
      @close="showReservationModal = false"
      @reservation-created="handleReservationCreated"
    />
  </div>
</template>

<style scoped>
/* Добавляем новые стили для условий бронирования */
.reservation-info {
  padding-top: 1.5rem;
  border-top: 1px solid var(--tts-gray-200);
}

.reservation-conditions {
  background: var(--tts-blue-50);
  border: 1px solid var(--tts-blue-100);
  border-radius: var(--tts-radius);
  padding: 1rem;
  margin-top: 0.5rem;
}

.condition-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.5rem;
  font-size: 0.875rem;
  color: var(--tts-blue-700);
}

.condition-item:last-child {
  margin-bottom: 0;
}

.condition-item svg {
  stroke: var(--tts-blue-500);
  flex-shrink: 0;
}

/* Остальные стили без изменений */
.car-details-page {
  min-height: 100vh;
  background: var(--tts-gray-50);
  padding: 2rem 1.5rem;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 60vh;
  text-align: center;
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

.loading-state p {
  color: var(--tts-gray-600);
  font-size: 0.875rem;
}

.error-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 60vh;
  text-align: center;
  background: var(--tts-white);
  border-radius: var(--tts-radius-lg);
  padding: 3rem 2rem;
  box-shadow: var(--tts-shadow);
  border: 1px solid var(--tts-gray-200);
}

.error-icon {
  color: var(--tts-error);
  margin-bottom: 1.5rem;
}

.error-icon svg {
  width: 64px;
  height: 64px;
}

.error-state h3 {
  color: var(--tts-gray-800);
  font-size: 1.25rem;
  margin-bottom: 1.5rem;
}

.back-btn {
  padding: 0.75rem 1.5rem;
  background: var(--tts-primary);
  color: white;
  border: none;
  border-radius: var(--tts-radius);
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
}

.back-btn:hover {
  background: var(--tts-primary-dark);
  box-shadow: var(--tts-shadow-md);
}

.car-details-container {
  max-width: 1200px;
  margin: 0 auto;
}

.breadcrumbs {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 2rem;
  font-size: 0.875rem;
  color: var(--tts-gray-600);
}

.breadcrumb-link {
  background: none;
  border: none;
  color: var(--tts-primary);
  cursor: pointer;
  padding: 0;
  font-size: 0.875rem;
  text-decoration: none;
}

.breadcrumb-link:hover {
  text-decoration: underline;
}

.breadcrumb-separator {
  color: var(--tts-gray-400);
}

.breadcrumb-current {
  color: var(--tts-gray-700);
  font-weight: 500;
}

.car-details-card {
  background: var(--tts-white);
  border-radius: var(--tts-radius-lg);
  overflow: hidden;
  box-shadow: var(--tts-shadow-lg);
  border: 1px solid var(--tts-gray-200);
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 3rem;
}

@media (max-width: 1024px) {
  .car-details-card {
    grid-template-columns: 1fr;
  }
}

.car-gallery {
  position: relative;
  min-height: 500px;
  overflow: hidden;
}

.main-image {
  width: 100%;
  height: 50%;
  object-fit: fill;
  display: block;
}

.car-status {
  position: absolute;
  top: 1.5rem;
  left: 1.5rem;
  padding: 0.5rem 1rem;
  border-radius: var(--tts-radius-full);
  font-size: 0.875rem;
  font-weight: 500;
  backdrop-filter: blur(4px);
}

.car-status.available {
  background: rgba(16, 185, 129, 0.1);
  color: var(--tts-success);
}

.car-status.sold {
  background: rgba(100, 116, 139, 0.1);
  color: var(--tts-gray-500);
}

.car-status.reserved {
  background: rgba(245, 158, 11, 0.1);
  color: var(--tts-warning);
}

.car-info {
  padding: 2.5rem 2.5rem 2.5rem 0;
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

@media (max-width: 1024px) {
  .car-info {
    padding: 2.5rem;
  }
}

.car-header {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.car-title {
  font-size: 2rem;
  font-weight: 700;
  color: var(--tts-gray-900);
  line-height: 1.2;
  margin: 0;
}

.car-year {
  font-size: 1rem;
  color: var(--tts-gray-600);
  font-weight: 500;
}

.price-section {
  padding-bottom: 1.5rem;
  border-bottom: 2px solid var(--tts-gray-200);
}

.price-value {
  font-size: 2.5rem;
  font-weight: 700;
  color: var(--tts-primary);
  line-height: 1;
  margin-bottom: 0.25rem;
}

.price-label {
  font-size: 0.875rem;
  color: var(--tts-gray-500);
  font-weight: 500;
}

.section-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--tts-gray-800);
  margin: 0 0 1rem 0;
}

.specs-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.5rem;
}

@media (max-width: 640px) {
  .specs-grid {
    grid-template-columns: 1fr;
  }
}

.spec-item {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.spec-label {
  font-size: 0.875rem;
  color: var(--tts-gray-600);
  font-weight: 500;
}

.spec-value {
  font-size: 1rem;
  color: var(--tts-gray-800);
  font-weight: 600;
}

.description-section {
  padding-top: 1.5rem;
  border-top: 1px solid var(--tts-gray-200);
}

.description-content {
  line-height: 1.6;
  color: var(--tts-gray-700);
  font-size: 0.875rem;
  white-space: pre-line;
}

.action-buttons {
  display: flex;
  gap: 1rem;
  padding-top: 1.5rem;
  border-top: 1px solid var(--tts-gray-200);
}

.action-btn {
  padding: 0.875rem 1.5rem;
  border: none;
  border-radius: var(--tts-radius);
  font-weight: 500;
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.2s;
  min-width: 120px;
}

.edit-btn {
  background: var(--tts-primary);
  color: white;
}

.edit-btn:hover {
  background: var(--tts-primary-dark);
  box-shadow: var(--tts-shadow-md);
}

.delete-btn {
  background: var(--tts-error);
  color: white;
}

.delete-btn:hover {
  background: #dc2626;
  box-shadow: var(--tts-shadow-md);
}

.reserve-btn {
  background: var(--tts-success);
  color: white;
  margin-left: auto;
}

.reserve-btn:hover:not(:disabled) {
  background: #059669;
  box-shadow: var(--tts-shadow-md);
}

.reserve-btn:disabled {
  background: var(--tts-gray-400);
  cursor: not-allowed;
  box-shadow: none;
}

.auth-notice {
  font-size: 0.875rem;
  color: var(--tts-gray-500);
  text-align: center;
  padding: 0.75rem;
  background: var(--tts-gray-100);
  border-radius: var(--tts-radius);
  margin-top: 0.5rem;
}

.reserved-notice {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem;
  background: rgba(245, 158, 11, 0.1);
  border: 1px solid rgba(245, 158, 11, 0.2);
  border-radius: var(--tts-radius);
  color: var(--tts-warning);
  font-size: 0.875rem;
  margin-top: 1rem;
}

.reserved-notice svg {
  stroke: var(--tts-warning);
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
</style>