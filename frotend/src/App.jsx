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
import Customer from "./pages/Customer";
import Category from "./pages/Category";
import { App as AntdApp } from 'antd';
import DailyBox from "./pages/DailyBox";
import VoucherPage from "./pages/VoucherPage";

function App() {
  return (
    <AntdApp>
      <AuthProvider>
        <BrowserRouter>
          <Routes>
            {/* Rutas Públicas */}
            <Route path="/login" element={<Login />} />

            {/* Rutas Privadas (Protegidas)*/}
            <Route element={<ProtectedRoute />}>
              <Route path="/dashboard" element={<Dashboard />} />
              <Route path="/sales" element={<Sale />} />
              <Route path="/products" element={<Products />} />
              <Route path="/configurations" element={<Configurations />} />
              <Route path="/business" element={<Business />} />
              <Route path="/pointofsales" element={<PointOfSale />} />
              <Route path="/paymentmethods" element={<PaymentMethod />} />
              <Route path="/users" element={<User />} />
              <Route path="/customers" element={<Customer />} />
              <Route path="/categories" element={<Category />} />
              <Route path="/daily-box" element={<DailyBox />} />
              <Route path="/vouchers" element={<VoucherPage />} />
            </Route>

            {/* Redirección por defecto si la ruta no existe */}
            <Route path="*" element={<Navigate to="/login" />} />
          </Routes>
        </BrowserRouter>
      </AuthProvider>
    </AntdApp>
  )
}

export default App
