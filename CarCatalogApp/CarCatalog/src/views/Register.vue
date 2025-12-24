<script setup>
import { ref, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const loading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

const errors = ref({
  email: '',
  password: '',
  confirmPassword: ''
})

const router = useRouter()
const authStore = useAuthStore()

const isFormValid = computed(() => {
  return email.value && 
         password.value.length >= 6 && 
         password.value === confirmPassword.value
})

const validateEmail = () => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  if (!email.value) {
    errors.value.email = 'Email обязателен'
  } else if (!emailRegex.test(email.value)) {
    errors.value.email = 'Введите корректный email'
  } else {
    errors.value.email = ''
  }
}

const validatePassword = () => {
  if (!password.value) {
    errors.value.password = 'Пароль обязателен'
  } else if (password.value.length < 6) {
    errors.value.password = 'Пароль должен быть минимум 6 символов'
  } else {
    errors.value.password = ''
  }
}

const validateConfirmPassword = () => {
  if (!confirmPassword.value) {
    errors.value.confirmPassword = 'Подтверждение пароля обязательно'
  } else if (password.value !== confirmPassword.value) {
    errors.value.confirmPassword = 'Пароли не совпадают'
  } else {
    errors.value.confirmPassword = ''
  }
}

watch(email, validateEmail)
watch(password, () => {
  validatePassword()
  validateConfirmPassword()
})
watch(confirmPassword, validateConfirmPassword)

const handleRegister = async () => {
  errorMessage.value = ''
  successMessage.value = ''
  
  validateEmail()
  validatePassword()
  validateConfirmPassword()
  
  if (Object.values(errors.value).some(error => error)) {
    errorMessage.value = 'Исправьте ошибки в форме'
    return
  }
  
  loading.value = true
  
  try {
    const result = await authStore.register({
      email: email.value,
      password: password.value,
      confirmPassword: confirmPassword.value
    })
    
    if (result.success) {
      successMessage.value = 'Регистрация прошла успешно!'
      
      setTimeout(() => {
        router.push('/')
      }, 2000)
    } else {
      errorMessage.value = result.message
    }
  } catch (error) {
    errorMessage.value = 'Ошибка регистрации. Попробуйте позже.'
    console.error('Register error:', error)
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="auth-page">
    <div class="auth-container">
      <div class="auth-header">
        <div class="brand-logo">
          <span class="brand-name">ТТС</span>
          <span class="brand-subtitle">ТрансТехСервис</span>
        </div>
        <h1 class="auth-title">Регистрация</h1>
        <p class="auth-subtitle">Создайте новый аккаунт</p>
      </div>
      
      <div class="auth-card">
        <form @submit.prevent="handleRegister" class="auth-form">
          <div class="form-group">
            <label class="form-label">
              Email
              <span class="required-mark">*</span>
            </label>
            <input 
              v-model="email" 
              type="email" 
              required 
              placeholder="user@example.com"
              class="form-input"
              :class="{ 'error': errors.email }"
            />
            <div v-if="errors.email" class="error-text">{{ errors.email }}</div>
          </div>
          
          <div class="form-group">
            <label class="form-label">
              Пароль
              <span class="required-mark">*</span>
            </label>
            <input 
              v-model="password" 
              type="password" 
              required 
              placeholder="Минимум 6 символов"
              class="form-input"
              :class="{ 'error': errors.password }"
            />
            <div v-if="errors.password" class="error-text">{{ errors.password }}</div>
          </div>
          
          <div class="form-group">
            <label class="form-label">
              Подтверждение пароля
              <span class="required-mark">*</span>
            </label>
            <input 
              v-model="confirmPassword" 
              type="password" 
              required 
              placeholder="Повторите пароль"
              class="form-input"
              :class="{ 'error': errors.confirmPassword }"
            />
            <div v-if="errors.confirmPassword" class="error-text">{{ errors.confirmPassword }}</div>
          </div>
          
          <button 
            type="submit" 
            :disabled="loading || !isFormValid" 
            class="auth-btn"
          >
            <span v-if="loading" class="loading-spinner"></span>
            {{ loading ? 'Регистрация...' : 'Зарегистрироваться' }}
          </button>
        </form>
        
        <div v-if="errorMessage" class="error-message">
          {{ errorMessage }}
        </div>
        
        <div v-if="successMessage" class="success-message">
          {{ successMessage }}
          <p>Перенаправление на главную страницу...</p>
        </div>
        
        <div class="auth-footer">
          <p class="login-link">
            Уже есть аккаунт? <router-link to="/login" class="link">Войти</router-link>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.auth-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--tts-gray-50);
  padding: 1.5rem;
}

.auth-container {
  width: 100%;
  max-width: 420px;
}

.auth-header {
  text-align: center;
  margin-bottom: 2.5rem;
}

.brand-logo {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.25rem;
  margin-bottom: 1.5rem;
}

.brand-name {
  font-size: 2rem;
  font-weight: 700;
  color: var(--tts-primary);
}

.brand-subtitle {
  font-size: 0.875rem;
  color: var(--tts-gray-500);
  letter-spacing: 0.5px;
}

.auth-title {
  font-size: 1.75rem;
  font-weight: 600;
  color: var(--tts-gray-900);
  margin-bottom: 0.5rem;
}

.auth-subtitle {
  font-size: 0.875rem;
  color: var(--tts-gray-600);
}

.auth-card {
  background: var(--tts-white);
  border-radius: var(--tts-radius-lg);
  box-shadow: var(--tts-shadow-lg);
  padding: 2.5rem;
  border: 1px solid var(--tts-gray-200);
}

.auth-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  margin-bottom: 1.5rem;
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
  font-size: 0.875em;
}

.form-input {
  width: 100%;
  padding: 0.875rem 1rem;
  border: 1px solid var(--tts-gray-300);
  border-radius: var(--tts-radius);
  font-size: 0.875rem;
  color: var(--tts-gray-800);
  background: var(--tts-white);
  transition: all 0.2s;
}

.form-input:focus {
  outline: none;
  border-color: var(--tts-primary);
  box-shadow: 0 0 0 3px rgba(26, 86, 219, 0.1);
}

.form-input.error {
  border-color: var(--tts-error);
}

.form-input::placeholder {
  color: var(--tts-gray-400);
}

.error-text {
  font-size: 0.75rem;
  color: var(--tts-error);
  margin-top: 0.25rem;
}

.auth-btn {
  width: 100%;
  padding: 0.875rem 1rem;
  background: var(--tts-primary);
  color: white;
  border: none;
  border-radius: var(--tts-radius);
  font-weight: 500;
  font-size: 0.875rem;
  cursor: pointer;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.auth-btn:hover:not(:disabled) {
  background: var(--tts-primary-dark);
  box-shadow: var(--tts-shadow-md);
}

.auth-btn:disabled {
  background: var(--tts-gray-400);
  cursor: not-allowed;
  box-shadow: none;
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
  padding: 1rem;
  background: rgba(239, 68, 68, 0.1);
  border: 1px solid rgba(239, 68, 68, 0.2);
  border-radius: var(--tts-radius);
  color: var(--tts-error);
  font-size: 0.875rem;
  text-align: center;
  margin-bottom: 1.5rem;
}

.success-message {
  padding: 1rem;
  background: rgba(16, 185, 129, 0.1);
  border: 1px solid rgba(16, 185, 129, 0.2);
  border-radius: var(--tts-radius);
  color: var(--tts-success);
  font-size: 0.875rem;
  text-align: center;
  margin-bottom: 1.5rem;
}

.success-message p {
  margin-top: 0.5rem;
  font-size: 0.75rem;
  opacity: 0.8;
}

.auth-footer {
  padding-top: 1.5rem;
  border-top: 1px solid var(--tts-gray-200);
  text-align: center;
}

.login-link {
  font-size: 0.875rem;
  color: var(--tts-gray-600);
  margin: 0;
}

.link {
  color: var(--tts-primary);
  font-weight: 500;
  text-decoration: none;
}

.link:hover {
  text-decoration: underline;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
</style>