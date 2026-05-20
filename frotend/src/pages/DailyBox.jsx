import { Button, message, Table, Modal, Tooltip, Grid, Card, Row, Col, Statistic  } from "antd";
import PageLayout from "../layouts/PageLayout";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { voucherService } from "../services/voucherService";
import { printService } from "../services/printService";
import { dailyBoxService } from "../services/dailyBoxService";
import { configurationService } from "../services/configurationService";
import { VoucherTypesEnum } from "../constants/voucherTypesEnum";
import {
  DeleteOutlined,
  ExclamationCircleOutlined,
  PrinterOutlined,
  FileDoneOutlined,
  DollarOutlined
} from "@ant-design/icons";

const { useBreakpoint } = Grid;

const DailyBox = () => {
  const navigate = useNavigate();
  const screens = useBreakpoint();
  const isMobile = screens.md === false; // Detecta celulares y tablets pequeñas

  const [loading, setLoading] = useState(false);
  const [vouchers, setVouchers] = useState();

  const currencyFormatter = new Intl.NumberFormat("es-AR", {
    style: "currency",
    currency: "ARS",
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  });

  const totalAccumulated = vouchers?.reduce((sum, item) => sum + (item.amountTotal || 0), 0) || 0;

  const fetchData = async () => {
    setLoading(true);
    try {
      const command = {
        filters: [
          {
            field: "SinCajaDiaria",
            value: "",
            ids: []
          }
        ]
      };
      var res = await voucherService.search(command);
      setVouchers(res || []);
    } catch (error) {
      message.error("Error al cargar las categorías: " + error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const printTicket = async (voucherId) => {
    setLoading(true);
    try {

      const blob = await printService.printTicket(voucherId);
      const url = window.URL.createObjectURL(new Blob([blob], { type: 'application/pdf' }));

      const resConfiguration = await configurationService.search(["imprimeTicketDirecto", "empresa", "cuit"]);
      const configItem = resConfiguration.find(item => item.variable === "imprimeTicketDirecto");
      const printsDirectly = configItem ? configItem.boolValue : false;

      if (printsDirectly) {
        // Creamos un iframe invisible
        const iframe = document.createElement('iframe');
        iframe.style.display = 'none';
        iframe.src = url;
        document.body.appendChild(iframe);

        // Cuando el PDF carga en el iframe, mandamos el comando de impresión
        iframe.onload = () => {
          iframe.contentWindow.print();
          // Limpiamos después de imprimir
          setTimeout(() => {
            document.body.removeChild(iframe);
            window.URL.revokeObjectURL(url);
          }, 1000);
        };
      }
      else {
        // Opción A: Abrir en una pestaña nueva (la más común para facturas)
        window.open(url, '_blank');

        // Limpieza: Liberar la memoria de la URL creada
        setTimeout(() => window.URL.revokeObjectURL(url), 100);
      }
    } catch (error) {
      console.error(error);
      message.error("No se pudo generar el PDF del comprobante.");
    } finally {
      setLoading(false);
    }
  };

  const handleConvertInvoice = (record) => {
    Modal.confirm({
      title: 'Convertir a Factura',
      content: `¿Deseas generar la factura legal para el comprobante ${record.description}?`,
      onOk: async () => {
        try {
          await voucherService.convertToInvoice(record.id);
          message.success("Factura generada con éxito");
          fetchData(); // Recargar para actualizar estado si es necesario
        } catch (e) {
          message.error("Error al facturar");
        }
      }
    });
  };

  const expandableConfig = {
    expandedRowRender: (record) => (
      <Table
        columns={[
          { title: "Producto", dataIndex: "productName", key: "productName" },
          { title: "Cant.", dataIndex: "quantity", key: "quantity", width: isMobile ? 60 : undefined },
          { title: "Subtotal", dataIndex: "amountFinal", key: "amountFinal", render: (a) => currencyFormatter.format(a) },
        ]}
        dataSource={record.voucherDetails}
        pagination={false}
        size="small"
        rowKey="id"
        scroll={isMobile ? { x: true } : undefined}
      />
    ),
    rowExpandable: (record) => record.voucherDetails?.length > 0,
  };

  const columns = [
    {
      title: "Comprobante",
      dataIndex: "description",
      key: "description",
      render: (text) => <span style={{ fontSize: isMobile ? '13px' : '14px' }}>{text}</span>
    },
    {
      title: "Importe",
      dataIndex: "amountTotal",
      key: "amountTotal",
      render: (i) => <b>{currencyFormatter.format(i)}</b>
    },
    {
      title: "Acción",
      key: "action",
      fixed: isMobile ? false : "right",
      width: isMobile ? 130 : 150,
      render: (_, record) => (
        <div style={{ display: 'flex', gap: isMobile ? '4px' : '8px' }}>
          <Tooltip title={isMobile ? "" : "Imprimir"}>
            <Button size={isMobile ? "small" : "default"} icon={<PrinterOutlined />} onClick={() => printTicket(record.id)} />
          </Tooltip>

          {record.voucherTypeId == VoucherTypesEnum.ORDEN_DE_COMPRA ?
            <Tooltip title={isMobile ? "" : "Convertir a Factura"}>
              <Button
                size={isMobile ? "small" : "default"}
                type="primary"
                ghost
                icon={<FileDoneOutlined />}
                onClick={() => handleConvertInvoice(record)}
              />
            </Tooltip>
            : null
          }

          <Button
            size={isMobile ? "small" : "default"}
            danger
            icon={<DeleteOutlined />}
            onClick={() => handleDelete(record)}
          />
        </div>
      ),
    },
  ];

  const { confirm } = Modal;

  const handleDelete = (record) => {
    confirm({
      title: "¿Estás seguro de eliminar el comprobante?",
      icon: <ExclamationCircleOutlined />,
      content: `Comprobante: ${record.description}`,
      okText: "Sí, eliminar",
      okType: "danger",
      cancelText: "Cancelar",
      onOk: async () => {
        try {
          await voucherService.delete(record.id);
          message.success("Eliminado correctamente");
          setVouchers((prev) => prev.filter((t) => t.id !== record.id));
        } catch (e) {
          message.error("Error al eliminar: " + e);
        }
      },
    });
  };

  const closeDailyBox = async () => {
    const executeClose = async () => {
      setLoading(true);
      try {
        const userDataRaw = localStorage.getItem('user_data');
        const userData = userDataRaw ? JSON.parse(userDataRaw) : null;
        const userId = userData?.userId;
        await dailyBoxService.closeDailyBox(userId);
        message.success("Caja cerrada correctamente");
        fetchData();
      } catch (error) {
        message.error("Error al cerrar la caja: " + error);
      } finally {
        setLoading(false);
      }
    };

    // Verificamos si hay comprobantes (asumiendo que vouchers es un array)
    if (!vouchers || vouchers.length === 0) {
      Modal.confirm({
        title: 'Caja sin movimientos',
        icon: <ExclamationCircleOutlined />,
        content: '¿Deseas cerrar la caja sin comprobantes?',
        okText: 'Sí, cerrar',
        cancelText: 'Cancelar',
        onOk: () => executeClose()
      });
    } else {
      // Si hay comprobantes, cerramos directamente o podrías pedir confirmación siempre
      Modal.confirm({
        title: 'Cerrar Caja',
        content: `¿Estás seguro de cerrar la caja con ${vouchers.length} comprobantes?`,
        onOk: () => executeClose()
      });
    }
  }

  return (
    <PageLayout title="Caja de Hoy - Comprobantes" onClose={() => navigate("/dashboard")}>

      {/* Botón superior destacado para celular */}
      <Row gutter={[16, 16]} align="middle" style={{ marginBottom: 16 }}>
        {/* Tarjeta del total: Ocupa todo el ancho en móvil (24) y se autoajusta en escritorio */}
        <Col xs={24} sm={12} md={7}>
          <Card size="small" bodyStyle={{ padding: '12px 16px' }}>
            <Statistic
              title="Acumulado del Día"
              value={totalAccumulated}
              formatter={(value) => currencyFormatter.format(value)}
              valueStyle={{ color: '#3f8600', fontWeight: 'bold', fontSize: isMobile ? '20px' : '24px' }}
              prefix={<DollarOutlined />}
            />
          </Card>
        </Col>
        
        {/* Contenedor del botón: Se alinea al final en pantallas grandes */}
        <Col xs={24} sm={12} md={16} style={{ display: 'flex', justifyContent: isMobile ? 'stretch' : 'flex-end' }}>
          <Button
            type="primary"
            danger
            icon={<DollarOutlined />}
            onClick={() => closeDailyBox()}
            block={isMobile}
            size={isMobile ? "large" : "default"}
            style={{ fontWeight: 600, width: isMobile ? '100%' : 'auto', height: isMobile ? '50px' : 'auto' }}
          >
            Cerrar Caja Diaria
          </Button>
        </Col>
      </Row>

      <div style={{ overflowX: 'auto' }}>
        <Table
          dataSource={vouchers}
          columns={columns}
          rowKey="id"
          loading={loading}
          expandable={expandableConfig}
          size={isMobile ? "small" : "default"}
          scroll={{ x: true }} // Hace que la grilla principal sea deslizable horizontalmente si no entra
        />
      </div>
    </PageLayout>
  );
};

export default DailyBox;
