import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Home from './pages/Home';
import Login from './pages/Login';
import Register from './pages/Register';
import Dashboard from './pages/Dashboard';
import AdminPanel from './pages/AdminPanel';
import PostDetail from './pages/PostDetail';
import Navbar from './components/Navbar';
import Footer from './components/Footer';
import './styles/global.css';

const ProtectedRoute = ({ children, roleRequired }) => {
  const token = localStorage.getItem('token');
  if (!token) {
    // If no token exists, redirect to the login page
    return <Navigate to="/login" />;
  }

  try {
    const decoded = JSON.parse(atob(token.split('.')[1])); // Decode JWT payload
    if (!decoded.role || (roleRequired && decoded.role.toLowerCase() !== roleRequired.toLowerCase())) {
      // If user role doesn't match the required role, redirect to home
      return <Navigate to="/" />;
    }
    return children; // Allow access if validation passes
  } catch (error) {
    console.error("Invalid token:", error);
    localStorage.removeItem('token'); // Clear invalid token
    return <Navigate to="/login" />;
  }
};

function App() {
  return (
    <Router>
      <Navbar />
      <Routes>
        {/* Public Routes */}
        <Route path="/" element={<Home />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/post/:id" element={<PostDetail />} />

        {/* Protected Routes */}
        <Route
          path="/dashboard"
          element={
            <ProtectedRoute allowedRoles={["Admin", "Blogger"]}>
              <Dashboard />
            </ProtectedRoute>
          }
        />
        <Route
          path="/admin"
          element={
            <ProtectedRoute roleRequired="Admin">
              <AdminPanel />
            </ProtectedRoute>
          }
        />
      </Routes>
      <Footer />
    </Router>
  );
}

export default App;
