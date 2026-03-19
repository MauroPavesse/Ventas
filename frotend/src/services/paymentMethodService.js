import api from "./api";
import { SearchCommand } from "../DTOs/SearchCommand";
import { PaymentMethodCreateCommand } from "../DTOs/paymentMethods/PaymentMethodCreateCommand";
import { PaymentMethodUpdateCommand } from "../DTOs/paymentMethods/PaymentMethodUpdateCommand";

export const paymentMethodService = {
  search: async (params) => {
    const body = new SearchCommand(params);

    const response = await api.post("/paymentMethod/search", body);
    return response.data;
  },

  create: async (params) => {
    const body = new PaymentMethodCreateCommand(params);
    console.log(body)
    const response = await api.post("/paymentMethod", body);
    return response.data;
  },

  update: async (params) => {
    const body = new PaymentMethodUpdateCommand(params);
    const response = await api.put("/paymentMethod", body);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/paymentMethod/${id}`);
    return response.data;
  },
};
