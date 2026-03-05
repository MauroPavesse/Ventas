import api from "./api";
import { SearchCommand } from "../DTOs/SearchCommand";
import { PointOfSaleVoucherTypeCreateCommand } from "../DTOs/pointOfSaleVoucherTypes/PointOfSaleVoucherTypeCreateCommand";
import { PointOfSaleVoucherTypeUpdateCommand } from "../DTOs/pointOfSaleVoucherTypes/PointOfSaleVoucherTypeUpdateCommand";

export const pointOfSaleVoucherTypeService = {
  search: async (params) => {
    const body = new SearchCommand(params);

    const response = await api.post("/pointOfSaleVoucherType/search", body);
    return response.data;
  },

  create: async (params) => {
    const body = new PointOfSaleVoucherTypeCreateCommand(params);

    const response = await api.post("/pointOfSaleVoucherType", body);
    return response.data;
  },

  update: async (params) => {
    const body = new PointOfSaleVoucherTypeUpdateCommand(params);
    const response = await api.put("/pointOfSaleVoucherType", body);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/pointOfSaleVoucherType/${id}`);
    return response.data;
  },
};
