<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const email = ref('test@test.com')
const password = ref('qweqwe')
const loading = ref(false)
const error = ref('')

const router = useRouter()
const authStore = useAuthStore()

const handleLogin = async () => {
  loading.value = true
  error.value = ''
  
  console.log('Начало процесса входа...')
  
  try {
    const result = await authStore.login({
      email: email.value,
      password: password.value
    })
    
    if (result.success) {
      console.log('Логин успешен, загружаю профиль...')
      
      // После успешного входа загружаем полный профиль
      try {
        await authStore.loadUserProfile()
        console.log('Профиль успешно загружен')
      } catch (profileError) {
        console.error('Ошибка загрузки профиля:', profileError)
        // Не прерываем вход, если профиль не загрузился
      }
      
      router.push('/')
    } else {
      error.value = result.message || 'Неизвестная ошибка'
    }
  } catch (err) {
    console.error('Ошибка при входе:', err)
    error.value = err.response?.data?.message || 'Ошибка сервера'
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
        <h1 class="auth-title">Вход в систему</h1>
        <p class="auth-subtitle">Введите ваши учетные данные для входа</p>
      </div>
      
      <div class="auth-card">
        <form @submit.prevent="handleLogin" class="auth-form">
          <div class="form-group">
            <label class="form-label">Email</label>
            <input 
              v-model="email" 
              type="email" 
              required 
              placeholder="admin@example.com"
              class="form-input"
              :class="{ 'error': error }"
            />
          </div>
          
          <div class="form-group">
            <label class="form-label">Пароль</label>
            <input 
              v-model="password" 
              type="password" 
              required 
              placeholder="Введите пароль"
              class="form-input"
              :class="{ 'error': error }"
            />
          </div>
          
          <button 
            type="submit" 
            :disabled="loading" 
            class="auth-btn"
          >
            <span v-if="loading" class="loading-spinner"></span>
            {{ loading ? 'Вход...' : 'Войти' }}
          </button>
        </form>
        
        <div v-if="error" class="error-message">
          {{ error }}
        </div>
        
        <div class="auth-footer">
          <p class="register-link">
            Нет аккаунта? <router-link to="/register" class="link">Зарегистрироваться</router-link>
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

.auth-footer {
  padding-top: 1.5rem;
  border-top: 1px solid var(--tts-gray-200);
  text-align: center;
}

.register-link {
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