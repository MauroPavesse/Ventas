import api from "./api";

export const printService = {
    printTicket: async (id) => {
        const response = await api.get(`/print/ticket/${id}`, {
            responseType: 'blob'
        });
        return response.data;
    },

    printInvoice: async (id) => {
        const response = await api.get(`/print/budget/${id}`, {
            responseType: 'blob'
        });
        return response.data;
    },

    printDailyBox: async (id) => {
        const response = await api.get(`/print/dailyBox/${id}`, {
            responseType: 'blob'
        });
        return response.data;
    }
};