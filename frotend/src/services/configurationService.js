import api from "./api";
import { ConfigurationSearchCommand } from "../DTOs/configurations/ConfigurationSearchCommand";
import { ConfigurationUpdateCommand } from "../DTOs/configurations/ConfigurationUpdateCommand"

export const configurationService = {
  search: async (params) => {
    const body = new ConfigurationSearchCommand(params);
    const response = await api.post("/configuration/search", body);
    return response.data;
  },

  update: async (params) => {
    const response = await api.put("/configuration", params);
    return response.data;
  },
  
};
