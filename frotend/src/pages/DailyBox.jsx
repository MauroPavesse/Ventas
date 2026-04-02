import { Button, message, Table, Modal, Tooltip } from "antd";
import PageLayout from "../layouts/PageLayout";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { voucherService } from "../services/voucherService";
import { printService } from "../services/printService";
import { dailyBoxService } from "../services/dailyBoxService";
import { configurationService } from "../services/configurationService";
import {
  DeleteOutlined,
  ExclamationCircleOutlined,
  PrinterOutlined,
  FileDoneOutlined
} from "@ant-design/icons";

const DailyBox = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [vouchers, setVouchers] = useState();

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
      setVouchers(res);
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
          // await voucherService.convertToInvoice(record.id);
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
          { title: "Cant.", dataIndex: "quantity", key: "quantity" },
          { title: "Precio Unit.", dataIndex: "priceUnit", key: "priceUnit", render: (p) => `$ ${p}` },
          { title: "Subtotal", dataIndex: "amountFinal", key: "amountFinal", render: (a) => `$ ${a}` },
        ]}
        dataSource={record.voucherDetails}
        pagination={false}
        size="small"
        rowKey="id"
      />
    ),
    rowExpandable: (record) => record.voucherDetails?.length > 0,
  };

  const columns = [
    { title: "Comprobante", dataIndex: "description", key: "description" },
    { title: "Importe", dataIndex: "amountTotal", key: "amountTotal", render: (i) => <b>$ {i.toLocaleString()}</b> },
    {
      title: "Acción",
      key: "action",
      fixed: "right",
      width: 150,
      render: (_, record) => (
        <div style={{ display: 'flex', gap: '8px' }}>
          <Tooltip title="Imprimir">
            <Button icon={<PrinterOutlined />} onClick={() => printTicket(record.id)} />
          </Tooltip>

          <Tooltip title="Convertir a Factura">
            <Button
              type="primary"
              ghost
              icon={<FileDoneOutlined />}
              onClick={() => handleConvertInvoice(record)}
            />
          </Tooltip>

          <Button
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
          vouchers.filter((t) => t.id != record.id);
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

      <Table
        dataSource={vouchers}
        columns={columns}
        rowKey="id"
        loading={loading}
        expandable={expandableConfig}
      />

      <Button
        type="primary"
        style={{ marginTop: 16, float: "inline-end" }}
        onClick={() => closeDailyBox()}
      >
        Cerrar Caja
      </Button>

    </PageLayout>
  );
};

export default DailyBox;
