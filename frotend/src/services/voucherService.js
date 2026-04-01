import api from "./api";
import { VoucherCreateCommand } from "../DTOs/vouchers/VoucherCreateCommand";
import { VoucherUpdateCommand } from "../DTOs/vouchers/VoucherUpdateCommand";
import { CloseSaleCommand } from "../DTOs/vouchers/closeSale/CloseSaleCommand";

export const voucherService = {
  search: async (params) => {
    const response = await api.post("/voucher/search", params);
    return response.data;
  },

  create: async (params) => {
    const body = new VoucherCreateCommand(params);

    const response = await api.post("/voucher", body);
    return response.data;
  },

  update: async (params) => {
    const body = new VoucherUpdateCommand(params);
    const response = await api.put("/voucher", body);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/voucher/${id}`);
    return response.data;
  },

  closeSale: async (params) => {
    const body = new CloseSaleCommand(params);
    const response = await api.post("/voucher/close-sale", body);
    return response.data;
  },
};
