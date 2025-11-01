import api from "./api";


export const getAllUsers = async () => {
  const response = await api.get("/user/all");
  return response.data;
};


export const getAllAgents = async () => {
  const response = await api.get("/user/agents");
  return response.data;
};


export const getAllAdmins = async () => {
  const response = await api.get("/user/admins");
  return response.data;
};


export const activateUser = async (id) => {
  const response = await api.put(`/user/${id}/activate`);
  return response.data;
};

export const deactivateUser = async (id) => {
  const response = await api.put(`/user/${id}/deactivate`);
  return response.data;
};
