import React, { useState } from "react";
import { Upload } from "lucide-react";

const XlsxUpload = () => {
  const [file, setFile] = useState(null);
  const [error, setError] = useState("");

  const handleFileChange = (e) => {
    const selectedFile = e.target.files[0];
    if (!selectedFile) return;

    if (
      selectedFile.name.endsWith(".xlsx") &&
      (selectedFile.type ===
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" ||
        selectedFile.type === "")
    ) {
      setFile(selectedFile);
      setError("");
    } else {
      setFile(null);
      setError("❌ Only .xlsx files are allowed");
    }
  };

  const handleUpload = () => {
    if (!file) {
      setError("Please select a valid .xlsx file first.");
      return;
    }

    // TODO: integrate upload logic or parsing
    alert(`📁 Uploading file: ${file.name}`);
  };

  return (
    <div className="bg-gradient-to-br from-blue-50 to-indigo-50 p-8 rounded-3xl shadow-lg max-w-xl mx-auto text-center border border-gray-100 mt-10">
      {/* Header */}
      <h2 className="text-3xl font-bold text-gray-800 mb-2">
        Excel Defect Uploader
      </h2>
      <p className="text-gray-500 mb-8 italic">
        “Turn your QA data into intelligent insights.”
      </p>

      {/* Upload Area */}
      <label
        htmlFor="xlsxUpload"
        className="flex flex-col items-center justify-center border-2 border-dashed border-gray-300 rounded-2xl py-10 px-6 cursor-pointer hover:border-blue-500 hover:bg-blue-50/50 transition"
      >
        <Upload className="h-12 w-12 text-blue-500 mb-3" />
        <span className="text-gray-700 font-medium">
          {file ? (
            <span className="text-blue-700">{file.name}</span>
          ) : (
            "Click to browse or drag & drop your Excel file"
          )}
        </span>
        <span className="text-sm text-gray-400 mt-1">
          Supported format: <span className="font-semibold text-blue-500">.xlsx</span>
        </span>
        <input
          id="xlsxUpload"
          type="file"
          accept=".xlsx"
          onChange={handleFileChange}
          className="hidden"
        />
      </label>

      {/* Error or File Info */}
      {error && (
        <p className="text-red-500 text-sm mt-3 font-medium">{error}</p>
      )}

      {file && !error && (
        <div className="mt-5 p-3 bg-white rounded-xl border border-gray-200 shadow-sm">
          <p className="text-gray-800 font-medium">📄 {file.name}</p>
          <p className="text-sm text-gray-500">
            {(file.size / 1024).toFixed(2)} KB
          </p>
        </div>
      )}

      {/* Upload Button */}
      <button
        onClick={handleUpload}
        className="mt-8 w-full py-3 bg-blue-600 text-white font-semibold rounded-xl shadow hover:bg-blue-700 transition-all"
      >
        Upload File
      </button>

      {/* Footer / Description */}
      <p className="text-sm text-gray-500 mt-6">
        Once uploaded, AI will extract and analyze defect data from your Excel sheet.
      </p>
    </div>
  );
};

export default XlsxUpload;
