import React, { useState, useEffect } from 'react'; // <-- Agregado useEffect
import { Card, Statistic, DatePicker, Row, Col, Space, message, Spin, List, Progress, Avatar, Grid, Flex } from 'antd'; // <-- Agregado message y Spin
import { ArrowUpOutlined, ArrowDownOutlined, ShoppingCartOutlined, ShoppingOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { statisticsService } from '../services/statisticsService';

const { RangePicker } = DatePicker;
const { useBreakpoint } = Grid; // Hook para detectar resoluciones

const Dashboard = () => {
  const [dateRange, setDateRange] = useState([dayjs(), dayjs()]);
  const [salesAmountData, setSalesAmountData] = useState(null);
  const [topProductsData, setTopProductsData] = useState([]);
  const [loading, setLoading] = useState(false); // <-- Agregado estado de carga

  const screens = useBreakpoint();
  const isMobile = screens.md === false; // Define si es pantalla móvil/tablet pequeña

  const maxQuantity = topProductsData.length > 0 ? topProductsData[0].quantity : 1;

  const currencyFormatter = new Intl.NumberFormat("es-AR", {
    style: "currency",
    currency: "ARS",
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  });

  // Modificamos la función para que opcionalmente reciba las fechas directamente
  const fetchData = async (currentDates = dateRange) => {
    setLoading(true);
    try {
      const command = {
        DateFrom: currentDates[0].format('YYYY-MM-DD'),
        DateTo: currentDates[1].format('YYYY-MM-DD')
      };

      const dataSalesAmount = await statisticsService.getSalesAmount(command);
      const dataTopProducts = await statisticsService.getTopProducts(command);
      setSalesAmountData(dataSalesAmount);
      setTopProductsData(dataTopProducts);
    } catch (error) {
      message.error("Error al cargar las estadísticas: " + error.message);
    } finally {
      setLoading(false);
    }
  };

  // Se ejecuta al montar el componente
  useEffect(() => {
    fetchData();
  }, []);

  const handleDateChange = (dates) => {
    if (dates) {
      setDateRange(dates);
      fetchData(dates);
    }
  };

  return (
    <div>
      {/* Selector de Fechas Global */}
      <Row justify="start" style={{ marginBottom: '20px' }}>
        <Col xs={24} sm={18} md={12}>
          <div style={{ display: 'flex', flexDirection: isMobile ? 'column' : 'row', gap: '8px', alignItems: isMobile ? 'flex-start' : 'center' }}>
            <span style={{ fontWeight: 500 }}>Filtrar período:</span>
            <RangePicker
              value={dateRange}
              onChange={handleDateChange}
              format="DD/MM/YYYY"
              allowClear={false}
              style={{ width: '100%' }} // Se estira al 100% en celulares
            />
          </div>
        </Col>
      </Row>

      {/* Grid de Tarjetas */}
      <Row gutter={[16, 16]} style={{ marginBottom: '24px' }}>
        {/* Tarjeta 1: Importe Total (La que ya tienes) */}
        <Col xs={24} sm={12} md={8}>
          <Spin spinning={loading}> {/* <-- Añadimos un spinner visual de ANTD */}
            <Card variant="borderless" hoverable styles={{ body: { padding: isMobile ? '16px' : '24px' } }}>
              <Statistic
                title="Importe de Ventas"
                value={currencyFormatter.format(salesAmountData?.currentSales ?? 0)} // Si es null, muestra 0
              />
              {salesAmountData && (
                <div style={{ marginTop: '12px', fontSize: '14px', color: '#8c8c8c', display: 'flex', alignItems: 'center', flexWrap: 'wrap' }}>
                  <span style={{ color: salesAmountData.isPositive ? '#3f8600' : '#cf1322', fontWeight: '600', marginRight: '8px' }}>
                    {salesAmountData.isPositive ? <ArrowUpOutlined /> : <ArrowDownOutlined />}
                    {Math.abs(salesAmountData.percentageChange)}%
                  </span>
                  <span>vs. período anterior</span>
                </div>
              )}

            </Card>
          </Spin>
        </Col>

        {/* Tarjeta 2: Cantidad de Pedidos (Nueva) */}
        <Col xs={24} sm={12} md={8}>
          <Spin spinning={loading}>
            <Card variant="borderless" hoverable styles={{ body: { padding: isMobile ? '16px' : '24px' } }}>
              <Statistic
                title="Pedidos Totales"
                value={salesAmountData?.totalOrders ?? 0} // Tendrías que mapearlo desde el backend
                prefix={<ShoppingOutlined style={{ color: '#1890ff' }} />}
              />
              <div style={{ marginTop: '12px', fontSize: '14px', color: '#8c8c8c' }}>
                <span>Órdenes finalizadas</span>
              </div>
            </Card>
          </Spin>
        </Col>

        {/* Tarjeta 3: Ticket Promedio (Nueva) */}
        <Col xs={24} sm={24} md={8}>
          <Spin spinning={loading}>
            <Card variant="borderless" hoverable styles={{ body: { padding: isMobile ? '16px' : '24px' } }}>
              <Statistic
                title="Ticket Promedio"
                value={currencyFormatter.format(salesAmountData?.averageTicket ?? 0)} // (CurrentSales / TotalOrders)
              />
              <div style={{ marginTop: '12px', fontSize: '14px', color: '#8c8c8c' }}>
                <span>Consumo medio por cliente</span>
              </div>
            </Card>
          </Spin>
        </Col>
      </Row>

      {/* Tarjeta de Productos Más Vendidos */}
      <Card
        title="Top 5 Productos Más Vendidos"
        hoverable
        loading={loading}
        style={{ marginTop: "5px" }}
        styles={{ body: { padding: isMobile ? '8px 12px' : '24px' } }} // Menos padding en móvil
      >
        <Flex vertical>
          {topProductsData.map((item, index) => (
            <div 
              key={item.id || index}
              style={{ 
                display: 'flex',
                flexDirection: isMobile ? 'column' : 'row', 
                alignItems: isMobile ? 'flex-start' : 'center',
                gap: isMobile ? '12px' : '0px',
                padding: isMobile ? '16px 4px' : '12px 0px',
                borderBottom: index < topProductsData.length - 1 ? '1px solid #f0f0f0' : 'none' // Simula la división nativa de la lista
              }}
            >
              {/* Contenido Izquierdo (Avatar e Info del Producto) */}
              <div style={{ display: 'flex', alignItems: 'center', gap: '16px', flex: 1, width: '100%' }}>
                <Avatar
                  style={{ backgroundColor: index === 0 ? '#fadb14' : '#f5f5f5', color: index === 0 ? '#000' : '#8c8c8c', flexShrink: 0 }}
                >
                  {index + 1}
                </Avatar>
                
                <div style={{ display: 'flex', flexDirection: 'column' }}>
                  <span style={{ fontWeight: 600, fontSize: isMobile ? '15px' : '14px', color: 'rgba(0, 0, 0, 0.88)' }}>
                    {item.name}
                  </span>
                  <span style={{ fontSize: isMobile ? '13px' : '14px', color: 'rgba(0, 0, 0, 0.45)' }}>
                    {item.quantity} und. — <span style={{ fontWeight: 500, color: '#595959' }}>Total: {currencyFormatter.format(item.totalAmount)}</span>
                  </span>
                </div>
              </div>
              
              {/* Barra visual derecha adaptativa */}
              <div style={{ 
                width: isMobile ? '100%' : 120, 
                marginLeft: isMobile ? 0 : 16,
                paddingLeft: isMobile ? 48 : 0 // Ajustado para alinearse perfecto en móvil
              }}>
                <Progress
                  percent={Math.round((item.quantity / maxQuantity) * 100)}
                  showInfo={false}
                  status="active"
                  strokeColor={index === 0 ? '#52c41a' : '#1890ff'}
                  size={{ strokeWidth: isMobile ? 6 : 8 }}
                />
              </div>
            </div>
          ))}
        </Flex>
      </Card>
    </div>
  );
};

export default Dashboard;