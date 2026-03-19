import {
  Col,
  Form,
  Input,
  InputNumber,
  Modal,
  Row,
  message,
  ColorPicker,
} from "antd";
import { useState, useEffect } from "react";
import { paymentMethodService } from "../services/paymentMethodService";

const PaymentMethodEditModal = ({
  open,
  onCancel,
  onSuccess,
  initialValues,
}) => {
  const [form] = Form.useForm();
  const [confirmLoading, setConfirmLoading] = useState(false);

  useEffect(() => {
    if (open) {
      if (initialValues) {
        setTimeout(() => {
          form.setFieldsValue({
            ...initialValues,
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
        descountPercentage: values.descountPercentage,
        increasePercentage: values.increasePercentage,
        color: values.color
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
      title={initialValues?.id ? "Editar Forma de pago" : "Nueva Forma de pago"}
      open={open}
      onOk={handleOk}
      confirmLoading={confirmLoading}
      onCancel={onCancel}
      width={700}
      destroyOnClose
    >
      <Form form={form} layout="vertical" preserve={false}>
        <Row gutter={15}>
          <Col span={18}>
            <Form.Item label="Nombre" name="name">
              <Input placeholder="Efectivo" />
            </Form.Item>
          </Col>
          <Col span={5}>
            <Form.Item
              label="Color"
              name="color"
              getValueFromEvent={(color) => {
                return typeof color === "string" ? color : color.toHexString();
              }}
            >
              <ColorPicker defaultValue="#1677ff" />
            </Form.Item>
          </Col>
        </Row>
        <Row gutter={15}>
          <Col span={6}>
            <Form.Item label="Descuento" name="descountPercentage">
              <InputNumber
                placeholder="20%"
                min={0}
                formatter={(value) => `${value}%`}
                parser={(value) => value?.replace("%", "")}
              />
            </Form.Item>
          </Col>
          <Col span={6}>
            <Form.Item label="Incremento" name="increasePercentage">
              <InputNumber
                placeholder="15%"
                min={0}
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
