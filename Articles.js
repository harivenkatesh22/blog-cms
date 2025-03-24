// src/pages/Articles.js
import React, { useEffect, useState } from 'react';
import '../styles/global.css';

const BACKEND_API_URL = process.env.REACT_APP_BACKEND_URL || "http://localhost:5285/api";

function Articles() {
  const [articles, setArticles] = useState([]);

  useEffect(() => {
    fetch(`${BACKEND_API_URL}/BlogPosts/published`)
      .then(response => response.json())
      .then(data => setArticles(data))
      .catch(err => console.error(err));
  }, []);

  // Helper: Convert relative image paths to absolute
  const getFullImageUrl = (imageUrl) => {
    if (!imageUrl) return '/assets/default-news.jpg';
    if (imageUrl.startsWith('/')) {
      const baseUrl = BACKEND_API_URL.replace('/api','');
      return `${baseUrl}${imageUrl}`;
    }
    return imageUrl;
  };

  return (
    <div className="articles-container">
      {articles.map(article => (
        <div key={article.id} className="article-card">
          <img 
            src={
              article.Media && article.Media.length > 0 && article.Media[0].MediaUrl
                ? getFullImageUrl(article.Media[0].MediaUrl)
                : '/assets/default-news.jpg'
            }
            alt={article.title}
          />
          <h3>{article.title}</h3>
          <p>{article.seoDescription}</p>
          <a href={`/post/${article.id}`}>Read More</a>
        </div>
      ))}
    </div>
  );
}

export default Articles;
