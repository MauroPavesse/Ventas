import { Button, Col, Row } from "antd";
import PageLayout from "../layouts/PageLayout";
import { useNavigate } from "react-router-dom";

const Configurations = () => {
  const navigate = useNavigate();

  return (
    <PageLayout title="Configuraciones" onClose={() => navigate("/dashboard")}>
      <Button
        style={{
          backgroundColor: "#bfb45c",
          width: "100%",
          height: "60px",
          color: "white",
          fontSize: 30,
        }}
        onClick={() => navigate("/business")}
      >
        DATOS DE LA EMPRESA
      </Button>
      <Button
        style={{
          backgroundColor: "#a35cbf",
          width: "100%",
          height: "60px",
          color: "white",
          fontSize: 30,
        }}
        onClick={() => navigate("/pointofsales")}
      >
        PUNTOS DE VENTA
      </Button>
      <Button
        style={{
          backgroundColor: "#5c9cbf",
          width: "100%",
          height: "60px",
          color: "white",
          fontSize: 30,
        }}
        onClick={() => navigate("/paymentmethods")}
      >
        FORMAS DE PAGO
      </Button>
      <Button
        style={{
          backgroundColor: "#a88947",
          width: "100%",
          height: "60px",
          color: "white",
          fontSize: 30,
        }}
        onClick={() => navigate("/users")}
      >
        PERSONAL
      </Button>
      <Button
        style={{
          backgroundColor: "#45c924",
          width: "100%",
          height: "60px",
          color: "white",
          fontSize: 30,
        }}
        onClick={() => navigate("/categories")}
      >
        CATEGORÍAS DE PRODUCTOS
      </Button>
    </PageLayout>
  );
};

export default Configurations;
