<script setup>
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import LogoIcon from '@/components/icons/LogoIcon.vue'
import { computed } from 'vue'

const router = useRouter()
const authStore = useAuthStore()

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
}

const goToProfile = () => {
  router.push('/profile')
}

// Получаем текущий путь для активного состояния
const currentPath = computed(() => router.currentRoute.value.path)
</script>

<template>
  <nav class="navbar">
    <div class="navbar-container">
      <router-link to="/" class="navbar-brand">
        <LogoIcon />
        <div class="brand-text">
          <span class="brand-name">ТТС</span>
          <span class="brand-subtitle">ТрансТехСервис</span>
        </div>
      </router-link>
      
      <div class="navbar-menu">
        <template v-if="authStore.isAuthenticated">
          <div class="nav-links">
            <router-link to="/" class="nav-link" :class="{ 'router-link-active': currentPath === '/' }">
              Каталог
            </router-link>
            
            <router-link 
              v-if="authStore.isAdmin" 
              to="/admin" 
              class="nav-link admin-link"
              :class="{ 'router-link-active': currentPath === '/admin' }"
            >
              Админ-панель
            </router-link>
          </div>
          
          <div class="user-menu">
            <!-- Информация о пользователе как кликабельный блок -->
            <div 
              class="user-info-wrapper"
              :class="{ 'user-info-active': currentPath === '/profile' }"
              @click="goToProfile"
            >
              <div class="user-info-content">
                <div class="user-avatar">
                  {{ authStore.user?.email?.charAt(0).toUpperCase() }}
                </div>
                <div class="user-details">
                  <span class="user-email">{{ authStore.user?.email }}</span>
                  <span class="user-role">{{ authStore.user?.role }}</span>
                </div>
              </div>
            </div>

            <button @click="handleLogout" class="logout-btn">
              <span class="logout-icon">↪</span>
              Выйти
            </button>
          </div>
        </template>
        
        <template v-else>
          <div class="auth-links">
            <router-link to="/login" class="nav-link login-link">
              Войти
            </router-link>
            <router-link to="/register" class="nav-link register-link">
              Регистрация
            </router-link>
          </div>
        </template>
      </div>
    </div>
  </nav>
</template>

<style scoped>
.navbar {
  background: var(--tts-white);
  border-bottom: 1px solid var(--tts-gray-200);
  box-shadow: var(--tts-shadow-sm);
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 1000;
  backdrop-filter: blur(10px);
  background: rgba(255, 255, 255, 0.95);
}

.navbar-container {
  max-width: 1440px;
  margin: 0 auto;
  padding: 0 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: var(--tts-navbar-height);
}

.navbar-brand {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  text-decoration: none;
  color: var(--tts-gray-900);
}

.brand-text {
  display: flex;
  flex-direction: column;
  line-height: 1.2;
}

.brand-name {
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--tts-primary);
}

.brand-subtitle {
  font-size: 0.75rem;
  color: var(--tts-gray-500);
  letter-spacing: 0.5px;
}

.navbar-menu {
  display: flex;
  align-items: center;
  gap: 2rem;
}

.nav-links {
  display: flex;
  gap: 1rem;
}

.auth-links {
  display: flex;
  gap: 0.75rem;
}

/* Стили для навигационных кнопок */
.nav-link {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 1rem;
  text-decoration: none;
  color: var(--tts-gray-700);
  font-weight: 500;
  border-radius: var(--tts-radius);
  transition: all 0.2s;
  position: relative;
  background: rgba(26, 86, 219, 0.1);
  border: none;
  cursor: pointer;
  font-size: 0.875rem;
  font-family: inherit;
}

.nav-link:hover {
  background: var(--tts-gray-100);
  color: var(--tts-primary);
}

.nav-link.router-link-active {
  background: var(--tts-primary);
  color: var(--tts-white);
}

.nav-link.router-link-active::after {
  content: '';
  position: absolute;
  bottom: -4px;
  left: 50%;
  transform: translateX(-50%);
  width: 4px;
  height: 4px;
  background: var(--tts-primary);
  border-radius: 50%;
}

/* Специфичные стили для разных типов кнопок */
.admin-link {
  background: rgba(26, 86, 219, 0.1);
  color: var(--tts-primary);
}

.admin-link:hover {
  background: rgba(26, 86, 219, 0.2);
}

/* Стили для кнопок входа и регистрации */
.login-link {
  padding: 0.625rem 1.5rem;
}

.register-link {
  padding: 0.625rem 1.5rem;
  background: var(--tts-primary);
  color: white;
}

.register-link:hover {
  background: var(--tts-primary-dark);
}

/* Стили для блока пользователя */
.user-menu {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 0.5rem;
  margin-left: 1rem;
}

/* Обертка для информации о пользователе */
.user-info-wrapper {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  cursor: pointer;
  padding: 0.5rem 1rem;
  border-radius: var(--tts-radius);
  transition: all 0.2s;
  background: rgba(26, 86, 219, 0.1);
  position: relative;
  min-width: 200px;
}

.user-info-wrapper:hover {
  background: var(--tts-gray-100);
  color: var(--tts-primary);
}

.user-info-wrapper.user-info-active {
  background: var(--tts-primary);
  color: var(--tts-white);
}

.user-info-wrapper.user-info-active::after {
  content: '';
  position: absolute;
  bottom: -4px;
  left: 50%;
  transform: translateX(-50%);
  width: 4px;
  height: 4px;
  background: var(--tts-primary);
  border-radius: 50%;
}

.user-info-content {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  width: 100%;
}

.user-avatar {
  width: 36px;
  height: 36px;
  background: linear-gradient(135deg, var(--tts-primary) 0%, var(--tts-primary-dark) 100%);
  color: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  font-size: 0.875rem;
  flex-shrink: 0;
}

.user-info-wrapper.user-info-active .user-avatar {
  background: white;
  color: var(--tts-primary);
}

.user-details {
  display: flex;
  flex-direction: column;
  min-width: 0; /* Для правильного обрезания текста */
}

.user-email {
  font-size: 0.875rem;
  font-weight: 500;
  color: var(--tts-gray-800);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  max-width: 150px;
}

.user-info-wrapper:hover .user-email,
.user-info-wrapper.user-info-active .user-email {
  color: inherit;
}

.user-info-wrapper.user-info-active .user-email {
  color: var(--tts-white);
}

.user-role {
  font-size: 0.75rem;
  color: var(--tts-gray-500);
  text-transform: capitalize;
}

.user-info-wrapper:hover .user-role,
.user-info-wrapper.user-info-active .user-role {
  color: inherit;
}

.user-info-wrapper.user-info-active .user-role {
  color: rgba(255, 255, 255, 0.9);
}

/* Стили для кнопки выхода */
.logout-btn {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 1rem;
  background: var(--tts-gray-100);
  color: var(--tts-gray-700);
  border: none;
  border-radius: var(--tts-radius);
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  font-size: 0.875rem;
}

.logout-btn:hover {
  background: var(--tts-error);
  color: white;
}

/* Адаптивность */
@media (max-width: 1024px) {
  .navbar-container {
    padding: 0 1.5rem;
  }
  
  .navbar-menu {
    gap: 1.5rem;
  }
  
  .user-info-wrapper {
    min-width: 180px;
  }
}

@media (max-width: 768px) {
  .navbar-container {
    padding: 0 1rem;
  }
  
  .brand-subtitle {
    display: none;
  }
  
  .auth-links {
    gap: 0.5rem;
  }
  
  .nav-link {
    padding: 0.5rem 0.75rem;
    font-size: 0.875rem;
  }
  
  .user-info-wrapper {
    min-width: auto;
    padding: 0.5rem 0.75rem;
  }
  
  .user-details {
    display: none;
  }
  
  .logout-btn {
    padding: 0.5rem 0.75rem;
    font-size: 0.875rem;
  }
  
  .user-menu {
    margin-left: 0.5rem;
    padding: 0.25rem;
  }
}

@media (max-width: 640px) {
  .navbar-menu {
    gap: 0.75rem;
  }
  
  .nav-links {
    gap: 0.5rem;
  }
  
  .user-menu {
    margin-left: 0;
    gap: 0.5rem;
  }
  
  .user-info-wrapper {
    padding: 0.5rem;
  }
  
  .logout-btn span:not(.logout-icon) {
    display: none;
  }
  
  .logout-btn {
    padding: 0.5rem;
    min-width: 40px;
    justify-content: center;
  }
}

/* Анимация для кружка активного состояния */
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateX(-50%) scale(0.5);
  }
  to {
    opacity: 1;
    transform: translateX(-50%) scale(1);
  }
}

.user-info-wrapper.user-info-active::after {
  animation: fadeIn 0.2s ease-out;
}
</style>