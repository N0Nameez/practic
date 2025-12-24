import api from './index'

export const authApi = {
  login(credentials) {
    return api.post('/Auth/login', credentials)
  },
  register(userData) {
    return api.post('/Auth/register', userData)
  },
  getProfile() {
    return api.get('/Auth/profile')
  },
  updateProfile(profileData) {
    return api.put('/Auth/profile', profileData)
  }
}