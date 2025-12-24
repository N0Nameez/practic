<script setup>
import { ref, onMounted, watch, computed } from 'vue'
import { useRouter } from 'vue-router'
import { carsApi } from '@/api/cars'
import CarCard from '@/components/CarCard.vue'

const router = useRouter()
const cars = ref([])
const loading = ref(false)
const showMobileFilters = ref(false)

const filters = ref({
  brandId: null,
  modelId: null,
  minYear: null,
  maxYear: null,
  minPrice: null,
  maxPrice: null,
  status: '',
  color: ''
})

const sortOptions = [
  { value: 'newest', label: 'По новизне', field: 'createdAt', order: 'desc' },
  { value: 'price_asc', label: 'По цене (возр.)', field: 'price', order: 'asc' },
  { value: 'price_desc', label: 'По цене (убыв.)', field: 'price', order: 'desc' },
  { value: 'year_desc', label: 'По году (новые)', field: 'year', order: 'desc' },
  { value: 'year_asc', label: 'По году (старые)', field: 'year', order: 'asc' }
]

const selectedSort = ref('newest')

const sortedCars = computed(() => {
  const carsArray = [...cars.value]
  const sortOption = sortOptions.find(opt => opt.value === selectedSort.value)
  
  if (!sortOption) return carsArray
  
  return carsArray.sort((a, b) => {
    let aValue = a[sortOption.field]
    let bValue = b[sortOption.field]
    
    if (sortOption.field === 'createdAt') {
      aValue = new Date(aValue).getTime()
      bValue = new Date(bValue).getTime()
    }
    
    if (sortOption.order === 'asc') {
      return (aValue || 0) - (bValue || 0)
    } else {
      return (bValue || 0) - (aValue || 0)
    }
  })
})

const loadCars = async () => {
  loading.value = true
  try {
    const response = await carsApi.getCars(filters.value)
    cars.value = response.data
  } catch (error) {
    console.error('Ошибка загрузки автомобилей:', error)
  } finally {
    loading.value = false
  }
}

const applyFilters = () => {
  loadCars()
  showMobileFilters.value = false
}

const resetFilters = () => {
  filters.value = {
    brandId: null,
    modelId: null,
    minYear: null,
    maxYear: null,
    minPrice: null,
    maxPrice: null,
    status: '',
    color: ''
  }
  selectedSort.value = 'newest'
  loadCars()
}

const viewCarDetails = (carId) => {
  router.push(`/car/${carId}`)
}

onMounted(() => {
  loadCars()
})

let filterTimeout
watch(filters, () => {
  clearTimeout(filterTimeout)
  filterTimeout = setTimeout(loadCars, 500)
}, { deep: true })
</script>

<template>
  <div class="catalog-page">
    <div class="catalog-header">
      <div class="header-content">
        <h1 class="page-title">Каталог автомобилей</h1>
        <p class="page-subtitle">ТТС "ТрансТехСервис" — профессиональный подбор автотранспорта</p>
      </div>
    </div>

    <div class="catalog-layout">
      <aside class="filters-panel desktop-filters">
        <div class="filters-header">
          <h3 class="filters-title">Фильтры</h3>
          <button 
            class="clear-filters-btn"
            @click="resetFilters"
          >
            Сбросить все
          </button>
        </div>

        <div class="filters-group">
          <label class="filter-label">Цена, ₽</label>
          <div class="price-range">
            <div class="range-inputs">
              <input 
                v-model.number="filters.minPrice"
                type="number"
                placeholder="От"
                class="range-input"
              />
              <span class="range-separator">–</span>
              <input 
                v-model.number="filters.maxPrice"
                type="number"
                placeholder="До"
                class="range-input"
              />
            </div>
          </div>
        </div>

        <div class="filters-group">
          <label class="filter-label">Год выпуска</label>
          <div class="year-range">
            <div class="range-inputs">
              <input 
                v-model.number="filters.minYear"
                type="number"
                placeholder="От"
                class="range-input"
                min="1990"
                max="2024"
              />
              <span class="range-separator">–</span>
              <input 
                v-model.number="filters.maxYear"
                type="number"
                placeholder="До"
                class="range-input"
                min="1990"
                max="2024"
              />
            </div>
          </div>
        </div>

        <div class="filters-group">
          <label class="filter-label">Статус</label>
          <select 
            v-model="filters.status"
            class="filter-select"
          >
            <option value="">Все статусы</option>
            <option value="available">В наличии</option>
            <option value="sold">Продано</option>
            <option value="reserved">Забронировано</option>
          </select>
        </div>

        <div class="filters-group">
          <label class="filter-label">Цвет</label>
          <input 
            v-model="filters.color"
            type="text"
            placeholder="Например: черный"
            class="filter-input"
          />
        </div>

        <button 
          class="apply-filters-btn"
          @click="applyFilters"
          :disabled="loading"
        >
          Применить фильтры
        </button>
      </aside>

      <div class="cars-section">
        <div class="stats-bar">
          <span class="stats-text">
            Найдено: <strong>{{ cars.length }}</strong> автомобилей
          </span>
          <div class="sort-controls">
            <select v-model="selectedSort" class="sort-select">
              <option 
                v-for="option in sortOptions" 
                :key="option.value" 
                :value="option.value"
              >
                {{ option.label }}
              </option>
            </select>
          </div>
        </div>

        <div v-if="loading" class="loading-state">
          <div class="loader"></div>
          <p>Загружаем автомобили...</p>
        </div>

        <div v-else-if="sortedCars.length > 0" class="cars-grid">
          <CarCard 
            v-for="car in sortedCars" 
            :key="car.id" 
            :car="car"
            @click="viewCarDetails(car.id)"
            class="car-card-item"
          />
        </div>

        <div v-else class="empty-state">
          <div class="empty-icon">
            <svg width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
              <path d="M16 3H1V20H23V8M16 3L23 8M16 3V8H23M5 14H10M5 17H19" stroke-linecap="round" stroke-linejoin="round"/>
            </svg>
          </div>
          <h3>Автомобили не найдены</h3>
          <p>Попробуйте изменить параметры фильтрации</p>
          <button 
            class="reset-filters-btn"
            @click="resetFilters"
          >
            Сбросить фильтры
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.catalog-page {
  min-height: 100vh;
  margin-top: -23px;
}

.catalog-header {
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

.mobile-filter-btn {
  display: none;
}

.catalog-layout {
  max-width: 1440px;
  margin: 0 auto;
  padding: 0 1.5rem 2rem;
  display: grid;
  grid-template-columns: 300px 1fr;
  gap: 2rem;
}

.filters-panel {
  background: var(--tts-white);
  border-radius: var(--tts-radius-lg);
  padding: 1.5rem;
  box-shadow: var(--tts-shadow-md);
  height: fit-content;
  position: sticky;
  top: calc(var(--tts-navbar-height) + 1.5rem);
  border: 1px solid var(--tts-gray-200);
}

.filters-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid var(--tts-gray-200);
}

.filters-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: var(--tts-gray-800);
}

.clear-filters-btn {
  background: none;
  border: none;
  color: var(--tts-error);
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  padding: 0.25rem 0.5rem;
  border-radius: var(--tts-radius-sm);
  transition: background-color 0.2s;
}

.clear-filters-btn:hover {
  background: rgba(239, 68, 68, 0.1);
}

.filters-group {
  margin-bottom: 1.5rem;
}

.filter-label {
  display: block;
  font-size: 0.875rem;
  font-weight: 500;
  color: var(--tts-gray-700);
  margin-bottom: 0.5rem;
}

.range-inputs {
  display: grid;
  grid-template-columns: 1fr auto 1fr;
  gap: 0.5rem;
  align-items: center;
}

.range-input {
  width: 100%;
  padding: 0.625rem 0.5rem;
  border: 1px solid var(--tts-gray-300);
  border-radius: var(--tts-radius);
  font-size: 0.875rem;
  background: var(--tts-white);
  color: var(--tts-gray-800);
  transition: all 0.2s;
  box-sizing: border-box;
}

.range-input:focus {
  border-color: var(--tts-primary);
  box-shadow: 0 0 0 3px rgba(26, 86, 219, 0.1);
  outline: none;
}

.range-input::placeholder {
  color: var(--tts-gray-400);
}

.range-separator {
  color: var(--tts-gray-500);
  font-weight: 500;
  text-align: center;
  padding: 0 0.25rem;
}

.filter-select,
.filter-input {
  width: 100%;
  padding: 0.625rem 0.75rem;
  border: 1px solid var(--tts-gray-300);
  border-radius: var(--tts-radius);
  font-size: 0.875rem;
  background: var(--tts-white);
  color: var(--tts-gray-800);
  transition: all 0.2s;
  box-sizing: border-box;
}

.filter-select:focus,
.filter-input:focus {
  border-color: var(--tts-primary);
  box-shadow: 0 0 0 3px rgba(26, 86, 219, 0.1);
  outline: none;
}

.filter-select::placeholder,
.filter-input::placeholder {
  color: var(--tts-gray-400);
}

.apply-filters-btn {
  width: 100%;
  padding: 0.875rem 1rem;
  background: var(--tts-primary);
  color: white;
  border: none;
  border-radius: var(--tts-radius);
  font-weight: 600;
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.2s;
  margin-top: 0.5rem;
}

.apply-filters-btn:hover:not(:disabled) {
  background: var(--tts-primary-dark);
  transform: translateY(-1px);
  box-shadow: var(--tts-shadow-md);
}

.apply-filters-btn:disabled {
  background: var(--tts-gray-400);
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

.cars-section {
  min-height: 500px;
}

.stats-bar {
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

.stats-text {
  color: var(--tts-gray-700);
  font-size: 0.875rem;
}

.stats-text strong {
  color: var(--tts-gray-900);
  font-weight: 600;
}

.sort-controls {
  display: flex;
  align-items: center;
}

.sort-select {
  padding: 0.5rem 1rem;
  border: 1px solid var(--tts-gray-300);
  border-radius: var(--tts-radius);
  font-size: 0.875rem;
  background: var(--tts-white);
  color: var(--tts-gray-700);
  min-width: 200px;
  transition: all 0.2s;
}

.sort-select:focus {
  border-color: var(--tts-primary);
  box-shadow: 0 0 0 3px rgba(26, 86, 219, 0.1);
  outline: none;
}

.cars-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
}

.car-card-item {
  transition: transform 0.3s ease;
}

.car-card-item:hover {
  transform: translateY(-4px);
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

.empty-state {
  text-align: center;
  padding: 4rem 2rem;
  background: var(--tts-white);
  border-radius: var(--tts-radius-lg);
  box-shadow: var(--tts-shadow);
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

.reset-filters-btn {
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

.reset-filters-btn:hover {
  background: var(--tts-primary-dark);
  box-shadow: var(--tts-shadow-md);
}

@media (max-width: 1024px) {
  .catalog-layout {
    grid-template-columns: 1fr;
  }
  
  .desktop-filters {
    display: none;
  }
  
  .mobile-filter-btn {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 0.5rem;
    position: absolute;
    right: 1.5rem;
    top: 50%;
    transform: translateY(-50%);
    padding: 0.75rem 1.25rem;
    background: rgba(255, 255, 255, 0.1);
    color: white;
    border: 1px solid rgba(255, 255, 255, 0.2);
    border-radius: var(--tts-radius);
    font-weight: 500;
    cursor: pointer;
    backdrop-filter: blur(10px);
  }
  
  .mobile-filter-btn .badge {
    position: absolute;
    top: -4px;
    right: -4px;
    color: var(--tts-warning);
    font-size: 1.25rem;
  }
}

@media (max-width: 768px) {
  .catalog-header {
    padding: 1.5rem 1rem;
  }
  
  .page-title {
    font-size: 1.75rem;
  }
  
  .catalog-layout {
    padding: 0 1rem 1.5rem;
  }
  
  .cars-grid {
    grid-template-columns: 1fr;
  }
  
  .stats-bar {
    flex-direction: column;
    gap: 1rem;
    align-items: stretch;
    padding: 1rem;
  }
  
  .sort-select {
    width: 100%;
  }
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
</style>