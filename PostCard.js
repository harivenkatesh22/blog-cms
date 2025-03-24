import React, { useEffect, useState } from 'react';
import '../styles/global.css';

// Use your backend URL from the environment variable (default to localhost if not provided)
const BACKEND_API_URL = process.env.REACT_APP_BACKEND_URL || "http://localhost:5285/api";
const API_URL = `${BACKEND_API_URL}/BlogPosts/published`;

// Helper function to build the full image URL.
function getImageUrl(url) {
  if (!url) return '/assets/default-news.jpg'; // Fallback to default image
  // If the URL starts with "http" or "https", it’s absolute and can be returned as is
  if (url.startsWith('http://') || url.startsWith('https://')) {
    return url;
  }
  // Otherwise, assume it is a relative URL and prepend the backend base URL
  const baseUrl = BACKEND_API_URL.replace('/api', '');
  return `${baseUrl}${url}`;
}

// Function to sanitize and render HTML content
function sanitizeContent(content) {
  return { __html: content }; // Ensures HTML is rendered securely
}

function PostCard() {
  const [posts, setPosts] = useState([]);

  useEffect(() => {
    fetch(API_URL)
      .then((response) => response.json())
      .then((data) => {
        console.log("Fetched posts:", data);
        if (data && Array.isArray(data)) {
          setPosts(data.slice(0, 6)); // Show top 6 posts
        }
      })
      .catch((error) => console.error('Error fetching posts:', error));
  }, []);

  return (
    <div className="news-container">
      {posts.map((post) => {
        // Check if media exists and resolve the first image in the array
        const mediaUrl = post.media && post.media.length > 0
          ? post.media[0].mediaUrl // Fetch the first media URL
          : null;
        const finalImageUrl = getImageUrl(mediaUrl);

        console.log(`Post "${post.title}" image URL:`, finalImageUrl);

        return (
          <div key={post.id} className="news-card">
            <img
              src={finalImageUrl}
              alt={post.title}
              className="news-image"
            />
            <div className="news-content">
              <h3>{post.title}</h3>
              {/* Render sanitized HTML content */}
              <div
                dangerouslySetInnerHTML={sanitizeContent(post.content)}
              />
              <a href={`/post/${post.id}`}>Read More</a>
            </div>
          </div>
        );
      })}
    </div>
  );
}

export default PostCard;
