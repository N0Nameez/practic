<template>
  <div class="car-form-container">
    <div class="form-header">
      <h2 class="form-title">{{ formTitle }}</h2>
      <p class="form-subtitle">Заполните информацию об автомобиле</p>
    </div>
    
    <form @submit.prevent="handleSubmit" class="car-form">
      <div class="form-grid">
        <div class="form-section">
          <h3 class="section-title">Основная информация</h3>
          
          <div class="form-group">
            <label class="form-label">
              Марка
              <span class="required-mark">*</span>
            </label>
            <select 
              v-model="formData.brandId" 
              @change="loadModels"
              class="form-select"
              :class="{ 'error': errors.brandId }"
              required
            >
              <option value="">Выберите марку</option>
              <option v-for="brand in brands" :key="brand.id" :value="brand.id">
                {{ brand.name }}
              </option>
            </select>
            <div v-if="errors.brandId" class="error-message">
              {{ errors.brandId }}
            </div>
          </div>
          
          <div class="form-group">
            <label class="form-label">
              Модель
              <span class="required-mark">*</span>
            </label>
            <select 
              v-model="formData.modelId" 
              :disabled="!formData.brandId"
              class="form-select"
              :class="{ 'error': errors.modelId }"
              required
            >
              <option value="">Выберите модель</option>
              <option v-for="model in models" :key="model.id" :value="model.id">
                {{ model.name }}
              </option>
            </select>
            <div v-if="errors.modelId" class="error-message">
              {{ errors.modelId }}
            </div>
          </div>
          
          <div class="form-row">
            <div class="form-group">
              <label class="form-label">
                Год выпуска
                <span class="required-mark">*</span>
              </label>
              <input 
                v-model="formData.year" 
                type="number" 
                min="2000" 
                max="2024"
                class="form-input"
                :class="{ 'error': errors.year }"
                placeholder="2024"
                required
              />
              <div v-if="errors.year" class="error-message">
                {{ errors.year }}
              </div>
            </div>
            
            <div class="form-group">
              <label class="form-label">
                Цвет
                <span class="required-mark">*</span>
              </label>
              <input 
                v-model="formData.color" 
                type="text"
                class="form-input"
                :class="{ 'error': errors.color }"
                placeholder="Черный"
                required
              />
              <div v-if="errors.color" class="error-message">
                {{ errors.color }}
              </div>
            </div>
          </div>
          
          <div class="form-group">
            <label class="form-label">VIN номер</label>
            <input 
              v-model="formData.vin" 
              type="text"
              maxlength="17"
              class="form-input"
              placeholder="1HGBH41JXMN109186"
            />
          </div>
        </div>
        
        <div class="form-section">
          <h3 class="section-title">Технические характеристики</h3>
          
          <div class="form-row">
            <div class="form-group">
              <label class="form-label">
                Цена, ₽
                <span class="required-mark">*</span>
              </label>
              <div class="input-with-suffix">
                <input 
                  v-model="formData.price" 
                  type="number" 
                  min="0" 
                  step="1000"
                  class="form-input"
                  :class="{ 'error': errors.price }"
                  placeholder="1500000"
                  required
                />
                <span class="input-suffix">₽</span>
              </div>
              <div v-if="errors.price" class="error-message">
                {{ errors.price }}
              </div>
            </div>
            
            <div class="form-group">
              <label class="form-label">
                Пробег, км
                <span class="required-mark">*</span>
              </label>
              <div class="input-with-suffix">
                <input 
                  v-model="formData.mileage" 
                  type="number" 
                  min="0" 
                  step="1000"
                  class="form-input"
                  :class="{ 'error': errors.mileage }"
                  placeholder="50000"
                  required
                />
                <span class="input-suffix">км</span>
              </div>
              <div v-if="errors.mileage" class="error-message">
                {{ errors.mileage }}
              </div>
            </div>
          </div>
          
          <div class="form-group">
            <label class="form-label">
              Статус
              <span class="required-mark">*</span>
            </label>
            <div class="status-selector">
              <button 
                type="button"
                v-for="statusOption in statusOptions"
                :key="statusOption.value"
                @click="formData.status = statusOption.value"
                :class="['status-option', { 'active': formData.status === statusOption.value }]"
                :style="{ '--status-color': statusOption.color }"
              >
                <span class="status-dot"></span>
                {{ statusOption.label }}
              </button>
            </div>
            <div v-if="errors.status" class="error-message">
              {{ errors.status }}
            </div>
          </div>
          
          <div class="form-group">
            <label class="form-label">URL фотографии</label>
            <div class="image-upload">
              <input 
                v-model="formData.photoUrl" 
                type="url"
                class="form-input"
                placeholder="https://example.com/photo.jpg"
              />
              <div v-if="formData.photoUrl" class="image-preview">
                <img 
                  :src="formData.photoUrl" 
                  alt="Превью"
                  @error="handleImageError"
                />
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <div class="form-section full-width">
        <h3 class="section-title">Описание</h3>
        <div class="form-group">
          <textarea 
            v-model="formData.description" 
            rows="4"
            class="form-textarea"
            placeholder="Подробное описание автомобиля, особенности, комплектация..."
          ></textarea>
        </div>
      </div>
      
      <div class="form-actions">
        <button 
          type="button" 
          @click="$emit('cancel')"
          class="btn-secondary"
        >
          Отмена
        </button>
        
        <button 
          type="submit" 
          :disabled="loading || !isFormValid"
          class="btn-primary"
        >
          <span v-if="loading" class="loading-spinner"></span>
          {{ submitText }}
        </button>
      </div>
    </form>
  </div>
</template>

<script setup>
import { carsApi } from '@/api/cars'
import { ref, reactive, computed, onMounted, watch } from 'vue'

const props = defineProps({
  carId: {
    type: Number,
    default: null
  },
  initialData: {
    type: Object,
    default: () => ({})
  }
})

const emit = defineEmits(['car-added', 'car-updated', 'cancel'])

const formData = reactive({
  brandId: '',
  modelId: '',
  year: new Date().getFullYear(),
  color: '',
  vin: '', // Добавлено поле VIN
  price: '',
  mileage: '',
  status: 'available',
  description: '',
  photoUrl: ''
})

const brands = ref([])
const models = ref([])
const loading = ref(false)

const errors = reactive({
  brandId: '',
  modelId: '',
  year: '',
  color: '',
  price: '',
  mileage: '',
  status: ''
})

const statusOptions = [
  { value: 'available', label: 'В наличии', color: 'var(--tts-success)' },
  { value: 'reserved', label: 'Забронировано', color: 'var(--tts-warning)' },
  { value: 'sold', label: 'Продано', color: 'var(--tts-gray-500)' }
]

const formTitle = computed(() => {
  return props.carId ? 'Редактировать автомобиль' : 'Добавить новый автомобиль'
})

const submitText = computed(() => {
  return props.carId ? 'Сохранить изменения' : 'Добавить автомобиль'
})

const isFormValid = computed(() => {
  return formData.brandId && 
         formData.modelId && 
         formData.year && 
         formData.color && 
         formData.price && 
         formData.mileage && 
         formData.status
})

const loadBrands = async () => {
  try {
    brands.value = [
      { id: 1, name: 'Toyota' },
      { id: 2, name: 'BMW' },
      { id: 3, name: 'Ford' }
    ]
    
    if (props.initialData && Object.keys(props.initialData).length > 0) {
      await processInitialData()
    }
  } catch (error) {
    console.error('Ошибка загрузки брендов:', error)
  }
}

const loadModels = async () => {
  if (!formData.brandId) {
    models.value = []
    formData.modelId = ''
    return
  }
  
  try {
        const brandModels = {
          1: [
            { id: 1, name: 'Camry' }
          ],
          2: [
            { id: 2, name: 'X5' }
          ],
          3: [
            { id: 3, name: 'Focus' }
          ]
        }
    
    models.value = brandModels[formData.brandId] || []
    
    if (props.initialData && props.initialData.modelName && models.value.length > 0) {
      const foundModel = models.value.find(model => model.name === props.initialData.modelName)
      if (foundModel) {
        formData.modelId = foundModel.id
      }
    }
  } catch (error) {
    console.error('Ошибка загрузки моделей:', error)
  }
}

const processInitialData = async () => {
  const data = props.initialData
  
  Object.keys(formData).forEach(key => {
    if (key in data && data[key] !== undefined && data[key] !== null) {
      formData[key] = data[key]
    }
  })
  
  if (data.year) {
    formData.year = data.year
  }
  
  if (data.brandName && brands.value.length > 0) {
    const foundBrand = brands.value.find(brand => brand.name === data.brandName)
    if (foundBrand) {
      formData.brandId = foundBrand.id
      await loadModels()
    }
  }
  
  if (data.modelName && models.value.length > 0) {
    const foundModel = models.value.find(model => model.name === data.modelName)
    if (foundModel) {
      formData.modelId = foundModel.id
    }
  }
}

const validateForm = () => {
  let isValid = true
  
  Object.keys(errors).forEach(key => errors[key] = '')
  
  if (!formData.brandId) {
    errors.brandId = 'Выберите марку автомобиля'
    isValid = false
  }
  
  if (!formData.modelId) {
    errors.modelId = 'Выберите модель автомобиля'
    isValid = false
  }
  
  const currentYear = new Date().getFullYear()
  if (!formData.year || formData.year < 2000 || formData.year > currentYear) {
    errors.year = `Введите корректный год (2000-${currentYear})`
    isValid = false
  }
  
  if (!formData.color || formData.color.trim().length < 2) {
    errors.color = 'Введите цвет автомобиля'
    isValid = false
  }
  
  if (!formData.price || formData.price <= 0) {
    errors.price = 'Введите корректную цену'
    isValid = false
  }
  
  if (!formData.mileage || formData.mileage < 0) {
    errors.mileage = 'Введите корректный пробег'
    isValid = false
  }
  
  if (!formData.status) {
    errors.status = 'Выберите статус автомобиля'
    isValid = false
  }
  
  return isValid
}

const handleSubmit = async () => {
  if (!validateForm()) return

  loading.value = true

  try {
    // Формируем объект для отправки БЕЗ поля brandId
    const carData = {
      modelId: formData.modelId, // Бэкенду нужно только это
      vin: formData.vin || null,
      year: parseInt(formData.year),
      color: formData.color.trim(),
      price: parseFloat(formData.price),
      mileage: parseInt(formData.mileage),
      status: formData.status,
      description: formData.description.trim() || '',
      photoUrl: formData.photoUrl.trim() || ''
    }

    // Логируем для отладки (можно убрать потом)
    console.log('Отправляемые данные (исправленные):', carData)

    if (props.carId) {
      const response = await carsApi.updateCar(props.carId, carData)
      console.log('Автомобиль обновлен:', response.data)
      emit('car-updated')
    } else {
      const response = await carsApi.createCar(carData)
      console.log('Автомобиль создан:', response.data)
      emit('car-added')
      resetForm()
    }
  } catch (error) {
    console.error('Ошибка сохранения автомобиля:', error)
    // Здесь можно добавить уведомление для пользователя
  } finally {
    loading.value = false
  }
}

const resetForm = () => {
  Object.keys(formData).forEach(key => {
    if (key === 'year') {
      formData[key] = new Date().getFullYear()
    } else if (key === 'status') {
      formData[key] = 'available'
    } else {
      formData[key] = ''
    }
  })
}

const handleImageError = (event) => {
  event.target.src = 'https://images.unsplash.com/photo-1553440569-bcc63803a83d?w=800&auto=format&fit=crop'
}

onMounted(() => {
  loadBrands()
})

// Следим за изменениями initialData
watch(() => props.initialData, (newData) => {
  if (newData && Object.keys(newData).length > 0 && brands.value.length > 0) {
    processInitialData()
  }
}, { immediate: true, deep: true })
</script>

<style scoped>
/* Стили остаются без изменений */
.car-form-container {
  background: var(--tts-white);
  border-radius: var(--tts-radius-lg);
  overflow: hidden;
  border: 1px solid var(--tts-gray-200);
  box-shadow: var(--tts-shadow);
}

.form-header {
  padding: 2rem 2rem 0;
}

.form-title {
  font-size: 1.75rem;
  font-weight: 600;
  color: var(--tts-gray-900);
  margin-bottom: 0.5rem;
}

.form-subtitle {
  color: var(--tts-gray-600);
  font-size: 0.875rem;
}

.car-form {
  padding: 2rem;
}

.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
  margin-bottom: 2rem;
}

.form-section {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-section.full-width {
  grid-column: 1 / -1;
}

.section-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: var(--tts-gray-800);
  margin-bottom: 0.5rem;
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

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.form-input,
.form-select,
.form-textarea {
  padding: 0.75rem;
  border: 1px solid var(--tts-gray-300);
  border-radius: var(--tts-radius);
  font-size: 0.875rem;
  color: var(--tts-gray-800);
  background: var(--tts-white);
  transition: all 0.2s;
  width: 100%;
}

.form-input:focus,
.form-select:focus,
.form-textarea:focus {
  outline: none;
  border-color: var(--tts-primary);
  box-shadow: 0 0 0 3px rgba(26, 86, 219, 0.1);
}

.form-input.error,
.form-select.error {
  border-color: var(--tts-error);
}

.form-input::placeholder {
  color: var(--tts-gray-400);
}

.input-with-suffix {
  position: relative;
}

.input-suffix {
  position: absolute;
  right: 0.75rem;
  top: 50%;
  transform: translateY(-50%);
  color: var(--tts-gray-500);
  font-size: 0.875rem;
}

.input-with-suffix .form-input {
  padding-right: 3rem;
}

.status-selector {
  display: flex;
  gap: 0.5rem;
}

.status-option {
  flex: 1;
  padding: 0.75rem;
  border: 1px solid var(--tts-gray-300);
  background: var(--tts-white);
  border-radius: var(--tts-radius);
  font-size: 0.875rem;
  color: var(--tts-gray-700);
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  transition: all 0.2s;
}

.status-option:hover {
  border-color: var(--tts-gray-400);
}

.status-option.active {
  border-color: var(--status-color);
  background: color-mix(in srgb, var(--status-color) 10%, transparent);
  color: var(--status-color);
}

.status-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: var(--status-color);
}

.image-upload {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.image-preview {
  width: 100%;
  max-width: 300px;
  border-radius: var(--tts-radius);
  overflow: hidden;
  border: 1px solid var(--tts-gray-200);
}

.image-preview img {
  width: 100%;
  height: auto;
  display: block;
}

.form-textarea {
  resize: vertical;
  min-height: 120px;
  line-height: 1.5;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  padding-top: 2rem;
  border-top: 1px solid var(--tts-gray-200);
  margin-top: 2rem;
}

.btn-primary,
.btn-secondary {
  padding: 0.75rem 2rem;
  border: none;
  border-radius: var(--tts-radius);
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.btn-primary {
  background: var(--tts-primary);
  color: var(--tts-white);
}

.btn-primary:hover:not(:disabled) {
  background: var(--tts-primary-dark);
  transform: translateY(-1px);
  box-shadow: var(--tts-shadow-md);
}

.btn-primary:disabled {
  background: var(--tts-gray-400);
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

.btn-secondary {
  background: var(--tts-gray-100);
  color: var(--tts-gray-700);
  border: 1px solid var(--tts-gray-300);
}

.btn-secondary:hover {
  background: var(--tts-gray-200);
  border-color: var(--tts-gray-400);
}

.loading-spinner {
  width: 16px;
  height: 16px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

.error-message {
  font-size: 0.75rem;
  color: var(--tts-error);
  margin-top: 0.25rem;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

@media (max-width: 1024px) {
  .form-grid {
    grid-template-columns: 1fr;
    gap: 1.5rem;
  }
}

@media (max-width: 640px) {
  .car-form {
    padding: 1.5rem;
  }
  
  .form-row {
    grid-template-columns: 1fr;
    gap: 1rem;
  }
  
  .status-selector {
    flex-direction: column;
  }
  
  .form-actions {
    flex-direction: column;
  }
  
  .btn-primary,
  .btn-secondary {
    width: 100%;
  }
}
</style>