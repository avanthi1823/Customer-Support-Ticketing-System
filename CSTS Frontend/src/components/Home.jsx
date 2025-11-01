import { Link } from "react-router-dom";
import { useEffect, useState } from "react";
import "./Home.css";

const Home = () => {
  const testimonials = [
    {
      name: "John",
      feedback:
        "CSTS revolutionized our support system. Our response time dropped by 45% within weeks!",
      
      image: "https://cdn-icons-png.flaticon.com/512/4140/4140048.png",
    },
    {
      name: "John",
      feedback:
        "Real-time tracking and analytics make managing agents effortless. It’s like having a 24/7 supervisor.",
      
      image: "https://cdn-icons-png.flaticon.com/512/4322/4322991.png",
    },
    {
      name: "Ritika",
      feedback:
        "The dashboard is clean, fast, and built for scale. Our clients love how seamless support feels now!",
    
      image: "https://cdn-icons-png.flaticon.com/512/4140/4140051.png",
    },
  
  ];

  const [index, setIndex] = useState(0);

  useEffect(() => {
    const interval = setInterval(() => {
      setIndex((prev) => (prev + 1) % testimonials.length);
    }, 5000);
    return () => clearInterval(interval);
  }, []);

  return (
    <div className="home-wrapper">
      {/* ===== NAVBAR ===== */}
      
      

      {/* ===== HERO SECTION ===== */}
      <section className="home-hero">
        <div className="hero-left">
          <h1>
            Empower Your <span>Support Team</span> in Real Time 🚀
          </h1>
          <p>
            Streamline customer communication, manage tickets, and track
            performance — all in one powerful platform built for modern support
            teams.
          </p>
          <div className="hero-buttons">
            <Link to="/login" className="btn primary-btn">
              Get Started
            </Link>
            <Link to="/register" className="btn primary-btn">
              Create Free Account
            </Link>
          </div>
        </div>
        <div className="hero-right">
          <img
            src="data:image/jpeg;base64,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"
            alt="Helpdesk Illustration"
          />
        </div>
      </section>

      {/* ===== FEATURES SECTION ===== */}
      <section className="home-features">
        <h2>💡 Why Choose CSTS?</h2>
        <div className="features-grid">
          <div className="feature-card">
            <img
              src="https://cdn-icons-png.flaticon.com/512/2170/2170155.png"
              alt="Instant Support"
            />
            <h3>Real-Time Support</h3>
            <p>
              Connect instantly with your users. Manage and respond to tickets
              live with zero delays.
            </p>
          </div>
          <div className="feature-card">
            <img
              src="https://cdn-icons-png.flaticon.com/512/3135/3135715.png"
              alt="Analytics"
            />
            <h3>Smart Analytics</h3>
            <p>
              Measure agent performance, track resolution time, and optimize
              your team's efficiency.
            </p>
          </div>
          <div className="feature-card">
            <img
              src="https://cdn-icons-png.flaticon.com/512/1250/1250594.png"
              alt="Dashboard"
            />
            <h3>Role-Based Dashboards</h3>
            <p>
              Separate panels for Admin, Agent, and Users — secure and easy to
              navigate.
            </p>
          </div>
        </div>
      </section>

      {/* ===== TESTIMONIAL SECTION ===== */}
<section className="testimonials">
  <h2>💬 What Our Clients Say</h2>
  <div className="testimonial-grid">
    {testimonials.map((item, i) => (
      <div className="testimonial-card" key={i}>
        <img src={item.image} alt={item.name} className="testimonial-img" />
        <p className="testimonial-text">“{item.feedback}”</p>
        <h4>{item.name}</h4>
        <p className="testimonial-role">{item.role}</p>
      </div>
    ))}
  </div>
</section>


     

      {/* ===== FOOTER ===== */}
      <footer className="home-footer">
        <div className="footer-content">
          <p>© 2025 CSTS | Built for Modern Support Teams 💙</p>
          <div className="footer-links">
            <Link to="/">Privacy Policy</Link>
            <Link to="/">Terms of Service</Link>
            <Link to="/">Contact Us</Link>
          </div>
        </div>
      </footer>
    </div>
  );
};

export default Home;
