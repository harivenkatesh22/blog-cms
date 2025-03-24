import React, { useState, useEffect } from 'react';
import '../styles/global.css';
import WYSIWYGEditor from '../components/WYSIWYGEditor';
import SeoOptimizer from '../components/SeoOptimizer'; // Import SeoOptimizer

const BACKEND_API_URL = process.env.REACT_APP_BACKEND_URL || "http://localhost:5285/api";

function Dashboard() {
  const [newArticle, setNewArticle] = useState({ title: '', content: '', Media: [] });
  const [newMedia, setNewMedia] = useState({ mediaUrl: '', mediaType: '' });
  const [seoSuggestions, setSeoSuggestions] = useState([]);
  const [successMessage, setSuccessMessage] = useState('');
  const [errorMessage, setErrorMessage] = useState('');

  const handleEditorChange = (content) => {
    setNewArticle({ ...newArticle, content });
  };

  const handleAddMedia = () => {
    if (!newMedia.mediaUrl && !newMedia.mediaType) {
      return;
    }
    if (!newMedia.mediaUrl || !newMedia.mediaType) {
      setErrorMessage("Incomplete media details. Please fill in both Media URL and Media Type, or leave both fields blank.");
      return;
    }
    setNewArticle({ ...newArticle, Media: [...newArticle.Media, newMedia] });
    setNewMedia({ mediaUrl: '', mediaType: '' });
  };

  const handleRemoveMedia = (index) => {
    const updatedMedia = newArticle.Media.filter((_, i) => i !== index);
    setNewArticle({ ...newArticle, Media: updatedMedia });
  };

  const handleAddArticle = async () => {
    if (!newArticle.title || !newArticle.content) {
      setErrorMessage("Title and content cannot be empty!");
      return;
    }

    const token = localStorage.getItem('token');
    if (!token) {
      setErrorMessage("User is not authenticated. Please log in.");
      return;
    }

    try {
      const response = await fetch(`${BACKEND_API_URL}/BlogPosts`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`,
        },
        body: JSON.stringify(newArticle),
      });

      const data = await response.json();
      if (response.ok) {
        setSuccessMessage("Article submitted for moderation!");
        setNewArticle({ title: '', content: '', Media: [] });
      } else {
        setErrorMessage(`Error: ${data.message || "Submission failed."}`);
      }
    } catch (error) {
      console.error("Error submitting article:", error);
      setErrorMessage("Error submitting article. Please try again later.");
    }
  };

  useEffect(() => {
    if (newArticle.title.trim()) {
      const query = newArticle.title;
      fetch(`${BACKEND_API_URL}/BlogPosts/search?query=${encodeURIComponent(query)}`)
        .then((response) => response.json())
        .then((data) => {
          setSeoSuggestions(data.map((item) => item.title));
        })
        .catch((error) => {
          console.error("Error fetching SEO suggestions:", error);
          setSeoSuggestions([]);
        });
    }
  }, [newArticle.title]);

  return (
    <div className="dashboard-container">
      <h2>Dashboard - Manage Articles</h2>
      {successMessage && <div className="success-message" style={{ color: 'green' }}>{successMessage}</div>}
      {errorMessage && <div className="error-message" style={{ color: 'red' }}>{errorMessage}</div>}
      
      <div className="article-form">
        <input
          type="text"
          placeholder="Enter Title"
          value={newArticle.title}
          onChange={(e) => setNewArticle({ ...newArticle, title: e.target.value })}
        />
        <WYSIWYGEditor value={newArticle.content} onChange={handleEditorChange} />
  
        <div className="media-form">
          <h4>Add Media Attachment (Optional)</h4>
          <input
            type="text"
            placeholder="Media URL"
            value={newMedia.mediaUrl}
            onChange={(e) => setNewMedia({ ...newMedia, mediaUrl: e.target.value })}
          />
          <input
            type="text"
            placeholder="Media Type (image, video, link)"
            value={newMedia.mediaType}
            onChange={(e) => setNewMedia({ ...newMedia, mediaType: e.target.value })}
          />
          <button type="button" onClick={handleAddMedia}>Add Media</button>
        </div>
  
        {newArticle.Media.length > 0 && (
          <div className="media-list">
            <h4>Media Attachments:</h4>
            <ul>
              {newArticle.Media.map((m, index) => (
                <li key={index}>
                  <strong>{m.mediaType}:</strong> {m.mediaUrl}
                  <button type="button" onClick={() => handleRemoveMedia(index)}>Remove</button>
                </li>
              ))}
            </ul>
          </div>
        )}
  
        <button onClick={handleAddArticle}>Add Article</button>
        
        {/* Add space after the "Add Article" button */}
        <div style={{ marginBottom: '20px' }}></div>
      </div>
  
      {/* SEO Optimizer Section */}
      <div className="seo-optimizer-section">
        <h3>SEO Suggestions</h3>
        <SeoOptimizer />
      </div>
    </div>
  );
  
}

export default Dashboard;
