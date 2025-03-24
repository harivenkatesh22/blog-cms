import React, { useEffect, useState } from 'react';
import '../styles/global.css';

const BACKEND_API_URL = process.env.REACT_APP_BACKEND_URL || "http://localhost:5285/api";

function AdminDashboard() {
  const [unapprovedPosts, setUnapprovedPosts] = useState([]);

  const fetchUnapprovedPosts = async () => {
    try {
      const token = localStorage.getItem('token');
      const response = await fetch(`${BACKEND_API_URL}/Admin/unapprovedPosts`, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      const data = await response.json();
      setUnapprovedPosts(data);
    } catch (error) {
      console.error("Error fetching unapproved posts:", error);
      alert("Error fetching posts");
    }
  };

  useEffect(() => {
    fetchUnapprovedPosts();
  }, []);

  const approvePost = async (postId) => {
    try {
      const token = localStorage.getItem('token');
      const response = await fetch(`${BACKEND_API_URL}/Admin/approvePost/${postId}`, {
        method: 'POST',
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (response.ok) {
        alert("Post approved");
        fetchUnapprovedPosts();
      } else {
        const errorText = await response.text();
        alert("Error approving post: " + errorText);
      }
    } catch (error) {
      console.error("Error approving post:", error);
      alert("Error approving post");
    }
  };

  const deletePost = async (postId) => {
    if (!window.confirm("Are you sure you want to delete this post?")) return;
    try {
      const token = localStorage.getItem('token');
      const response = await fetch(`${BACKEND_API_URL}/BlogPosts/${postId}`, {
        method: 'DELETE',
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (response.ok) {
        alert("Post deleted successfully.");
        fetchUnapprovedPosts();
      } else {
        const errorText = await response.text();
        alert("Error deleting post: " + errorText);
      }
    } catch (error) {
      console.error("Error deleting post:", error);
      alert("Error deleting post");
    }
  };

  return (
    <div className="admin-dashboard">
      <h3>Moderate Blog Posts</h3>
      {unapprovedPosts.length === 0 ? (
        <p>No posts awaiting moderation.</p>
      ) : (
        <table>
          <thead>
            <tr>
              <th>Post ID</th>
              <th>Title</th>
              <th>Author</th>
              <th>Created Date</th>
              <th>Approve</th>
              <th>Delete</th>
            </tr>
          </thead>
          <tbody>
            {unapprovedPosts.map(post => (
              <tr key={post.id}>
                <td>{post.id}</td>
                <td>{post.title}</td>
                <td>{post.Author}</td>
                <td>{new Date(post.createdDate).toLocaleString()}</td>
                <td>
                  <button onClick={() => approvePost(post.id)}>Approve</button>
                </td>
                <td>
                  <button onClick={() => deletePost(post.id)}>Delete</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

export default AdminDashboard;
