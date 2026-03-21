import React, { useState, useEffect } from "react";
import {
  Modal,
  Select,
  InputNumber,
  Divider,
  Row,
  Col,
  Tag,
  Typography,
  Button,
} from "antd";
import { DollarOutlined, UserOutlined } from "@ant-design/icons";
import { SearchCommand } from "../DTOs/SearchCommand";
import { paymentMethodService } from "../services/paymentMethodService";
import { customerService } from "../services/customerService";

const { Text, Title } = Typography;

const CloseSale = ({ visible, onClose, totalAmount, onConfirm }) => {
  const [selectedMethod, setSelectedMethod] = useState(null);
  const [selectedCustomer, setSelectedCustomer] = useState(null);
  const [paidAmount, setPaidAmount] = useState(0);
  const [paymentMethods, setPaymentMethods] = useState([]);
  const [customers, setCustomers] = useState([]);

  useEffect(() => {
    const loadMetadata = async () => {
      try {
        const command = new SearchCommand();
        const [resPaymentMethod, resCustomer] = await Promise.all([
          paymentMethodService.search(command),
          customerService.search(command),
        ]);

        setPaymentMethods(resPaymentMethod);
        setCustomers(resCustomer);
      } catch (error) {
        console.error("Error cargando metadatos", error);
      }
    };
    loadMetadata();
  }, []);

  // Calcular montos finales basados en el método de pago
  const getFinalAmount = () => {
    if (!selectedMethod) return totalAmount;
    const withDiscount = totalAmount * (1 - selectedMethod.descountPercentage / 100);
    const withSurcharge = withDiscount * (1 + selectedMethod.increasePercentage / 100);
    return Math.round(withSurcharge * 100) / 100; // Redondeo a 2 decimales
  };

  const finalAmount = getFinalAmount();

  // Sincronizar el monto pagado cuando cambia el total final
  useEffect(() => {
    setPaidAmount(finalAmount);
  }, [finalAmount]);

  const handleFinish = () => {
    onConfirm({
      method: selectedMethod,
      customer: selectedCustomer,
      finalAmount,
      paidAmount,
      change: paidAmount - finalAmount,
    });
  };

  return (
    <Modal
      title="Finalizar Pedido"
      open={visible}
      onCancel={onClose}
      footer={[
        <Button key="back" onClick={onClose}>
          Cancelar
        </Button>,
        <Button
          key="submit"
          type="primary"
          size="large"
          onClick={handleFinish}
          disabled={!selectedMethod}
        >
          Cerrar Pedido
        </Button>,
      ]}
      width={600}
    >
      <Row gutter={[16, 24]}>
        <Col span={24}>
          <Text type="secondary">Resumen de Venta</Text>
          <Title level={4} style={{ marginTop: 0 }}>
            Subtotal: ${totalAmount}
          </Title>
        </Col>

        <Col span={12}>
          <Text strong>Forma de Pago:</Text>
          <Select
            placeholder="Seleccione método"
            style={{ width: "100%" }}
            onChange={(id) =>
              setSelectedMethod(paymentMethods.find((m) => m.id === id))
            }
          >
            {paymentMethods.map((m) => (
              <Select.Option key={m.id} value={m.id}>
                <Tag color={m.color}>{m.name}</Tag>
                {m.descountPercentage > 0 && <Text type="success">-{m.descountPercentage}% </Text>}
                {m.increasePercentage > 0 && <Text type="danger">+{m.increasePercentage}%</Text>}
              </Select.Option>
            ))}
          </Select>
        </Col>

        <Col span={12}>
          <Text strong>Cliente (Opcional):</Text>
          <Select
            showSearch
            placeholder="Buscar cliente"
            style={{ width: "100%" }}
            optionFilterProp="children"
            onChange={(val) => setSelectedCustomer(val)}
            allowClear
          >
            {customers.map((c) => (
              <Select.Option key={c.id} value={c.id}>
                {c.firstName + " " + c.lastName}
              </Select.Option>
            ))}
          </Select>
        </Col>

        <Divider style={{ margin: "12px 0" }} />

        <Col
          span={24}
          style={{
            textAlign: "center",
            background: "#f5f5f5",
            padding: "20px",
            borderRadius: "8px",
          }}
        >
          <Text type="secondary">TOTAL A PAGAR</Text>
          <Title level={2} style={{ margin: 0, color: "#1890ff" }}>
            ${finalAmount}
          </Title>
        </Col>

        <Col span={24}>
          <Text strong>Monto recibido:</Text>
          <InputNumber
            prefix={<DollarOutlined />}
            style={{ width: "100%" }}
            size="large"
            value={paidAmount}
            onChange={(val) => setPaidAmount(val)}
          />
          {paidAmount > finalAmount && (
            <div style={{ marginTop: 10 }}>
              <Tag color="green">
                Cambio: ${(paidAmount - finalAmount).toFixed(2)}
              </Tag>
            </div>
          )}
        </Col>
      </Row>
    </Modal>
  );
};

export default CloseSale;
