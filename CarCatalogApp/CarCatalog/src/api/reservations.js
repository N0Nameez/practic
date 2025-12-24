import api from './index'

export const reservationsApi = {
  createReservation(data) {
    return api.post('/reservations', data)
  },
  
  getUserReservations() {
    return api.get('/reservations/my')
  },
  
  getReservation(id) {
    return api.get(`/reservations/${id}`)
  },
  
  cancelReservation(id) {
    return api.delete(`/reservations/${id}`)
  },
  
  getCarReservations(carId) {
    return api.get(`/reservations/car/${carId}`)
  },
  
  updateReservationStatus(id, statusData) {
    return api.put(`/reservations/${id}/status`, statusData)
  }
}