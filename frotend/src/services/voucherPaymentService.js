import api from "./api";
import { SearchCommand } from "../DTOs/SearchCommand";
import { VoucherPaymentCreateCommand } from "../DTOs/voucherPayments/VoucherPaymentCreateCommand";
import { VoucherPaymentUpdateCommand } from "../DTOs/voucherPayments/VoucherPaymentUpdateCommand";

export const voucherPaymentService = {
  search: async (params) => {
    const body = new SearchCommand(params);

    const response = await api.post("/voucherPayment/search", body);
    return response.data;
  },

  create: async (params) => {
    const body = new VoucherPaymentCreateCommand(params);

    const response = await api.post("/voucherPayment", body);
    return response.data;
  },

  update: async (params) => {
    const body = new VoucherPaymentUpdateCommand(params);
    const response = await api.put("/voucherPayment", body);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/voucherPayment/${id}`);
    return response.data;
  },
};
