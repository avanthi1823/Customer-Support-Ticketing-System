import api from "./api";


export const getCommentsByTicket = async (ticketId) => {
  const res = await api.get(`/api/comments/${ticketId}`);
  return res.data;
};


export const addComment = async (ticketId, userId, message) => {
  const res = await api.post("/api/comments", { ticketId, userId, message });
  return res.data;
};
