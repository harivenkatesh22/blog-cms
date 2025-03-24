import React, { useEffect, useState } from 'react';
import DOMPurify from 'dompurify'; // Import DOMPurify for sanitization
import '../styles/global.css';

const BACKEND_API_URL = process.env.REACT_APP_BACKEND_URL || "http://localhost:5285/api";
const API_URL = `${BACKEND_API_URL}/BlogPosts/published`;

function FeaturedArticles() {
  const [articles, setArticles] = useState([]);

  useEffect(() => {
    fetch(API_URL)
      .then(response => {
        if (!response.ok) {
          throw new Error(`Server error: ${response.status}`);
        }
        return response.json();
      })
      .then(data => {
        if (data && Array.isArray(data)) {
          // Sort by clickCount descending and take the top article.
          const sorted = data.sort((a, b) => (b.clickCount || 0) - (a.clickCount || 0));
          setArticles(sorted.slice(0, 1));
        }
      })
      .catch(error => console.error('Error fetching featured articles:', error));
  }, []);

  return (
    <div className="featured-container">
      <h2>Featured Article</h2>
      {articles.map((article) => (
        <div key={article.id} className="featured-card">
          <h3>{article.title}</h3>
          {/* Sanitize and render the SEO description */}
          <p
            dangerouslySetInnerHTML={{
              __html: DOMPurify.sanitize(article.seoDescription),
            }}
          />
          <a href={`/post/${article.id}`}>Read More</a>
        </div>
      ))}
    </div>
  );
}

export default FeaturedArticles;
