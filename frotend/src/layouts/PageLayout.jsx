import React from 'react';
import { Typography, Button, Divider, Grid } from 'antd';
import { CloseOutlined } from '@ant-design/icons';

const { Title } = Typography;
const { useBreakpoint } = Grid;

const PageLayout = ({ title, onClose, children }) => {
  const screens = useBreakpoint();
  const isMobile = screens.md === false;

  return (
    <div style={{ background: '#fff', borderRadius: 8 }}>
      {/* Cabecera del contenido */}
      <div style={{ 
        display: 'flex', 
        justifyContent: 'space-between', 
        alignItems: 'center', 
        paddingBottom: isMobile ? 8 : 16 // Reducimos espacio en celular
      }}>
        <Title level={isMobile ? 4 : 3} style={{ margin: 0 }}>
          {title}
        </Title>
        {onClose && (
          <Button 
            type="text" 
            shape="circle" 
            icon={<CloseOutlined />} 
            onClick={onClose} 
            style={{ color: '#999' }}
          />
        )}
      </div>
      
      <Divider style={{ margin: isMobile ? '0 0 16px 0' : '0 0 24px 0' }} />

      {/* Cuerpo del formulario o contenido */}
      <div className="page-content">
        {children}
      </div>
    </div>
  );
};

export default PageLayout;