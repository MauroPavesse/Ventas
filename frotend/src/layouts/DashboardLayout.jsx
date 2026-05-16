import React, { useState } from 'react';
import { Layout, Menu, Button, Typography, ConfigProvider } from 'antd';
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
  HomeOutlined
} from '@ant-design/icons';
import { useAuth } from '../context/AuthContext';
import { useNavigate, useLocation } from 'react-router-dom';

const { Header, Sider, Content } = Layout;
const { Title } = Typography;

const DashboardLayout = ({ children }) => {
  const [collapsed, setCollapsed] = useState(false);
  const { logout } = useAuth();
  const navigate = useNavigate();

  // Inicializamos useLocation
  const location = useLocation();

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

          <div style={{ display: 'flex', flexDirection: 'column', height: 'calc(100% - 96px)', justifyContent: 'space-between' }}>
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

        <Layout style={{ height: '100vh', overflow: 'hidden' }}>
          {/* CONTENIDO (Formulario hijo) */}
          <Content style={{
            margin: '24px 16px',
            padding: 24,
            background: '#fff',
            borderRadius: 8,
            height: 'calc(100vh - 32px)',
            overflowY: 'auto',
            overflowX: 'hidden'
          }}>
            {/* Aquí se renderiza tu formulario */}
            <div style={{ marginTop: 20 }}>
              {children}
            </div>
          </Content>
        </Layout>
      </Layout>
    </ConfigProvider>
  );
};

export default DashboardLayout;