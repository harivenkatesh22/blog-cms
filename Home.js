// src/pages/Home.js
import React, { useState } from 'react';
import PostCard from '../components/PostCard';
import FeaturedArticles from '../components/FeaturedArticles';
import SearchBar from '../components/SearchBar';
import News from './News';
import '../styles/global.css';

function Home() {
  const [activeTab, setActiveTab] = useState('articles');

  return (
    <div className="home-container">
      {/* Left-side tab navigation */}
      <div className="left-tabs">
        <button 
          className={activeTab === 'articles' ? 'active' : ''}
          onClick={() => setActiveTab('articles')}
        >
          Articles
        </button>
        <button 
          className={activeTab === 'news' ? 'active' : ''}
          onClick={() => setActiveTab('news')}
        >
          News
        </button>
      </div>

      {/* Render SearchBar only when Articles tab is active */}
      {activeTab === 'articles' && (
        <div className="centered-search">
          <SearchBar />
        </div>
      )}

      {/* Main content area */}
      <div className="main-content">
        {activeTab === 'articles' ? (
          <>
            <h1>Latest Blog Posts</h1>
            <p>Stay updated with our latest content.</p>
            <PostCard />
          </>
        ) : (
          <News />
        )}
      </div>

      {/* Feature sidebar only for articles */}
      {activeTab === 'articles' && (
        <aside className="sidebar">
          <FeaturedArticles />
        </aside>
      )}
    </div>
  );
}

export default Home;
