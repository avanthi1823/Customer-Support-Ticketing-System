import { useState } from "react";
import { Link } from "react-router-dom"; 
import api from "../api/api";
import "./Login.css";

const Login = () => {
  const [form, setForm] = useState({ email: "", password: "" });
  const [message, setMessage] = useState("");

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
    
      const res = await api.post("/auth/login", form);

     
      const { token, role, name, userId } = res.data;

     
      localStorage.setItem("token", token);
      localStorage.setItem("role", role);
      localStorage.setItem("name", name);
      localStorage.setItem("userId", String(userId));

      setMessage("Login successful!");

      if (role === "Admin") window.location.href = "/admin";
      else if (role === "Agent") window.location.href = "/agent";
      else window.location.href = "/customer";
    } catch (error) {
      console.error("Login error:", error);
      setMessage("Invalid credentials!");
    }
  };

  return (
    <div className="auth-page">
      <div className="auth-card">
        <h2>Login</h2>
        <form onSubmit={handleSubmit}>
          <input
            name="email"
            placeholder="Email"
            value={form.email}
            onChange={handleChange}
            required
          />
          <input
            name="password"
            type="password"
            placeholder="Password"
            value={form.password}
            onChange={handleChange}
            required
          />
          <button type="submit">Login</button>
        </form>

        {/* ✅ Registration link for new users */}
        <p className="register-link">
          New user? <Link to="/register">Please register</Link>
        </p>

        {/* ✅ Message display */}
        {message && <p className="msg">{message}</p>}
      </div>
    </div>
  );
};

export default Login;
