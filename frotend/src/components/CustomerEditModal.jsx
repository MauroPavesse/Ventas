import { Col, Form, Input, Select, Modal, Row, message } from "antd";
import { useState, useEffect } from "react";
import { customerService } from "../services/customerService";
import { taxConditionService } from "../services/taxConditionService";
import { SearchCommand } from "../DTOs/SearchCommand";

const CustomerEditModal = ({ open, onCancel, onSuccess, initialValues }) => {
  const [form] = Form.useForm();
  const [confirmLoading, setConfirmLoading] = useState(false);
  const [loadingLists, setLoadingLists] = useState(false);
  const [taxConditions, setTaxConditions] = useState([]);

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

      setTaxConditions(resTaxCondition);
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
      title={initialValues?.id ? "Editar Cliente" : "Nuevo Cliente"}
      open={open}
      onOk={handleOk}
      confirmLoading={confirmLoading}
      onCancel={onCancel}
      width={700}
    >
      <Form form={form} layout="vertical" preserve={false}>
        <Row gutter={15}>
          <Col span={12}>
            <Form.Item label="Nombre" name="firstName">
              <Input placeholder="Juan" />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item label="Apellido" name="lastName">
              <Input placeholder="Perez" />
            </Form.Item>
          </Col>
        </Row>
        <Row gutter={15}>
          <Col span={12}>
            <Form.Item label="Documento" name="document">
              <Input placeholder="123456789" />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item label="CUIL/CUIT" name="cuit">
              <Input placeholder="11-123456789-1" />
            </Form.Item>
          </Col>
        </Row>
        <Form.Item label="Condición Fiscal" name="taxConditionId">
          <Select
            placeholder="Seleccione CF"
            loading={loadingLists}
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
