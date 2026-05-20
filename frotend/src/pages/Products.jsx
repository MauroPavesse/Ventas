import React, { useEffect, useRef, useState } from "react";
import PageLayout from "../layouts/PageLayout";
import { useNavigate } from "react-router-dom";
import { Button, Input, message, Table, Modal, Row, Col, Image, Grid } from "antd";
import { SearchCommand } from "../DTOs/SearchCommand";
import { productService } from "../services/productService";
import {
  DeleteOutlined,
  EditOutlined,
  ExclamationCircleOutlined,
  PlusOutlined
} from "@ant-design/icons";
import ProductEditModal from "../components/ProductEditModal";

const noImagePlaceholder = "data:image/svg+xml;utf8,<svg xmlns='http://www.w3.org/2000/svg' width='50' height='50' viewBox='0 0 50 50'><rect width='100%' height='100%' fill='%23eee'/><text x='50%' y='50%' font-family='sans-serif' font-size='8' fill='%23aaa' dominant-baseline='middle' text-anchor='middle'>Sin Imagen</text></svg>";
const { useBreakpoint } = Grid;

const Products = () => {
  const navigate = useNavigate();
  const screens = useBreakpoint();
  const isMobile = screens.md === false;

  const searchInputRef = useRef(null);
  const [products, setProducts] = useState([]);
  const [filteredProducts, setFilteredProducts] = useState([]);
  const [searchText, setSearchText] = useState("");
  const [isModalProductOpen, setIsModalProductOpen] = useState(false);
  const [selectedRecord, setSelectedRecord] = useState(null);
  const [loading, setLoading] = useState(false);

  const currencyFormatter = new Intl.NumberFormat("es-AR", {
    style: "currency",
    currency: "ARS",
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  });

  // Carga inicial
  const fetchProducts = async () => {
    setLoading(true);
    try {
      const command = new SearchCommand({});
      const data = await productService.search(command); // Ajusta según tu servicio
      setProducts(data || []);
      setFilteredProducts(data || []);
    } catch (error) {
      message.error("Error al cargar productos: " + error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchProducts();
  }, []);

  const { confirm } = Modal;

  const handleCancel = () => {
    setIsModalProductOpen(false);
    setSelectedRecord(null);
  };

  const handleSuccess = () => {
    setIsModalProductOpen(false);
    fetchProducts();
  };

  const addProduct = () => {
    setSelectedRecord(null);
    setIsModalProductOpen(true);
  };

  const handleSearch = (e) => {
    const value = e.target.value.toLowerCase();
    setSearchText(value);
    const filtered = products.filter((p) =>
      p.name.toLowerCase().includes(value) ||
      (p.code && p.code.toLowerCase().includes(value)) ||
      p.codeBar.includes(value)
    );
    setFilteredProducts(filtered);
  };

  const handleDelete = (record) => {
    confirm({
      title: "¿Estás seguro de eliminar este producto?",
      icon: <ExclamationCircleOutlined />,
      content: `Producto: ${record.name}`,
      okText: "Sí, eliminar",
      okType: "danger",
      cancelText: "Cancelar",
      onOk: async () => {
        try {
          await productService.delete(record.id);
          message.success("Eliminado correctamente");
          setProducts((prev) => prev.filter((t) => t.id !== record.id));
          setFilteredProducts((prev) => prev.filter((t) => t.id !== record.id));
        } catch (e) {
          message.error("Error al eliminar: " + e);
        }
      },
    });
  };

  // Columnas
  const productColumns = [
    {
      title: "Imagen",
      dataIndex: "imagePath",
      key: "imagePath",
      width: isMobile ? 60 : 80,
      render: (src) => (
        <Image
          // SI src está vacío, le pasamos null o la url por defecto directamente
          src={src ? src : noImagePlaceholder}
          alt="producto"
          width={isMobile ? 40 : 50}
          height={isMobile ? 40 : 50}
          fallback={noImagePlaceholder}
          style={{ borderRadius: "4px", objectFit: "cover" }}
        />
      ),
    },
    {
      title: "Código",
      dataIndex: "code",
      key: "code",
      responsive: ['sm'], // Oculto en celulares muy chicos, visible en tablets en adelante
    },
    {
      title: "Producto",
      dataIndex: "name",
      key: "name",
      render: (text, record) => (
        <div>
          <b style={{ fontSize: isMobile ? '13px' : '14px' }}>{text}</b>
          {isMobile && record.code && <div style={{ fontSize: '11px', color: '#8c8c8c' }}>Cód: {record.code}</div>}
        </div>
      )
    },
    {
      title: "Descripción",
      dataIndex: "description",
      key: "description",
      responsive: ['md'], // Solo visible en pantallas medianas/grandes de PC
    },
    {
      title: "Categoría",
      key: "category",
      responsive: ['sm'], // Se oculta en móvil para dar aire al diseño
      render: (_, record) => record.category?.name || "Sin categoría",
    },
    {
      title: "Precio",
      dataIndex: "sellingPrice",
      key: "sellingPrice",
      width: isMobile ? 80 : 100,
      render: (p) => <b>{currencyFormatter.format(p)}</b>,
    },
    {
      title: "Acción",
      key: "action",
      fixed: isMobile ? false : "right",
      width: isMobile ? 90 : 110,
      render: (_, record) => (
        <div style={{ display: 'flex', gap: '4px' }}>
          <Button
            size={isMobile ? "small" : "default"}
            icon={<EditOutlined />}
            onClick={() => {
              setSelectedRecord(record);
              setIsModalProductOpen(true);
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
    <PageLayout title="Productos" onClose={() => navigate("/dashboard")}>
      {/* Sistema de buscador y alta responsivo */}
      <Row gutter={[16, 16]} style={{ marginBottom: 16 }} align="middle">
        <Col xs={24} sm={6} md={4}>
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={addProduct}
            block={isMobile}
            size={isMobile ? "large" : "default"}
          >
            Agregar Producto
          </Button>
        </Col>
        <Col xs={24} sm={18} md={20}>
          <Input.Search
            ref={searchInputRef}
            placeholder="Buscar por nombre o código..."
            value={searchText}
            onChange={handleSearch}
            autoFocus
            size={isMobile ? "default" : "large"}
            style={{ width: '100%' }}
          />
        </Col>
      </Row>

      <Table
        dataSource={filteredProducts}
        columns={productColumns}
        loading={loading}
        rowKey="id"
        pagination={{ pageSize: isMobile ? 6 : 5, size: isMobile ? "small" : "default" }}
        scroll={{ x: true }}
        size={isMobile ? "small" : "default"}
      />

      <ProductEditModal
        open={isModalProductOpen}
        initialValues={selectedRecord}
        onCancel={handleCancel}
        onSuccess={handleSuccess}
        width={isMobile ? "95%" : 600} // Ajuste del modal emergente para pantallas táctiles
      />
    </PageLayout>
  );
};

export default Products;
