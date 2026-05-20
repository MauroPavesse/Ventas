import React, { useState, useEffect } from "react";
import {
  message,
  Form,
  Input,
  Modal,
  Select,
  Upload,
  Image,
  Tabs,
  Button,
  Row,
  Col,
  InputNumber,
  Grid,
} from "antd";
import { UploadOutlined, LinkOutlined, BarcodeOutlined } from "@ant-design/icons";
import { productService } from "../services/productService";
import { SearchCommand } from "../DTOs/SearchCommand";
import { categoryService } from "../services/categoryService";
import { taxRateService } from "../services/taxRateService";

const { useBreakpoint } = Grid;

const ProductEditModal = ({ open, onCancel, onSuccess, initialValues }) => {
  const [form] = Form.useForm();
  const [confirmLoading, setConfirmLoading] = useState(false);
  const [loadingLists, setLoadingLists] = useState(false);
  const [categories, setCategories] = useState([]);
  const [taxRates, setTaxRates] = useState([]);
  const [imageUrl, setImageUrl] = useState("");

  const screens = useBreakpoint();
  const isMobile = screens.md === false; // Detecta celulares y pantallas compactas

  const unitTypeEnum = [
    { id: 1, name: "Unidad (Ud)" },
    { id: 2, name: "Kilogramo (Kg)" },
    { id: 3, name: "Litro (L)" },
  ];

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

      setCategories(resCategory || []);
      setTaxRates(resTaxRate || []);
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
        name: values.name || "",
        description: values.description || "",
        imagePath: values.imagePath || "",
        sellingPrice: values.sellingPrice || 0,
        costPrice: values.costPrice || 0,
        codeBar: values.codeBar || "",
        categoryId: values.categoryId,
        taxRateId: values.taxRateId,
        unitOfMeasurement: values.unitOfMeasurement
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
      message.error(error);
      if (error.errorFields) {
        console.log("Campos inválidos en formulario:", error.errorFields);
      } else {
        const errorMsg = error.response?.data?.Message || error.response?.data?.message || "Error al guardar el producto";
        message.error(errorMsg);
      }
    } finally {
      setConfirmLoading(false);
    }
  };

  return (
    <Modal
      title={
        <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
          <BarcodeOutlined style={{ color: '#1677ff' }} />
          <span>{initialValues?.id ? "Editar Producto" : "Nuevo Producto"}</span>
        </div>
      }
      open={open}
      onOk={handleOk}
      confirmLoading={confirmLoading}
      onCancel={onCancel}
      width={isMobile ? "95%" : 750} // Ancho elástico inteligente para escritorio y tablets
      forceRender
      okText="Guardar"
      cancelText="Cancelar"
      centered={isMobile}
      styles={{
        body: {
          maxHeight: isMobile ? 'calc(100vh - 180px)' : '75vh',
          overflowY: 'auto',
          padding: '4px 4px'
        }
      }}
    >
      <Form
        form={form}
        layout="vertical"
        preserve={false}
        style={{ marginTop: '16px' }}
      >
        <Row gutter={[16, 0]}>
          {/* COLUMNA IZQUIERDA: DATOS PRINCIPALES */}
          <Col xs={24} md={15}>
            <Form.Item
              label="Nombre del Producto"
              name="name"
              rules={[{ required: true, message: 'Por favor ingrese el nombre del artículo' }]}
            >
              <Input placeholder="Ej. Helado Arcor Chocolate" size="large" />
            </Form.Item>

            <Row gutter={[12, 0]}>
              <Col xs={24} sm={10}>
                <Form.Item label="Código Interno" name="code">
                  <Input placeholder="COD-001" size="large" />
                </Form.Item>
              </Col>
              <Col xs={24} sm={14}>
                <Form.Item label="Código de Barras" name="codeBar">
                  <Input placeholder="789123456789" size="large" />
                </Form.Item>
              </Col>
            </Row>

            <Form.Item
              label="Categoría"
              name="categoryId"
              rules={[{ required: false, message: 'Seleccione una categoría' }]}
            >
              <Select
                placeholder="Seleccione categoría de catálogo"
                loading={loadingLists}
                size="large"
                options={categories.map((c) => ({
                  value: c.id,
                  label: c.name,
                }))}
              />
            </Form.Item>

            <Row gutter={[12, 0]}>
              <Col xs={12} sm={12}>
                <Form.Item
                  label="Precio de Venta"
                  name="sellingPrice"
                  rules={[{ required: true, message: 'Ingrese precio' }]}
                >
                  <InputNumber
                    prefix="$"
                    placeholder="0.00"
                    size="large"
                    style={{ width: '100%' }}
                    min={0}
                  />
                </Form.Item>
              </Col>
              <Col xs={12} sm={12}>
                <Form.Item
                  label="Precio de Costo"
                  name="costPrice"
                  rules={[{ required: true, message: 'Ingrese precio' }]}
                >
                  <InputNumber
                    prefix="$"
                    placeholder="0.00"
                    size="large"
                    style={{ width: '100%' }}
                    min={0}
                  />
                </Form.Item>
              </Col>
              <Col xs={12} sm={12}>
                <Form.Item
                  label="Tasa de IVA"
                  name="taxRateId"
                  rules={[{ required: true, message: 'Seleccione IVA' }]}
                >
                  <Select
                    placeholder="21%"
                    size="large"
                    loading={loadingLists}
                    options={taxRates.map((t) => ({
                      value: t.id,
                      label: t.description,
                    }))}
                  />
                </Form.Item>
              </Col>
              <Col xs={12} sm={12}>
                <Form.Item
                  label="Tipo de Unidad"
                  name="unitOfMeasurement"
                  rules={[{ required: true, message: 'Seleccione un tipo de unidad' }]}
                >
                  <Select
                    placeholder="Seleccione un tipo de unidad"
                    size="large"
                    options={unitTypeEnum.map((c) => ({
                      value: c.id,
                      label: c.name,
                    }))}
                  />
                </Form.Item>
              </Col>
            </Row>
          </Col>

          {/* COLUMNA DERECHA: MEDIA/IMAGEN */}
          <Col xs={24} md={9} style={{ textAlign: "center", marginTop: isMobile ? '16px' : '0' }}>
            <Form.Item label="Imagen Ilustrativa">
              <div
                style={{
                  marginBottom: 12,
                  border: "1px dashed #d9d9d9",
                  borderRadius: "8px",
                  padding: "8px",
                  height: "150px",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  backgroundColor: "#fafafa"
                }}
              >
                {imageUrl ? (
                  <Image
                    src={imageUrl}
                    alt="preview"
                    style={{ maxHeight: "130px", maxWidth: "100%", objectFit: "contain" }}
                  />
                ) : (
                  <span style={{ color: "#bfbfbf", fontSize: '13px' }}>Sin imagen configurada</span>
                )}
              </div>

              <Tabs
                defaultActiveKey="1"
                type="card"
                size="small"
                items={[
                  {
                    key: "1",
                    label: (
                      <span>
                        <LinkOutlined /> Web URL
                      </span>
                    ),
                    children: (
                      <div style={{ padding: '8px 0' }}>
                        <Form.Item name="imagePath" noStyle>
                          <Input
                            placeholder="https://imagenes.com/foto.jpg"
                            onChange={(e) => setImageUrl(e.target.value)}
                            size="large"
                          />
                        </Form.Item>
                      </div>
                    ),
                  },
                  {
                    key: "2",
                    label: (
                      <span>
                        <UploadOutlined /> Subir File
                      </span>
                    ),
                    children: (
                      <div style={{ padding: '8px 0' }}>
                        <Upload
                          beforeUpload={handleFileUpload}
                          showUploadList={false}
                          maxCount={1}
                        >
                          <Button icon={<UploadOutlined />} size="large" block>
                            Buscar archivo local
                          </Button>
                        </Upload>
                      </div>
                    ),
                  },
                ]}
              />
            </Form.Item>
          </Col>
        </Row>

        {/* DESCRIPCIÓN EXTENDIDA */}
        <Form.Item label="Descripción Extendida" name="description">
          <Input.TextArea rows={3} placeholder="Detalles comerciales opcionales del producto..." />
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default ProductEditModal;
