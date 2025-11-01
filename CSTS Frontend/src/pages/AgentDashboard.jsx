import React, { useEffect, useState } from "react";
import {
  FaHeadset,
  FaTicketAlt,
  FaClipboardCheck,
  FaCheckCircle,
  FaCommentDots,
  FaUserCircle,
} from "react-icons/fa";
import "bootstrap/dist/css/bootstrap.min.css";

const AgentDashboard = () => {
  const [tickets, setTickets] = useState([]);
  const [agentName, setAgentName] = useState("Agent");
  const [loading, setLoading] = useState(true);
  const [comment, setComment] = useState("");
  const [selectedTicket, setSelectedTicket] = useState(null);

  const BASE_URL = "http://localhost:5041/api";
  const token = localStorage.getItem("token");

  const headers = {
    "Content-Type": "application/json",
    Authorization: `Bearer ${token}`,
  };

  const decodeToken = () => {
    if (!token) return null;
    try {
      const payload = JSON.parse(atob(token.split(".")[1]));
      return payload;
    } catch {
      return null;
    }
  };

  const getUserNameFromToken = () => {
    const decoded = decodeToken();
    return (
      decoded?.unique_name ||
      decoded?.name ||
      decoded?.email ||
      decoded?.given_name ||
      "Agent"
    );
  };

  const fetchTickets = async () => {
    try {
      const res = await fetch(`${BASE_URL}/ticket`, { headers });
      const data = await res.json();
      const user = getUserNameFromToken();

      const filtered = data.filter(
        (t) =>
          t.assignedTo?.toLowerCase() === user.toLowerCase() ||
          t.agentName?.toLowerCase() === user.toLowerCase()
      );

      setTickets(filtered);
      setAgentName(user);
    } catch (err) {
      console.error("Error fetching tickets:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTickets();
  }, []);

  const resolveTicket = async (id) => {
    try {
      const res = await fetch(`${BASE_URL}/ticket/${id}/resolve`, {
        method: "PUT",
        headers,
      });

      if (res.ok) {
        alert(`✅ Ticket #${id} marked as resolved!`);
        fetchTickets();
      } else {
        alert(" Ticket already resolved");
      }
    } catch (err) {
      console.error("Error resolving ticket:", err);
    }
  };

  const addComment = async (ticketId) => {
    if (!comment.trim()) return alert("Please enter a comment.");
    const decoded = decodeToken();
    const userId =
      decoded?.userId ||
      decoded?.UserId ||
      decoded?.sub ||
      decoded?.nameid ||
      decoded?.[
        "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
      ];

    if (!userId) return alert("⚠️ userId missing in token.");

    try {
      const res = await fetch(`${BASE_URL}/comment`, {
        method: "POST",
        headers,
        body: JSON.stringify({
          ticketId: Number(ticketId),
          userId: Number(userId),
          message: comment.trim(),
        }),
      });

      if (res.ok) {
        alert("💬 Comment added!");
        setComment("");
        setSelectedTicket(null);
      } else {
        alert("❌ Failed to add comment");
      }
    } catch (err) {
      console.error("Error adding comment:", err);
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

  if (loading)
    return (
      <div style={{ textAlign: "center", padding: "50px", fontSize: "1.2rem" }}>
        Loading Agent Dashboard...
      </div>
    );



    
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
      width: "400px",
      boxShadow: "0 4px 12px rgba(0,0,0,0.2)",
    },
  };

  return (
    <div style={styles.container}>
      {/* 🧑‍💻 Header */}
      <div style={styles.header}>
        <h3 style={{ color: "#0056b3", fontWeight: "700" }}>
          <FaHeadset className="me-2" /> Welcome, {agentName}
        </h3>
        <p style={{ color: "#555" }}>Manage and resolve your assigned tickets below</p>
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

      {/* Tickets Table */}
      <div style={styles.tableCard}>
        <h5 className="text-primary fw-bold mb-3">
          <FaTicketAlt className="me-2" /> My Assigned Tickets
        </h5>

        <table className="table table-bordered align-middle">
          <thead className="table-light">
            <tr>
              <th>ID</th>
              <th>Title</th>
              <th>Description</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {tickets.length === 0 ? (
              <tr>
                <td colSpan="5" className="text-center text-muted">
                  No tickets assigned yet.
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
                  <td>
                    {t.status !== 5 && (
                      <button
                        className="btn btn-sm btn-success me-2"
                        onClick={() => resolveTicket(t.ticketId)}
                      >
                        Resolve
                      </button>
                    )}
                    <button
                      className="btn btn-sm btn-outline-primary"
                      onClick={() => setSelectedTicket(t.ticketId)}
                    >
                      <FaCommentDots className="me-1" /> Comment
                    </button>
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
              <FaUserCircle className="me-2 text-primary" />
              Add Comment for Ticket #{selectedTicket}
            </h5>
            <textarea
              className="form-control my-2"
              rows="3"
              value={comment}
              onChange={(e) => setComment(e.target.value)}
              placeholder="Write your comment..."
            ></textarea>
            <div className="text-end">
              <button
                className="btn btn-primary me-2"
                onClick={() => addComment(selectedTicket)}
              >
                Submit
              </button>
              <button
                className="btn btn-secondary"
                onClick={() => setSelectedTicket(null)}
              >
                Cancel
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default AgentDashboard;
