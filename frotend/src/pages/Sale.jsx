import React, { useState, useEffect, useRef } from "react";
import { useNavigate } from "react-router-dom";
import {
  Row,
  Col,
  Input,
  Table,
  Card,
  message,
  Button,
  InputNumber,
  Modal,
  Space,
  Grid
} from "antd";
import { PrinterOutlined, DeleteOutlined, FolderOpenOutlined } from '@ant-design/icons';
import PageLayout from "../layouts/PageLayout";
import { productService } from "../services/productService"; // Asumiendo esta ruta
import { SearchCommand } from "../DTOs/SearchCommand";
import CloseSale from "../components/CloseSale";
import { voucherService } from "../services/voucherService";
import { VoucherTypesEnum } from '../constants/voucherTypesEnum';
import { VoucherStateEnum } from '../constants/stateEntityEnum';
import { printService } from "../services/printService";

const { useBreakpoint } = Grid;

const Sale = () => {
  const navigate = useNavigate();
  const searchInputRef = useRef(null);
  const screens = useBreakpoint();
  const isMobile = screens.md === false; // Detecta celulares y tablets chicas

  const [products, setProducts] = useState([]);
  const [filteredProducts, setFilteredProducts] = useState([]);
  const [cart, setCart] = useState([]);
  const [searchText, setSearchText] = useState("");
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [voucherId, setVoucherId] = useState(0);
  const [isPendingModalVisible, setIsPendingModalVisible] = useState(false);
  const [pendingVouchers, setPendingVouchers] = useState([]);
  const [loadingPending, setLoadingPending] = useState(false);

  // Carga inicial
  useEffect(() => {
    const fetchProducts = async () => {
      try {
        const command = new SearchCommand({});
        const data = await productService.search(command); // Ajusta según tu servicio
        setProducts(data);
        setFilteredProducts(data);
      } catch (error) {
        message.error("Error al cargar productos: " + error);
      }
    };
    fetchProducts();
  }, []);

  const handleClose = () => {
    if (cart.length === 0) {
      return navigate("/dashboard");
    }

    // Si hay productos, preguntamos
    const instance = Modal.confirm({
      title: "¿Deseas guardar el carrito actual?",
      content: "Si guardas, podrás recuperarlo después como un presupuesto iniciado.",
      okText: "Guardar y Salir",
      cancelText: "Salir sin guardar",
      closable: true,
      // Acción si dice "SÍ" (Guardar)
      onOk: async () => {
        try {
          const userDataRaw = localStorage.getItem('user_data');
          const userData = userDataRaw ? JSON.parse(userDataRaw) : null;
          const userId = userData?.userId;

          await voucherService.closeSale({
            id: voucherId,
            items: cart,
            userId: userId,
            voucherTypeId: VoucherTypesEnum.ORDEN_DE_COMPRA,
            stateEntityId: VoucherStateEnum.INICIADO,
            payment: null,
            customerId: null
          });

          message.success("Carrito guardado correctamente");
          instance.destroy();
          navigate("/dashboard");
        } catch (error) {
          console.error(error);
          message.error("Error al guardar el carrito.");
        }
      },
      // Acción si dice "NO" (Salir sin guardar)
      onCancel: (e) => {
        // Si no es la "X" o ESC, navegamos y destruimos
        if (e && !e.triggerCancel) {
          instance.destroy();
          navigate("/dashboard");
        }
      },
    });
  };

  // Lógica de filtrado
  const handleSearch = (e) => {
    const value = e.target.value.toLowerCase();
    setSearchText(value);
    const filtered = products.filter((p) =>
      p.name.toLowerCase().includes(value) || 
      (p.code && p.code.toLowerCase().includes(value)) ||
      p.codeBar.includes(value)
    );
    setFilteredProducts(filtered);
  };

  // Manejo de ENTER en el buscador
  const handleKeyDown = (e) => {
    if (e.key === "Enter") {
      if (filteredProducts.length === 1) {
        addToCart(filteredProducts[0]);
        setSearchText("");
        setFilteredProducts(products); // Reset lista
      } else if (filteredProducts.length > 1) {
        // Foco en la primera fila de la tabla si es necesario
        message.info("Selecciona un producto de la lista");
      }
    }
  };

  const addToCart = (product) => {
    setCart((prevCart) => {
      // Buscamos por ID (que en ambos casos ahora es el ID del producto)
      const existing = prevCart.find((item) => item.id === product.id);

      if (existing) {
        return prevCart.map((item) =>
          item.id === product.id
            ? {
              ...item,
              quantity: item.quantity + 1,
              amountFinal: (item.quantity + 1) * item.price,
            }
            : item
        );
      }

      return [
        ...prevCart,
        {
          id: product.id,
          productId: product.id, // Lo guardamos explícitamente para el backend
          productName: product.name,
          price: product.price,
          quantity: 1,
          amountFinal: product.price,
        },
      ];
    });
    message.success(`${product.name} añadido`);
  };

  const updateQuantity = (id, newQuantity) => {
    if (newQuantity < 1) return;
    setCart((prevCart) =>
      prevCart.map((item) => {
        if (item.id === id) {
          return {
            ...item,
            quantity: newQuantity,
            amountFinal: newQuantity * (item.price || 0), // Usamos price que guardamos al normalizar
          };
        }
        return item;
      })
    );
  };

  const removeFromCart = (id) => {
    setCart((prevCart) => prevCart.filter((item) => item.id !== id));
    message.warning("Producto eliminado del carrito");
  };

  // Columnas
  const productColumns = [
    { title: "Nombre", dataIndex: "name", key: "name" },
    {
      title: "Precio",
      dataIndex: "price",
      key: "price",
      render: (p) => `$ ${p}`,
    },
    {
      title: "Acción",
      key: "action",
      render: (_, record) => (
        <Button type="link" onClick={() => addToCart(record)}>
          Agregar
        </Button>
      ),
    },
  ];

  const cartColumns = [
    { title: "Nombre", dataIndex: "productName", key: "productName" },
    {
      title: "Cant.",
      dataIndex: "quantity",
      key: "quantity",
      render: (value, record) => (
        <InputNumber
          min={1}
          value={value}
          onChange={(val) => updateQuantity(record.id, val)}
          style={{ width: 60 }}
        />
      ),
    },
    {
      title: "Importe",
      dataIndex: "amountFinal",
      key: "amountFinal",
      render: (i) => <strong>$ {i}</strong>,
    },
    {
      title: "",
      key: "delete",
      render: (_, record) => (
        <Button
          type="text"
          danger
          icon={<DeleteOutlined />}
          onClick={() => removeFromCart(record.id)}
        />
      ),
    },
  ];

  const totalCart = cart.reduce((acc, item) => acc + (item.amountFinal || 0), 0);

  const handleConfirmSale = async (saleData) => {
    const userDataRaw = localStorage.getItem('user_data');
    const userData = userDataRaw ? JSON.parse(userDataRaw) : null;
    const userId = userData?.userId;

    await voucherService.closeSale({
      id: voucherId,
      number: 0,
      items: cart,
      userId: userId,
      voucherTypeId: VoucherTypesEnum.ORDEN_DE_COMPRA,
      stateEntityId: VoucherStateEnum.FINALIZADO,
      payment: saleData.method,
      customerId: saleData.customer,
      ...saleData
    });
    message.success("¡Venta procesada con éxito!");
    setVoucherId(0); // Limpiar id de comprobante
    setCart([]); // Limpiar carrito
    setIsModalVisible(false);
  };

  const printBudget = async () => {
    try {
      const userDataRaw = localStorage.getItem('user_data');
      const userData = userDataRaw ? JSON.parse(userDataRaw) : null;
      const userId = userData?.userId;

      const id = voucherId;
      const response = await voucherService.closeSale({
        id: id,
        items: cart,
        userId: userId,
        voucherTypeId: VoucherTypesEnum.ORDEN_DE_COMPRA,
        stateEntityId: VoucherStateEnum.INICIADO,
        payment: null,
        customerId: null
      });

      setVoucherId(response.id);
      const blob = await printService.printBudget(response.id);
      const url = window.URL.createObjectURL(new Blob([blob], { type: 'application/pdf' }));
      window.open(url, '_blank');
      setTimeout(() => window.URL.revokeObjectURL(url), 100);
    } catch (error) {
      console.error(error);
      message.error("No se pudo generar el PDF del comprobante.");
    }
  };

  const fetchPendingVouchers = async () => {
    setLoadingPending(true);
    try {
      // Asumiendo que tu servicio tiene un método de búsqueda
      const data = await voucherService.search({
        filters: [{
          field: "StateEntityId",
          value: VoucherStateEnum.INICIADO.toString()
        }]
      });
      setPendingVouchers(data);
      setIsPendingModalVisible(true);
    } catch (error) {
      message.error("Error al cargar pendientes");
    } finally {
      setLoadingPending(false);
    }
  };

  const loadVoucher = (voucher) => {
    // Normalizamos los detalles para que tengan el campo 'price' que usa tu updateQuantity
    const normalizedDetails = voucher.voucherDetails.map(detail => ({
      ...detail,
      id: detail.productId, // IMPORTANTE: Usar el productId como key para que addToCart lo encuentre
      productName: detail.productName,
      price: detail.priceUnit, // Mapeamos priceUnit a price
      quantity: detail.quantity,
      amountFinal: detail.amountFinal
    }));

    setCart(normalizedDetails);
    setVoucherId(voucher.id);
    setIsPendingModalVisible(false);
    message.success(`Comprobante N° ${voucher.id} cargado`);
  };

  const deleteVoucher = async (voucherDeleteId) => {
    try {
      // 1. Esperamos al servidor
      await voucherService.delete(voucherDeleteId);

      // 2. Si el voucher eliminado es el que teníamos cargado en el carrito, lo limpiamos
      if (voucherId === voucherDeleteId) {
        setVoucherId(0);
        setCart([]);
      }

      // 3. ACTUALIZACIÓN CRÍTICA: Quitamos el voucher de la lista del modal
      setPendingVouchers((prev) => prev.filter(v => v.id !== voucherDeleteId));

      message.success("Presupuesto eliminado correctamente");
    } catch (error) {
      console.error(error);
      message.error("No se pudo eliminar el presupuesto");
    }
  };

  return (
    <PageLayout title="Nueva Venta" onClose={handleClose}>
      <Row gutter={[16, 16]}>

        {/* Input Buscador */}
        <Col span={24}>
          <Input.Search
            ref={searchInputRef}
            placeholder="Buscar producto y presionar ENTER..."
            value={searchText}
            onChange={handleSearch}
            onKeyDown={handleKeyDown}
            autoFocus
            size="large"
          />
        </Col>

        {/* Sección Productos Disponibles */}
        <Col xs={24} md={12}>
          <Card title="Productos Disponibles" styles={{ body: { padding: isMobile ? '8px' : '24px' } }}>
            <Table
              dataSource={filteredProducts}
              columns={productColumns}
              rowKey="id"
              pagination={{ pageSize: isMobile ? 4 : 5, size: "small" }}
              scroll={{ x: true }} // Evita desbordamiento en móviles ultra chicos
            />
          </Card>
        </Col>

        {/* Sección Carrito de Compras */}
        <Col xs={24} md={12}>
          <Card title="Carrito de Compras" styles={{ body: { padding: isMobile ? '8px' : '24px' } }}>
            <Table
              dataSource={cart}
              columns={cartColumns}
              rowKey="id"
              pagination={isMobile ? { pageSize: 4, size: "small" } : false} // Paginación en móvil para acortar vista
              locale={{ emptyText: "El carrito está vacío" }}
              scroll={{ x: true }}
            />
            <div style={{ marginTop: 16, textAlign: "right" }}>
              <h3 style={{ margin: 0, fontSize: isMobile ? '18px' : '22px' }}>
                Total: ${cart.reduce((acc, item) => acc + item.amountFinal, 0)}
              </h3>
            </div>
          </Card>

          {/* Botonera de Acciones Adaptativa */}
          <div style={{
            display: 'flex',
            flexDirection: isMobile ? 'column-reverse' : 'row',
            gap: '10px',
            justifyContent: 'flex-end',
            marginTop: 16
          }}>
            <Button
              onClick={fetchPendingVouchers}
              type="dashed"
              block={isMobile}
              icon={<FolderOpenOutlined />}
            >
              Recuperar Pendiente
            </Button>

            <Button
              style={{ marginRight: isMobile ? 0 : 2 }}
              icon={<PrinterOutlined />}
              onClick={() => {
                if (cart.length === 0) return message.warning("El carrito está vacío");
                printBudget();
              }}
              block={isMobile}
            >
              PRESUPUESTO
            </Button>

            <Button
              type="primary"
              onClick={() => {
                if (cart.length === 0) return message.warning("El carrito está vacío");
                setIsModalVisible(true);
              }}
              block={isMobile}
              size={isMobile ? "large" : "default"} // Botón de pago más grande en celular
            >
              PAGAR
            </Button>
          </div>
        </Col>
      </Row>

      <CloseSale
        visible={isModalVisible}
        onClose={() => setIsModalVisible(false)}
        totalAmount={totalCart}
        onConfirm={handleConfirmSale}
      />

      {/* Modal de Pendientes Adaptativo */}
      <Modal
        title="Ventas Pendientes / Presupuestos"
        open={isPendingModalVisible}
        onCancel={() => setIsPendingModalVisible(false)}
        footer={null}
        width={isMobile ? "95%" : 700}
        style={{ top: isMobile ? 20 : 100 }}
      >
        <Table
          dataSource={pendingVouchers}
          rowKey="id"
          loading={loadingPending}
          size={isMobile ? "small" : "default"}
          scroll={{ x: true }}
          columns={[
            { title: "Nro", dataIndex: "id", key: "id", width: 60 },
            {
              title: "Fecha",
              dataIndex: "dateCreation",
              key: "dateCreation",
              render: (date) => new Date(date).toLocaleDateString('es-AR', { day: '2-digit', month: '2-digit' })
            },
            { title: "Total", dataIndex: "amountTotal", key: "amountTotal", render: (t) => `$${t}` },
            {
              title: "Acciones",
              key: "action",
              width: isMobile ? 110 : 140,
              render: (_, record) => (
                <Space size={isMobile ? "small" : "middle"}>
                  <Button
                    type="primary"
                    size="small"
                    onClick={() => loadVoucher(record)}
                  >
                    Cargar
                  </Button>
                  <Button
                    type="text"
                    danger
                    size="small"
                    icon={<DeleteOutlined />}
                    onClick={() => deleteVoucher(record.id)}
                  />
                </Space>
              ),
            },
          ]}
        />
      </Modal>
    </PageLayout>
  );
};

export default Sale;
