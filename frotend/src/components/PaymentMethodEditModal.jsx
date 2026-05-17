import React, { useState, useEffect } from "react";
import {
  Col,
  Form,
  Input,
  InputNumber,
  Modal,
  Row,
  message,
  ColorPicker,
  Grid,
} from "antd";
import { paymentMethodService } from "../services/paymentMethodService";
import { CreditCardOutlined } from "@ant-design/icons";

const { useBreakpoint } = Grid;

const PaymentMethodEditModal = ({
  open,
  onCancel,
  onSuccess,
  initialValues,
}) => {
  const [form] = Form.useForm();
  const [confirmLoading, setConfirmLoading] = useState(false);

  const screens = useBreakpoint();
  const isMobile = screens.md === false; // Detecta celulares y tablets chicas

  useEffect(() => {
    if (open) {
      if (initialValues) {
        setTimeout(() => {
          form.setFieldsValue({
            ...initialValues,
            color: initialValues.color || "#1677ff",
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

      const colorHex = typeof values.color === "string"
        ? values.color
        : values.color?.toHexString?.() || "#1677ff";

      const payload = {
        id: initialValues?.id ? initialValues.id : 0,
        name: values.name,
        // Fallback preventivo a 0 si el usuario borra el contenido del InputNumber
        descountPercentage: values.descountPercentage ?? 0,
        increasePercentage: values.increasePercentage ?? 0,
        color: colorHex
      };

      if (initialValues?.id) {
        await paymentMethodService.update(payload);
        message.success("Forma de pago actualizada");
      } else {
        await paymentMethodService.create(payload);
        message.success("Forma de pago creado");
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
          <CreditCardOutlined style={{ color: '#1677ff' }} />
          <span>{initialValues?.id ? "Editar Forma de Pago" : "Nueva Forma de Pago"}</span>
        </div>
      }
      open={open}
      onOk={handleOk}
      confirmLoading={confirmLoading}
      onCancel={onCancel}
      width={isMobile ? "95%" : 550} // Ajuste elástico del ancho general
      forceRender // Resuelve la inicialización del formulario sin delays
      okText="Guardar"
      cancelText="Cancelar"
      centered={isMobile}
    >
      <Form
        form={form}
        layout="vertical"
        preserve={false}
        style={{ marginTop: '16px' }}
        initialValues={{ color: '#1677ff', descountPercentage: 0, increasePercentage: 0 }}
      >
        <Row gutter={[16, 0]}>
          {/* NOMBRE DEL MÉTODO */}
          <Col xs={24} sm={16}>
            <Form.Item
              label="Nombre"
              name="name"
              rules={[{ required: true, message: 'Por favor ingrese el nombre (ej: Tarjeta Débito)' }]}
            >
              <Input placeholder="Efectivo, Transferencia..." size="large" />
            </Form.Item>
          </Col>

          {/* SELECCIÓN DE COLOR */}
          <Col xs={24} sm={8}>
            <Form.Item
              label="Color Identificador"
              name="color"
              getValueFromEvent={(color) => {
                return typeof color === "string" ? color : color.toHexString();
              }}
            >
              <ColorPicker
                showText
                disabledAlpha
                style={{ width: '100%', height: '40px', display: 'flex', alignItems: 'center' }}
              />
            </Form.Item>
          </Col>
        </Row>

        <Row gutter={[16, 0]}>
          {/* PORCENTAJE DE DESCUENTO */}
          <Col xs={12} sm={12}>
            <Form.Item label="Descuento" name="descountPercentage">
              <InputNumber
                placeholder="0%"
                min={0}
                max={100}
                size="large"
                style={{ width: '100%' }}
                formatter={(value) => `${value}%`}
                parser={(value) => value?.replace("%", "")}
              />
            </Form.Item>
          </Col>

          {/* PORCENTAJE DE INCREMENTO */}
          <Col xs={12} sm={12}>
            <Form.Item label="Recargo / Incremento" name="increasePercentage">
              <InputNumber
                placeholder="0%"
                min={0}
                max={100}
                size="large"
                style={{ width: '100%' }}
                formatter={(value) => `${value}%`}
                parser={(value) => value?.replace("%", "")}
              />
            </Form.Item>
          </Col>
        </Row>
      </Form>
    </Modal>
  );
};

export default PaymentMethodEditModal;
