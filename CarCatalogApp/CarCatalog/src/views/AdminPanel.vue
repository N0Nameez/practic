<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { carsApi } from '@/api/cars'

const router = useRouter()
const cars = ref([])
const deletingCarId = ref(null)

const loadCars = async () => {
  try {
    const response = await carsApi.getCars()
    cars.value = response.data.sort((a, b) => b.id - a.id)
  } catch (error) {
    console.error('Ошибка загрузки автомобилей:', error)
  }
}

const editCar = (carId) => {
  router.push({
    path: `/admin/cars/${carId}/edit`,
    query: { from: 'admin' }
  })
}

const deleteCar = async (carId) => {
  if (!confirm('Вы уверены, что хотите удалить этот автомобиль?')) {
    return
  }
  
  deletingCarId.value = carId
  try {
    await carsApi.deleteCar(carId)
    cars.value = cars.value.filter(car => car.id !== carId)
  } catch (error) {
    console.error('Ошибка удаления автомобиля:', error)
    alert('Не удалось удалить автомобиль')
  } finally {
    deletingCarId.value = null
  }
}

const addNewCar = () => {
  router.push({
    path: '/admin/cars/new',
    query: { from: 'admin' }
  })
}

const getStatusText = (status) => {
  const statusMap = {
    available: 'В наличии',
    sold: 'Продано',
    reserved: 'Забронировано'
  }
  return statusMap[status] || status
}

const formatPrice = (price) => {
  return new Intl.NumberFormat('ru-RU').format(price)
}

onMounted(() => {
  loadCars()
})
</script>

<template>
  <div class="admin-panel">
    <div class="admin-header">
      <h1 class="page-title">Панель администратора</h1>
      <p class="page-subtitle">Управление автомобилями ТТС "ТрансТехСервис"</p>
    </div>
    
    <div class="admin-content">
      <div class="admin-controls">
        <button 
          @click="addNewCar" 
          class="admin-btn add-btn"
        >
          Добавить автомобиль
        </button>
        
        <div class="stats">
          <span class="stats-text">Всего автомобилей: <strong>{{ cars.length }}</strong></span>
          <button @click="loadCars" class="refresh-btn">
            Обновить список
          </button>
        </div>
      </div>
      
      <div class="cars-table-container">
        <div class="table-wrapper">
          <table class="cars-table">
            <thead>
              <tr>
                <th class="id-column">ID</th>
                <th>Марка и модель</th>
                <th class="year-column">Год</th>
                <th class="price-column">Цена</th>
                <th class="status-column">Статус</th>
                <th class="actions-column">Действия</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="car in cars" :key="car.id">
                <td class="id-column">{{ car.id }}</td>
                <td>
                  <div class="car-info-cell">
                    <div class="car-brand">{{ car.brandName }}</div>
                    <div class="car-model">{{ car.modelName }}</div>
                  </div>
                </td>
                <td class="year-column">{{ car.year }}</td>
                <td class="price-column">{{ formatPrice(car.price) }} ₽</td>
                <td class="status-column">
                  <span :class="['status-badge', car.status]">
                    {{ getStatusText(car.status) }}
                  </span>
                </td>
                <td class="actions-column">
                  <div class="action-buttons">
                    <button 
                      @click="editCar(car.id)"
                      class="table-btn edit-btn"
                    >
                      Редактировать
                    </button>
                    <button 
                      @click="deleteCar(car.id)" 
                      class="table-btn delete-btn"
                      :disabled="deletingCarId === car.id"
                    >
                      {{ deletingCarId === car.id ? 'Удаление...' : 'Удалить' }}
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        
        <div v-if="cars.length === 0" class="empty-table">
          <div class="empty-icon">
            <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
              <path d="M16 3H1V20H23V8M16 3L23 8M16 3V8H23M5 14H10M5 17H19" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
          </div>
          <h3>Автомобили не найдены</h3>
          <p>Начните с добавления нового автомобиля</p>
          <button @click="addNewCar" class="add-car-btn">
            Добавить первый автомобиль
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.admin-panel {
  margin-top: -23px;
  min-height: 100vh;
  background: var(--tts-gray-50);
  padding-bottom: 2rem;
}

.admin-header {
  background: linear-gradient(135deg, var(--tts-primary) 0%, var(--tts-primary-dark) 100%);
  color: white;
  padding: 2rem 1.5rem;
  border-radius: 0 0 var(--tts-radius-lg) var(--tts-radius-lg);
  margin-bottom: 2rem;
  box-shadow: var(--tts-shadow-md);
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

.admin-content {
  max-width: 1440px;
  margin: 0 auto;
  padding: 0 1.5rem;
}

.admin-controls {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
  padding: 1.5rem;
  background: var(--tts-white);
  border-radius: var(--tts-radius-lg);
  box-shadow: var(--tts-shadow);
  border: 1px solid var(--tts-gray-200);
}

.admin-btn {
  padding: 0.875rem 1.5rem;
  border: none;
  border-radius: var(--tts-radius);
  font-weight: 500;
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.2s;
}

.add-btn {
  background: var(--tts-primary);
  color: white;
}

.add-btn:hover {
  background: var(--tts-primary-dark);
  box-shadow: var(--tts-shadow-md);
}

.stats {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.stats-text {
  color: var(--tts-gray-700);
  font-size: 0.875rem;
}

.stats-text strong {
  color: var(--tts-gray-900);
  font-weight: 600;
}

.refresh-btn {
  padding: 0.625rem 1.25rem;
  background: var(--tts-gray-100);
  color: var(--tts-gray-700);
  border: 1px solid var(--tts-gray-300);
  border-radius: var(--tts-radius);
  font-weight: 500;
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.2s;
}

.refresh-btn:hover {
  background: var(--tts-gray-200);
  border-color: var(--tts-gray-400);
}

.cars-table-container {
  background: var(--tts-white);
  border-radius: var(--tts-radius-lg);
  box-shadow: var(--tts-shadow);
  border: 1px solid var(--tts-gray-200);
  overflow: hidden;
}

.table-wrapper {
  overflow-x: auto;
}

.cars-table {
  width: 100%;
  border-collapse: collapse;
  min-width: 800px;
}

.cars-table thead {
  background: var(--tts-gray-50);
  border-bottom: 2px solid var(--tts-gray-200);
}

.cars-table th {
  padding: 1rem 1.5rem;
  text-align: left;
  font-size: 0.875rem;
  font-weight: 600;
  color: var(--tts-gray-700);
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.cars-table tbody tr {
  border-bottom: 1px solid var(--tts-gray-200);
  transition: background-color 0.2s;
}

.cars-table tbody tr:hover {
  background: var(--tts-gray-50);
}

.cars-table td {
  padding: 1.25rem 1.5rem;
  font-size: 0.875rem;
  color: var(--tts-gray-800);
}

.id-column {
  width: 80px;
  font-weight: 600;
  color: var(--tts-gray-700);
}

.year-column {
  width: 100px;
}

.price-column {
  width: 140px;
  font-weight: 600;
  color: var(--tts-primary);
}

.status-column {
  width: 150px;
}

.actions-column {
  width: 220px;
}

.car-info-cell {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.car-brand {
  font-weight: 600;
  color: var(--tts-gray-900);
}

.car-model {
  color: var(--tts-gray-600);
  font-size: 0.75rem;
}

.status-badge {
  display: inline-block;
  padding: 0.375rem 0.75rem;
  border-radius: var(--tts-radius-full);
  font-size: 0.75rem;
  font-weight: 500;
}

.status-badge.available {
  background: rgba(16, 185, 129, 0.1);
  color: var(--tts-success);
}

.status-badge.sold {
  background: rgba(100, 116, 139, 0.1);
  color: var(--tts-gray-500);
}

.status-badge.reserved {
  background: rgba(245, 158, 11, 0.1);
  color: var(--tts-warning);
}

.action-buttons {
  display: flex;
  gap: 0.5rem;
}

.table-btn {
  padding: 0.5rem 0.875rem;
  border: none;
  border-radius: var(--tts-radius);
  font-size: 0.75rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  white-space: nowrap;
}

.edit-btn {
  background: rgba(26, 86, 219, 0.1);
  color: var(--tts-primary);
  border: 1px solid rgba(26, 86, 219, 0.2);
}

.edit-btn:hover {
  background: rgba(26, 86, 219, 0.2);
  border-color: rgba(26, 86, 219, 0.3);
}

.delete-btn {
  background: rgba(239, 68, 68, 0.1);
  color: var(--tts-error);
  border: 1px solid rgba(239, 68, 68, 0.2);
}

.delete-btn:hover:not(:disabled) {
  background: rgba(239, 68, 68, 0.2);
  border-color: rgba(239, 68, 68, 0.3);
}

.delete-btn:disabled {
  background: var(--tts-gray-200);
  color: var(--tts-gray-500);
  border-color: var(--tts-gray-300);
  cursor: not-allowed;
}

.empty-table {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 3rem 2rem;
  text-align: center;
}

.empty-icon {
  color: var(--tts-gray-400);
  margin-bottom: 1rem;
}

.empty-icon svg {
  width: 48px;
  height: 48px;
}

.empty-table h3 {
  color: var(--tts-gray-800);
  font-size: 1.25rem;
  margin-bottom: 0.5rem;
}

.empty-table p {
  color: var(--tts-gray-600);
  font-size: 0.875rem;
  margin-bottom: 1.5rem;
}

.add-car-btn {
  padding: 0.75rem 1.5rem;
  background: var(--tts-primary);
  color: white;
  border: none;
  border-radius: var(--tts-radius);
  font-weight: 500;
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.2s;
}

.add-car-btn:hover {
  background: var(--tts-primary-dark);
  box-shadow: var(--tts-shadow-md);
}

@media (max-width: 768px) {
  .admin-controls {
    flex-direction: column;
    gap: 1rem;
    align-items: stretch;
  }
  
  .stats {
    flex-direction: column;
    align-items: stretch;
    gap: 1rem;
  }
  
  .admin-btn, .refresh-btn {
    width: 100%;
  }
  
  .action-buttons {
    flex-direction: column;
  }
  
  .table-btn {
    width: 100%;
  }
}
</style>