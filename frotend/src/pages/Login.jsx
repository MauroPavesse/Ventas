import React, { useState, useEffect } from 'react';
import { Form, Input, Button, Card, Typography, App, Grid } from 'antd';
import { UserOutlined, LockOutlined } from '@ant-design/icons';
import { userService } from '../services/userService'; // Ajusta la ruta a tu service
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const { Title } = Typography;
const { useBreakpoint } = Grid;

const Login = () => {
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();
    const { user, login } = useAuth();
    const { message } = App.useApp();
    const screens = useBreakpoint();
    const isMobile = screens.xs; // Verdadero en pantallas ultra chicas (< 576px)

    useEffect(() => {
        if (user) {
            navigate('/dashboard');
        }
    }, [user, navigate]);

    const onFinish = async (values) => {
        setLoading(true);
        try {
            const response = await userService.login(values);

            login(response);

            message.success(`Bienvenido, ${response.userName}`);
            navigate('/dashboard');
        } catch (error) {
            const errorMsg = error.response?.data?.Message || error.response?.data?.message || 'Error al conectar con el servidor';
            message.error(errorMsg);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div
            style={{
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'center',
                minHeight: '100vh', // Permite scroll si el teclado virtual colapsa el espacio
                background: '#f0f2f5',
                padding: '0 16px', // Previene que la tarjeta toque los bordes físicos del celular
            }}
        >
            <Card
                style={{
                    width: '100%',
                    maxWidth: 400, // Comportamiento fluido (elástico)
                    boxShadow: '0 4px 12px rgba(0,0,0,0.1)',
                    borderRadius: isMobile ? '12px' : '8px'
                }}
                bodyStyle={{
                    padding: isMobile ? '24px 16px' : '24px 32px' // Menos padding interno en móvil para ganar espacio
                }}
            >
                <div style={{ textAlign: 'center', marginBottom: 24 }}>
                    <Title level={isMobile ? 3 : 2} style={{ margin: 0 }}>
                        Iniciar Sesión
                    </Title>
                </div>

                <Form
                    name="login_form"
                    initialValues={{ remember: true }}
                    onFinish={onFinish}
                    layout="vertical"
                >
                    <Form.Item
                        name="username"
                        rules={[{ required: true, message: 'Por favor ingresa tu usuario' }]}
                        style={{ marginBottom: 20 }}
                    >
                        <Input
                            prefix={<UserOutlined style={{ color: '#bfbfbf' }} />}
                            placeholder="Usuario"
                            size="large"
                            autoComplete="username"
                        />
                    </Form.Item>

                    <Form.Item
                        name="password"
                        rules={[{ required: true, message: 'Por favor ingresa tu contraseña' }]}
                        style={{ marginBottom: 24 }}
                    >
                        <Input.Password
                            prefix={<LockOutlined style={{ color: '#bfbfbf' }} />}
                            placeholder="Contraseña"
                            size="large"
                            autoComplete="current-password"
                        />
                    </Form.Item>

                    <Form.Item style={{ marginBottom: 0 }}>
                        <Button
                            type="primary"
                            htmlType="submit"
                            loading={loading}
                            block
                            size="large"
                            style={{ height: '45px', fontSize: '16px', fontWeight: '500' }}
                        >
                            Entrar
                        </Button>
                    </Form.Item>
                </Form>
            </Card>
        </div>
    );
};

export default Login;