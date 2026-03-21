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
} from "antd";
import PageLayout from "../layouts/PageLayout";
import { productService } from "../services/productService"; // Asumiendo esta ruta
import { SearchCommand } from "../DTOs/SearchCommand";
import { DeleteOutlined } from "@ant-design/icons";
import CloseSale from "../components/CloseSale";

const Sale = () => {
  const navigate = useNavigate();
  const searchInputRef = useRef(null);
  const [products, setProducts] = useState([]);
  const [filteredProducts, setFilteredProducts] = useState([]);
  const [cart, setCart] = useState([]);
  const [searchText, setSearchText] = useState("");
  const [isModalVisible, setIsModalVisible] = useState(false);

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

  // Lógica de filtrado
  const handleSearch = (e) => {
    const value = e.target.value.toLowerCase();
    setSearchText(value);
    const filtered = products.filter((p) =>
      p.name.toLowerCase().includes(value),
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
      const existing = prevCart.find((item) => item.id === product.id);
      if (existing) {
        return prevCart.map((item) =>
          item.id === product.id
            ? {
                ...item,
                quantity: item.quantity + 1,
                import: (item.quantity + 1) * item.price,
              }
            : item,
        );
      }
      return [...prevCart, { ...product, quantity: 1, import: product.price }];
    });
    message.success(`${product.name} añadido`);
  };

  const updateQuantity = (id, newQuantity) => {
    if (newQuantity < 1) return; // Evitar cantidades negativas o cero
    setCart((prevCart) =>
      prevCart.map((item) => {
        if (item.id === id) {
          return {
            ...item,
            quantity: newQuantity,
            import: newQuantity * item.price,
          };
        }
        return item;
      }),
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
      render: (p) => `$${p}`,
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
    { title: "Nombre", dataIndex: "name", key: "name" },
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
      dataIndex: "import",
      key: "import",
      render: (i) => <strong>${i}</strong>,
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

  const totalCart = cart.reduce((acc, item) => acc + item.import, 0);

  const handleConfirmSale = (saleData) => {
    
    console.log("Enviando venta al backend:", {
      items: cart,
      ...saleData,
    });
    message.success("¡Venta procesada con éxito!");
    setCart([]); // Limpiar carrito
    setIsModalVisible(false);
  };

  return (
    <PageLayout title="Nueva Venta" onClose={() => navigate("/dashboard")}>
      <Row gutter={[16, 16]}>
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

        <Col span={12}>
          <Card title="Productos Disponibles">
            <Table
              dataSource={filteredProducts}
              columns={productColumns}
              rowKey="id"
              pagination={{ pageSize: 5 }}
            />
          </Card>
        </Col>

        <Col span={12}>
          <Card title="Carrito de Compras">
            <Table
              dataSource={cart}
              columns={cartColumns}
              rowKey="id"
              locale={{ emptyText: "El carrito está vacío" }}
            />
            <div style={{ marginTop: 16, textAlign: "right" }}>
              <h3>
                Total: ${cart.reduce((acc, item) => acc + item.import, 0)}
              </h3>
            </div>
          </Card>
          <Button
            type="primary"
            style={{ marginTop: 16, float: "inline-end" }}
            onClick={() => {
              if (cart.length === 0)
                return message.warning("El carrito está vacío");
              setIsModalVisible(true);
            }}
          >
            PAGAR
          </Button>
        </Col>
      </Row>

      <CloseSale
        visible={isModalVisible}
        onClose={() => setIsModalVisible(false)}
        totalAmount={totalCart}
        onConfirm={handleConfirmSale}
      />
    </PageLayout>
  );
};

export default Sale;
