import {
  message,
  Form,
  Input,
  Modal,
  Select,
  Upload,
  Image,
  Tabs,
  Space,
  Button,
  Row,
  Col,
} from "antd";
import { UploadOutlined, LinkOutlined } from "@ant-design/icons";
import { useState, useEffect } from "react";
import { productService } from "../services/productService";
import { SearchCommand } from "../DTOs/SearchCommand";
import { categoryService } from "../services/categoryService";
import { taxRateService } from "../services/taxRateService";

const ProductEditModal = ({ open, onCancel, onSuccess, initialValues }) => {
  const [form] = Form.useForm();
  const [confirmLoading, setConfirmLoading] = useState(false);
  const [loadingLists, setLoadingLists] = useState(false);
  const [categories, setCategories] = useState([]);
  const [taxRates, setTaxRates] = useState([]);
  const [imageUrl, setImageUrl] = useState("");

  useEffect(() => {
    if (open) {
      loadSelectData();
      if (initialValues) {
        setTimeout(() => {
          form.setFieldsValue({
            ...initialValues,
          });
          setImageUrl(initialValues.imagePath || "");
        }, 0);
      } else {
        form.resetFields();
        setImageUrl("");
      }
    }
  }, [open, initialValues, form]);

  const loadSelectData = async () => {
    setLoadingLists(true);
    try {
      const command = new SearchCommand();
      const [resCategory, resTaxRate] = await Promise.all([
        categoryService.search(command),
        taxRateService.search(command),
      ]);

      setCategories(resCategory);
      setTaxRates(resTaxRate);
    } catch (error) {
      message.error("Error al cargar listas: " + error);
    } finally {
      setLoadingLists(false);
    }
  };

  const handleFileUpload = (info) => {
    // Aquí podrías subirlo a un servidor y obtener la URL,
    // o usar Base64 para previsualización local:
    const reader = new FileReader();
    reader.onload = (e) => {
      const url = e.target.result;
      setImageUrl(url);
      form.setFieldValue("imagePath", url); // Guardamos el base64 o la URL en el form
    };
    reader.readAsDataURL(info.file);
    return false; // Evita la subida automática por defecto de AntD
  };

  const handleOk = async () => {
    try {
      const values = await form.validateFields();

      setConfirmLoading(true);

      const payload = {
        id: initialValues?.id ? initialValues.id : 0,
        code: values.code,
        name: values.name,
        description: values.description,
        imagePath: values.imagePath,
        price: values.price,
        codeBar: values.codeBar,
        categoryId: values.categoryId,
        taxRateId: values.taxRateId,
      };

      if (initialValues?.id) {
        await productService.update(payload);
        message.success("Producto actualizado");
      } else {
        await productService.create(payload);
        message.success("Producto creado");
      }

      onSuccess();
    } catch (error) {
      console.error(error);
      message.error("Error al guardar");
    } finally {
      setConfirmLoading(false);
    }
  };

  return (
    <Modal
      title={initialValues?.id ? "Editar Producto" : "Nuevo Producto"}
      open={open}
      onOk={handleOk}
      confirmLoading={confirmLoading}
      onCancel={onCancel}
      width={700}
    >
      <Form form={form} layout="vertical" preserve={false}>
        <div style={{ display: "flex", gap: "20px" }}>
          {/* Columna Izquierda: Datos */}
          <div style={{ flex: 2 }}>
            <Form.Item
              label="Nombre del Producto"
              name="name"
              rules={[{ required: true }]}
            >
              <Input placeholder="Ej. Helado Arcor" />
            </Form.Item>

            <Row gutter={10} style={{ alignItems: "baseline" }}>
              <Col span={8}>
                <Form.Item label="Código Interno" name="code">
                  <Input placeholder="COD-001" />
                </Form.Item>
              </Col>
              <Col span={16}>
                <Form.Item label="Código de Barras" name="codeBar">
                  <Input placeholder="789..." />
                </Form.Item>
              </Col>
            </Row>

            <Form.Item label="Categoría" name="categoryId">
              <Select
                placeholder="Seleccione categoría"
                loading={loadingLists}
                options={categories.map((c) => ({
                  value: c.id,
                  label: c.name,
                }))}
              />
            </Form.Item>

            <div
              style={{
                display: "grid",
                gridTemplateColumns: "1fr 1fr",
                gap: "10px",
              }}
            >
              <Form.Item label="Precio" name="price">
                <Input prefix="$" type="number" />
              </Form.Item>
              <Form.Item label="IVA" name="taxRateId">
                <Select
                  options={taxRates.map((t) => ({
                    value: t.id,
                    label: t.description,
                  }))}
                />
              </Form.Item>
            </div>
          </div>

          {/* Columna Derecha: Imagen */}
          <div style={{ flex: 1, textAlign: "center" }}>
            <Form.Item label="Imagen del Producto">
              <div
                style={{
                  marginBottom: 10,
                  border: "1px dashed #d9d9d9",
                  borderRadius: "8px",
                  padding: "5px",
                  minHeight: "150px",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                }}
              >
                {imageUrl ? (
                  <Image
                    src={imageUrl}
                    alt="preview"
                    style={{ maxHeight: "140px", objectFit: "contain" }}
                  />
                ) : (
                  <span style={{ color: "#999" }}>Sin imagen</span>
                )}
              </div>

              <Tabs
                defaultActiveKey="1"
                items={[
                  {
                    key: "1",
                    label: (
                      <span>
                        <LinkOutlined /> URL
                      </span>
                    ),
                    children: (
                      <Form.Item name="imagePath" noStyle>
                        <Input
                          placeholder="http://..."
                          onChange={(e) => setImageUrl(e.target.value)}
                        />
                      </Form.Item>
                    ),
                  },
                  {
                    key: "2",
                    label: (
                      <span>
                        <UploadOutlined /> Subir
                      </span>
                    ),
                    children: (
                      <Upload
                        beforeUpload={handleFileUpload}
                        showUploadList={false}
                        maxCount={1}
                      >
                        <Button icon={<UploadOutlined />} block>
                          Seleccionar archivo
                        </Button>
                      </Upload>
                    ),
                  },
                ]}
              />
            </Form.Item>
          </div>
        </div>

        <Form.Item label="Descripción" name="description">
          <Input.TextArea rows={2} placeholder="Descripción breve..." />
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default ProductEditModal;
