import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { jwtDecode } from 'jwt-decode'; // Correctly imported
import '../styles/global.css';

function Navbar() {
  const navigate = useNavigate();
  const token = localStorage.getItem('token');
  let role = "";

  if (token) {
    try {
      const decoded = jwtDecode(token); // Decodes the JWT token
      role = decoded.role || ""; // Extracts the role from the token
    } catch (error) {
      console.error("Error decoding token:", error);
      localStorage.removeItem('token');
    }
  }

  const handleLogout = () => {
    localStorage.removeItem("token");
    navigate("/login");
  };

  return (
    <nav className="navbar">
      <div className="nav-container">
        <Link to="/" className="nav-logo">MyBlog</Link>
        <div className="nav-links">
          <Link to="/">Home</Link>
          {token ? (
            <>
              <Link to="/dashboard">Dashboard</Link>
              {role.toLowerCase() === "admin" && <Link to="/admin">Admin Panel</Link>}
              <button
                onClick={handleLogout}
                style={{
                  background: "none", border: "none", color: "white", cursor: "pointer", fontSize: "1rem"
                }}
              >
                Logout
              </button>
            </>
          ) : (
            <>
              <Link to="/login">Login</Link>
              <Link to="/register">Register</Link>
            </>
          )}
        </div>
      </div>
    </nav>
  );
}

export default Navbar;
