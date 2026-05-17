import React, { useEffect, useState } from "react";
import { Button, message, Table, Modal, Input, Row, Col, Grid } from "antd";
import PageLayout from "../layouts/PageLayout";
import { useNavigate } from "react-router-dom";
import { customerService } from "../services/customerService";
import { SearchCommand } from "../DTOs/SearchCommand";
import {
  DeleteOutlined,
  EditOutlined,
  ExclamationCircleOutlined,
  PlusOutlined
} from "@ant-design/icons";
import CustomerEditModal from "../components/CustomerEditModal";

const { useBreakpoint } = Grid;

const Customer = () => {
  const navigate = useNavigate();
  const screens = useBreakpoint();
  const isMobile = screens.md === false;

  const [loading, setLoading] = useState(false);
  const [customers, setCustomers] = useState();
  const [filteredCustomers, setFilteredCustomers] = useState([]);
  const [searchText, setSearchText] = useState("");
  const [selectedRecord, setSelectedRecord] = useState(null);
  const [isModalCustomerOpen, setIsModalCustomerOpen] = useState(false);

  const fetchData = async () => {
    setLoading(true);
    try {
      const command = new SearchCommand();
      var res = await customerService.search(command);
      setCustomers(res || []);
      setFilteredCustomers(res || []);
    } catch (error) {
      message.error("Error al cargar los clientes: " + error);
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
    const filtered = customers.filter((c) => {
      const fullName = `${c.firstName || ''} ${c.lastName || ''}`.toLowerCase();
      const documentStr = (c.document || '').toString().toLowerCase();
      return fullName.includes(value) || documentStr.includes(value);
    });
    setFilteredCustomers(filtered);
  };

  const columns = [
    { 
      title: "Cliente", 
      key: "fullName",
      render: (_, record) => (
        <div>
          <b style={{ fontSize: isMobile ? '13px' : '14px' }}>
            {`${record.firstName || ''} ${record.lastName || ''}`}
          </b>
          {isMobile && record.document && (
            <div style={{ fontSize: '11px', color: '#8c8c8c', marginTop: '2px' }}>
              Doc: {record.document}
            </div>
          )}
        </div>
      )
    },
    { 
      title: "Documento", 
      dataIndex: "document", 
      key: "document",
      responsive: ['sm'], // Oculto en móviles, se integra en la primera columna
    },
    { 
      title: "Condición Fiscal", 
      dataIndex: "taxConditionDescription", 
      key: "taxConditionDescription",
      responsive: ['md'], // Solo visible en pantallas medianas o grandes
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
              setIsModalCustomerOpen(true);
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

  const addCustomer = () => {
    setSelectedRecord(null);
    setIsModalCustomerOpen(true);
  };

  const { confirm } = Modal;

  const handleDelete = (record) => {
    confirm({
      title: "¿Estás seguro de eliminar este cliente?",
      icon: <ExclamationCircleOutlined />,
      content: `Cliente: ${record.firstName} ${record.lastName}`,
      okText: "Sí, eliminar",
      okType: "danger",
      cancelText: "Cancelar",
      onOk: async () => {
        try {
          await customerService.delete(record.id);
          message.success("Eliminado correctamente");

          setCustomers((prev) => prev.filter((t) => t.id !== record.id));
          setFilteredCustomers((prev) => prev.filter((t) => t.id !== record.id));
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
      {/* Barra superior responsiva con Buscador integrado */}
      <Row gutter={[16, 16]} style={{ marginBottom: 16 }} align="middle">
        <Col xs={24} sm={6} md={4}>
          <Button 
            type="primary" 
            icon={<PlusOutlined />} 
            onClick={addCustomer}
            block={isMobile}
            size={isMobile ? "large" : "default"}
          >
            Agregar Cliente
          </Button>
        </Col>
        <Col xs={24} sm={18} md={20}>
          <Input.Search
            placeholder="Buscar por nombre, apellido o documento..."
            value={searchText}
            onChange={handleSearch}
            size={isMobile ? "default" : "large"}
            style={{ width: '100%' }}
          />
        </Col>
      </Row>

      <Table
        dataSource={filteredCustomers}
        columns={columns}
        rowKey="id"
        loading={loading}
        size={isMobile ? "small" : "default"}
        scroll={{ x: true }}
        pagination={{ pageSize: isMobile ? 6 : 10, size: isMobile ? "small" : "default" }}
      />

      <CustomerEditModal
        open={isModalCustomerOpen}
        initialValues={selectedRecord}
        onCancel={handleCancel}
        onSuccess={handleSuccess}
        width={isMobile ? "95%" : 500}
      />
    </PageLayout>
  );
};

export default Customer;
