import api from "./api";
import { SearchCommand } from "../DTOs/SearchCommand";
import { RolCreateCommand } from "../DTOs/roles/RolCreateCommand";
import { RolUpdateCommand } from "../DTOs/roles/RolUpdateCommand";

export const rolService = {
  search: async (params) => {
    const body = new SearchCommand(params);

    const response = await api.post("/rol/search", body);
    return response.data;
  },

  create: async (params) => {
    const body = new RolCreateCommand(params);

    const response = await api.post("/rol", body);
    return response.data;
  },

  update: async (params) => {
    const body = new RolUpdateCommand(params);
    const response = await api.put("/rol", body);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/rol/${id}`);
    return response.data;
  },
};
