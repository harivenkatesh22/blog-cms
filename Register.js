import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import '../styles/global.css';

const BACKEND_API_URL = process.env.REACT_APP_BACKEND_URL || "http://localhost:5285/api";

function Register() {
  const [user, setUser] = useState({
    firstName: '',
    lastName: '',
    country: '',
    email: '',
    password: ''
  });
  const [message, setMessage] = useState('');
  const navigate = useNavigate();

  const handleRegister = async (e) => {
    e.preventDefault();
    setMessage('');
    
    try {
      const response = await fetch(`${BACKEND_API_URL}/Auth/signup`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(user)
      });
      
      const data = await response.text();
      
      if (!response.ok) {
        throw new Error(data);
      }
      
      if (data.trim() === "Signup successful") {
        setMessage("Registration successful. Please log in.");
        setTimeout(() => navigate('/login'), 2000);
      } else {
        setMessage("Registration failed: " + data);
      }
    } catch (error) {
      setMessage(error.message);
    }
  };

  return (
    <div className="auth-container">
      <h2>Register</h2>
      {message && <div className="error-message" style={{ color: 'red' }}>{message}</div>}
      <form onSubmit={handleRegister}>
        <input type="text" placeholder="First Name" value={user.firstName} onChange={(e) => setUser({ ...user, firstName: e.target.value })} required />
        <input type="text" placeholder="Last Name" value={user.lastName} onChange={(e) => setUser({ ...user, lastName: e.target.value })} required />
        <input type="text" placeholder="Country" value={user.country} onChange={(e) => setUser({ ...user, country: e.target.value })} required />
        <input type="email" placeholder="Email" value={user.email} onChange={(e) => setUser({ ...user, email: e.target.value })} required />
        <input type="password" placeholder="Password" value={user.password} onChange={(e) => setUser({ ...user, password: e.target.value })} required />
        <button type="submit">Register</button>
      </form>
    </div>
  );
}

export default Register;
