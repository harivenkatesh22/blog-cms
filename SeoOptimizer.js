import React, { useState } from 'react';

const BACKEND_API_URL = process.env.REACT_APP_BACKEND_URL || "http://localhost:5285/api";

function SeoOptimizer() {
  const [seoTitle, setSeoTitle] = useState("");
  const [seoDescription, setSeoDescription] = useState("");
  const [content, setContent] = useState("");
  const [suggestions, setSuggestions] = useState([]);

  const handleGetSuggestions = async () => {
    try {
      const response = await fetch(`${BACKEND_API_URL}/Seo/suggest`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ seoTitle, seoDescription, content })
      });

      if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
      }

      const data = await response.json();
      console.log("Response data:", data); // Debug log
      setSuggestions(data.suggestions || []); // Corrected key
    } catch (error) {
      console.error("Error fetching SEO suggestions:", error);
    }
  };

  return (
    <div className="seo-optimizer">
      <h3>SEO Optimizer</h3>
      <div>
        <label>SEO Title:</label>
        <input
          type="text"
          value={seoTitle}
          onChange={(e) => setSeoTitle(e.target.value)}
          placeholder="Enter your SEO title"
        />
      </div>
      <div>
        <label>SEO Description:</label>
        <textarea
          value={seoDescription}
          onChange={(e) => setSeoDescription(e.target.value)}
          placeholder="Enter your SEO description"
        />
      </div>
      <div>
        <label>Content:</label>
        <textarea
          value={content}
          onChange={(e) => setContent(e.target.value)}
          placeholder="Paste your article content here"
        />
      </div>
      <button onClick={handleGetSuggestions}>Get SEO Suggestions</button>
      {suggestions && suggestions.length > 0 && (
        <div className="seo-suggestions">
          <h4>Suggestions:</h4>
          <ul>
            {suggestions.map((suggestion, index) => (
              <li key={index}>{suggestion}</li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}

export default SeoOptimizer;
