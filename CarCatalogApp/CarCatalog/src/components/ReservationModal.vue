<script setup>
import { ref, computed, watch } from 'vue'
import { reservationsApi } from '@/api/reservations'

const props = defineProps({
  car: {
    type: Object,
    required: true
  },
  showModal: {
    type: Boolean,
    required: true
  }
})

const emit = defineEmits(['close', 'reservation-created'])

const reservationLoading = ref(false)
const reservationData = ref({
  startDate: '',
  endDate: '',
  comment: ''
})
const bookingPercent = 1 // Фиксированный 1% от стоимости автомобиля в день

// Вычисляемые свойства
const today = computed(() => {
  return new Date().toISOString().split('T')[0]
})

// Для совместимости с шаблоном
const minDate = computed(() => today.value)
const maxStartDate = computed(() => today.value)

const tomorrow = computed(() => {
  const date = new Date()
  date.setDate(date.getDate() + 1)
  return date.toISOString().split('T')[0]
})

const maxEndDate = computed(() => {
  const date = new Date()
  date.setDate(date.getDate() + 7)
  return date.toISOString().split('T')[0]
})

const minEndDate = computed(() => {
  if (!reservationData.value.startDate) return tomorrow.value
  
  const start = new Date(reservationData.value.startDate)
  const minEnd = new Date(start)
  minEnd.setDate(start.getDate() + 1)
  return minEnd.toISOString().split('T')[0]
})

const reservationDays = computed(() => {
  if (!reservationData.value.startDate || !reservationData.value.endDate) return 0
  const start = new Date(reservationData.value.startDate)
  const end = new Date(reservationData.value.endDate)
  const diffTime = Math.abs(end - start)
  return Math.ceil(diffTime / (1000 * 60 * 60 * 24))
})

const dailyPrice = computed(() => {
  return Math.floor(props.car.price * bookingPercent / 100)
})

const totalPrice = computed(() => {
  return reservationDays.value * dailyPrice.value
})

// Методы форматирования
const formatPrice = (price) => {
  return new Intl.NumberFormat('ru-RU').format(price || 0)
}

// Методы
const closeModal = () => {
  resetForm()
  emit('close')
}

const resetForm = () => {
  reservationData.value = {
    startDate: '',
    endDate: '',
    comment: ''
  }
}

const validateDates = () => {
  if (!reservationData.value.startDate || !reservationData.value.endDate) {
    alert('Пожалуйста, выберите даты бронирования')
    return false
  }
  
  const start = new Date(reservationData.value.startDate)
  const end = new Date(reservationData.value.endDate)
  
  // Проверяем, что дата начала - сегодня
  const todayDate = new Date().toISOString().split('T')[0]
  if (reservationData.value.startDate !== todayDate) {
    alert('Дата начала бронирования должна быть сегодняшним днем')
    return false
  }
  
  // Проверяем, что дата окончания как минимум на 1 день больше
  if (end <= start) {
    alert('Дата окончания должна быть как минимум на 1 день позже даты начала')
    return false
  }
  
  // Проверяем максимальный срок
  const maxDate = new Date(start)
  maxDate.setDate(start.getDate() + 7)
  if (end > maxDate) {
    alert('Максимальный срок бронирования - 7 дней')
    return false
  }
  
  return true
}

const submitReservation = async (event) => {
  event.preventDefault()
  
  if (!validateDates()) return
  
  if (reservationDays.value === 0) {
    alert('Пожалуйста, выберите корректные даты бронирования')
    return
  }
  
  reservationLoading.value = true

  try {
    // Отправляем даты в формате ISO 8601 (с временем)
    const data = {
      carId: props.car.id,
      startDate: new Date(reservationData.value.startDate).toISOString(),
      endDate: new Date(reservationData.value.endDate).toISOString(),
      comment: reservationData.value.comment || '',
      totalPrice: totalPrice.value
    }

    console.log('=== Отправка данных бронирования ===')
    console.log('Данные:', JSON.stringify(data, null, 2))
    
    const response = await reservationsApi.createReservation(data)
    
    console.log('=== Ответ сервера ===')
    console.log('Status:', response.status)
    console.log('Data:', response.data)
    
    alert('Автомобиль успешно забронирован!')
    resetForm()
    emit('reservation-created', response.data)
    closeModal()
    
  } catch (error) {
    console.error('=== Ошибка бронирования ===')
    console.error('Полная ошибка:', error)
    
    if (error.response) {
      console.error('Статус ответа:', error.response.status)
      console.error('Данные ответа:', JSON.stringify(error.response.data, null, 2))
      console.error('Заголовки:', error.response.headers)
      
      if (error.response.data && error.response.data.errors) {
        // Показываем все ошибки валидации
        const errorMessages = Object.values(error.response.data.errors)
          .flat()
          .join('\n')
        alert('Ошибки валидации:\n' + errorMessages)
      } else if (error.response.data && error.response.data.message) {
        alert('Ошибка: ' + error.response.data.message)
      } else if (error.response.data) {
        alert('Ошибка сервера: ' + JSON.stringify(error.response.data))
      } else {
        alert(`Ошибка ${error.response.status}`)
      }
    } else if (error.request) {
      console.error('Запрос:', error.request)
      alert('Нет ответа от сервера')
    } else {
      console.error('Сообщение ошибки:', error.message)
      alert('Ошибка: ' + error.message)
    }
  } finally {
    reservationLoading.value = false
  }
}

// Устанавливаем значения по умолчанию при открытии модального окна
watch(() => props.showModal, (isOpen) => {
  if (isOpen) {
    const todayDate = new Date().toISOString().split('T')[0]
    const tomorrowDate = new Date()
    tomorrowDate.setDate(tomorrowDate.getDate() + 1)
    
    reservationData.value.startDate = todayDate
    reservationData.value.endDate = tomorrowDate.toISOString().split('T')[0]
  }
})

const formatDaysText = (days) => {
  if (days === 1) return 'день'
  if (days >= 2 && days <= 4) return 'дня'
  return 'дней'
}
</script>

<template>
  <div v-if="showModal" class="modal-overlay" @click.self="closeModal">
    <div class="modal-content">
      <div class="modal-header">
        <h3 class="modal-title">Бронирование автомобиля</h3>
        <button class="modal-close" @click="closeModal">×</button>
      </div>
      <div class="modal-body">
        <div class="reservation-car-info">
          <img 
            :src="car.photoUrl || 'https://images.unsplash.com/photo-1553440569-bcc63803a83d?w=800&auto=format&fit=crop'" 
            :alt="car.modelName" 
            @error="e => e.target.src = 'https://images.unsplash.com/photo-1553440569-bcc63803a83d?w=800&auto=format&fit=crop'"
          />
          <div>
            <h4>{{ car.brandName }} {{ car.modelName }}</h4>
            <p>{{ car.year }} год • {{ car.color }}</p>
            <p class="car-price">{{ formatPrice(car.price) }} ₽</p>
          </div>
        </div>
        
        <div class="reservation-rules">
          <div class="rule-item">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor">
              <circle cx="12" cy="12" r="10" stroke-width="2"/>
              <path d="M12 6V12L16 14" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
            <span>Бронирование начинается сегодня</span>
          </div>
          <div class="rule-item">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor">
              <circle cx="12" cy="12" r="10" stroke-width="2"/>
              <path d="M12 6V12L16 14" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
            <span>Минимальный срок бронирования - 1 день (с сегодня до завтра)</span>
          </div>
          <div class="rule-item">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor">
              <circle cx="12" cy="12" r="10" stroke-width="2"/>
              <path d="M12 6V12L16 14" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
            <span>Максимальный срок бронирования - 7 дней</span>
          </div>
          <div class="rule-item">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor">
              <circle cx="12" cy="12" r="10" stroke-width="2"/>
              <path d="M12 6V12L16 14" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
            <span>Стоимость бронирования - 1% от цены автомобиля в день</span>
          </div>
        </div>
        
        <form @submit.prevent="submitReservation" class="reservation-form">
          <div class="form-row">
            <div class="form-group">
              <label class="form-label">Дата начала *</label>
              <input 
                v-model="reservationData.startDate" 
                type="date" 
                :min="minDate"
                :max="maxStartDate"
                class="form-input"
                required
                readonly
              />
              <div class="form-hint">Бронирование начинается сегодня</div>
            </div>
            
            <div class="form-group">
              <label class="form-label">Дата окончания *</label>
              <input 
                v-model="reservationData.endDate" 
                type="date" 
                :min="minEndDate"
                :max="maxEndDate"
                class="form-input"
                required
              />
              <div class="form-hint">
                <span v-if="reservationData.startDate">
                  Минимум: {{ minEndDate }}, максимум: {{ maxEndDate }}
                </span>
                <span v-else>Выберите дату начала</span>
              </div>
            </div>
          </div>
          
          <div class="form-group">
            <label class="form-label">Комментарий (необязательно)</label>
            <textarea 
              v-model="reservationData.comment" 
              rows="3"
              class="form-input"
              placeholder="Дополнительные пожелания, контактная информация..."
              maxlength="500"
            ></textarea>
            <div class="char-count">{{ reservationData.comment?.length || 0 }}/500</div>
          </div>
          
          <div class="reservation-summary">
            <div class="summary-item">
              <span>Процент бронирования:</span>
              <span>{{ bookingPercent }}% в день</span>
            </div>
            <div class="summary-item">
              <span>Количество дней:</span>
              <span>{{ reservationDays }} {{ formatDaysText(reservationDays) }}</span>
            </div>
            <div class="summary-item">
              <span>Стоимость в день:</span>
              <span>{{ formatPrice(dailyPrice) }} ₽</span>
            </div>
            <div class="summary-item total">
              <span>Общая стоимость брони:</span>
              <span>{{ formatPrice(totalPrice) }} ₽</span>
            </div>
            <div class="summary-note">
              * Бронирование автомобиля гарантирует, что он будет зарезервирован за вами на выбранный период
            </div>
          </div>
          
          <div class="modal-actions">
            <button type="button" class="btn-secondary" @click="closeModal" :disabled="reservationLoading">
              Отмена
            </button>
            <button type="submit" class="btn-primary" :disabled="reservationLoading || reservationDays < 1">
              <span v-if="reservationLoading" class="loading-spinner"></span>
              Забронировать за {{ formatPrice(totalPrice) }} ₽
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Стили без изменений */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2000;
  backdrop-filter: blur(4px);
  animation: fadeIn 0.2s ease;
}

.modal-content {
  background: var(--tts-white);
  border-radius: var(--tts-radius-lg);
  width: 100%;
  max-width: 500px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: var(--tts-shadow-xl);
  animation: slideUp 0.3s ease;
}

.modal-header {
  padding: 1.5rem;
  border-bottom: 1px solid var(--tts-gray-200);
  display: flex;
  justify-content: space-between;
  align-items: center;
  position: sticky;
  top: 0;
  background: var(--tts-white);
  z-index: 1;
}

.modal-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--tts-gray-800);
  margin: 0;
}

.modal-close {
  background: none;
  border: none;
  font-size: 1.5rem;
  color: var(--tts-gray-500);
  cursor: pointer;
  line-height: 1;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: var(--tts-radius);
  transition: all 0.2s;
}

.modal-close:hover {
  background: var(--tts-gray-100);
  color: var(--tts-gray-700);
}

.modal-body {
  padding: 1.5rem;
}

.reservation-car-info {
  display: flex;
  gap: 1rem;
  margin-bottom: 1rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid var(--tts-gray-200);
  align-items: center;
}

.reservation-car-info img {
  width: 100px;
  height: 70px;
  object-fit: cover;
  border-radius: var(--tts-radius);
}

.reservation-car-info h4 {
  margin: 0 0 0.25rem 0;
  font-size: 1.125rem;
  color: var(--tts-gray-800);
}

.reservation-car-info p {
  margin: 0;
  color: var(--tts-gray-600);
  font-size: 0.875rem;
}

.car-price {
  margin-top: 0.5rem !important;
  font-weight: 600;
  color: var(--tts-primary) !important;
  font-size: 1rem !important;
}

.reservation-rules {
  background: var(--tts-blue-50);
  border: 1px solid var(--tts-blue-100);
  border-radius: var(--tts-radius);
  padding: 1rem;
  margin-bottom: 1.5rem;
}

.rule-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin-bottom: 0.5rem;
  font-size: 0.875rem;
  color: var(--tts-blue-700);
}

.rule-item:last-child {
  margin-bottom: 0;
}

.rule-item svg {
  stroke: var(--tts-blue-500);
  flex-shrink: 0;
}

.reservation-form {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
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

.form-input:read-only {
  background: var(--tts-gray-100);
  color: var(--tts-gray-600);
  cursor: not-allowed;
}

.form-hint {
  font-size: 0.75rem;
  color: var(--tts-gray-500);
  margin-top: 0.25rem;
  line-height: 1.2;
}

textarea.form-input {
  resize: vertical;
  min-height: 80px;
  line-height: 1.5;
}

.char-count {
  text-align: right;
  font-size: 0.75rem;
  color: var(--tts-gray-500);
}

.reservation-summary {
  background: var(--tts-gray-50);
  padding: 1.25rem;
  border-radius: var(--tts-radius);
  border: 1px solid var(--tts-gray-200);
}

.summary-item {
  display: flex;
  justify-content: space-between;
  padding: 0.5rem 0;
  color: var(--tts-gray-700);
  font-size: 0.875rem;
}

.summary-item.total {
  border-top: 2px solid var(--tts-gray-300);
  margin-top: 0.75rem;
  padding-top: 0.75rem;
  font-weight: 600;
  color: var(--tts-primary);
  font-size: 1rem;
}

.summary-note {
  margin-top: 1rem;
  padding-top: 0.75rem;
  border-top: 1px dashed var(--tts-gray-300);
  font-size: 0.75rem;
  color: var(--tts-gray-500);
  line-height: 1.4;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 1rem;
}

.btn-primary,
.btn-secondary {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: var(--tts-radius);
  font-weight: 500;
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.2s;
  min-width: 120px;
}

.btn-primary {
  background: var(--tts-success);
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background: #059669;
  box-shadow: var(--tts-shadow-md);
}

.btn-primary:disabled {
  background: var(--tts-gray-400);
  cursor: not-allowed;
  opacity: 0.7;
}

.btn-secondary {
  background: var(--tts-gray-100);
  color: var(--tts-gray-700);
}

.btn-secondary:hover:not(:disabled) {
  background: var(--tts-gray-200);
}

.loading-spinner {
  width: 16px;
  height: 16px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
  display: inline-block;
  margin-right: 0.5rem;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

@media (max-width: 640px) {
  .modal-content {
    margin: 1rem;
  }
  
  .form-row {
    grid-template-columns: 1fr;
  }
  
  .modal-actions {
    flex-direction: column;
  }
  
  .btn-primary,
  .btn-secondary {
    width: 100%;
  }
}
</style>