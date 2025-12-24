import { defineStore } from 'pinia'
import { authApi } from '@/api/auth'
import { ref, computed } from 'vue'

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem('token'))
  const user = ref(JSON.parse(localStorage.getItem('user')) || null)

  const isAuthenticated = computed(() => !!token.value)
  const isAdmin = computed(() => user.value?.role === 'admin')

  const login = async (credentials) => {
    try {
      console.log('Отправка запроса на вход с данными:', credentials)
      const response = await authApi.login(credentials)
      console.log('Ответ от сервера при входе:', response.data)
      
      token.value = response.data.token
      
      user.value = {
        id: response.data.user?.id || 0,
        email: response.data.user?.email || credentials.email,
        role: response.data.user?.role || 'user'
      }
      
      localStorage.setItem('token', token.value)
      localStorage.setItem('user', JSON.stringify(user.value))
      
      console.log('Токен сохранен:', token.value)
      console.log('Пользователь сохранен:', user.value)
      
      return { success: true }
    } catch (error) {
      console.error('Ошибка входа:', error)
      return { 
        success: false, 
        message: error.response?.data?.message || 'Ошибка входа' 
      }
    }
  }

  const register = async (userData) => {
    try {
      const response = await authApi.register(userData)
      token.value = response.data.token
      user.value = {
        id: response.data.user?.id || 0,
        email: response.data.user?.email || userData.email,
        role: response.data.user?.role || 'user'
      }
      
      localStorage.setItem('token', token.value)
      localStorage.setItem('user', JSON.stringify(user.value))
      
      return { success: true }
    } catch (error) {
      return { 
        success: false, 
        message: error.response?.data?.message || 'Ошибка регистрации' 
      }
    }
  }

  const logout = () => {
    token.value = null
    user.value = null
    localStorage.removeItem('token')
    localStorage.removeItem('user')
  }

  // Метод для загрузки полного профиля
  const loadUserProfile = async () => {
    try {
      if (!token.value) {
        throw new Error('Токен отсутствует')
      }
      
      console.log('Загрузка профиля с токеном:', token.value)
      const response = await authApi.getProfile()
      const userData = response.data
      console.log('Данные профиля с сервера:', userData)
      
      // Обновляем пользователя
      user.value = {
        ...user.value,
        ...userData
      }
      
      localStorage.setItem('user', JSON.stringify(user.value))
      
      return userData
    } catch (error) {
      console.error('Ошибка загрузки профиля:', error)
      throw error
    }
  }

  // Метод для обновления пользователя
  const updateUser = (userData) => {
    if (user.value) {
      user.value = {
        ...user.value,
        ...userData
      }
      localStorage.setItem('user', JSON.stringify(user.value))
    }
  }

  return {
    token,
    user,
    isAuthenticated,
    isAdmin,
    login,
    register,
    logout,
    loadUserProfile,
    updateUser
  }
})