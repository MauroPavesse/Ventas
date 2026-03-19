import api from "./api";
import { SearchCommand } from "../DTOs/SearchCommand";
import { UserCreateCommand } from "../DTOs/users/UserCreateCommand";
import { UserUpdateCommand } from "../DTOs/users/UserUpdateCommand";

export const userService = {
  search: async (params) => {
    const body = new SearchCommand(params);
    const response = await api.post("/user/search", body);
    return response.data;
  },

  create: async (params) => {
    const body = new UserCreateCommand(params);
    const response = await api.post("/user", body);
    return response.data;
  },

  update: async (params) => {
    const body = new UserUpdateCommand(params);
    const response = await api.put("/user", body);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/user/${id}`);
    return response.data;
  },
};
