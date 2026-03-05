import api from "./api";
import { SearchCommand } from "../DTOs/SearchCommand";
import { PointOfSaleCreateCommand } from "../DTOs/pointOfSales/PointOfSaleCreateCommand";
import { PointOfSaleUpdateCommand } from "../DTOs/pointOfSales/PointOfSaleUpdateCommand";

export const pointOfSaleService = {
  search: async (params) => {
    const body = new SearchCommand(params);

    const response = await api.post("/pointOfSale/search", body);
    return response.data;
  },

  create: async (params) => {
    const body = new PointOfSaleCreateCommand(params);

    const response = await api.post("/pointOfSale", body);
    return response.data;
  },

  update: async (params) => {
    const body = new PointOfSaleUpdateCommand(params);
    const response = await api.put("/pointOfSale", body);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/pointOfSale/${id}`);
    return response.data;
  },
};
