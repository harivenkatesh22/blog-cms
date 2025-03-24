import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import DOMPurify from 'dompurify'; // Import DOMPurify for sanitization
import '../styles/global.css';

const BACKEND_API_URL = process.env.REACT_APP_BACKEND_URL || "http://localhost:5285/api";

function SearchBar() {
  const [query, setQuery] = useState('');
  const [suggestions, setSuggestions] = useState([]);
  const [searchResults, setSearchResults] = useState([]);
  const [errorMessage, setErrorMessage] = useState('');

  // Fetch suggestions while typing
  useEffect(() => {
    const timer = setTimeout(() => {
      if (query.trim()) {
        fetch(`${BACKEND_API_URL}/Search/seo-suggestions?query=${encodeURIComponent(query)}`)
          .then((response) => response.json())
          .then((data) => setSuggestions(data))
          .catch((error) => {
            console.error("Error fetching search suggestions:", error);
            setSuggestions([]);
          });
      } else {
        setSuggestions([]);
      }
    }, 500);
    return () => clearTimeout(timer);
  }, [query]);

  // Handle search button click
  const handleSearch = () => {
    if (!query.trim()) {
      setErrorMessage("Search query cannot be empty!");
      return;
    }

    fetch(`${BACKEND_API_URL}/Search?query=${encodeURIComponent(query)}`)
      .then((response) => response.json())
      .then((data) => {
        setSearchResults(data);
        setErrorMessage('');
      })
      .catch((error) => {
        console.error("Error performing search:", error);
        setErrorMessage("Error performing search. Please try again.");
      });
  };

  return (
    <div className="search-bar-container">
      <input
        type="text"
        placeholder="Search articles..."
        value={query}
        onChange={(e) => setQuery(e.target.value)}
      />
      <button onClick={handleSearch}>Search</button>

      {errorMessage && <div className="error-message" style={{ color: 'red' }}>{errorMessage}</div>}

      {/* Display suggestions dynamically */}
      {suggestions && suggestions.length > 0 && (
        <ul className="suggestions-list">
          {suggestions.map((item, index) => (
            <li key={index}>
              <Link to={`/post/${item.PostId}`}>{item.Suggestion}</Link>
            </li>
          ))}
        </ul>
      )}

      {/* Display search results */}
      {searchResults && searchResults.length > 0 && (
        <div className="search-results">
          <h4>Search Results:</h4>
          {searchResults.map((result, index) => (
            <div key={index} className="result-item">
              <Link to={`/post/${result.id}`}>{result.title}</Link>
              {/* Safely sanitize and render the seoDescription */}
              <p
                dangerouslySetInnerHTML={{
                  __html: DOMPurify.sanitize(result.seoDescription),
                }}
              />
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

export default SearchBar;
