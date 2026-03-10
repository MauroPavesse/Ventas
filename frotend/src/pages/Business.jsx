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
} from "antd";
import { UploadOutlined } from "@ant-design/icons";
import { configurationService } from "../services/configurationService";
import { taxConditionService } from "../services/taxConditionService";
import { SearchCommand } from "../DTOs/SearchCommand";
import { uploadsService } from "../services/uploadsService";
import dayjs from "dayjs";

const Business = () => {
  const navigate = useNavigate();
  const [loadingLists, setLoadingLists] = useState(false);
  const [taxConditions, setTaxConditions] = useState([]);
  const [form] = Form.useForm();
  const [saving, setSaving] = useState(false);
  const [fileList, setFileList] = useState([]);

  useEffect(() => {
    const fetchData = async () => {
      setLoadingLists(true);
      try {
        const command = new SearchCommand();
        const [resConfiguration, resTaxCondition] = await Promise.all([
          configurationService.search([]),
          taxConditionService.search(command),
        ]);

        setTaxConditions(resTaxCondition);

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
  };

  return (
    <PageLayout title="Datos de la empresa" onClose={() => navigate("/configurations")}>
      <Form form={form} layout="vertical" onFinish={onFinish}>
        <Row gutter={15}>
          <Col span={8}>
            <Form.Item label="Nombre empresa" name="empresa">
              <Input placeholder="Nombre de la empresa" />
            </Form.Item>
          </Col>
          <Col>
            <Form.Item label="Fecha de inicio" name="fechaInicio">
              <DatePicker style={{ width: "100%" }} />
            </Form.Item>
          </Col>
        </Row>
        <Row gutter={15}>
          <Col span={4}>
            <Form.Item label="CUIT / CUIL" name="cuit">
              <Input placeholder="xxxxxxxxxxx" />
            </Form.Item>
          </Col>
          <Col span={4}>
            <Form.Item label="Condición fiscal" name="condicionFiscalId">
              <Select
                placeholder="Seleccione condición fiscal"
                loading={loadingLists}
                options={taxConditions.map((c) => ({
                  value: c.id,
                  label: c.description,
                }))}
              />
            </Form.Item>
          </Col>
        </Row>
        <Row gutter={15}>
          <Col span={4}>
            <Form.Item label="Alias ARCA" name="arcaAlias">
              <Input placeholder="Alias de ARCA" />
            </Form.Item>
          </Col>
          <Col span={4}>
            <Form.Item label="Clave certificado" name="arcaClave">
              <Input placeholder="*************" />
            </Form.Item>
          </Col>
        </Row>

        <Form.Item label="Certificado ARCA" name="arcaCertificado">
          <Input hidden />

          <Upload
            customRequest={handleCustomUpload}
            fileList={fileList} // <--- Vinculamos el estado visual
            onRemove={handleRemove} // <--- Usamos nuestra función de limpieza
            maxCount={1}
          >
            {/* Ocultamos el botón si ya hay un archivo para evitar confusiones, 
        o lo dejamos para permitir el reemplazo */}
            {fileList.length < 1 && (
              <Button icon={<UploadOutlined />}>Seleccionar Certificado</Button>
            )}
          </Upload>
        </Form.Item>

        <Form.Item>
          <Button type="primary" htmlType="submit" loading={saving}>
            Guardar Cambios
          </Button>
        </Form.Item>
      </Form>
    </PageLayout>
  );
};

export default Business;
