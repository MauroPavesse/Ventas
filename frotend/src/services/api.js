import axios from 'axios';

const api = axios.create({
  // Vite elegirá automáticamente según el entorno
  baseURL: import.meta.env.VITE_API_URL, 
  headers: { 'Content-Type': 'application/json' },
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
    // 1. Manejo de Seguridad (401 Unauthorized)
    if (error.response && error.response.status === 401) {
      const isLoginRequest = error.config.url.includes('/login');
      if (!isLoginRequest) {
        localStorage.removeItem('user_data');
        window.location.href = '/login'; 
        return Promise.reject("Sesión expirada. Por favor, reingrese.");
      }
    }

    // 2. Extraer el mensaje del estándar Problem Details (C#)
    let friendlyMessage = "Ocurrió un error inesperado";
    
    if (error.response && error.response.data) {
      const data = error.response.data;

      // Si hay errores de validación (FluentValidation)
      if (data.errors) {
        // Convertimos el objeto de errores en una lista de strings
        // Ejemplo: { Name: ["Obligatorio"], Code: ["Invalido"] } => "Obligatorio. Invalido."
        friendlyMessage = Object.values(data.errors).flat().join(" ");
      } 
      // Si es un error de negocio o NotFound (BaseException)
      else if (data.detail) {
        friendlyMessage = data.detail;
      }
      // Si el backend mandó un título pero no un detalle
      else if (data.title) {
        friendlyMessage = data.title;
      }
    } else if (error.message === "Network Error") {
      friendlyMessage = "No se pudo conectar con el servidor.";
    }

    // Devolvemos el string para que el 'catch' haga message.error(error)
    return Promise.reject(friendlyMessage);
  }
);

export default api;