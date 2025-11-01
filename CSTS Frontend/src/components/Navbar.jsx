import { Link, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { LogOut } from "lucide-react";
import "./Navbar.css";

const Navbar = () => {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [role, setRole] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    const token = localStorage.getItem("token");
    const userRole = localStorage.getItem("role");
    if (token) {
      setIsLoggedIn(true);
      setRole(userRole);
    } else {
      setIsLoggedIn(false);
      setRole(null);
    }
  }, []);

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("role");
    setIsLoggedIn(false);
    navigate("/login");
  };

  const goToDashboard = () => {
    if (role === "Admin") navigate("/admin");
    else if (role === "Agent") navigate("/agent");
    else if (role === "Customer") navigate("/customer");
  };

  return (
    <nav className="main-navbar">
      <div className="nav-logo" onClick={() => navigate("/")}>
        CSTS<span></span>
      </div>

      <div className="nav-links">
        <Link to="/">Home</Link>

        {!isLoggedIn ? (
          <>
            <Link to="/login">Login</Link>
            <Link to="/register">Register</Link>
          </>
        ) : (
          <>
            <button className="dash-btn" onClick={goToDashboard}>
              Dashboard
            </button>
            <button className="logout-btn" onClick={handleLogout}>
              <LogOut size={20} />
              Logout
            </button>
          </>
        )}
      </div>
    </nav>
  );
};

export default Navbar;
