import api from "./api";
import { SearchCommand } from "../DTOs/SearchCommand";
import { VoucherDetailCreateCommand } from "../DTOs/voucherDetails/VoucherDetailCreateCommand";
import { VoucherDetailUpdateCommand } from "../DTOs/voucherDetails/VoucherDetailUpdateCommand";

export const voucherDetailService = {
  search: async (params) => {
    const body = new SearchCommand(params);

    const response = await api.post("/voucherDetail/search", body);
    return response.data;
  },

  create: async (params) => {
    const body = new VoucherDetailCreateCommand(params);

    const response = await api.post("/voucherDetail", body);
    return response.data;
  },

  update: async (params) => {
    const body = new VoucherDetailUpdateCommand(params);
    const response = await api.put("/voucherDetail", body);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/voucherDetail/${id}`);
    return response.data;
  },
};
