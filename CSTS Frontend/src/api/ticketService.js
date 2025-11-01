import api from "./api";


export const createTicket = async (ticketData) => {
  const response = await api.post("/ticket", ticketData);
  return response.data;
};


export const getAllTickets = async () => {
  const response = await api.get("/ticket");
  return response.data;
};


export const getTicketById = async (id) => {
  const response = await api.get(`/ticket/${id}`);
  return response.data;
};


export const getTicketsByUser = async (userId) => {
  const response = await api.get(`/ticket/user/${userId}`);
  return response.data;
};


export const assignTicket = async (ticketId, adminId, agentId) => {
  const response = await api.put(`/ticket/${ticketId}/assign/${adminId}/to/${agentId}`);
  return response.data;
};


export const updateTicket = async (id, data) => {
  const response = await api.put(`/ticket/${id}`, data);
  return response.data;
};


export const resolveTicket = async (id) => {
  const response = await api.put(`/ticket/${id}/resolve`);
  return response.data;
};


export const closeTicket = async (id) => {
  const response = await api.put(`/ticket/${id}/close`);
  return response.data;
};


export const deleteTicket = async (id) => {
  const response = await api.delete(`/ticket/${id}`);
  return response.data;
};
