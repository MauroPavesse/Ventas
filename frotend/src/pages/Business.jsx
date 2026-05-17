import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import PageLayout from "../layouts/PageLayout";
import {
  DatePicker,
  Form,
  Input,
  message,
  Select,
  Button,
  Upload,
  Col,
  Row,
  Checkbox,
  Card,
  Divider,
  Grid
} from "antd";
import { UploadOutlined, CheckCircleFilled, CloseCircleFilled, LoadingOutlined } from "@ant-design/icons";
import { configurationService } from "../services/configurationService";
import { taxConditionService } from "../services/taxConditionService";
import { SearchCommand } from "../DTOs/SearchCommand";
import { uploadsService } from "../services/uploadsService";
import dayjs from "dayjs";
import { afipService } from "../services/afipService";

const { useBreakpoint } = Grid;

const Business = () => {
  const navigate = useNavigate();
  const screens = useBreakpoint();
  const isMobile = screens.md === false; // Detecta celulares y tablets chicas

  const [loadingLists, setLoadingLists] = useState(false);
  const [taxConditions, setTaxConditions] = useState([]);
  const [form] = Form.useForm();
  const [saving, setSaving] = useState(false);
  const [fileList, setFileList] = useState([]);
  const [testingConnection, setTestingConnection] = useState(false);
  const [connectionStatus, setConnectionStatus] = useState(null);
  const [watchValues, setWatchValues] = useState({ clave: "", certificado: "" });

  useEffect(() => {
    const fetchData = async () => {
      setLoadingLists(true);
      try {
        const command = new SearchCommand();
        const [resConfiguration, resTaxCondition] = await Promise.all([
          configurationService.search([]),
          taxConditionService.search(command),
        ]);

        setTaxConditions(resTaxCondition || []);

        // --- TRANSFORMACIÓN DE LA LISTA A OBJETO ---
        const initialValues = {};
        resConfiguration.forEach((item) => {
          let value = item.stringValue;
          switch (item.variable) {
            case "fechaInicio":
              if (value) value = dayjs(value);
              break;
            case "condicionFiscalId":
              value = item.numericValue ? Number(item.numericValue) : null;
              break;
            case "arcaCertificado":
              if (value) {
                setFileList([
                  {
                    uid: "-1", // ID único interno para ANTD
                    name: value.split("/").pop(), // Extrae el nombre del archivo de la URL
                    status: "done", // Marcado como ya subido
                    url: value, // URL para descargar/ver
                  },
                ]);
              }
              break;
            case "imprimeTicketDirecto":
              // Forzamos a booleano con !! o validando explícitamente
              value = !!item.boolValue;
              break;
          }

          initialValues[item.variable] = value;
        });

        // Cargamos los valores en el formulario
        form.setFieldsValue(initialValues);
      } catch (error) {
        message.error("Error al cargar datos: " + error);
      } finally {
        setLoadingLists(false);
      }
    };
    fetchData();
  }, []);

  const onFinish = async (values) => {
    setSaving(true);
    try {
      // Usamos Object.entries para recorrer cada campo del formulario
      const configurationItems = Object.entries(values).map(([key, value]) => {
        return {
          variable: key,
          stringValue:
            typeof value === "string" ? value : value?.toString() || "",
          numericValue: typeof value === "number" ? value : 0,
          boolValue: typeof value === "boolean" ? value : false,
          // Si es una fecha (dayjs), la convertimos a string ISO
          ...(dayjs.isDayjs(value) && { stringValue: value.toISOString() }),
        };
      });

      const command = {
        items: configurationItems,
      };

      await configurationService.update(command);
      message.success("Configuraciones actualizadas correctamente");
    } catch (error) {
      message.error("Error al guardar: " + error.message);
    } finally {
      setSaving(false);
    }
  };

  const handleCustomUpload = async (options) => {
    const { onSuccess, onError, file } = options;

    try {
      const response = await uploadsService.uploadCertificate(file);

      // 1. Actualizamos el valor en el Form de ANTD con la URL que devolvió el servidor
      form.setFieldsValue({ arcaCertificado: response.url });

      setFileList([
        {
          uid: file.uid,
          name: file.name,
          status: "done",
          url: response.url,
        },
      ]);

      // 2. Notificamos al componente Upload que terminó con éxito
      onSuccess("Ok");
      message.success(
        "Archivo subido temporalmente. No olvide guardar los cambios.",
      );
    } catch (err) {
      onError(err);
      message.error("Error al subir el archivo.");
    }
  };

  const handleRemove = () => {
    form.setFieldsValue({ arcaCertificado: "" });
    setFileList([]); // Limpiamos la lista visual
    setConnectionStatus(null);
  };

  const testAfipConnection = async (clave, certificado) => {
    if (!clave || !certificado) return;

    setTestingConnection(true);
    setConnectionStatus(null);
    try {
      await afipService.testConnection(certificado, clave);
      setConnectionStatus("success");
    } catch (error) {
      setConnectionStatus("error");
    } finally {
      setTestingConnection(false);
    }
  };

  useEffect(() => {
    const { clave, certificado } = watchValues;

    // No disparamos si falta alguno
    if (!clave || !certificado) {
      setConnectionStatus(null);
      return;
    }

    // Creamos el temporizador de 800ms (ajustalo a tu gusto)
    const timer = setTimeout(() => {
      testAfipConnection(clave, certificado);
    }, 800);

    // LIMPIEZA: Si el usuario escribe antes de los 800ms, este return mata al timer anterior
    // y el useEffect vuelve a empezar. ¡Magia!
    return () => clearTimeout(timer);
  }, [watchValues]);

  return (
    <PageLayout title="Datos de la empresa" onClose={() => navigate("/configurations")}>
      <Form
        form={form}
        layout="vertical"
        onFinish={onFinish}
        onValuesChange={(changedValues, allValues) => {
          if (changedValues.arcaClave || changedValues.arcaCertificado) {
            setWatchValues({
              clave: allValues.arcaClave || "",
              certificado: allValues.arcaCertificado || ""
            });
          }
        }}
        requiredMark="optional"
      >
        <Card variant="borderless" style={{ padding: 0, background: 'transparent' }}>
          {/* SECCIÓN 1: IDENTIDAD DE LA EMPRESA */}
          <Divider titlePlacement="left" style={{ marginTop: 0 }}>Empresa e Identificación</Divider>
          <Row gutter={[16, 0]}>
            <Col xs={24} sm={12} md={10}>
              <Form.Item label="Nombre empresa" name="empresa">
                <Input placeholder="Nombre de la empresa" size={isMobile ? "large" : "default"} />
              </Form.Item>
            </Col>

            <Col xs={24} sm={12} md={6}>
              <Form.Item label="Fecha de inicio" name="fechaInicio">
                <DatePicker style={{ width: "100%" }} size={isMobile ? "large" : "default"} />
              </Form.Item>
            </Col>
          </Row>
          <Row gutter={[16, 0]}>
            <Col xs={24} sm={12} md={8}>
              <Form.Item label="CUIT / CUIL" name="cuit">
                <Input placeholder="xxxxxxxxxxx" size={isMobile ? "large" : "default"} inputMode="numeric" />
              </Form.Item>
            </Col>

            <Col xs={24} sm={12} md={8}>
              <Form.Item label="Condición fiscal" name="condicionFiscalId">
                <Select
                  placeholder="Seleccione condición"
                  loading={loadingLists}
                  size={isMobile ? "large" : "default"}
                  options={taxConditions.map((c) => ({
                    value: c.id,
                    label: c.description,
                  }))}
                />
              </Form.Item>
            </Col>
          </Row>

          {/* SECCIÓN 2: INTEGRACIÓN FACTURA ELECTRÓNICA */}
          <Divider titlePlacement="left">Conectividad Fiscal (ARCA / AFIP)</Divider>
          <Row gutter={[16, 0]}>
            <Col xs={24}>
              <Form.Item name="arcaCertificado" noStyle>
                <Input type="hidden" />
              </Form.Item>

              {/* El Form.Item principal ahora solo tiene un único hijo: el Upload */}
              <Form.Item label="Certificado digital (.key / .crt)">
                <Upload
                  customRequest={handleCustomUpload}
                  fileList={fileList}
                  onRemove={handleRemove}
                  maxCount={1}
                >
                  {fileList.length < 1 && (
                    <Button icon={<UploadOutlined />} block={isMobile} size={isMobile ? "large" : "default"}>
                      Seleccionar Certificado
                    </Button>
                  )}
                </Upload>
              </Form.Item>
            </Col>

            <Col xs={24} sm={12} md={12}>
              <Form.Item label="Alias ARCA" name="arcaAlias">
                <Input placeholder="Ej: HomologacionEmpresa" size={isMobile ? "large" : "default"} />
              </Form.Item>
            </Col>

            <Col xs={24} sm={12} md={12}>
              <Form.Item
                label="Clave privada del certificado"
                name="arcaClave"
                help={
                  testingConnection ? (
                    <span style={{ color: "#1677ff" }}><LoadingOutlined /> Validando credenciales con ARCA...</span>
                  ) : connectionStatus === "success" ? (
                    <span style={{ color: "#52c41a" }}><CheckCircleFilled /> Conexión exitosa y clave válida</span>
                  ) : connectionStatus === "error" ? (
                    <span style={{ color: "#ff4d4f" }}><CloseCircleFilled /> Error de autenticación o archivo corrupto</span>
                  ) : null
                }
              >
                <Input.Password placeholder="Ingrese la contraseña del certificado" size={isMobile ? "large" : "default"} />
              </Form.Item>
            </Col>
          </Row>

          <Divider />

          {/* PARÁMETROS ADICIONALES */}
          <Row style={{ marginBottom: 24 }}>
            <Col xs={24}>
              <Form.Item name="imprimeTicketDirecto" valuePropName="checked" style={{ marginBottom: 8 }}>
                <Checkbox style={{ fontSize: isMobile ? '15px' : '14px' }}>
                  Imprimir comprobante automáticamente (Ticket directo)
                </Checkbox>
              </Form.Item>
            </Col>
          </Row>

          {/* BOTÓN PRINCIPAL */}
          <Form.Item style={{ marginBottom: 0 }}>
            <Button
              type="primary"
              htmlType="submit"
              loading={saving}
              block={isMobile}
              size="large"
            >
              Guardar Configuraciones
            </Button>
          </Form.Item>
        </Card>
      </Form>
    </PageLayout>
  );
};

export default Business;
