import React from 'react';
import { Typography, Button, Divider } from 'antd';
import { CloseOutlined } from '@ant-design/icons';

const { Title } = Typography;

const PageLayout = ({ title, onClose, children }) => {
  return (
    <div style={{ background: '#fff', borderRadius: 8 }}>
      {/* Cabecera del contenido */}
      <div style={{ 
        display: 'flex', 
        justifyContent: 'space-between', 
        alignItems: 'center', 
        paddingBottom: 16 
      }}>
        <Title level={3} style={{ margin: 0 }}>{title}</Title>
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
      
      <Divider style={{ margin: '0 0 24px 0' }} />

      {/* Cuerpo del formulario o contenido */}
      <div className="page-content">
        {children}
      </div>
    </div>
  );
};

export default PageLayout;