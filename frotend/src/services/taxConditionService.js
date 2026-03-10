import api from "./api";
import { SearchCommand } from "../DTOs/SearchCommand";

export const taxConditionService = {
  search: async (params) => {
    const body = new SearchCommand(params);
    const response = await api.post("/taxcondition/search", body);
    return response.data;
  },
};
