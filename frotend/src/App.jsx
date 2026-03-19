import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext"
import Login from './pages/Login';
import ProtectedRoute from './routes/ProtectedRoute';
import Dashboard from "./pages/Dashboard";
import Sale from "./pages/Sale";
import Products from "./pages/Products";
import Business from "./pages/Business";
import Configurations from "./pages/Configurations";
import PointOfSale from "./pages/PointOfSale";
import PaymentMethod from "./pages/PaymentMethod";
import User from "./pages/User";

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          {/* Rutas Públicas */}
          <Route path="/login" element={<Login />} />

          {/* Rutas Privadas (Protegidas)*/}
          <Route element={<ProtectedRoute />}>
            <Route path="/dashboard" element={<Dashboard />} />
            <Route path="/sale" element={<Sale />} />
            <Route path="/products" element={<Products />} />
            <Route path="/configurations" element={<Configurations />} />
            <Route path="/business" element={<Business />} />
            <Route path="/pointofsale" element={<PointOfSale />} />
            <Route path="/paymentmethod" element={<PaymentMethod />} />
            <Route path="/user" element={<User />} />
          </Route>

          {/* Redirección por defecto si la ruta no existe */}
          <Route path="*" element={<Navigate to="/login" />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  )
}

export default App
