<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import ReservationModal from '@/components/ReservationModal.vue'

const props = defineProps({
  car: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['click', 'reservation-created'])

const router = useRouter()
const authStore = useAuthStore()
const showReservationModal = ref(false)

const placeholderImage = 'https://images.unsplash.com/photo-1553440569-bcc63803a83d?w=800&auto=format&fit=crop'

const statusConfig = computed(() => {
  const configs = {
    available: {
      text: 'В наличии',
      color: 'var(--tts-success)',
      bg: 'rgba(16, 185, 129, 0.1)'
    },
    sold: {
      text: 'Продано',
      color: 'var(--tts-gray-500)',
      bg: 'rgba(100, 116, 139, 0.1)'
    },
    reserved: {
      text: 'Забронировано',
      color: 'var(--tts-warning)',
      bg: 'rgba(245, 158, 11, 0.1)'
    }
  }
  return configs[props.car.status] || configs.available
})

// Методы форматирования
const formatPrice = (price) => {
  return new Intl.NumberFormat('ru-RU').format(price || 0)
}

const formatMileage = (mileage) => {
  return new Intl.NumberFormat('ru-RU').format(mileage || 0)
}

// Методы бронирования
const openReservationModal = (event) => {
  event.stopPropagation()
  
  if (!authStore.isAuthenticated) {
    router.push('/login')
    return
  }
  
  if (props.car.status !== 'available') {
    alert('Этот автомобиль недоступен для бронирования')
    return
  }
  
  showReservationModal.value = true
}

const handleReservationCreated = (reservation) => {
  // Эмитим событие для обновления данных в родительском компоненте
  emit('reservation-created', reservation)
  showReservationModal.value = false
}

const viewDetails = (event) => {
  event.stopPropagation()
  emit('click', props.car)
}

const handleImageError = (event) => {
  event.target.src = placeholderImage
}
</script>

<template>
  <div class="car-card" @click="viewDetails($event)">
    <div class="car-image">
      <img 
        :src="car.photoUrl || placeholderImage"
        :alt="car.brandName + ' ' + car.modelName"
        class="car-image-img"
        @error="handleImageError"
      />
      <div class="car-status" :style="{ backgroundColor: statusConfig.bg, color: statusConfig.color }">
        {{ statusConfig.text }}
      </div>
    </div>
    
    <div class="car-content">
      <div class="car-header">
        <h3 class="car-title">{{ car.brandName }} {{ car.modelName }}</h3>
        <span class="car-year">{{ car.year }} год</span>
      </div>
      
      <div class="car-details">
        <div class="car-detail">
          <span class="detail-text">{{ car.color }}</span>
        </div>
        <div class="car-detail">
          <span class="detail-text">{{ formatMileage(car.mileage) }} км</span>
        </div>
      </div>
      
      <div class="car-footer">
        <div class="car-price">
          <span class="price-label">Цена</span>
          <span class="price-value">{{ formatPrice(car.price) }} ₽</span>
        </div>
        <div class="card-actions">
          <button 
            class="btn-primary" 
            @click.stop="openReservationModal"
            :disabled="car.status !== 'available'"
          >
            Забронировать
          </button>
        </div>
      </div>
      
      <div class="reservation-hint">
        Бронирование от 1 до 7 дней • 1% от стоимости в день
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
</template>

<style scoped>
.car-card {
  background: var(--tts-white);
  border-radius: var(--tts-radius-lg);
  overflow: hidden;
  box-shadow: var(--tts-shadow);
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  cursor: pointer;
  display: flex;
  flex-direction: column;
  height: 100%;
  border: 1px solid var(--tts-gray-200);
}

.car-card:hover {
  transform: translateY(-8px);
  box-shadow: var(--tts-shadow-xl);
  border-color: var(--tts-primary);
}

.car-image {
  position: relative;
  height: 200px;
  overflow: hidden;
}

.car-image-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.5s ease;
}

.car-card:hover .car-image-img {
  transform: scale(1.05);
}

.car-status {
  position: absolute;
  top: 1rem;
  left: 1rem;
  padding: 0.375rem 0.75rem;
  border-radius: var(--tts-radius-full);
  font-size: 0.75rem;
  font-weight: 500;
  backdrop-filter: blur(4px);
  z-index: 1;
}

.car-content {
  padding: 1.5rem;
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.car-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 0.5rem;
}

.car-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: var(--tts-gray-900);
  line-height: 1.4;
  margin: 0;
  flex: 1;
}

.car-year {
  font-size: 0.875rem;
  color: var(--tts-gray-500);
  background: var(--tts-gray-100);
  padding: 0.25rem 0.5rem;
  border-radius: var(--tts-radius);
  white-space: nowrap;
}

.car-details {
  display: flex;
  gap: 1rem;
  margin-top: 0.5rem;
}

.car-detail {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.detail-text {
  font-size: 0.875rem;
  color: var(--tts-gray-600);
}

.car-footer {
  margin-top: auto;
  padding-top: 1rem;
  border-top: 1px solid var(--tts-gray-200);
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
}

.car-price {
  display: flex;
  flex-direction: column;
}

.price-label {
  font-size: 0.75rem;
  color: var(--tts-gray-500);
  margin-bottom: 0.25rem;
}

.price-value {
  font-size: 1.25rem;
  font-weight: 700;
  color: var(--tts-primary);
}

.card-actions {
  display: flex;
  gap: 0.5rem;
}

.btn-primary {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 1rem;
  border: none;
  border-radius: var(--tts-radius);
  font-weight: 500;
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.2s;
  white-space: nowrap;
}

.btn-primary {
  background: var(--tts-success);
  color: var(--tts-white);
}

.btn-primary:hover:not(:disabled) {
  background: #059669;
  transform: translateY(-1px);
  box-shadow: var(--tts-shadow-md);
}

.btn-primary:disabled {
  background: var(--tts-gray-400);
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

.reservation-hint {
  font-size: 0.75rem;
  color: var(--tts-gray-500);
  text-align: center;
  padding: 0.5rem;
  background: var(--tts-gray-100);
  border-radius: var(--tts-radius);
  margin-top: 0.5rem;
}

@media (max-width: 768px) {
  .car-footer {
    flex-direction: column;
    align-items: stretch;
  }
  
  .card-actions {
    width: 100%;
  }
  
  .btn-primary {
    width: 100%;
    justify-content: center;
  }
}

@media (max-width: 640px) {
  .car-card {
    margin: 0.5rem;
  }
  
  .car-content {
    padding: 1rem;
  }
  
  .car-header {
    flex-direction: column;
    align-items: flex-start;
  }
  
  .car-year {
    align-self: flex-start;
  }
}
</style>