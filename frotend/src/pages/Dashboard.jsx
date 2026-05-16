import React, { useState, useEffect } from 'react'; // <-- Agregado useEffect
import { Card, Statistic, DatePicker, Row, Col, Space, message, Spin, List, Progress, Avatar } from 'antd'; // <-- Agregado message y Spin
import { ArrowUpOutlined, ArrowDownOutlined, ShoppingCartOutlined, ShoppingOutlined  } from '@ant-design/icons';
import dayjs from 'dayjs';
import { statisticsService } from '../services/statisticsService';

const { RangePicker } = DatePicker;

const Dashboard = () => {
  const [dateRange, setDateRange] = useState([dayjs(), dayjs()]);
  const [salesAmountData, setSalesAmountData] = useState(null);
  const [topProductsData, setTopProductsData] = useState([]);
  const [loading, setLoading] = useState(false); // <-- Agregado estado de carga

  const topProductsMock = [
    { id: 1, name: "Zapatillas Running Pro", quantity: 142, totalAmount: 7100 },
    { id: 2, name: "Remera Algodón Premium", quantity: 98, totalAmount: 2940 },
    { id: 3, name: "Gorra Trucker Urbana", quantity: 65, totalAmount: 1300 },
    { id: 4, name: "Medias Deportivas (Pack x3)", quantity: 42, totalAmount: 420 },
    { id: 5, name: "Pantalón Jogger Slim", quantity: 19, totalAmount: 950 },
  ];
  const maxQuantity = topProductsData.length > 0 ? topProductsData[0].quantity : 1;

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
        <Col>
          <Space>
            <span>Filtrar período:</span>
            <RangePicker
              value={dateRange} // <-- Cambiado 'defaultValue' por 'value' para que sea un componente controlado
              onChange={handleDateChange}
              format="DD/MM/YYYY"
              allowClear={false}
            />
          </Space>
        </Col>
      </Row>

      {/* Grid de Tarjetas */}
      <Row gutter={[16, 16]} style={{ marginBottom: '24px' }}>

        {/* Tarjeta 1: Importe Total (La que ya tienes) */}
        <Col xs={24} sm={12} md={8}>
          <Spin spinning={loading}> {/* <-- Añadimos un spinner visual de ANTD */}
            <Card bordered={false} hoverable>

              {/* Usamos el operador '?.' para evitar crashes si la API tarda en responder */}
              <Statistic
                title="Importe de Ventas"
                value={salesAmountData?.currentSales ?? 0} // Si es null, muestra 0
                precision={2}
                //valueStyle={{ color: '#000', fontSize: '28px', fontWeight: 'bold' }}
                prefix="$"
              />

              {/* Solo mostramos la comparativa si ya tenemos los datos de la API */}
              {salesAmountData && (
                <div style={{ marginTop: '12px', fontSize: '14px', color: '#8c8c8c' }}>
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
          <Card bordered={false} hoverable>
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
          <Card bordered={false} hoverable>
            <Statistic
              title="Ticket Promedio"
              value={salesAmountData?.averageTicket ?? 0} // (CurrentSales / TotalOrders)
              precision={2}
              prefix="$"
            />
            <div style={{ marginTop: '12px', fontSize: '14px', color: '#8c8c8c' }}>
              <span>Consumo medio por cliente</span>
            </div>
          </Card>
          </Spin>
        </Col>

      </Row>

      <Card title="Top 5 Productos Más Vendidos" hoverable loading={loading} style={{ marginTop: "5px" }}>
        <List
          itemLayout="horizontal"
          dataSource={topProductsData}
          renderItem={(item, index) => (
            <List.Item>
              <List.Item.Meta
                avatar={
                  <Avatar
                    style={{ backgroundColor: index === 0 ? '#fadb14' : '#f5f5f5', color: index === 0 ? '#000' : '#8c8c8c' }}
                  >
                    {index + 1}
                  </Avatar>
                }
                title={item.name}
                description={`${item.quantity} unidades vendidas — Total: $${item.totalAmount}`}
              />
              {/* Barra visual para notar la diferencia de volumen rápidamente */}
              <div style={{ width: 100, marginLeft: 16 }}>
                <Progress
                  percent={Math.round((item.quantity / maxQuantity) * 100)}
                  showInfo={false}
                  status="active"
                  strokeColor={index === 0 ? '#52c41a' : '#1890ff'}
                />
              </div>
            </List.Item>
          )}
        />
      </Card>
    </div>
  );
};

export default Dashboard;