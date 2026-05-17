import React, { useEffect, useState } from "react";
import { Button, message, Table, Modal, Input, Row, Col, Grid } from "antd";
import PageLayout from "../layouts/PageLayout";
import { useNavigate } from "react-router-dom";
import { userService } from "../services/userService";
import { SearchCommand } from "../DTOs/SearchCommand";
import {
  DeleteOutlined,
  EditOutlined,
  ExclamationCircleOutlined,
  PlusOutlined
} from "@ant-design/icons";
import UserEditModal from "../components/UserEditModal";

const { useBreakpoint } = Grid;

const User = () => {
  const navigate = useNavigate();
  const screens = useBreakpoint();
  const isMobile = screens.md === false;

  const [loading, setLoading] = useState(false);
  const [users, setUsers] = useState();
  const [filteredUsers, setFilteredUsers] = useState([]);
  const [searchText, setSearchText] = useState("");
  const [selectedRecord, setSelectedRecord] = useState(null);
  const [isModalUserOpen, setIsModalUserOpen] = useState(false);

  const fetchData = async () => {
    setLoading(true);
    try {
      const command = new SearchCommand();
      var res = await userService.search(command);
      setUsers(res || []);
      setFilteredUsers(res || []);
    } catch (error) {
      message.error("Error al cargar el personal: " + error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const handleSearch = (e) => {
    const value = e.target.value.toLowerCase();
    setSearchText(value);
    const filtered = users.filter((u) => {
      const username = (u.username || "").toLowerCase();
      const roleName = (u.roleName || "").toLowerCase();
      const posName = (u.pointOfSaleName || "").toLowerCase();
      return username.includes(value) || roleName.includes(value) || posName.includes(value);
    });
    setFilteredUsers(filtered);
  };

  const columns = [
    {
      title: "Usuario",
      dataIndex: "username",
      key: "username",
      render: (text, record) => (
        <div>
          <b style={{ fontSize: isMobile ? '13px' : '14px' }}>{text}</b>
          {isMobile && (
            <div style={{ fontSize: '11px', color: '#8c8c8c', marginTop: '2px' }}>
              {record.roleName} {record.pointOfSaleName ? `• ${record.pointOfSaleName}` : ''}
            </div>
          )}
        </div>
      )
    },
    {
      title: "Rol",
      dataIndex: "roleName",
      key: "roleName",
      responsive: ['sm'], // Se oculta en móviles y se acopla debajo del usuario
    },
    {
      title: "Punto de venta",
      dataIndex: "pointOfSaleName",
      key: "pointOfSaleName",
      responsive: ['md'], // Solo visible en pantallas de tablets/PCs
    },
    {
      title: "Acción",
      key: "action",
      fixed: isMobile ? false : "right",
      width: isMobile ? 90 : 100,
      render: (_, record) => (
        <div style={{ display: 'flex', gap: '4px' }}>
          <Button
            size={isMobile ? "small" : "default"}
            icon={<EditOutlined />}
            onClick={() => {
              setSelectedRecord(record);
              setIsModalUserOpen(true);
            }}
          />
          <Button
            size={isMobile ? "small" : "default"}
            danger
            icon={<DeleteOutlined />}
            onClick={() => handleDelete(record)}
          />
        </div>
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
          setUsers((prev) => prev.filter((t) => t.id !== record.id));
          setFilteredUsers((prev) => prev.filter((t) => t.id !== record.id));
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
      {/* Cabecera dinámica y responsiva */}
      <Row gutter={[16, 16]} style={{ marginBottom: 16 }} align="middle">
        <Col xs={24} sm={6} md={4}>
          <Button 
            type="primary" 
            icon={<PlusOutlined />} 
            onClick={addUser}
            block={isMobile}
            size={isMobile ? "large" : "default"}
          >
            Agregar Usuario
          </Button>
        </Col>
        <Col xs={24} sm={18} md={20}>
          <Input.Search
            placeholder="Buscar por usuario, rol o punto de venta..."
            value={searchText}
            onChange={handleSearch}
            size={isMobile ? "default" : "large"}
            style={{ width: '100%' }}
          />
        </Col>
      </Row>

      <Table
        dataSource={filteredUsers} // Muestra la lista filtrada de usuarios
        columns={columns}
        rowKey="id"
        loading={loading}
        size={isMobile ? "small" : "default"}
        scroll={{ x: true }}
        pagination={{ pageSize: isMobile ? 6 : 10, size: isMobile ? "small" : "default" }}
      />

      <UserEditModal
        open={isModalUserOpen}
        initialValues={selectedRecord}
        onCancel={handleCancel}
        onSuccess={handleSuccess}
        width={isMobile ? "95%" : 500}
      />
    </PageLayout>
  );
};

export default User;
