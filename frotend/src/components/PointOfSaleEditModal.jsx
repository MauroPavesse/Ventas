import React, { useState, useEffect } from "react";
import { Col, Form, Input, InputNumber, Modal, Row, message, Grid } from "antd";
import { pointOfSaleService } from "../services/pointOfSaleService";
import { ShopOutlined } from "@ant-design/icons";

const { useBreakpoint } = Grid;

const PointOfSaleEditModal = ({ open, onCancel, onSuccess, initialValues }) => {
  const [form] = Form.useForm();
  const [confirmLoading, setConfirmLoading] = useState(false);

  const screens = useBreakpoint();
  const isMobile = screens.md === false; // Detecta celulares y tablets en vertical

  useEffect(() => {
    if (open) {
      if (initialValues) {
        setTimeout(() => {
          form.setFieldsValue({
            ...initialValues,
            provincie: initialValues.provincie || initialValues.province,
          });
        }, 0);
      } else {
        form.resetFields();
      }
    }
  }, [open, initialValues, form]);

  const handleOk = async () => {
    try {
      const values = await form.validateFields();

      setConfirmLoading(true);

      const payload = {
        id: initialValues?.id ? initialValues.id : 0,
        name: values.name,
        // Conversión segura que evita crasheos si el número viene indefinido
        number: values.number !== undefined && values.number !== null ? values.number.toString() : "0",
        address: values.address,
        city: values.city,
        provincie: values.provincie, // Mantenemos la propiedad esperada por tu servicio
        postalCode: values.postalCode,
      };

      if (initialValues?.id) {
        await pointOfSaleService.update(payload);
        message.success("Punto de Venta actualizado");
      } else {
        await pointOfSaleService.create(payload);
        message.success("Punto de Venta creado");
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
      title={
        <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
          <ShopOutlined style={{ color: '#1677ff' }} />
          <span>{initialValues?.id ? "Editar Punto de Venta" : "Nuevo Punto de Venta"}</span>
        </div>
      }
      open={open}
      onOk={handleOk}
      confirmLoading={confirmLoading}
      onCancel={onCancel}
      width={isMobile ? "95%" : 600} // Elástico en móviles, contenido en escritorio
      forceRender // Carga inmediata de campos en el DOM sin usar setTimeout
      okText="Guardar"
      cancelText="Cancelar"
      centered={isMobile}
      styles={{
        body: {
          maxHeight: isMobile ? 'calc(100vh - 200px)' : '70vh', // Protege la vista si emerge el teclado táctil
          overflowY: 'auto',
          padding: '4px 8px'
        }
      }}
    >
      <Form
        form={form}
        layout="vertical"
        preserve={false}
        style={{ marginTop: '16px' }}
      >
        <Row gutter={[12, 0]}>
          {/* NOMBRE */}
          <Col xs={24} sm={16}>
            <Form.Item
              label="Nombre del Punto de Venta"
              name="name"
              rules={[{ required: true, message: 'Por favor ingrese el nombre' }]}
            >
              <Input placeholder="Ej: Caja Central, Showroom" size="large" />
            </Form.Item>
          </Col>

          {/* NUMERACIÓN */}
          <Col xs={24} sm={8}>
            <Form.Item
              label="Numeración POS"
              name="number"
              rules={[{ required: true, message: 'Requerido' }]}
            >
              <InputNumber
                placeholder="Ej: 5"
                min={1}
                size="large"
                style={{ width: '100%' }}
              />
            </Form.Item>
          </Col>
        </Row>

        {/* PROVINCIA */}
        <Form.Item
          label="Provincia"
          name="provincie"
          rules={[{ required: true, message: 'Por favor ingrese la provincia' }]}
        >
          <Input placeholder="Buenos Aires" size="large" />
        </Form.Item>

        <Row gutter={[12, 0]}>
          {/* CIUDAD */}
          <Col xs={24} sm={16}>
            <Form.Item
              label="Ciudad / Localidad"
              name="city"
              rules={[{ required: true, message: 'Por favor ingrese la ciudad' }]}
            >
              <Input placeholder="General Lavalle" size="large" />
            </Form.Item>
          </Col>

          {/* CÓDIGO POSTAL */}
          <Col xs={24} sm={8}>
            <Form.Item
              label="Código Postal"
              name="postalCode"
              rules={[{ required: true, message: 'Requerido' }]}
            >
              <Input placeholder="7103" size="large" />
            </Form.Item>
          </Col>
        </Row>

        {/* DIRECCIÓN */}
        <Form.Item
          label="Dirección / Calle y Altura"
          name="address"
          rules={[{ required: true, message: 'Por favor ingrese la dirección' }]}
        >
          <Input placeholder="Av. Mitre 1580" size="large" />
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default PointOfSaleEditModal;
