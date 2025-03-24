import React, { useState } from 'react';
import AdminDashboard from './AdminDashboard';
import AdminUserManagement from './AdminUserManagement';
import '../styles/global.css';

function AdminPanel() {
  const [activeTab, setActiveTab] = useState('moderate'); // 'moderate' or 'users'

  return (
    <div className="admin-panel container">
      <h2>Admin Panel</h2>
      <div className="admin-tabs">
        <button
          className={activeTab === 'moderate' ? 'active' : ''}
          onClick={() => setActiveTab('moderate')}
        >
          Moderate Posts
        </button>
        <button
          className={activeTab === 'users' ? 'active' : ''}
          onClick={() => setActiveTab('users')}
        >
          Manage Users
        </button>
      </div>
      <div className="admin-content">
        {activeTab === 'moderate' && <AdminDashboard />}
        {activeTab === 'users' && <AdminUserManagement />}
      </div>
    </div>
  );
}

export default AdminPanel;
