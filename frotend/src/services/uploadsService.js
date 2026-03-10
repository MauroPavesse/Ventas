import api from "./api";

export const uploadsService = {
  uploadCertificate: async (file) => {
    // FormData es necesario para enviar archivos binarios
    const formData = new FormData();
    formData.append("file", file);

    const response = await api.post("/uploads/certificate", formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
    return response.data;
  },
};