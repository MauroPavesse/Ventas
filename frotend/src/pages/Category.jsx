import { Button, message, Table, Modal } from "antd";
import PageLayout from "../layouts/PageLayout";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { categoryService } from "../services/categoryService";
import { SearchCommand } from "../DTOs/SearchCommand";
import {
  DeleteOutlined,
  EditOutlined,
  ExclamationCircleOutlined,
} from "@ant-design/icons";
import CategoryEditModal from "../components/CategoryEditModal";

const Category = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [categories, setCategories] = useState();
  const [selectedRecord, setSelectedRecord] = useState(null);
  const [isModalCategoryOpen, setIsModalCategoryOpen] = useState(false);

  const fetchData = async () => {
    setLoading(true);
    try {
      const command = new SearchCommand();
      var res = await categoryService.search(command);
      setCategories(res);
    } catch (error) {
      message.error("Error al cargar las categorías: " + error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const columns = [
    { title: "Categoría", dataIndex: "name", key: "name" },
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
              setIsModalCategoryOpen(true);
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
          categories.filter((t) => t.id != record.id);
        } catch (e) {
          message.error("Error al eliminar: " + e);
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
      <Button type="primary" style={{ marginBottom: 10 }} onClick={addCategory}>
        Agregar
      </Button>

      <Table
        dataSource={categories}
        columns={columns}
        rowKey="id"
        loading={loading}
      />

      <CategoryEditModal
        open={isModalCategoryOpen}
        initialValues={selectedRecord}
        onCancel={handleCancel}
        onSuccess={handleSuccess}
      />
    </PageLayout>
  );
};

export default Category;
