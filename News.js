import React, { useState, useEffect } from 'react';
import '../styles/global.css';

function News() {
  const [newsArticles, setNewsArticles] = useState([]);
  const [error, setError] = useState('');

  useEffect(() => {
    const NEWS_API_URL = `https://newsapi.org/v2/top-headlines?country=us&apiKey=1ad95843d343426b85416ce807a034ba`;

    fetch(NEWS_API_URL)
      .then((response) => {
        if (!response.ok) {
          throw new Error(`Failed to fetch news. Status: ${response.status}`);
        }
        return response.json();
      })
      .then((data) => setNewsArticles(data.articles || []))
      .catch((err) => setError("Unable to load news at this time."));
  }, []);

  return (
    <div className="news-container">
      <h2>Latest News</h2>
      {error && <div className="error-message">{error}</div>}
      {newsArticles.map((article, index) => (
        <div key={index} className="news-card">
          {article.urlToImage && <img src={article.urlToImage} alt={article.title} />}
          <h3>{article.title}</h3>
          <p>{article.description}</p>
          <a href={article.url} target="_blank" rel="noopener noreferrer">Read More</a>
        </div>
      ))}
    </div>
  );
}

export default News;
