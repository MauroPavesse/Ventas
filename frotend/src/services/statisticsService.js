import api from "./api";

export const statisticsService = {
  getSalesAmount: async (body) => {
    const response = await api.post("/statistics/sales-amount", body);
    return response.data;
  },

  getTopProducts: async (body) => {
    const response = await api.post("/statistics/top-products", body);
    return response.data;
  },
};