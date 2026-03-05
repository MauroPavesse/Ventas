import api from "./api";
import { SearchCommand } from "../DTOs/SearchCommand";
import { DailyBoxCreateCommand } from "../DTOs/dailyBoxes/DailyBoxCreateCommand";
import { DailyBoxUpdateCommand } from "../DTOs/dailyBoxes/DailyBoxUpdateCommand";

export const dailyBoxService = {
  search: async (params) => {
    const body = new SearchCommand(params);

    const response = await api.post("/dailyBox/search", body);
    return response.data;
  },

  create: async (params) => {
    const body = new DailyBoxCreateCommand(params);

    const response = await api.post("/dailyBox", body);
    return response.data;
  },

  update: async (params) => {
    const body = new DailyBoxUpdateCommand(params);
    const response = await api.put("/dailyBox", body);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/dailyBox/${id}`);
    return response.data;
  },
};
