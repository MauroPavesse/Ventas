import api from "./api";
import { SearchCommand } from "../DTOs/SearchCommand";
import { CategoryCreateCommand } from "../DTOs/categories/CategoryCreateCommand"
import { CategoryUpdateCommand } from "../DTOs/categories/CategoryUpdateCommand"

export const categoryService = {
  search: async (params) => {
    const body = new SearchCommand(params);

    const response = await api.post("/category/search", body);
    return response.data;
  },

  create: async (params) => {
    const body = new CategoryCreateCommand(params);

    const response = await api.post("/category", body);
    return response.data;
  },

  update: async (params) => {
    const body = new CategoryUpdateCommand(params);
    const response = await api.put("/category", body);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/category/${id}`);
    return response.data;
  },
};
