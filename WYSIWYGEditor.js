import React from 'react';
import ReactQuill from 'react-quill';
import 'react-quill/dist/quill.snow.css';

const WYSIWYGEditor = ({ value, onChange }) => {
  return (
    <ReactQuill 
      value={value}
      onChange={onChange}
      theme="snow"
      style={{ height: '200px', marginBottom: '10px' }}
    />
  );
};

export default WYSIWYGEditor;
