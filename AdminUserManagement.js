import React, { useEffect, useState } from 'react';
import '../styles/global.css';

const BACKEND_API_URL = process.env.REACT_APP_BACKEND_URL || "http://localhost:5285/api";

function AdminUserManagement() {
  const [users, setUsers] = useState([]);
  const [successMessage, setSuccessMessage] = useState('');
  const [errorMessage, setErrorMessage] = useState('');

  const fetchUsers = async () => {
    try {
      const token = localStorage.getItem('token');
      const response = await fetch(`${BACKEND_API_URL}/Admin/users`, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      const data = await response.json();
      setUsers(data);
    } catch (error) {
      console.error("Error fetching users:", error);
      setErrorMessage("Error fetching users.");
    }
  };

  useEffect(() => {
    fetchUsers();
  }, []);

  const updateUserRole = async (userId, newRole) => {
    setSuccessMessage('');
    setErrorMessage('');
    try {
      const token = localStorage.getItem('token');
      const response = await fetch(`${BACKEND_API_URL}/Admin/updateUserRole/${userId}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(newRole)
      });
      if (response.ok) {
        setSuccessMessage("User role updated successfully.");
        fetchUsers();
      } else {
        const errorText = await response.text();
        setErrorMessage("Error updating role: " + errorText);
      }
    } catch (error) {
      console.error("Error updating role:", error);
      setErrorMessage("Error updating user role.");
    }
  };

  return (
    <div className="admin-user-management">
      <h3>Manage Users</h3>
      {successMessage && <div className="success-message" style={{ color: 'green' }}>{successMessage}</div>}
      {errorMessage && <div className="error-message" style={{ color: 'red' }}>{errorMessage}</div>}
      {users.length === 0 ? (
        <p>No users found.</p>
      ) : (
        <table>
          <thead>
            <tr>
              <th>ID</th>
              <th>Email</th>
              <th>Name</th>
              <th>Country</th>
              <th>Role</th>
              <th>Change Role</th>
            </tr>
          </thead>
          <tbody>
            {users.map(user => (
              <tr key={user.id}>
                <td>{user.id}</td>
                <td>{user.email}</td>
                <td>{user.firstName} {user.lastName}</td>
                <td>{user.country}</td>
                <td>{user.role}</td>
                <td>
                  <select defaultValue={user.role} onChange={(e) => updateUserRole(user.id, e.target.value)}>
                    <option value="Blogger">Blogger</option>
                    <option value="Admin">Admin</option>
                  </select>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

export default AdminUserManagement;
