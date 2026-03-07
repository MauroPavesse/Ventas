import React, { useEffect, useRef, useState } from "react";
import PageLayout from "../layouts/PageLayout";
import { useNavigate } from "react-router-dom";
import { Button, Input, message, Table, Modal, Row, Col, Image } from "antd";
import { SearchCommand } from "../DTOs/SearchCommand";
import { productService } from "../services/productService";
import {
  DeleteOutlined,
  EditOutlined,
  ExclamationCircleOutlined,
} from "@ant-design/icons";
import ProductEditModal from "../components/ProductEditModal";

const Products = () => {
  const navigate = useNavigate();

  const searchInputRef = useRef(null);
  const [products, setProducts] = useState([]);
  const [filteredProducts, setFilteredProducts] = useState([]);
  const [searchText, setSearchText] = useState("");
  const [isModalProductOpen, setIsModalProductOpen] = useState(false);
  const [selectedRecord, setSelectedRecord] = useState(null);
  const [loading, setLoading] = useState(false);

  // Carga inicial
  const fetchProducts = async () => {
    setLoading(true);
    try {
      const command = new SearchCommand({});
      const data = await productService.search(command); // Ajusta según tu servicio
      setProducts(data);
      setFilteredProducts(data);
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
      p.name.toLowerCase().includes(value),
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
          products.filter((t) => t.id != record.id);
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
      render: (src) => (
        <Image
          src={src}
          alt="producto"
          width={50}
          fallback="https://via.placeholder.com/50?text=Sin+Imagen" // Por si la URL falla
          style={{ borderRadius: "4px", objectFit: "cover" }}
        />
      ),
    },
    { title: "Código", dataIndex: "code", key: "code" },
    { title: "Producto", dataIndex: "name", key: "name" },
    { title: "Descripción", dataIndex: "description", key: "description" },
    {
      title: "Categoría",
      key: "category",
      render: (_, record) => record.category?.name || "Sin categoría",
    },
    {
      title: "Precio",
      dataIndex: "price",
      key: "price",
      render: (p) => `$${p}`,
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
              setIsModalProductOpen(true);
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

  return (
    <PageLayout title="Productos" onClose={() => navigate("/dashboard")}>
      <Row gutter={15} style={{ alignItems: "baseline" }}>
        <Col>
          <Button
            type="primary"
            style={{ marginBottom: 10 }}
            onClick={addProduct}
          >
            Agregar
          </Button>
        </Col>
        <Col span={14}>
          <Input.Search
            ref={searchInputRef}
            placeholder="Buscar producto"
            value={searchText}
            onChange={handleSearch}
            autoFocus
            size="large"
          />
        </Col>
      </Row>

      <Table
        dataSource={filteredProducts}
        columns={productColumns}
        loading={loading}
        rowKey="id"
        pagination={{ pageSize: 5 }}
      />

      <ProductEditModal
        open={isModalProductOpen}
        initialValues={selectedRecord}
        onCancel={handleCancel}
        onSuccess={handleSuccess}
      />
    </PageLayout>
  );
};

export default Products;
