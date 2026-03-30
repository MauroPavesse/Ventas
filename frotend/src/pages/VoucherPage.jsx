import { Button, message, Table, Modal, Tooltip } from "antd";
import PageLayout from "../layouts/PageLayout";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { dailyBoxService } from "../services/dailyBoxService";
import { SearchCommand } from "../DTOs/SearchCommand";
import {
  PrinterOutlined,
  FileDoneOutlined
} from "@ant-design/icons";

const VoucherPage = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [dailyBoxes, setDailyBoxes] = useState();

  const fetchData = async () => {
    setLoading(true);
    try {
      const command = new SearchCommand();
      var res = await dailyBoxService.search(command);
      setDailyBoxes(res);
    } catch (error) {
      message.error("Error al cargar las categorías: " + error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const printVoucher = async (voucherId) => {
    setLoading(true);
    try {

      const blob = await voucherService.printTicket(voucherId);
      const url = window.URL.createObjectURL(new Blob([blob], { type: 'application/pdf' }));

      if (true) {
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
          { title: "Comprobante", dataIndex: "description", key: "description" },
          { title: "Importe", dataIndex: "amountTotal", key: "amountTotal", render: (i) => <b>$ {i.toLocaleString()}</b> },
          {
            title: "Acción",
            key: "action",
            fixed: "right",
            width: 100,
            render: (_, record) => (
              <div style={{ display: 'flex', gap: '8px' }}>
                <Tooltip title="Imprimir">
                  <Button icon={<PrinterOutlined />} onClick={() => printVoucher(record.id)} />
                </Tooltip>

                <Tooltip title="Convertir a Factura">
                  <Button
                    type="primary"
                    ghost
                    icon={<FileDoneOutlined />}
                    onClick={() => handleConvertInvoice(record)}
                  />
                </Tooltip>
              </div>
            ),
          },
        ]}
        dataSource={record.vouchers}
        pagination={false}
        size="small"
        rowKey="id"
      />
    ),
    rowExpandable: (record) => record.vouchers?.length > 0,
  };

  const columns = [
    { title: "N° Caja Diaria", dataIndex: "number", key: "number", render: (i) => `Caja diaria ${i} ` },
    { title: "Cant. Comp.", dataIndex: "quantityVouchers", key: "quantityVouchers" },
    { title: "Importe total", dataIndex: "amount", key: "amount", render: (i) => <b>$ {i.toLocaleString()}</b> },
    {
      title: "Acción",
      key: "action",
      fixed: "right",
      width: 50,
      render: (_, record) => (
        <div style={{ display: 'flex', gap: '8px' }}>
          <Tooltip title="Imprimir">
            <Button icon={<PrinterOutlined />} onClick={() => printVoucher(record.id)} />
          </Tooltip>
        </div>
      ),
    },
  ];

  return (
    <PageLayout title="Cajas diarias" onClose={() => navigate("/dashboard")}>

      <Table
        dataSource={dailyBoxes}
        columns={columns}
        rowKey="id"
        loading={loading}
        expandable={expandableConfig}
      />

    </PageLayout>
  );
};

export default VoucherPage;
