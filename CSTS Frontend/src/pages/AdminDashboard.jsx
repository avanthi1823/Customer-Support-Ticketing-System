import React, { useEffect, useState } from "react";
import {
  PieChart,
  Pie,
  Cell,
  Tooltip,
  ResponsiveContainer,
  Legend,
} from "recharts";
import "./AdminDashboard.css";

const AdminDashboard = () => {
  const [users, setUsers] = useState([]);
  const [agents, setAgents] = useState([]);
  const [admins, setAdmins] = useState([]);
  const [tickets, setTickets] = useState([]);
  const [adminName, setAdminName] = useState("Admin");
  const [loading, setLoading] = useState(true);
  const [activeSection, setActiveSection] = useState("dashboard");

  const [newAgent, setNewAgent] = useState({ name: "", email: "", password: "" });
  const [newAdmin, setNewAdmin] = useState({ name: "", email: "", password: "" });
  const [assignInfo, setAssignInfo] = useState({ ticketId: "", agentId: "" });

  const BASE_URL = "http://localhost:5041/api";
  const token = localStorage.getItem("token");

  const headers = {
    "Content-Type": "application/json",
    Authorization: `Bearer ${token}`,
  };

  
  const getAdminNameFromToken = () => {
    if (!token) return "Admin";
    try {
      const base64Url = token.split(".")[1];
      const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
      const decoded = JSON.parse(window.atob(base64));
      return decoded.unique_name || decoded.name || decoded.email || "Admin";
    } catch {
      return "Admin";
    }
  };


  const fetchData = async () => {
    try {
      const [userRes, agentRes, adminRes, ticketRes] = await Promise.all([
        fetch(`${BASE_URL}/user/all`, { headers }),
        fetch(`${BASE_URL}/user/agents`, { headers }),
        fetch(`${BASE_URL}/user/admins`, { headers }),
        fetch(`${BASE_URL}/ticket`, { headers }),
      ]);

      const [usersData, agentsData, adminsData, ticketsData] = await Promise.all([
        userRes.json(),
        agentRes.json(),
        adminRes.json(),
        ticketRes.json(),
      ]);

      setUsers(usersData);
      setAgents(agentsData);
      setAdmins(adminsData);
      setTickets(ticketsData);
      setAdminName(getAdminNameFromToken());
    } catch (err) {
      console.error("Error fetching data:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

 
  const handleAddAgent = async () => {
    if (!newAgent.name || !newAgent.email || !newAgent.password)
      return alert("All fields required!");
    try {
      const res = await fetch(`${BASE_URL}/user/add-agent`, {
        method: "POST",
        headers,
        body: JSON.stringify(newAgent),
      });
      if (res.ok) {
        alert("✅ Agent added successfully!");
        setNewAgent({ name: "", email: "", password: "" });
        fetchData();
      } else {
        alert("❌ Failed to add agent");
      }
    } catch (err) {
      console.error(err);
    }
  };


  const handleAddAdmin = async () => {
    if (!newAdmin.name || !newAdmin.email || !newAdmin.password)
      return alert("All fields required!");
    try {
      const res = await fetch(`${BASE_URL}/user/add-admin`, {
        method: "POST",
        headers,
        body: JSON.stringify(newAdmin),
      });
      if (res.ok) {
        alert("✅ Admin added successfully!");
        setNewAdmin({ name: "", email: "", password: "" });
        fetchData();
      } else {
        alert("❌ Failed to add admin");
      }
    } catch (err) {
      console.error(err);
    }
  };

  const toggleUserStatus = async (user) => {
    const action = user.isActive ? "deactivate" : "activate";
    const res = await fetch(`${BASE_URL}/user/${user.userId}/${action}`, {
      method: "PUT",
      headers,
    });
    if (res.ok) {
      alert(`✅ User ${action}d successfully`);
      fetchData();
    } else {
      alert("❌ Operation failed");
    }
  };


  const removeAgent = async (id) => {
    if (!window.confirm("Remove this agent?")) return;
    const res = await fetch(`${BASE_URL}/user/remove-agent/${id}`, {
      method: "DELETE",
      headers,
    });
    if (res.ok) {
      alert("🗑 Agent removed successfully");
      fetchData();
    } else {
      alert("❌ Failed to remove agent");
    }
  };

  
  const removeCustomer = async (id) => {
    if (!window.confirm("Remove this customer?")) return;
    const res = await fetch(`${BASE_URL}/user/remove-customer/${id}`, {
      method: "DELETE",
      headers,
    });
    if (res.ok) {
      alert("🗑 Customer removed successfully");
      fetchData();
    } else {
      alert("❌ Failed to remove customer");
    }
  };

 
  const handleAssign = async () => {
    if (!assignInfo.ticketId || !assignInfo.agentId)
      return alert("Please select both Ticket ID and Agent.");

    const admin = admins[0];
    const adminId = admin ? admin.userId : 1;
    const url = `${BASE_URL}/ticket/${assignInfo.ticketId}/assign/${adminId}/to/${assignInfo.agentId}`;

    const res = await fetch(url, { method: "PUT", headers });
    if (res.ok) {
      alert("✅ Ticket assigned successfully!");
      const assignedAgent = agents.find(
        (a) => a.userId === parseInt(assignInfo.agentId)
      );
      const agentName = assignedAgent ? assignedAgent.name : "Unassigned";
      const updatedTickets = tickets.map((t) =>
        t.ticketId === parseInt(assignInfo.ticketId)
          ? { ...t, assignedTo: agentName, status: 2 }
          : t
      );
      setTickets(updatedTickets);
      setAssignInfo({ ticketId: "", agentId: "" });
    } else {
      alert("❌ Assignment failed");
    }
  };

  const getStatusLabel = (status) => {
    switch (status) {
      case 1:
        return "Open";
      case 2:
        return "Assigned";
      case 3:
        return "Resolved";
      case 4:
        return "Closed";
      default:
        return "Closed";
    }
  };


  const ticketChartData = [
    { name: "Open", value: tickets.filter((t) => t.status === 1).length },
    { name: "Assigned", value: tickets.filter((t) => t.status === 2).length },
    { name: "Resolved", value: tickets.filter((t) => t.status === 3).length },
    { name: "Closed", value: tickets.filter((t) => t.status === 4).length },
  ];

  const COLORS = ["#4e79a7", "#f28e2b", "#76b7b2", "#e15759"];

  if (loading) return <div className="loading">Loading dashboard...</div>;

  return (
    <div className="dashboard-container">
      {/* ✅ Sidebar */}
      <aside className="sidebar">
        <h2>CSTS Helpdesk</h2>
        <ul>
          <li
            className={activeSection === "dashboard" ? "active" : ""}
            onClick={() => setActiveSection("dashboard")}
          >
            🏠 Dashboard
          </li>
          <li
            className={activeSection === "users" ? "active" : ""}
            onClick={() => setActiveSection("users")}
          >
            👥 Users
          </li>
          <li
            className={activeSection === "tickets" ? "active" : ""}
            onClick={() => setActiveSection("tickets")}
          >
            🎫 Tickets
          </li>
        </ul>
      </aside>

      {/* ✅ Main Content */}
      <main className="main-content">
        <header className="header">
          <h1>Welcome, {adminName}</h1>
          <p>Manage your support operations efficiently</p>
        </header>

        {/* 🏠 Dashboard Section */}
        {activeSection === "dashboard" && (
          <>
            <div className="stats-grid">
              <div className="card blue">Total Users: {users.length}</div>
              <div className="card green">Agents: {agents.length}</div>
              <div className="card orange">Admins: {admins.length}</div>
              <div className="card purple">Tickets: {tickets.length}</div>
            </div>

            <section className="chart-section">
              <h2>📊 Ticket Status Overview</h2>
              <div className="chart-wrapper">
                <ResponsiveContainer width="100%" height={300}>
                  <PieChart>
                    <Pie
                      data={ticketChartData}
                      dataKey="value"
                      nameKey="name"
                      cx="50%"
                      cy="50%"
                      outerRadius={100}
                      label
                    >
                      {ticketChartData.map((_, i) => (
                        <Cell key={i} fill={COLORS[i % COLORS.length]} />
                      ))}
                    </Pie>
                    <Tooltip />
                    <Legend />
                  </PieChart>
                </ResponsiveContainer>
              </div>
            </section>
          </>
        )}

        {/* 👥 User Management */}
        {activeSection === "users" && (
          <>
            <section className="add-user-section">
              <h2>➕ Add Agent / Admin</h2>
              <div className="form-row">
                <h3>Add Agent</h3>
                <input type="text" placeholder="Name" value={newAgent.name}
                  onChange={(e) => setNewAgent({ ...newAgent, name: e.target.value })} />
                <input type="email" placeholder="Email" value={newAgent.email}
                  onChange={(e) => setNewAgent({ ...newAgent, email: e.target.value })} />
                <input type="password" placeholder="Password" value={newAgent.password}
                  onChange={(e) => setNewAgent({ ...newAgent, password: e.target.value })} />
                <button onClick={handleAddAgent}>Add Agent</button>
              </div>

              <div className="form-row">
                <h3>Add Admin</h3>
                <input type="text" placeholder="Name" value={newAdmin.name}
                  onChange={(e) => setNewAdmin({ ...newAdmin, name: e.target.value })} />
                <input type="email" placeholder="Email" value={newAdmin.email}
                  onChange={(e) => setNewAdmin({ ...newAdmin, email: e.target.value })} />
                <input type="password" placeholder="Password" value={newAdmin.password}
                  onChange={(e) => setNewAdmin({ ...newAdmin, password: e.target.value })} />
                <button onClick={handleAddAdmin}>Add Admin</button>
              </div>
            </section>

            <section className="admin-section">
              <h2>👥 User Management</h2>
              <table>
                <thead>
                  <tr>
                    <th>User ID</th>
                    <th>Name</th>
                    <th>Email</th>
                    <th>Role</th>
                    <th>Status</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {users.map((u) => (
                    <tr key={u.userId}>
                      <td>{u.userId}</td>
                      <td>{u.name}</td>
                      <td>{u.email}</td>
                      <td>{u.role}</td>
                      <td>{u.isActive ? "Active" : "Inactive"}</td>
                      <td>
                        <button onClick={() => toggleUserStatus(u)}>
                          {u.isActive ? "Deactivate" : "Activate"}
                        </button>
                        {u.role === "Agent" && (
                          <button onClick={() => removeAgent(u.userId)}>Remove</button>
                        )}
                        {u.role === "Customer" && (
                          <button onClick={() => removeCustomer(u.userId)}>Remove</button>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </section>
          </>
        )}

        {/* 🎫 Ticket Management */}
        {activeSection === "tickets" && (
          <>
            <section className="ticket-section">
              <h2>🎫 Ticket Management</h2>
              <table>
                <thead>
                  <tr>
                    <th>Ticket ID</th>
                    <th>Title</th>
                    <th>Status</th>
                    <th>Assigned To</th>
                  </tr>
                </thead>
                <tbody>
                  {tickets.map((t) => (
                    <tr key={t.ticketId}>
                      <td>{t.ticketId}</td>
                      <td>{t.title}</td>
                      <td>{getStatusLabel(t.status)}</td>
                      <td>{t.assignedTo || "Unassigned"}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </section>

            <section className="assign-section">
              <h2>🧾 Assign Ticket</h2>
              <input
                type="number"
                placeholder="Ticket ID"
                value={assignInfo.ticketId}
                onChange={(e) =>
                  setAssignInfo({ ...assignInfo, ticketId: e.target.value })
                }
              />
              <select
                value={assignInfo.agentId}
                onChange={(e) =>
                  setAssignInfo({ ...assignInfo, agentId: e.target.value })
                }
              >
                <option value="">Select Agent</option>
                {agents.map((a) => (
                  <option key={a.userId} value={a.userId}>
                    {a.name}
                  </option>
                ))}
              </select>
              <button onClick={handleAssign}>Assign</button>
            </section>
          </>
        )}
      </main>
    </div>
  );
};

export default AdminDashboard;
