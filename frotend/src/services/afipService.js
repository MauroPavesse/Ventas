import api from "./api";

export const afipService = {
  testConnection: async (certificadoPath, clave) => {
    const response = await api.post("/afip/test-connection", {
        "CertificadoPath": certificadoPath,
        "Clave": clave
    });
    return response.data;
  },
};
