import React, { useEffect, useState } from "react";
import { Button, message, Table, Modal, Input, Row, Col, Grid } from "antd";
import PageLayout from "../layouts/PageLayout";
import { useNavigate } from "react-router-dom";
import { categoryService } from "../services/categoryService";
import { SearchCommand } from "../DTOs/SearchCommand";
import {
  DeleteOutlined,
  EditOutlined,
  ExclamationCircleOutlined,
  PlusOutlined
} from "@ant-design/icons";
import CategoryEditModal from "../components/CategoryEditModal";

const { useBreakpoint } = Grid;
const { confirm } = Modal;

const Category = () => {
  const navigate = useNavigate();
  const screens = useBreakpoint();
  const isMobile = screens.md === false; // Detecta resoluciones móviles y tablets chicas

  const [loading, setLoading] = useState(false);
  const [categories, setCategories] = useState([]);
  const [filteredCategories, setFilteredCategories] = useState([]);
  const [searchText, setSearchText] = useState("");
  const [selectedRecord, setSelectedRecord] = useState(null);
  const [isModalCategoryOpen, setIsModalCategoryOpen] = useState(false);

  const fetchData = async () => {
    setLoading(true);
    try {
      const command = new SearchCommand();
      var res = await categoryService.search(command);
      setCategories(res || []);
      setFilteredCategories(res || []);
    } catch (error) {
      message.error("Error al cargar las categorías: " + error);
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
    const filtered = categories.filter((c) =>
      (c.name || "").toLowerCase().includes(value)
    );
    setFilteredCategories(filtered);
  };

  const columns = [
    { 
      title: "Categoría", 
      dataIndex: "name", 
      key: "name",
      render: (text) => (
        <span style={{ fontWeight: 600, fontSize: isMobile ? '13px' : '14px' }}>
          {text}
        </span>
      )
    },
    {
      title: "Acción",
      key: "action",
      fixed: isMobile ? false : "right",
      width: isMobile ? 90 : 100,
      render: (_, record) => (
        <div style={{ display: "flex", gap: "6px" }}>
          <Button
            size={isMobile ? "small" : "default"}
            icon={<EditOutlined />}
            onClick={() => {
              setSelectedRecord(record);
              setIsModalCategoryOpen(true);
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

  const addCategory = () => {
    setSelectedRecord(null);
    setIsModalCategoryOpen(true);
  };

  const { confirm } = Modal;

  const handleDelete = (record) => {
    confirm({
      title: "¿Estás seguro de eliminar esta categoría?",
      icon: <ExclamationCircleOutlined />,
      content: `Categoría: ${record.name}`,
      okText: "Sí, eliminar",
      okType: "danger",
      cancelText: "Cancelar",
      onOk: async () => {
        try {
          await categoryService.delete(record.id);
          message.success("Eliminado correctamente");
          setCategories((prev) => prev.filter((t) => t.id !== record.id));
          setFilteredCategories((prev) => prev.filter((t) => t.id !== record.id));
        } catch (errorMsg) {
          message.error(errorMsg);
        }
      },
    });
  };

  const handleCancel = () => {
    setIsModalCategoryOpen(false);
    setSelectedRecord(null);
  };

  const handleSuccess = () => {
    setIsModalCategoryOpen(false);
    fetchData();
  };

  return (
    <PageLayout title="Categorías" onClose={() => navigate("/configurations")}>
      {/* Barra superior de control fluida */}
      <Row gutter={[16, 16]} style={{ marginBottom: 16 }} align="middle">
        <Col xs={24} sm={6} md={4}>
          <Button 
            type="primary" 
            icon={<PlusOutlined />} 
            onClick={addCategory}
            block={isMobile}
            size={isMobile ? "large" : "default"}
          >
            Agregar Categoría
          </Button>
        </Col>
        <Col xs={24} sm={18} md={20}>
          <Input.Search
            placeholder="Buscar categorías por nombre..."
            value={searchText}
            onChange={handleSearch}
            size={isMobile ? "default" : "large"}
            style={{ width: "100%" }}
            allowClear
          />
        </Col>
      </Row>

      <Table
        dataSource={filteredCategories}
        columns={columns}
        rowKey="id"
        loading={loading}
        size={isMobile ? "small" : "default"}
        scroll={{ x: true }}
        pagination={{ 
          pageSize: isMobile ? 8 : 10, 
          size: isMobile ? "small" : "default",
          showTotal: (total, range) => `${range[0]}-${range[1]} de ${total}`
        }}
      />

      <CategoryEditModal
        open={isModalCategoryOpen}
        initialValues={selectedRecord}
        onCancel={handleCancel}
        onSuccess={handleSuccess}
        width={isMobile ? "95%" : 450}
      />
    </PageLayout>
  );
};

export default Category;
