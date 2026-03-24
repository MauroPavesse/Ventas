import { Button, message, Table, Modal } from "antd";
import PageLayout from "../layouts/PageLayout";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { userService } from "../services/userService";
import { SearchCommand } from "../DTOs/SearchCommand";
import {
  DeleteOutlined,
  EditOutlined,
  ExclamationCircleOutlined,
} from "@ant-design/icons";
import UserEditModal from "../components/UserEditModal";

const User = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [users, setUsers] = useState();
  const [selectedRecord, setSelectedRecord] = useState(null);
  const [isModalUserOpen, setIsModalUserOpen] = useState(false);

  const fetchData = async () => {
    setLoading(true);
    try {
      const command = new SearchCommand();
      var res = await userService.search(command);
      setUsers(res);
    } catch (error) {
      message.error("Error al cargar el personal: " + error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const columns = [
    { title: "Usuario", dataIndex: "username", key: "username" },
    { title: "Rol", dataIndex: "roleName", key: "roleName" },
    {
      title: "Punto de venta",
      dataIndex: "pointOfSaleName",
      key: "pointOfSaleName",
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
              setIsModalUserOpen(true);
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

  const addUser = () => {
    setSelectedRecord(null);
    setIsModalUserOpen(true);
  };

  const { confirm } = Modal;

  const handleDelete = (record) => {
    confirm({
      title: "¿Estás seguro de eliminar este usuario?",
      icon: <ExclamationCircleOutlined />,
      content: `Usuario: ${record.username}`,
      okText: "Sí, eliminar",
      okType: "danger",
      cancelText: "Cancelar",
      onOk: async () => {
        try {
          await userService.delete(record.id);
          message.success("Eliminado correctamente");
          users.filter((t) => t.id != record.id);
        } catch (e) {
          message.error("Error al eliminar: " + e);
        }
      },
    });
  };

  const handleCancel = () => {
    setIsModalUserOpen(false);
    setSelectedRecord(null);
  };

  const handleSuccess = () => {
    setIsModalUserOpen(false);
    fetchData();
  };

  return (
    <PageLayout title="Personal" onClose={() => navigate("/configurations")}>
      <Button type="primary" style={{ marginBottom: 10 }} onClick={addUser}>
        Agregar
      </Button>

      <Table
        dataSource={users}
        columns={columns}
        rowKey="id"
        loading={loading}
      />

      <UserEditModal
        open={isModalUserOpen}
        initialValues={selectedRecord}
        onCancel={handleCancel}
        onSuccess={handleSuccess}
      />
    </PageLayout>
  );
};

export default User;
