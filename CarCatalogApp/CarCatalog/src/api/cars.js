import api from './index'

export const carsApi = {
  getCars(filters = {}) {
    const params = new URLSearchParams()
    Object.keys(filters).forEach(key => {
      if (filters[key] !== null && filters[key] !== undefined) {
        params.append(key, filters[key])
      }
    })
    return api.get('/cars', { params })
  },
  getCar(id) {
    return api.get(`/cars/${id}`)
  },
  createCar(carData) {
    return api.post('/cars', carData)
  },
  updateCar(id, carData) {
    return api.put(`/cars/${id}`, carData)
  },
  deleteCar(id) {
    return api.delete(`/cars/${id}`)
  }
}