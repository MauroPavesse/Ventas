import { Col, Form, Input, Select, Modal, Row, message } from "antd";
import { useState, useEffect } from "react";
import { userService } from "../services/userService";
import { rolService } from "../services/rolService";
import { pointOfSaleService } from "../services/pointOfSaleService";
import { SearchCommand } from "../DTOs/SearchCommand";

const UserEditModal = ({ open, onCancel, onSuccess, initialValues }) => {
  const [form] = Form.useForm();
  const [confirmLoading, setConfirmLoading] = useState(false);
  const [loadingLists, setLoadingLists] = useState(false);
  const [roles, setRoles] = useState([]);
  const [pointOfSales, setPointOfSales] = useState([]);

  useEffect(() => {
    if (open) {
      loadSelectData();
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

  const loadSelectData = async () => {
    setLoadingLists(true);
    try {
      const command = new SearchCommand();
      const [resRole, resPointOfSale] = await Promise.all([
        rolService.search(command),
        pointOfSaleService.search(command),
      ]);

      setRoles(resRole);
      setPointOfSales(resPointOfSale);
    } catch (error) {
      message.error("Error al cargar listas: " + error);
    } finally {
      setLoadingLists(false);
    }
  };

  const handleOk = async () => {
    try {
      const values = await form.validateFields();

      setConfirmLoading(true);

      const payload = {
        id: initialValues?.id ? initialValues.id : 0,
        username: values.username,
        password: values.password,
        roleId: values.roleId > 0 ? values.roleId : null,
        pointOfSaleId: values.pointOfSaleId > 0 ? values.pointOfSaleId : null,
      };

      if (initialValues?.id) {
        await userService.update(payload);
        message.success("Personal actualizado");
      } else {
        await userService.create(payload);
        message.success("Personal creado");
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
      title={initialValues?.id ? "Editar Personal" : "Nuevo Personal"}
      open={open}
      onOk={handleOk}
      confirmLoading={confirmLoading}
      onCancel={onCancel}
      width={700}
    >
      <Form form={form} layout="vertical" preserve={false}>
        <Row gutter={15}>
          <Col span={18}>
            <Form.Item label="Usuario" name="username">
              <Input placeholder="Cajero" />
            </Form.Item>
          </Col>
          <Col span={6}>
            <Form.Item label="Clave" name="password">
              <Input.Password placeholder="Clave" />
            </Form.Item>
          </Col>
        </Row>
        <Form.Item label="Role" name="roleId">
          <Select
            placeholder="Seleccione rol"
            loading={loadingLists}
            options={roles.map((c) => ({
              value: c.id,
              label: c.name,
            }))}
          />
        </Form.Item>
        <Form.Item label="Punto de venta" name="pointOfSaleId">
          <Select
            placeholder="Seleccione POS"
            loading={loadingLists}
            options={pointOfSales.map((c) => ({
              value: c.id,
              label: c.name,
            }))}
          />
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default UserEditModal;
