import axios from 'axios'

const api = axios.create({
  baseURL: 'http://localhost:5041/api',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json'
  }
})

api.interceptors.request.use(config => {
  const token = localStorage.getItem('token')
  console.log('Интерцептор запроса. Токен из localStorage:', token)
  console.log('URL запроса:', config.url)
  
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
    console.log('Заголовок Authorization установлен:', config.headers.Authorization)
  }
  
  return config
})

api.interceptors.response.use(
  response => {
    console.log('Успешный ответ от сервера:', {
      url: response.config.url,
      status: response.status,
      data: response.data
    })
    return response
  },
  error => {
    console.error('Ошибка ответа от сервера:', {
      url: error.config?.url,
      status: error.response?.status,
      message: error.message,
      data: error.response?.data
    })
    
    if (error.response?.status === 401) {
      console.log('Получен статус 401 - неавторизован')
      console.log('Токен в localStorage:', localStorage.getItem('token'))
      
      // Не удаляем токен автоматически, пусть компоненты решают
      // localStorage.removeItem('token')
      // window.location.href = '/login'
    }
    
    return Promise.reject(error)
  }
)

export default api