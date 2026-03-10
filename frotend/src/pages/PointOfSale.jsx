import { Button, message, Table, Modal } from "antd";
import PageLayout from "../layouts/PageLayout";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { pointOfSaleService } from "../services/pointOfSaleService";
import { SearchCommand } from "../DTOs/SearchCommand";
import { DeleteOutlined, EditOutlined, ExclamationCircleOutlined } from "@ant-design/icons";
import PointOfSaleEditModal from "../components/PointOfSaleEditModal";

const PointOfSale = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [pointOfSales, setPointOfSales] = useState();
  const [selectedRecord, setSelectedRecord] = useState(null);
  const [isModalPointOfSaleOpen, setIsModalPointOfSaleOpen] = useState(false);

  const fetchData = async () => {
    setLoading(true);
    try {
      const command = new SearchCommand();
      var res = await pointOfSaleService.search(command);
      setPointOfSales(res);
    } catch (error) {
      message.error("Error al cargar puntos de ventas: " + error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const columns = [
    { title: "Nombre", dataIndex: "name", key: "name" },
    { title: "Numero", dataIndex: "number", key: "number" },
    { title: "Dirección", dataIndex: "address", key: "address" },
    {
      title: "Acción",
      key: "action",
      fixed: "right",
      width: 80,
      render: (_, record) => (
        <span className="flex">
          <Button
            icon={<EditOutlined />}
            onClick={() => {
              setSelectedRecord(record);
              setIsModalPointOfSaleOpen(true);
            }}
          />
          <Button
            danger
            icon={<DeleteOutlined />}
            onClick={() => handleDelete(record)}
          />
        </span>
      ),
    },
  ];

  const addPointOfSale = () => {
    setSelectedRecord(null);
    setIsModalPointOfSaleOpen(true);
  };

  const { confirm } = Modal;

  const handleDelete = (record) => {
    confirm({
      title: "¿Estás seguro de eliminar este punto de venta?",
      icon: <ExclamationCircleOutlined />,
      content: `Punto de venta: ${record.name}`,
      okText: "Sí, eliminar",
      okType: "danger",
      cancelText: "Cancelar",
      onOk: async () => {
        try {
          await pointOfSaleService.delete(record.id);
          message.success("Eliminado correctamente");
          pointOfSales.filter((t) => t.id != record.id);
        } catch (e) {
          message.error("Error al eliminar: " + e);
        }
      },
    });
  };

  const handleCancel = () => {
    setIsModalPointOfSaleOpen(false);
    setSelectedRecord(null);
  };

  const handleSuccess = () => {
    setIsModalPointOfSaleOpen(false);
    fetchData();
  };

  return (
    <PageLayout
      title="Puntos de venta"
      onClose={() => navigate("/configurations")}
    >
      <Button
        type="primary"
        style={{ marginBottom: 10 }}
        onClick={addPointOfSale}
      >
        Agregar
      </Button>

      <Table
        dataSource={pointOfSales}
        columns={columns}
        rowKey="id"
        loading={loading}
      />

      <PointOfSaleEditModal
        open={isModalPointOfSaleOpen}
        initialValues={selectedRecord}
        onCancel={handleCancel}
        onSuccess={handleSuccess}
      />
    </PageLayout>
  );
};

export default PointOfSale;
