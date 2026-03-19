import { Col, Form, Input, Select, Modal, Row, message } from "antd";
import { useState, useEffect } from "react";
import { categoryService } from "../services/categoryService";

const CategoryEditModal = ({ open, onCancel, onSuccess, initialValues }) => {
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
      };

      if (initialValues?.id) {
        await categoryService.update(payload);
        message.success("Categoría actualizado");
      } else {
        await categoryService.create(payload);
        message.success("Categoría creado");
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
      title={initialValues?.id ? "Editar Categoría" : "Nuevo Categoría"}
      open={open}
      onOk={handleOk}
      confirmLoading={confirmLoading}
      onCancel={onCancel}
      width={700}
      destroyOnClose
    >
      <Form form={form} layout="vertical" preserve={false}>
        <Form.Item label="Nombre de la categoría" name="name">
            <Input placeholder="Frios" />
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default CategoryEditModal;
