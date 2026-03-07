import api from "./api";
import { SearchCommand } from "../DTOs/SearchCommand";

export const taxRateService = {
  search: async (params) => {
    const body = new SearchCommand(params);
    const response = await api.post("/taxrate/search", body);
    return response.data;
  },
};
