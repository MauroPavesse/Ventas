import React, { useState, useEffect } from 'react';
import { Form, Input, Button, Card, Typography, message, App} from 'antd';
import { UserOutlined, LockOutlined } from '@ant-design/icons';
import { userService } from '../services/userService'; // Ajusta la ruta a tu service
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const { Title } = Typography;

const Login = () => {
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();
    const { user, login } = useAuth();
    const { message } = App.useApp();

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
        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh', background: '#f0f2f5' }}>
            <Card style={{ width: 400, boxShadow: '0 4px 12px rgba(0,0,0,0.1)' }}>
                <div style={{ textAlign: 'center', marginBottom: 24 }}>
                    <Title level={2}>Iniciar Sesión</Title>
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
                    >
                        <Input prefix={<UserOutlined />} placeholder="Usuario" size="large" />
                    </Form.Item>

                    <Form.Item
                        name="password"
                        rules={[{ required: true, message: 'Por favor ingresa tu contraseña' }]}
                    >
                        <Input.Password prefix={<LockOutlined />} placeholder="Contraseña" size="large" />
                    </Form.Item>

                    <Form.Item>
                        <Button type="primary" htmlType="submit" loading={loading} block size="large">
                            Entrar
                        </Button>
                    </Form.Item>
                </Form>
            </Card>
        </div>
    );
};

export default Login;