import React from "react";
import { Link, useNavigate, useLocation } from "react-router-dom";

const Header = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const token = localStorage.getItem("token");

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("userEmail");
    navigate("/login");
  };

  return (
    <header className="header">
      <h1>Kyuubi Events App</h1>
      <nav>
        {token ? (
          <>
            {location.pathname !== "/wishlists" && <Link to="/">Home</Link>}
            {location.pathname === "/wishlists" && <Link to="/">Home</Link>}
            <Link to="/wishlists">Wishlists</Link>
            <button onClick={handleLogout} style={{ background: "none", color: "white", cursor: "pointer" }}
            onMouseEnter={(e) => e.target.style.textDecoration = "underline"}
            onMouseLeave={(e) => e.target.style.textDecoration = "none"}>
              Logout
            </button>
          </>
        ) : location.pathname === "/login" ? (
          <Link to="/register">Register</Link>
        ) : location.pathname === "/register" ? (
          <Link to="/login">Login</Link>
        ) : (
          <>
            <Link to="/login">Login</Link>
            <Link to="/register">Register</Link>
          </>
        )}
      </nav>
    </header>
  );
};

export default Header;