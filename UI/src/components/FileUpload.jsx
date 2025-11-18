import React, { useState } from "react";
import { Upload } from "lucide-react";

const XlsxUpload = () => {
  const [file, setFile] = useState(null);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [successMsg, setSuccessMsg] = useState("");

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
      setSuccessMsg("");
    } else {
      setFile(null);
      setError("❌ Only .xlsx files are allowed");
      setSuccessMsg("");
    }
  };

  const handleUpload = async () => {
    if (!file) {
      setError("Please select a valid .xlsx file first.");
      return;
    }

    setLoading(true);
    setError("");
    setSuccessMsg("");

    try {
      const formData = new FormData();
      formData.append("file", file);

      const response = await fetch(
        "https://defect-management-assistant-api-gtdgazavazbfc8bk.southindia-01.azurewebsites.net/api/Defects/upload-excel",
        {
          method: "POST",
          body: formData,
        }
      );

      if (!response.ok) {
        throw new Error("Failed to upload file");
      }

      const data = await response.json();
      setSuccessMsg("✅ File uploaded successfully!");
      console.log("API Response:", data);
    } catch (err) {
      setError(`❌ Upload failed: ${err.message}`);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="bg-gradient-to-br from-blue-50 to-indigo-50 p-8 rounded-3xl shadow-lg max-w-xl mx-auto text-center border border-gray-100 mt-10">
      <h2 className="text-3xl font-bold text-gray-800 mb-2">
        Excel Defect Uploader
      </h2>
      <p className="text-gray-500 mb-8 italic">
        “Turn your QA data into intelligent insights.”
      </p>

      <label
        htmlFor="xlsxUpload"
        className="flex flex-col items-center justify-center border-2 border-dashed border-gray-300 rounded-2xl py-10 px-6 cursor-pointer hover:border-blue-500 hover:bg-blue-50/50 transition"
      >
        <Upload className="h-12 w-12 text-blue-500 mb-3" />
        <span className="text-gray-700 font-medium">
          {file ? <span className="text-blue-700">{file.name}</span> : "Click to browse or drag & drop your Excel file"}
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

      {error && <p className="text-red-500 text-sm mt-3 font-medium">{error}</p>}
      {successMsg && <p className="text-green-600 text-sm mt-3 font-medium">{successMsg}</p>}

      <button
        onClick={handleUpload}
        disabled={loading}
        className={`mt-8 w-full py-3 text-white font-semibold rounded-xl shadow transition-all ${
          loading ? "bg-blue-400 cursor-not-allowed" : "bg-blue-600 hover:bg-blue-700"
        }`}
      >
        {loading ? "Uploading..." : "Upload File"}
      </button>

      <p className="text-sm text-gray-500 mt-6">
        Once uploaded, AI will extract and analyze defect data from your Excel sheet.
      </p>
    </div>
  );
};

export default XlsxUpload;
