import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import '../styles/global.css';

const BACKEND_API_URL = process.env.REACT_APP_BACKEND_URL || "http://localhost:5285/api";

function Login() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const navigate = useNavigate();

  // Redirect if already logged in.
  useEffect(() => {
    if (localStorage.getItem('token')) {
      navigate("/");
    }
  }, [navigate]);

  const handleLogin = async (e) => {
    e.preventDefault();
    setErrorMessage('');

    const trimmedEmail = email.trim();
    const trimmedPassword = password.trim();

    try {
      const response = await fetch(`${BACKEND_API_URL}/Auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ Email: trimmedEmail, Password: trimmedPassword })
      });
      
      if (!response.ok) {
        if (response.status === 401) {
          throw new Error("Invalid username or password.");
        } else {
          throw new Error("Server error. Please try again later.");
        }
      }
      
      const data = await response.json();
      const token = data.Token || data.token;
      
      if (token) {
        localStorage.setItem('token', token);
        navigate("/");
      } else {
        throw new Error("Login failed. Please try again.");
      }
    } catch (error) {
      console.error("Error during login:", error);
      setErrorMessage(error.message);
    }
  };

  return (
    <div className="auth-container">
      <h2>Login</h2>
      {errorMessage && <div className="error-message" style={{ color: 'red' }}>{errorMessage}</div>}
      <form onSubmit={handleLogin}>
        <input 
          type="email" 
          placeholder="Email" 
          value={email} 
          onChange={(e) => setEmail(e.target.value)}
          required 
        />
        <input 
          type="password" 
          placeholder="Password" 
          value={password} 
          onChange={(e) => setPassword(e.target.value)}
          required 
        />
        <button type="submit">Login</button>
      </form>
    </div>
  );
}

export default Login;
