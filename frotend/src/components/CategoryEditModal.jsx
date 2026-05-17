import React, { useEffect, useState } from "react";
import { Form, Input, Modal, message } from "antd";
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
        id: initialValues?.id || 0,
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
      if (typeof error === "string") {
        message.error(error);
      }
      // Si el error viene de form.validateFields() de AntD (campos vacíos en el front)
      else if (error.errorFields) {
        console.log("Validación local fallida", error);
      }
      // Cualquier otro error inesperado
      else {
        message.error("Error inesperado al procesar la solicitud");
      }
    } finally {
      setConfirmLoading(false);
    }
  };

  return (
    <Modal
      title={initialValues?.id ? "Editar Categoría" : "Nueva Categoría"}
      open={open}
      onOk={handleOk}
      confirmLoading={confirmLoading}
      onCancel={onCancel}
      // Si el componente padre envía un ancho (ej: 95% en móvil), lo adopta; si no, usa un ancho base de 500
      width={500}
      forceRender // Asegura que el formulario esté en el DOM para inyectar campos sin delays
      okText="Guardar"
      cancelText="Cancelar"
    >
      <Form
        form={form}
        layout="vertical"
        preserve={false}
        style={{ marginTop: '16px' }}
      >
        <Form.Item
          label="Nombre de la categoría"
          name="name"
          rules={[{ required: true, message: 'Por favor ingrese el nombre' }]}
        >
          <Input
            placeholder="Ej: Frios, Bebidas, Almacén"
            size="large" // Input cómodo para pulsación táctil
          />
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default CategoryEditModal;
