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
  Grid
} from "antd";
import { DollarOutlined, UserOutlined, CreditCardOutlined } from "@ant-design/icons";
import { SearchCommand } from "../DTOs/SearchCommand";
import { paymentMethodService } from "../services/paymentMethodService";
import { customerService } from "../services/customerService";

const { Text, Title } = Typography;
const { useBreakpoint } = Grid;

const CloseSale = ({ visible, onClose, totalAmount, onConfirm }) => {
  const screens = useBreakpoint();
  const isMobile = screens.md === false; // Detecta celulares y tablets chicas

  const [selectedMethod, setSelectedMethod] = useState(null);
  const [selectedCustomer, setSelectedCustomer] = useState(null);
  const [paidAmount, setPaidAmount] = useState(0);
  const [paymentMethods, setPaymentMethods] = useState([]);
  const [customers, setCustomers] = useState([]);

  const currencyFormatter = new Intl.NumberFormat("es-AR", {
    style: "currency",
    currency: "ARS",
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  });

  useEffect(() => {
    if (!visible) return; // Solo carga si el modal está abierto
    const loadMetadata = async () => {
      try {
        const command = new SearchCommand();
        const [resPaymentMethod, resCustomer] = await Promise.all([
          paymentMethodService.search(command),
          customerService.search(command),
        ]);

        setPaymentMethods(resPaymentMethod || []);
        setCustomers(resCustomer || []);
      } catch (error) {
        console.error("Error cargando metadatos", error);
      }
    };
    loadMetadata();
  }, [visible]);

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
      change: Math.max(0, paidAmount - finalAmount),
    });
  };

  return (
    <Modal
      title={
        <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
          <CreditCardOutlined style={{ color: '#1677ff' }} />
          <span>Finalizar Pedido</span>
        </div>
      }
      open={visible}
      onCancel={onClose}
      centered={isMobile}
      width={isMobile ? "100%" : 550}
      style={isMobile ? { top: 0, margin: 0, maxWidth: '100vw' } : {}}
      styles={{
        body: {
          maxHeight: isMobile ? 'calc(100vh - 150px)' : '75vh',
          overflowY: 'auto',
          padding: isMobile ? '8px 4px' : '0 8px'
        }
      }}
      footer={[
        <Button key="back" onClick={onClose} size={isMobile ? "large" : "default"} block={isMobile} style={isMobile ? { marginBottom: 8 } : {}}>
          Cancelar
        </Button>,
        <Button
          key="submit"
          type="primary"
          size="large"
          onClick={handleFinish}
          disabled={!selectedMethod}
          block={isMobile}
        >
          Confirmar y Cerrar Venta
        </Button>,
      ]}
    >
      <Row gutter={[16, 16]} style={{ marginTop: 8 }}>
        {/* RESUMEN DE COMPRA */}
        <Col span={24}>
          <div style={{ background: '#fcfcfc', padding: '12px', borderRadius: '8px', border: '1px solid #f0f0f0' }}>
            <Text type="secondary" style={{ fontSize: '12px' }}>SUBTOTAL ORIGINAL</Text>
            <Title level={4} style={{ margin: 0, fontWeight: 500 }}>
              {currencyFormatter.format(totalAmount)}
            </Title>
          </div>
        </Col>

        {/* FORMA DE PAGO */}
        <Col xs={24} sm={12}>
          <Text strong style={{ display: 'block', marginBottom: 6 }}>Forma de Pago:</Text>
          <Select
            placeholder="Seleccione método"
            style={{ width: "100%" }}
            size="large"
            onChange={(id) =>
              setSelectedMethod(paymentMethods.find((m) => m.id === id))
            }
          >
            {paymentMethods.map((m) => (
              <Select.Option key={m.id} value={m.id} label={m.name}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                  <span>
                    <Tag color={m.color} style={{ marginRight: 6 }}>{m.name}</Tag>
                  </span>
                  <div>
                    {m.descountPercentage > 0 && <Text type="success">-{m.descountPercentage}%</Text>}
                    {m.increasePercentage > 0 && <Text type="danger">+{m.increasePercentage}%</Text>}
                  </div>
                </div>
              </Select.Option>
            ))}
          </Select>
        </Col>

        {/* SELECTOR DE CLIENTE */}
        <Col xs={24} sm={12}>
          <Text strong style={{ display: 'block', marginBottom: 6 }}>Cliente (Opcional):</Text>
          <Select
            showSearch
            placeholder="Buscar por nombre..."
            style={{ width: "100%" }}
            size="large"
            optionFilterProp="label" // Crucial para que el buscador funcione con el texto concatenado
            onChange={(val) => setSelectedCustomer(val)}
            allowClear
          >
            {customers.map((c) => {
              const fullName = `${c.firstName} ${c.lastName}`;
              return (
                <Select.Option key={c.id} value={c.id} label={fullName}>
                  <UserOutlined style={{ marginRight: 8, color: '#bfbfbf' }} />
                  {fullName}
                </Select.Option>
              );
            })}
          </Select>
        </Col>

        {/* VISUALIZADOR DE MONTO NETO FINAL */}
        <Col span={24}>
          <div
            style={{
              textAlign: "center",
              background: "#e6f7ff",
              border: "1px solid #91d5ff",
              padding: "16px 20px",
              borderRadius: "8px",
              marginTop: 4
            }}
          >
            <Text type="secondary" strong style={{ letterSpacing: '0.5px', fontSize: '11px' }}>TOTAL CON RECARGOS / DESCUENTOS</Text>
            <Title level={1} style={{ margin: 0, color: "#096dd9", fontSize: isMobile ? '36px' : '42px', fontWeight: 700 }}>
              {currencyFormatter.format(finalAmount)}
            </Title>
          </div>
        </Col>

        {/* CAJA DE MONTO ENTRANTE Y VUELTO */}
        <Col span={24}>
          <Text strong style={{ display: 'block', marginBottom: 6 }}>Monto recibido por el cliente:</Text>
          <InputNumber
            prefix={<DollarOutlined style={{ color: '#bfbfbf' }} />}
            style={{ width: "100%" }}
            size="large"
            value={paidAmount}
            onChange={(val) => setPaidAmount(val || 0)}
            min={0}
            stringMode={false}
            onFocus={(e) => e.target.select()} // Auto-selecciona el texto para agilizar el borrado en cajas rápidas
          />

          {paidAmount > finalAmount ? (
            <div style={{ marginTop: 12 }}>
              <div style={{ background: '#f6ffed', border: '1px solid #b7eb8f', padding: '12px', borderRadius: '6px', textAlign: 'center' }}>
                <Text type="secondary" style={{ display: 'block', fontSize: '11px', color: '#52c41a' }} strong>VUELTO (CAMBIO)</Text>
                <Title level={3} style={{ margin: 0, color: '#52c41a', fontWeight: 600 }}>
                  {currencyFormatter.format(paidAmount - finalAmount)}
                </Title>
              </div>
            </div>
          ) : paidAmount < finalAmount && paidAmount > 0 ? (
            <div style={{ marginTop: 12 }}>
              <div style={{ background: '#fff1f0', border: '1px solid #ffa39e', padding: '8px', borderRadius: '6px', textAlign: 'center' }}>
                <Text type="danger" strong style={{ fontSize: '12px' }}>
                  Faltan: {currencyFormatter.format(finalAmount - paidAmount)}
                </Text>
              </div>
            </div>
          ) : null}
        </Col>
      </Row>
    </Modal>
  );
};

export default CloseSale;
