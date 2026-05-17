import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { SearchCommand } from "../DTOs/SearchCommand";
import { paymentMethodService } from "../services/paymentMethodService";
import { Button, message, Table, Modal, Input, Row, Col, Grid } from "antd";
import {
 DeleteOutlined,
  EditOutlined,
  ExclamationCircleOutlined,
  PlusOutlined
} from "@ant-design/icons";
import PageLayout from "../layouts/PageLayout";
import PaymentMethodEditModal from "../components/PaymentMethodEditModal";

const { useBreakpoint } = Grid;
const { confirm } = Modal;

const PaymentMethod = () => {
  const navigate = useNavigate();
  const screens = useBreakpoint();
  const isMobile = screens.md === false;

  const [loading, setLoading] = useState(false);
  const [paymentMethods, setPaymentMethods] = useState([]);
  const [filteredMethods, setFilteredMethods] = useState([]);
  const [searchText, setSearchText] = useState("");
  const [selectedRecord, setSelectedRecord] = useState(null);
  const [isModalPaymentMethodOpen, setIsModalPaymentMethodOpen] = useState(false);

  const fetchData = async () => {
    setLoading(true);
    try {
      const command = new SearchCommand();
      var res = await paymentMethodService.search(command);
      setPaymentMethods(res || []);
      setFilteredMethods(res || []);
    } catch (error) {
      message.error("Error al cargar formas de pago: " + error);
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
    const filtered = paymentMethods.filter((p) =>
      (p.name || "").toLowerCase().includes(value)
    );
    setFilteredMethods(filtered);
  };

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
          setPaymentMethods((prev) => prev.filter((t) => t.id !== record.id));
          setFilteredMethods((prev) => prev.filter((t) => t.id !== record.id));
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
    { 
      title: "Forma de Pago", 
      key: "nameInfo",
      render: (_, record) => (
        <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
          {/* Círculo indicador de color */}
          <div
            style={{
              backgroundColor: record.color || '#d9d9d9',
              width: isMobile ? '14px' : '18px',
              height: isMobile ? '14px' : '18px',
              borderRadius: '50%',
              flexShrink: 0,
              border: "1px solid #d9d9d9",
            }}
          />
          <div>
            <b style={{ fontSize: isMobile ? '13px' : '14px' }}>{record.name}</b>
            {isMobile && (
              <div style={{ fontSize: '11px', color: '#8c8c8c', marginTop: '2px' }}>
                {record.descountPercentage > 0 && `Desc: ${record.descountPercentage}% `}
                {record.increasePercentage > 0 && `Recargo: ${record.increasePercentage}%`}
                {record.descountPercentage === 0 && record.increasePercentage === 0 && "Sin recargos/descuentos"}
              </div>
            )}
          </div>
        </div>
      )
    },
    {
      title: "Descuento",
      dataIndex: "descountPercentage",
      key: "descountPercentage",
      responsive: ['sm'],
      render: (valor) => `${valor}%`
    },
    {
      title: "Incremento",
      dataIndex: "increasePercentage",
      key: "increasePercentage",
      responsive: ['sm'],
      render: (valor) => `${valor}%`
    },
    {
      title: "Color",
      dataIndex: "color",
      key: "color",
      responsive: ['sm'],
      width: 100,
      render: (color) => (
        <div
          style={{
            backgroundColor: color,
            width: "100%",
            height: "20px",
            borderRadius: "4px",
            border: "1px solid #d9d9d9",
          }}
        />
      ),
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
              setIsModalPaymentMethodOpen(true);
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

  return (
    <PageLayout
      title="Formas de pago"
      onClose={() => navigate("/configurations")}
    >
      {/* Cabecera superior fluida */}
      <Row gutter={[16, 16]} style={{ marginBottom: 16 }} align="middle">
        <Col xs={24} sm={6} md={4}>
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={addPaymentMethod}
            block={isMobile}
            size={isMobile ? "large" : "default"}
          >
            Agregar Forma
          </Button>
        </Col>
        <Col xs={24} sm={18} md={20}>
          <Input.Search
            placeholder="Buscar forma de pago..."
            value={searchText}
            onChange={handleSearch}
            size={isMobile ? "default" : "large"}
            style={{ width: '100%' }}
          />
        </Col>
      </Row>

      <Table
        dataSource={filteredMethods}
        columns={columns}
        rowKey="id"
        loading={loading}
        size={isMobile ? "small" : "default"}
        scroll={{ x: true }}
        pagination={{ pageSize: isMobile ? 6 : 10, size: isMobile ? "small" : "default" }}
      />

      <PaymentMethodEditModal
        open={isModalPaymentMethodOpen}
        initialValues={selectedRecord}
        onCancel={handleCancel}
        onSuccess={handleSuccess}
        width={isMobile ? "95%" : 500}
      />
    </PageLayout>
  );
};

export default PaymentMethod;
