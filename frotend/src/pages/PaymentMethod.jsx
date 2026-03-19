import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { SearchCommand } from "../DTOs/SearchCommand";
import { paymentMethodService } from "../services/paymentMethodService";
import { Button, message, Table, Modal } from "antd";
import {
  DeleteOutlined,
  EditOutlined,
  ExclamationCircleOutlined,
} from "@ant-design/icons";
import PageLayout from "../layouts/PageLayout";
import PaymentMethodEditModal from "../components/PaymentMethodEditModal";

const PaymentMethod = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [paymentMethods, setPaymentMethods] = useState();
  const [selectedRecord, setSelectedRecord] = useState(null);
  const [isModalPaymentMethodOpen, setIsModalPaymentMethodOpen] =
    useState(false);

  const fetchData = async () => {
    setLoading(true);
    try {
      const command = new SearchCommand();
      var res = await paymentMethodService.search(command);
      setPaymentMethods(res);
    } catch (error) {
      message.error("Error al cargar formas de pago: " + error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const addPaymentMethod = () => {
    setSelectedRecord(null);
    setIsModalPaymentMethodOpen(true);
  };

  const handleDelete = (record) => {
    confirm({
      title: "¿Estás seguro de eliminar esta forma de pago?",
      icon: <ExclamationCircleOutlined />,
      content: `Forma de pago: ${record.name}`,
      okText: "Sí, eliminar",
      okType: "danger",
      cancelText: "Cancelar",
      onOk: async () => {
        try {
          await paymentMethodService.delete(record.id);
          message.success("Eliminado correctamente");
          paymentMethods.filter((t) => t.id != record.id);
        } catch (e) {
          message.error("Error al eliminar: " + e);
        }
      },
    });
  };

  const handleCancel = () => {
    setIsModalPaymentMethodOpen(false);
    setSelectedRecord(null);
  };

  const handleSuccess = () => {
    setIsModalPaymentMethodOpen(false);
    fetchData();
  };

  const columns = [
    { title: "Nombre", dataIndex: "name", key: "name" },
    {
      title: "Descuento",
      dataIndex: "descountPercentage",
      key: "descountPercentage",
      render: (valor) => valor + "%"
    },
    {
      title: "Incremento",
      dataIndex: "increasePercentage",
      key: "increasePercentage",
      render: (valor) => valor + "%"
    },
    {
      title: "Color",
      dataIndex: "color",
      key: "color",
      render: (color) => (
        <div
          style={{
            backgroundColor: color,
            width: "100%",
            height: "20px",
            borderRadius: "4px",
            border: "1px solid #d9d9d9", // Opcional: para que se vea si el color es blanco
          }}
        />
      ),
    },
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
              setIsModalPaymentMethodOpen(true);
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

  const { confirm } = Modal;

  return (
    <PageLayout
      title="Formas de pago"
      onClose={() => navigate("/configurations")}
    >
      <Button
        type="primary"
        style={{ marginBottom: 10 }}
        onClick={addPaymentMethod}
      >
        Agregar
      </Button>

      <Table
        dataSource={paymentMethods}
        columns={columns}
        rowKey="id"
        loading={loading}
      />

      <PaymentMethodEditModal
        open={isModalPaymentMethodOpen}
        initialValues={selectedRecord}
        onCancel={handleCancel}
        onSuccess={handleSuccess}
      />
    </PageLayout>
  );
};

export default PaymentMethod;
