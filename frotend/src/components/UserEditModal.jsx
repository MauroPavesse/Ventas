import React, { useState, useEffect } from "react";
import { Col, Form, Input, Select, Modal, Row, message, Grid } from "antd";
import { userService } from "../services/userService";
import { rolService } from "../services/rolService";
import { pointOfSaleService } from "../services/pointOfSaleService";
import { SearchCommand } from "../DTOs/SearchCommand";
import { UserOutlined } from "@ant-design/icons";

const { useBreakpoint } = Grid;

const UserEditModal = ({ open, onCancel, onSuccess, initialValues }) => {
  const [form] = Form.useForm();
  const [confirmLoading, setConfirmLoading] = useState(false);
  const [loadingLists, setLoadingLists] = useState(false);
  const [roles, setRoles] = useState([]);
  const [pointOfSales, setPointOfSales] = useState([]);

  const screens = useBreakpoint();
  const isMobile = screens.md === false; // Detecta resoluciones móviles de forma dinámica

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

      setRoles(resRole || []);
      setPointOfSales(resPointOfSale || []);
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
        // Si está editando y el campo clave queda vacío, enviamos null o string vacío según tu backend
        password: values.password || "", 
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
      title={
        <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
          <UserOutlined style={{ color: '#1677ff' }} />
          <span>{initialValues?.id ? "Editar Personal" : "Nuevo Personal"}</span>
        </div>
      }
      open={open}
      onOk={handleOk}
      confirmLoading={confirmLoading}
      onCancel={onCancel}
      width={isMobile ? "95%" : 600} // Ajuste elástico responsivo
      forceRender // Resuelve la inicialización del formulario al instante
      okText="Guardar"
      cancelText="Cancelar"
      centered={isMobile}
    >
      <Form 
        form={form} 
        layout="vertical" 
        preserve={false}
        style={{ marginTop: '16px' }}
      >
        <Row gutter={[16, 0]}>
          {/* USUARIO */}
          <Col xs={24} sm={12}>
            <Form.Item 
              label="Nombre de Usuario" 
              name="username"
              rules={[{ required: true, message: 'Por favor ingrese el identificador de acceso' }]}
            >
              <Input placeholder="Ej: cajero.central" size="large" />
            </Form.Item>
          </Col>
          
          {/* CONTRASENIA */}
          <Col xs={24} sm={12}>
            <Form.Item 
              label="Clave / Contraseña" 
              name="password"
              rules={[
                { 
                  required: !initialValues?.id, 
                  message: 'La contraseña es obligatoria para nuevos registros' 
                }
              ]}
              extra={initialValues?.id ? "Dejar en blanco si no desea cambiarla" : null}
            >
              <Input.Password placeholder="••••••••" size="large" />
            </Form.Item>
          </Col>
        </Row>

        <Row gutter={[16, 0]}>
          {/* ROL */}
          <Col xs={24} sm={12}>
            <Form.Item 
              label="Rol Asignado" 
              name="roleId"
              rules={[{ required: false, message: 'Seleccione un perfil de acceso' }]}
            >
              <Select
                placeholder="Seleccione el rol"
                loading={loadingLists}
                size="large"
                options={roles.map((c) => ({
                  value: c.id,
                  label: c.name,
                }))}
              />
            </Form.Item>
          </Col>

          {/* PUNTO DE VENTA */}
          <Col xs={24} sm={12}>
            <Form.Item 
              label="Punto de Venta (POS)" 
              name="pointOfSaleId"
              rules={[{ required: true, message: 'Asigne un punto operativo' }]}
            >
              <Select
                placeholder="Seleccione puesto de trabajo"
                loading={loadingLists}
                size="large"
                options={pointOfSales.map((c) => ({
                  value: c.id,
                  label: c.name,
                }))}
              />
            </Form.Item>
          </Col>
        </Row>
      </Form>
    </Modal>
  );
};

export default UserEditModal;
