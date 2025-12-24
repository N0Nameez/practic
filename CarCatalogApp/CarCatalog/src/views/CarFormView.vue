<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import CarForm from '@/components/CarForm.vue'
import { carsApi } from '@/api/cars'

const route = useRoute()
const router = useRouter()

const carId = route.params.id ? Number(route.params.id) : null
const carData = ref({})
const loading = ref(false)
const from = route.query.from || 'admin' // Получаем параметр from

const loadCarData = async () => {
  if (!carId) return
  
  loading.value = true
  try {
    const response = await carsApi.getCar(carId)
    // Преобразуем данные автомобиля для формы
    carData.value = {
      ...response.data,
      // Если API возвращает brandName и modelName, они уже в нужном формате
      // Форма сама найдет соответствующие ID
    }
  } catch (error) {
    console.error('Ошибка загрузки данных автомобиля:', error)
  } finally {
    loading.value = false
  }
}

const handleCarAdded = () => {
  // После добавления нового автомобиля всегда возвращаемся в админ-панель
  router.push('/admin')
}

const handleCarUpdated = () => {
  // После обновления возвращаемся туда, откуда пришли
  if (from === 'car' && carId) {
    router.push(`/car/${carId}`)
  } else {
    router.push('/admin')
  }
}

const handleCancel = () => {
  // При отмене возвращаемся туда, откуда пришли
  if (from === 'car' && carId) {
    router.push(`/car/${carId}`)
  } else {
    router.push('/admin')
  }
}

onMounted(() => {
  if (carId) {
    loadCarData()
  }
})
</script>

<template>
  <div class="car-form-view">
    <div class="form-header">
      <div class="breadcrumbs">
        <button @click="handleCancel" class="breadcrumb-link">
          {{ from === 'car' ? 'Назад к автомобилю' : 'Админ-панель' }}
        </button>
        <span class="breadcrumb-separator">/</span>
        <span class="breadcrumb-current">
          {{ carId ? 'Редактирование автомобиля' : 'Добавление автомобиля' }}
        </span>
      </div>
    </div>

    <div class="form-content">
      <div v-if="loading" class="loading-state">
        <div class="loader"></div>
        <p>Загрузка данных автомобиля...</p>
      </div>
      
      <CarForm
        v-else
        :car-id="carId"
        :initial-data="carData"
        @car-added="handleCarAdded"
        @car-updated="handleCarUpdated"
        @cancel="handleCancel"
      />
    </div>
  </div>
</template>

<style scoped>
.car-form-view {
  min-height: 100vh;
  background: var(--tts-gray-50);
  padding: 2rem 1.5rem;
}

.form-header {
  max-width: 1200px;
  margin: 0 auto 2rem;
}

.breadcrumbs {
  display: flex;
  align-items: center;
  gap: 0.5rem;
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

.form-content {
  max-width: 1200px;
  margin: 0 auto;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 400px;
  text-align: center;
  background: var(--tts-white);
  border-radius: var(--tts-radius-lg);
  box-shadow: var(--tts-shadow);
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

.loading-state p {
  color: var(--tts-gray-600);
  font-size: 0.875rem;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
</style>