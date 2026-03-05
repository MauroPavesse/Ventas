import api from "./api";
import { SearchCommand } from "../DTOs/SearchCommand";
import { VoucherCreateCommand } from "../DTOs/vouchers/VoucherCreateCommand";
import { VoucherUpdateCommand } from "../DTOs/vouchers/VoucherUpdateCommand";

export const voucherService = {
  search: async (params) => {
    const body = new SearchCommand(params);

    const response = await api.post("/voucher/search", body);
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
};
