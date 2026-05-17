import React, { useState, useEffect } from "react";
import { Col, Form, Input, Select, Modal, Row, message, Grid } from "antd";
import { customerService } from "../services/customerService";
import { taxConditionService } from "../services/taxConditionService";
import { SearchCommand } from "../DTOs/SearchCommand";
import { UserOutlined, IdcardOutlined } from "@ant-design/icons";

const { useBreakpoint } = Grid;

const CustomerEditModal = ({ open, onCancel, onSuccess, initialValues }) => {
  const [form] = Form.useForm();
  const [confirmLoading, setConfirmLoading] = useState(false);
  const [loadingLists, setLoadingLists] = useState(false);
  const [taxConditions, setTaxConditions] = useState([]);

  const screens = useBreakpoint();
  const isMobile = screens.md === false; // Detecta celulares y tablets chicas

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
      const [resTaxCondition] = await Promise.all([
        taxConditionService.search(command)
      ]);

      setTaxConditions(resTaxCondition || []);
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
        document: values.document,
        cuit: values.cuit,
        firstName: values.firstName,
        lastName: values.lastName,
        taxConditionId: values.taxConditionId > 0 ? values.taxConditionId : null
      };

      if (initialValues?.id) {
        await customerService.update(payload);
        message.success("Cliente actualizado");
      } else {
        await customerService.create(payload);
        message.success("Cliente creado");
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
          <span>{initialValues?.id ? "Editar Cliente" : "Nuevo Cliente"}</span>
        </div>
      }
      open={open}
      onOk={handleOk}
      confirmLoading={confirmLoading}
      onCancel={onCancel}
      width={isMobile ? "95%" : 650} // 95% de la pantalla en móviles, 650px en computadoras
      forceRender // Habilita la inserción inmediata de valores sin usar setTimeout
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
        <Row gutter={[12, 0]}>
          {/* NOMBRE */}
          <Col xs={24} sm={12}>
            <Form.Item 
              label="Nombre" 
              name="firstName"
              rules={[{ required: true, message: 'Por favor ingrese el nombre' }]}
            >
              <Input placeholder="Juan" size="large" />
            </Form.Item>
          </Col>
          
          {/* APELLIDO */}
          <Col xs={24} sm={12}>
            <Form.Item 
              label="Apellido" 
              name="lastName"
              rules={[{ required: true, message: 'Por favor ingrese el apellido' }]}
            >
              <Input placeholder="Pérez" size="large" />
            </Form.Item>
          </Col>
        </Row>

        <Row gutter={[12, 0]}>
          {/* DOCUMENTO */}
          <Col xs={24} sm={12}>
            <Form.Item 
              label="Documento (DNI / LE)" 
              name="document"
              rules={[{ required: true, message: 'Por favor ingrese el documento' }]}
            >
              <Input 
                placeholder="12345678" 
                size="large"
                inputMode="numeric"
                pattern={[0-9]}
                prefix={<IdcardOutlined style={{ color: '#bfbfbf' }} />}
              />
            </Form.Item>
          </Col>
          
          {/* CUIL / CUIT */}
          <Col xs={24} sm={12}>
            <Form.Item 
              label="CUIL / CUIT" 
              name="cuit"
            >
              <Input 
                placeholder="201234567892" 
                size="large"
                inputMode="numeric"
              />
            </Form.Item>
          </Col>
        </Row>

        {/* CONDICIÓN FISCAL */}
        <Form.Item 
          label="Condición Fiscal" 
          name="taxConditionId"
          rules={[{ required: true, message: 'Seleccione una condición fiscal' }]}
        >
          <Select
            placeholder="Seleccione la condición frente al IVA"
            loading={loadingLists}
            size="large"
            options={taxConditions.map((c) => ({
              value: c.id,
              label: c.description,
            }))}
          />
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default CustomerEditModal;
