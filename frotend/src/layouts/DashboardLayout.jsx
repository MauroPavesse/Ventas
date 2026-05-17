import React, { useState } from 'react';
import { Layout, Menu, Button, Typography, ConfigProvider, Drawer, Grid } from 'antd';
import {
  ShoppingOutlined,
  CalculatorOutlined,
  FileTextOutlined,
  InboxOutlined,
  SettingOutlined,
  LogoutOutlined,
  MenuFoldOutlined,
  MenuUnfoldOutlined,
  UsergroupAddOutlined,
  HomeOutlined,
  MenuOutlined
} from '@ant-design/icons';
import { useAuth } from '../context/AuthContext';
import { useNavigate, useLocation } from 'react-router-dom';

const { Sider, Content } = Layout;
const { Title } = Typography;
const { useBreakpoint } = Grid; // Hook de ANTD para detectar tamaños de pantalla

const DashboardLayout = ({ children }) => {
  const [collapsed, setCollapsed] = useState(false);
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false); // Estado para el menú de celular
  const { logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const screens = useBreakpoint();

  // Si screens.md es falso, significa que estamos en un celular/tablet pequeña
  const isMobile = screens.md === false;

  // Ítems del menú superior
  const mainItems = [
    { key: 'dashboard', icon: <HomeOutlined />, label: 'Inicio' },
    { key: 'sales', icon: <ShoppingOutlined />, label: 'Venta' },
    { key: 'daily-box', icon: <CalculatorOutlined />, label: 'Caja Diaria' },
    { key: 'vouchers', icon: <FileTextOutlined />, label: 'Comprobantes' },
    { key: 'products', icon: <InboxOutlined />, label: 'Productos' },
    { key: 'customers', icon: <UsergroupAddOutlined />, label: 'Clientes' },
  ];

  // Ítems del menú inferior (Ajustes y Logout)
  const footerItems = [
    { key: 'configurations', icon: <SettingOutlined />, label: 'Ajustes' },
    {
      key: 'logout',
      icon: <LogoutOutlined />,
      label: 'Cerrar Sesión',
      danger: true,
      onClick: logout
    },
  ];

  // Lógica para extraer la llave activa basada en la URL actual
  const currentKey = location.pathname.split('/')[1] || 'sales';

  return (
    <ConfigProvider
      theme={{
        token: {
          colorPrimary: '#1677ff', // Color azul estándar de ANTD
        },
      }}
    >
      <Layout style={{ minHeight: '100vh', overflow: 'hidden' }}>
        {/* SIDEBAR */}
        {!isMobile && (
          <Sider
            trigger={null}
            collapsible
            collapsed={collapsed}
            breakpoint="lg"
            collapsedWidth="80"
            theme="light"
            style={{ borderRight: '1px solid #f0f0f0' }}
          >
            <div style={{
              height: 90, // Aumentamos la altura para dar aire a los dos niveles
              padding: '12px 16px',
              display: 'flex',
              flexDirection: 'column', // Los elementos se apilan verticalmente
              alignItems: 'center',
              justifyContent: 'center',
              gap: '8px', // Espacio controlado entre el texto y el botón
              borderBottom: '1px solid #f0f0f0',
              marginBottom: 8
            }}>
              {/* Texto dinámico según el estado 'collapsed' */}
              <Title level={4} style={{ margin: 0, color: '#1677ff', whiteSpace: 'nowrap' }}>
                {collapsed ? 'M&M' : 'M&M POS'}
              </Title>

              {/* Botón justo debajo */}
              <Button
                type="text"
                icon={collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
                onClick={() => setCollapsed(!collapsed)}
                style={{
                  fontSize: '16px',
                  width: '100%', // Ocupa el ancho disponible para facilitar el clic
                  height: 32,
                  color: '#8c8c8c'
                }}
              />
            </div>

            <div style={{ display: 'flex', flexDirection: 'column', height: 'calc(100% - 98px)', justifyContent: 'space-between' }}>
              <Menu
                mode="inline"
                selectedKeys={[currentKey]}
                items={mainItems}
                onClick={({ key }) => navigate(`/${key}`)}
              />

              <Menu
                mode="inline"
                selectable={false}
                items={footerItems}
                onClick={({ key }) => navigate(`/${key}`)}
                style={{ borderTop: '1px solid #f0f0f0' }}
              />
            </div>
          </Sider>
        )}

        {/* MENÚ DE CELULAR (Drawer flotante oculto por defecto) */}
        <Drawer
          title="M&M POS"
          placement="left"
          onClose={() => setMobileMenuOpen(false)}
          open={mobileMenuOpen}
          styles={{ body: { padding: 0 }, width: 260 }} // Quita paddings innecesarios
        >
          <div style={{ display: 'flex', flexDirection: 'column', height: '100%', justifyContent: 'space-between' }}>
            <Menu
              mode="inline"
              selectedKeys={[currentKey]}
              items={mainItems}
              onClick={({ key }) => handleMenuClick(key)}
              style={{ border: 'none' }}
            />
            <Menu
              mode="inline"
              selectable={false}
              items={footerItems}
              onClick={({ key }) => handleMenuClick(key)}
              style={{ borderTop: '1px solid #f0f0f0' }}
            />
          </div>
        </Drawer>

        {/* CONTENEDOR DE CONTENIDO */}
        <Layout style={{ height: '100vh', overflow: 'hidden' }}>
          {/* Si es móvil, agregamos una mini barra superior para poder abrir el menú */}
          {isMobile && (
            <div style={{
              height: 50,
              background: '#fff',
              display: 'flex',
              alignItems: 'center',
              padding: '0 16px',
              borderBottom: '1px solid #f0f0f0'
            }}>
              <Button
                type="text"
                icon={<MenuOutlined />}
                onClick={() => setMobileMenuOpen(true)}
                style={{ fontSize: 18 }}
              />
              <Title level={4} style={{ margin: '0 0 0 16px', color: '#1677ff' }}>M&M POS</Title>
            </div>
          )}

          {/* CONTENIDO (Formulario hijo) */}
          <Content style={{
            margin: isMobile ? '8px' : '16px',
            padding: isMobile ? 12 : 24,
            background: '#fff',
            borderRadius: 8,
            height: isMobile ? 'calc(100vh - 66px)' : 'calc(100vh - 32px)',
            overflowY: 'auto',
            overflowX: 'hidden'
          }}>
            {/* Aquí se renderiza tu formulario */}
            <div>
              {children}
            </div>
          </Content>
        </Layout>
      </Layout>
    </ConfigProvider>
  );
};

export default DashboardLayout;