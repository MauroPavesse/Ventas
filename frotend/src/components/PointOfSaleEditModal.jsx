import { Col, Form, Input, InputNumber, Modal, Row, message } from "antd";
import { useState } from "react";
import { pointOfSaleService } from "../services/pointOfSaleService";

const PointOfSaleEditModal = ({ open, onCancel, onSuccess, initialValues }) => {
  const [form] = Form.useForm();
  const [confirmLoading, setConfirmLoading] = useState(false);

  const handleOk = async () => {
    try {
      const values = await form.validateFields();

      setConfirmLoading(true);

      const payload = {
        id: initialValues?.id ? initialValues.id : 0,
        name: values.name,
        number: values.number.toString(),
        address: values.address,
        city: values.city,
        provincie: values.provincie,
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
        initialValues?.id ? "Editar Punto de Venta" : "Nuevo Punto de Venta"
      }
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
              <Input placeholder="Punto de venta 05" />
            </Form.Item>
          </Col>
          <Col span={6}>
            <Form.Item label="Numeración" name="number">
              <InputNumber placeholder="5" min={1} />
            </Form.Item>
          </Col>
        </Row>
        <Form.Item label="Provincia" name="provincie">
          <Input placeholder="Buenos Aires" />
        </Form.Item>
        <Row gutter={15}>
          <Col span={14}>
            <Form.Item label="Ciudad" name="city">
              <Input placeholder="General Lavalle" />
            </Form.Item>
          </Col>
          <Col span={10}>
            <Form.Item label="Codigo postal" name="postalCode">
              <Input placeholder="7103" />
            </Form.Item>
          </Col>
        </Row>

        <Form.Item label="Dirección" name="address">
          <Input placeholder="Av. Mitre 1580" />
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default PointOfSaleEditModal;
