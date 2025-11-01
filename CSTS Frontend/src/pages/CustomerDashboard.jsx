import React, { useEffect, useState } from "react";
import {
  FaUser,
  FaTicketAlt,
  FaClipboardCheck,
  FaPlusCircle,
  FaCommentDots,
  FaPaperPlane,
  FaCheckCircle,
} from "react-icons/fa";
import axios from "axios";
import "bootstrap/dist/css/bootstrap.min.css";

const CustomerDashboard = () => {
  const [userName, setUserName] = useState("Customer");
  const [userId, setUserId] = useState("");
  const [tickets, setTickets] = useState([]);
  const [selectedTicket, setSelectedTicket] = useState(null);
  const [comments, setComments] = useState([]);
  const [newTicket, setNewTicket] = useState({
    title: "",
    description: "",
    priority: 1,
  });
  const [newComment, setNewComment] = useState("");
  const [loading, setLoading] = useState(true);

  const BASE_URL = "http://localhost:5041/api";
  const token = localStorage.getItem("token");
  const headers = { Authorization: `Bearer ${token}` };

  useEffect(() => {
    const name = localStorage.getItem("name");
    const id = localStorage.getItem("userId");
    if (name) setUserName(name);
    if (id) setUserId(id);
    if (id) fetchTickets(id);
  }, []);


  const fetchTickets = async (id) => {
    try {
      const res = await axios.get(`${BASE_URL}/ticket/user/${id}`, { headers });
      setTickets(res.data);
    } catch (err) {
      console.error("Error fetching tickets:", err);
    } finally {
      setLoading(false);
    }
  };

  
  const handleCreateTicket = async (e) => {
    e.preventDefault();
    if (!newTicket.title || !newTicket.description)
      return alert("Please fill all fields!");

    const payload = {
      title: newTicket.title,
      description: newTicket.description,
      priority: Number(newTicket.priority),
      createdById: Number(userId),
    };

    try {
      await axios.post(`${BASE_URL}/ticket`, payload, { headers });
      alert("✅ Ticket created successfully!");
      setNewTicket({ title: "", description: "", priority: 1 });
      fetchTickets(userId);
    } catch (err) {
      console.error("Error creating ticket:", err);
      alert("❌ Failed to create ticket.");
    }
  };

 
  const fetchComments = async (ticketId) => {
    try {
      const res = await axios.get(`${BASE_URL}/comment/${ticketId}`, { headers });
      setComments(res.data);
      setSelectedTicket(ticketId);
    } catch (err) {
      console.error("Error fetching comments:", err);
    }
  };

  const handleAddComment = async (ticketId) => {
    if (!newComment.trim()) return alert("Please write a comment.");
    const payload = {
      ticketId: Number(ticketId),
      userId: Number(userId),
      message: newComment.trim(),
    };

    try {
      await axios.post(`${BASE_URL}/comment`, payload, { headers });
      alert("💬 Comment added!");
      setNewComment("");
      fetchComments(ticketId);
    } catch (err) {
      console.error("Error adding comment:", err);
      alert("❌ Failed to add comment.");
    }
  };

  
  const handleCloseTicket = async (ticketId) => {
    if (!window.confirm("Are you sure you want to close this ticket?")) return;

    try {
      await axios.put(`${BASE_URL}/Ticket/${ticketId}/close`, {}, { headers });
      alert("✅ Ticket closed successfully!");
      fetchTickets(userId);
    } catch (err) {
      console.error("Error closing ticket:", err.response?.data || err.message);
      alert(`❌ Failed to close ticket: ${err.response?.data?.message || err.message}`);
    }
  };

  const getStatusLabel = (status) => {
    switch (status) {
      case 1:
        return "New";
      case 2:
        return "Assigned";
      case 3:
        return "In Progress";
      case 4:
        return "Resolved";
      case 5:
        return "Closed";
      default:
        return "Unknown";
    }
  };

  const totalTickets = tickets.length;
  const closedTickets = tickets.filter((t) => t.status === 5).length;
  const activeTickets = tickets.filter((t) => t.status !== 5).length;

  const styles = {
    container: { padding: "2rem", backgroundColor: "#f8f9fa", minHeight: "100vh" },
    header: { textAlign: "center", marginBottom: "2rem" },
    summaryCard: {
      borderRadius: "10px",
      padding: "20px",
      textAlign: "center",
      fontWeight: "500",
      boxShadow: "0 2px 8px rgba(0,0,0,0.1)",
    },
    blueCard: { backgroundColor: "#4a90e2", color: "white" },
    orangeCard: { backgroundColor: "#f39c12", color: "white" },
    greenCard: { backgroundColor: "#28a745", color: "white" },
    tableCard: {
      background: "white",
      borderRadius: "10px",
      boxShadow: "0 2px 8px rgba(0,0,0,0.1)",
      padding: "20px",
      marginTop: "20px",
    },
    modalBackdrop: {
      position: "fixed",
      top: 0,
      left: 0,
      right: 0,
      bottom: 0,
      background: "rgba(0,0,0,0.4)",
      display: "flex",
      alignItems: "center",
      justifyContent: "center",
      zIndex: 1000,
    },
    commentModal: {
      background: "#fff",
      borderRadius: "10px",
      padding: "20px",
      width: "450px",
      boxShadow: "0 4px 12px rgba(0,0,0,0.2)",
    },
  };

  if (loading)
    return (
      <div style={{ textAlign: "center", padding: "50px", fontSize: "1.2rem" }}>
        Loading Customer Dashboard...
      </div>
    );

  return (
    <div style={styles.container}>
      {/* Header */}
      <div style={styles.header}>
        <h3 style={{ color: "#0056b3", fontWeight: "700" }}>
          <FaUser className="me-2" /> Welcome, {userName} 👋
        </h3>
        <p style={{ color: "#555" }}>Manage your support tickets below</p>
      </div>

      {/* Summary Cards */}
      <div className="row text-center g-4 mb-4">
        <div className="col-md-4">
          <div style={{ ...styles.summaryCard, ...styles.blueCard }}>
            <h5>Total Tickets</h5>
            <h3>{totalTickets}</h3>
          </div>
        </div>
        <div className="col-md-4">
          <div style={{ ...styles.summaryCard, ...styles.orangeCard }}>
            <h5>Active Tickets</h5>
            <h3>{activeTickets}</h3>
          </div>
        </div>
        <div className="col-md-4">
          <div style={{ ...styles.summaryCard, ...styles.greenCard }}>
            <h5>Closed Tickets</h5>
            <h3>{closedTickets}</h3>
          </div>
        </div>
      </div>

      {/* Create Ticket */}
      <div style={styles.tableCard}>
        <h5 className="text-primary fw-bold mb-3">
          <FaPlusCircle className="me-2" /> Create New Ticket
        </h5>

        <form onSubmit={handleCreateTicket}>
          <div className="mb-3">
            <label className="form-label fw-semibold">Title</label>
            <input
              type="text"
              className="form-control"
              placeholder="Enter ticket title"
              value={newTicket.title}
              onChange={(e) => setNewTicket({ ...newTicket, title: e.target.value })}
            />
          </div>

          <div className="mb-3">
            <label className="form-label fw-semibold">Description</label>
            <textarea
              className="form-control"
              rows="3"
              placeholder="Describe your issue..."
              value={newTicket.description}
              onChange={(e) =>
                setNewTicket({ ...newTicket, description: e.target.value })
              }
            />
          </div>

          <div className="mb-3">
            <label className="form-label fw-semibold">Priority</label>
            <select
              className="form-select"
              value={newTicket.priority}
              onChange={(e) =>
                setNewTicket({ ...newTicket, priority: e.target.value })
              }
            >
              <option value={1}>Low</option>
              <option value={2}>Medium</option>
              <option value={3}>High</option>
            </select>
          </div>

          <button type="submit" className="btn btn-primary">
            <FaClipboardCheck className="me-1" /> Submit Ticket
          </button>
        </form>
      </div>

      {/* Tickets Table */}
      <div style={styles.tableCard}>
        <h5 className="text-primary fw-bold mb-3">
          <FaTicketAlt className="me-2" /> My Tickets
        </h5>

        <table className="table table-bordered align-middle">
          <thead className="table-light">
            <tr>
              <th>ID</th>
              <th>Title</th>
              <th>Description</th>
              <th>Status</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            {tickets.length === 0 ? (
              <tr>
                <td colSpan="5" className="text-center text-muted">
                  No tickets found.
                </td>
              </tr>
            ) : (
              tickets.map((t) => (
                <tr key={t.ticketId}>
                  <td>{t.ticketId}</td>
                  <td>{t.title}</td>
                  <td>{t.description}</td>
                  <td>
                    <span
                      className={`badge ${
                        t.status === 5
                          ? "bg-secondary"
                          : t.status === 4
                          ? "bg-success"
                          : "bg-warning text-dark"
                      }`}
                    >
                      {getStatusLabel(t.status)}
                    </span>
                  </td>
                  <td className="d-flex gap-2">
                    <button
                      className="btn btn-sm btn-outline-primary"
                      onClick={() => fetchComments(t.ticketId)}
                    >
                      <FaCommentDots className="me-1" /> Comments
                    </button>

                    {/* ✅ Show "Close Ticket" only if status is Resolved */}
                    {t.status === 4 && (
                      <button
                        className="btn btn-sm btn-outline-success"
                        onClick={() => handleCloseTicket(t.ticketId)}
                      >
                        <FaCheckCircle className="me-1" /> Close Ticket
                      </button>
                    )}
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {/* 💬 Comment Modal */}
      {selectedTicket && (
        <div
          style={styles.modalBackdrop}
          onClick={(e) => {
            if (e.target === e.currentTarget) setSelectedTicket(null);
          }}
        >
          <div
            style={styles.commentModal}
            onClick={(e) => e.stopPropagation()}
          >
            <h5 className="mb-3">
              <FaCommentDots className="me-2 text-primary" />
              Comments for Ticket #{selectedTicket}
            </h5>

            {comments.length === 0 ? (
              <p className="text-muted">No comments yet.</p>
            ) : (
              <ul className="list-group mb-3">
                {comments.map((c) => (
                  <li key={c.id} className="list-group-item">
                    <strong>{c.userName || "User"}:</strong> {c.message}
                  </li>
                ))}
              </ul>
            )}

            <textarea
              className="form-control my-2"
              rows="3"
              value={newComment}
              onChange={(e) => setNewComment(e.target.value)}
              placeholder="Write a comment..."
            ></textarea>

            <div className="text-end">
              <button
                className="btn btn-primary me-2"
                onClick={() => handleAddComment(selectedTicket)}
              >
                <FaPaperPlane className="me-1" /> Send
              </button>
              <button
                className="btn btn-secondary"
                onClick={() => setSelectedTicket(null)}
              >
                Close
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default CustomerDashboard;
