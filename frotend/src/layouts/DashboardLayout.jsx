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
  CloseOutlined
} from '@ant-design/icons';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';

const { Header, Sider, Content } = Layout;
const { Title } = Typography;

const DashboardLayout = ({ children }) => {
  const [collapsed, setCollapsed] = useState(false);
  const { logout } = useAuth();
  const navigate = useNavigate();

  // Ítems del menú superior
  const mainItems = [
    { key: 'sale', icon: <ShoppingOutlined />, label: 'Venta' },
    { key: 'caja', icon: <CalculatorOutlined />, label: 'Caja Diaria' },
    { key: 'comprobantes', icon: <FileTextOutlined />, label: 'Comprobantes' },
    { key: 'products', icon: <InboxOutlined />, label: 'Productos' },
  ];

  // Ítems del menú inferior (Ajustes y Logout)
  const footerItems = [
    { key: 'configurations', icon: <SettingOutlined />, label: 'Ajustes'},
    { 
      key: 'logout', 
      icon: <LogoutOutlined />, 
      label: 'Cerrar Sesión', 
      danger: true,
      onClick: logout 
    },
  ];

  return (
    <ConfigProvider
      theme={{
        token: {
          colorPrimary: '#1677ff', // Color azul estándar de ANTD
        },
      }}
    >
      <Layout style={{ minHeight: '100vh' }}>
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
          <div style={{ height: 64, margin: 12, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
            <Title level={4} style={{ margin: 0, color: '#1677ff' }}>
              {collapsed ? 'M&M' : 'M&M POS'}
            </Title>
          </div>

          <div style={{ display: 'flex', flexDirection: 'column', height: 'calc(100% - 96px)', justifyContent: 'space-between' }}>
            <Menu
              mode="inline"
              defaultSelectedKeys={['1']}
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

        <Layout>
          {/* HEADER */}
          <Header style={{ padding: 0, background: '#fff', display: 'flex', alignItems: 'center', paddingLeft: 16 }}>
            <Button
              type="text"
              icon={collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
              onClick={() => setCollapsed(!collapsed)}
              style={{ fontSize: '16px', width: 64, height: 64 }}
            />
          </Header>

          {/* CONTENIDO (Formulario hijo) */}
          <Content style={{ margin: '24px 16px', padding: 24, background: '#fff', borderRadius: 8, minHeight: 280, overflow: 'initial' }}>            
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