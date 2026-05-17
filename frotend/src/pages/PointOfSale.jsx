import React, { useEffect, useState } from "react";
import { Button, message, Table, Modal, Input, Row, Col, Grid } from "antd";
import PageLayout from "../layouts/PageLayout";
import { useNavigate } from "react-router-dom";
import { pointOfSaleService } from "../services/pointOfSaleService";
import { SearchCommand } from "../DTOs/SearchCommand";
import {
  DeleteOutlined,
  EditOutlined,
  ExclamationCircleOutlined,
  PlusOutlined
} from "@ant-design/icons";
import PointOfSaleEditModal from "../components/PointOfSaleEditModal";

const { useBreakpoint } = Grid;

const PointOfSale = () => {
  const navigate = useNavigate();
  const screens = useBreakpoint();
  const isMobile = screens.md === false;

  const [loading, setLoading] = useState(false);
  const [pointOfSales, setPointOfSales] = useState();
  const [filteredPOS, setFilteredPOS] = useState([]);
  const [searchText, setSearchText] = useState("");
  const [selectedRecord, setSelectedRecord] = useState(null);
  const [isModalPointOfSaleOpen, setIsModalPointOfSaleOpen] = useState(false);

  const fetchData = async () => {
    setLoading(true);
    try {
      const command = new SearchCommand();
      var res = await pointOfSaleService.search(command);
      setPointOfSales(res || []);
      setFilteredPOS(res || []);
    } catch (error) {
      message.error("Error al cargar puntos de ventas: " + error);
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
    const filtered = pointOfSales.filter((p) => {
      const name = (p.name || "").toLowerCase();
      const number = (p.number || "").toString().toLowerCase();
      const address = (p.address || "").toLowerCase();
      return name.includes(value) || number.includes(value) || address.includes(value);
    });
    setFilteredPOS(filtered);
  };

  const columns = [
    {
      title: "Punto de Venta",
      key: "posInfo",
      render: (_, record) => (
        <div>
          <b style={{ fontSize: isMobile ? '13px' : '14px' }}>{record.name}</b>
          <div style={{ fontSize: '11px', color: '#8c8c8c', marginTop: '2px' }}>
            N° {record.number}
            {isMobile && record.address && ` • ${record.address}`}
          </div>
        </div>
      )
    },
    {
      title: "Número",
      dataIndex: "number",
      key: "number",
      responsive: ['sm'], // Oculto en móviles ya que se integra arriba
    },
    {
      title: "Dirección",
      dataIndex: "address",
      key: "address",
      responsive: ['md'], // Solo visible de tablets en adelante
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
              setIsModalPointOfSaleOpen(true);
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
          setPointOfSales((prev) => prev.filter((t) => t.id !== record.id));
          setFilteredPOS((prev) => prev.filter((t) => t.id !== record.id));
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
      {/* Barra superior de acciones fluida */}
      <Row gutter={[16, 16]} style={{ marginBottom: 16 }} align="middle">
        <Col xs={24} sm={6} md={4}>
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={addPointOfSale}
            block={isMobile}
            size={isMobile ? "large" : "default"}
          >
            Agregar P.V.
          </Button>
        </Col>
        <Col xs={24} sm={18} md={20}>
          <Input.Search
            placeholder="Buscar por nombre, número o dirección..."
            value={searchText}
            onChange={handleSearch}
            size={isMobile ? "default" : "large"}
            style={{ width: '100%' }}
          />
        </Col>
      </Row>

      <Table
        dataSource={filteredPOS}
        columns={columns}
        rowKey="id"
        loading={loading}
        size={isMobile ? "small" : "default"}
        scroll={{ x: true }}
        pagination={{ pageSize: isMobile ? 6 : 10, size: isMobile ? "small" : "default" }}
      />

      <PointOfSaleEditModal
        open={isModalPointOfSaleOpen}
        initialValues={selectedRecord}
        onCancel={handleCancel}
        onSuccess={handleSuccess}
        width={isMobile ? "95%" : 500}
      />
    </PageLayout>
  );
};

export default PointOfSale;
