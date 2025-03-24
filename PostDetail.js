import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import '../styles/global.css';

const BACKEND_API_URL = process.env.REACT_APP_BACKEND_URL || "http://localhost:5285/api";

// Helper to render each media item based on type
function renderMediaItem(media, index) {
  if (media.mediaType === "image") {
    return (
      <img
        src={media.mediaUrl}
        alt={`Media ${index + 1}`}
        style={{ maxWidth: "100%", height: "auto", marginBottom: "10px" }}
      />
    );
  }
  
  if (media.mediaType === "video") {
    const url = media.mediaUrl;
    // Check if the URL is a YouTube link:
    const youtubeRegex = /(?:youtu\.be\/|youtube\.com\/(?:watch\?v=|embed\/))([\w-]+)/;
    const match = url.match(youtubeRegex);
    if (match && match[1]) {
      const videoId = match[1];
      return (
        <div key={index} className="video-container">
          <iframe
            src={`https://www.youtube.com/embed/${videoId}`}
            title="YouTube video"
            frameBorder="0"
            allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
            allowFullScreen
            style={{ maxWidth: "100%", marginBottom: "10px", height: "360px" }}
          ></iframe>
        </div>
      );
    }
    // If not a YouTube link, then attempt normal video playback:
    return (
      <video key={index} controls style={{ maxWidth: "100%", marginBottom: "10px" }}>
        <source src={url} type="video/mp4" />
        Your browser does not support the video tag.
      </video>
    );
  }
  
  if (media.mediaType === "link") {
    return (
      <a
        key={index}
        href={media.mediaUrl}
        target="_blank"
        rel="noopener noreferrer"
        style={{ color: "blue", textDecoration: "underline", marginBottom: "10px" }}
      >
        {media.mediaUrl}
      </a>
    );
  }
  
  return null;
}

function PostDetail() {
  const { id } = useParams();
  const [post, setPost] = useState(null);
  const [error, setError] = useState('');

  useEffect(() => {
    // Fetch the article details from the backend using the id
    fetch(`${BACKEND_API_URL}/BlogPosts/${id}`)
      .then((response) => {
        if (!response.ok) {
          throw new Error("Article not found or still pending approval.");
        }
        return response.json();
      })
      .then((data) => setPost(data))
      .catch((error) => {
        console.error("Error fetching post:", error);
        setError(error.message);
      });
  }, [id]);

  if (error) {
    return <div className="error-message" style={{ color: 'red' }}>{error}</div>;
  }

  if (!post) return <div>Loading article...</div>;

  return (
    <div className="post-detail">
      {/* Article Title */}
      <h2>{post.title}</h2>

      {/* Author Info */}
      <p><em>By {post.author || "Unknown Author"}</em></p>

      {/* Article Content - rendered as HTML */}
      <div dangerouslySetInnerHTML={{ __html: post.content }} />

      {/* Media Gallery */}
      {post.media && post.media.length > 0 && (
        <div className="media-gallery">
          <h3>Media Attachments</h3>
          {post.media.map((media, index) => (
            <div key={index} className="media-item">
              {renderMediaItem(media, index)}
            </div>
          ))}
        </div>
      )}

      {/* Navigation to All Articles */}
      <div className="all-articles">
        <h3>All Articles</h3>
        <Link to="/">View All Articles</Link>
      </div>
    </div>
  );
}

export default PostDetail;