import axios from 'axios';

const api = axios.create({
  //baseURL: 'http://72.60.60.66:5000/api',
  baseURL: 'https://localhost:7189/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use(
  (config) => {
    const userData = localStorage.getItem('user_data');
    if (userData) {
      const { token } = JSON.parse(userData);
      // Inyectamos el token con el formato Bearer
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

api.interceptors.response.use(
  (response) => response,
  (error) => {
    // 1. Verificamos si es un 401
    if (error.response && error.response.status === 401) {
      
      // 2. IMPORTANTE: Si la URL incluye "login", NO redirecciones. 
      // Deja que el componente Login maneje el error.
      const isLoginRequest = error.config.url.includes('/login');

      if (!isLoginRequest) {
        localStorage.removeItem('user_data');
        window.location.href = '/login'; 
      }
    }
    
    // Devolvemos el error para que el 'catch' del componente pueda leerlo
    return Promise.reject(error);
  }
);

export default api;