import axios from 'axios'
 
const Api = axios.create({
  baseURL: 'http://localhost:65148',
  headers: { 'Content-Type': 'application/json' },
})
 
// Injeta JWT automaticamente em toda requisição
Api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})
 
// Redireciona para login se 401
Api.interceptors.response.use(
  (res) => res,
  (err) => {
    if (err.response?.status === 401) {
      localStorage.clear()
      window.location.href = '/login'
    }
    return Promise.reject(err)
  }
)
 
export default Api
 