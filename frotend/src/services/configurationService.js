import api from "./api";
import { ConfigurationSearchCommand } from "../DTOs/configurations/ConfigurationSearchCommand";

export const configurationService = {
  search: async (variablesArray) => {
    const response = await api.post("/configuration/search", null, {
      params: { command: variablesArray },
      // Esto serializa automáticamente a: ?command=var1&command=var2
      paramsSerializer: {
        indexes: null // Importante para que no mande command[0]=...
      }
    });
    return response.data;
  },

  update: async (params) => {
    const response = await api.put("/configuration", params);
    return response.data;
  },
  
};
