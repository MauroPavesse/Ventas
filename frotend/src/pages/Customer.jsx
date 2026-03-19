import { Button, message, Table, Modal } from "antd";
import PageLayout from "../layouts/PageLayout";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { customerService } from "../services/customerService";
import { SearchCommand } from "../DTOs/SearchCommand";
import {
  DeleteOutlined,
  EditOutlined,
  ExclamationCircleOutlined,
} from "@ant-design/icons";
import CustomerEditModal from "../components/CustomerEditModal";

const Customer = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [customers, setCustomers] = useState();
  const [selectedRecord, setSelectedRecord] = useState(null);
  const [isModalCustomerOpen, setIsModalCustomerOpen] = useState(false);

  const fetchData = async () => {
    setLoading(true);
    try {
      const command = new SearchCommand();
      var res = await customerService.search(command);
      setCustomers(res);
    } catch (error) {
      message.error("Error al cargar los clientes: " + error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const columns = [
    { title: "Nombre", dataIndex: "firstName", key: "firstName" },
    { title: "Apellido", dataIndex: "lastName", key: "lastName" },
    { title: "Documento", dataIndex: "document", key: "document" },
    { title: "Condición Fiscal", dataIndex: "taxConditionDescription", key: "taxConditionDescription" },
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
              setIsModalCustomerOpen(true);
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

  const addCustomer = () => {
    setSelectedRecord(null);
    setIsModalCustomerOpen(true);
  };

  const { confirm } = Modal;

  const handleDelete = (record) => {
    confirm({
      title: "¿Estás seguro de eliminar este cliente?",
      icon: <ExclamationCircleOutlined />,
      content: `Cliente: ${record.name}`,
      okText: "Sí, eliminar",
      okType: "danger",
      cancelText: "Cancelar",
      onOk: async () => {
        try {
          await customerService.delete(record.id);
          message.success("Eliminado correctamente");
          customers.filter((t) => t.id != record.id);
        } catch (e) {
          message.error("Error al eliminar: " + e);
        }
      },
    });
  };

  const handleCancel = () => {
    setIsModalCustomerOpen(false);
    setSelectedRecord(null);
  };

  const handleSuccess = () => {
    setIsModalCustomerOpen(false);
    fetchData();
  };

  return (
    <PageLayout title="Clientes" onClose={() => navigate("/configurations")}>
      <Button type="primary" style={{ marginBottom: 10 }} onClick={addCustomer}>
        Agregar
      </Button>

      <Table
        dataSource={customers}
        columns={columns}
        rowKey="id"
        loading={loading}
      />

      <CustomerEditModal
        open={isModalCustomerOpen}
        initialValues={selectedRecord}
        onCancel={handleCancel}
        onSuccess={handleSuccess}
      />
    </PageLayout>
  );
};

export default Customer;
