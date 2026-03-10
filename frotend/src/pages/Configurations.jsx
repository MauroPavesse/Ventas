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
        onClick={() => navigate("/pointofsale")}
      >
        PUNTOS DE VENTA
      </Button>
    </PageLayout>
  );
};

export default Configurations;
