import React from "react";
import { Button, Col, Row, Grid } from "antd";
import {
  ShopOutlined,
  CreditCardOutlined,
  TeamOutlined,
  TagsOutlined,
  BankOutlined,
} from "@ant-design/icons";
import PageLayout from "../layouts/PageLayout";
import { useNavigate } from "react-router-dom";

const { useBreakpoint } = Grid;

const Configurations = () => {
  const navigate = useNavigate();
  const screens = useBreakpoint();
  const isMobile = screens.md === false; // Detecta pantallas móviles o tablets en vertical

  const menuItems = [
    {
      title: "DATOS DE LA EMPRESA",
      path: "/business",
      color: "#bfb45c",
      icon: <BankOutlined />,
    },
    {
      title: "PUNTOS DE VENTA",
      path: "/pointofsales",
      color: "#a35cbf",
      icon: <ShopOutlined />,
    },
    {
      title: "FORMAS DE PAGO",
      path: "/paymentmethods",
      color: "#5c9cbf",
      icon: <CreditCardOutlined />,
    },
    {
      title: "PERSONAL",
      path: "/users",
      color: "#a88947",
      icon: <TeamOutlined />,
    },
    {
      title: "CATEGORÍAS DE PRODUCTOS",
      path: "/categories",
      color: "#45c924",
      icon: <TagsOutlined />,
    },
  ];

  return (
    <PageLayout title="Configuraciones" onClose={() => navigate("/dashboard")}>
      <div style={{ padding: isMobile ? "8px 0" : "16px 0" }}>
        <Row gutter={[16, 16]}>
          {menuItems.map((item, index) => (
            <Col 
              key={index} 
              xs={24}     // 1 columna en celulares
              sm={12}     // 2 columnas en tablets
              md={12}     // 2 columnas en pantallas medianas
              lg={8}      // 3 columnas en monitores grandes
            >
              <Button
                type="text"
                icon={React.cloneElement(item.icon, { 
                  style: { 
                    fontSize: isMobile ? "24px" : "32px", 
                    color: "white" 
                  } 
                })}
                style={{
                  backgroundColor: item.color,
                  width: "100%",
                  height: isMobile ? "70px" : "100px", // Más alto en escritorio para dar aire de "Tarjetas"
                  color: "white",
                  fontSize: isMobile ? "14px" : "18px", // Tipografía equilibrada y legible
                  fontWeight: "600",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: isMobile ? "flex-start" : "center", // Izquierda en móvil, centrado en desktop
                  flexDirection: isMobile ? "row" : "column", // Fila en móvil, columna (arriba/abajo) en PC
                  gap: isMobile ? "16px" : "12px",
                  padding: "0 24px",
                  borderRadius: "8px",
                  boxShadow: "0 2px 4px rgba(0,0,0,0.08)",
                  transition: "transform 0.2s, box-shadow 0.2s",
                }}
                onClick={() => navigate(item.path)}
                className="config-menu-btn"
              >
                <span>{item.title}</span>
              </Button>
            </Col>
          ))}
        </Row>
      </div>

      {/* Efecto hover suave para pantallas de escritorio */}
      <style>{`
        .config-menu-btn:hover {
          transform: translateY(-2px);
          box-shadow: 0 4px 12px rgba(0,0,0,0.15) !important;
          opacity: 0.95;
        }
      `}</style>
    </PageLayout>
  );
};

export default Configurations;
