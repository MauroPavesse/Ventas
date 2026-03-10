import PageLayout from "../layouts/PageLayout";
import { useNavigate } from "react-router-dom";

const PointOfSale = () => {
    const navigate = useNavigate();

  return (
    <PageLayout title="Puntos de venta" onClose={() => navigate("/configurations")}>

    </PageLayout>
  )
};

export default PointOfSale;