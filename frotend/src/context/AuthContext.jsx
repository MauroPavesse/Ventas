import { createContext, useState, useContext, useEffect } from 'react';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  // Al iniciar, revisa si ya habías entrado antes (se guarda en el navegador)
  const [user, setUser] = useState(() => {
    const savedUser = localStorage.getItem('user_data');
    return savedUser ? JSON.parse(savedUser) : null;
  });

  const login = (data) => {
    setUser(data);
    localStorage.setItem('user_data', JSON.stringify(data)); // Guardamos Token y Nombre
  };

  const logout = () => {
    setUser(null);
    localStorage.removeItem('user_data');
  };

  const isAuthenticated = !!user;

  return (
    <AuthContext.Provider value={{ user, login, logout, isAuthenticated }}>
      {children}
    </AuthContext.Provider>
  );
};

// eslint-disable-next-line react-refresh/only-export-components
export const useAuth = () => useContext(AuthContext);