import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import XlsxUpload from "../components/FileUpload";


import { useUser, UserButton } from "@clerk/clerk-react";
// Sidebar items example
const sidebarItems = [
  { name: "Dashboard", link: "/dashboard" },
  { name: "Defect Assistant", link: "/defects" },
  { name: "Settings", link: "/settings" },
];

const Defects = () => {
  const navigate = useNavigate();

  const [defects, setDefects] = useState([
    { id: 1, title: "AI model misclassified input data" },
    { id: 2, title: "API timeout under heavy load" },
  ]);

  const handleCreateDefect = () => {
    navigate("/create-defect")
  };

  const handleListDefects = () => {
    navigate("/defectlist")
  };

  return (
    <div className="flex h-screen bg-gray-100">
      {/* Sidebar */}
      <aside className="w-64 bg-gray-800 text-white flex flex-col">
        <div className="text-2xl font-bold p-6 border-b border-gray-700">DefectIQ</div>
        <nav className="flex-1 p-4 space-y-2">
          {sidebarItems.map((item) => (
            <a
              key={item.name}
              href={item.link}
              className="block px-4 py-2 rounded hover:bg-gray-700 transition"
            >
              {item.name}
            </a>
          ))}
        </nav>
      </aside>

      {/* Main Content */}
      <main className="flex-1 p-8 overflow-auto">
        {/* Header */}
        <div className="flex items-center justify-between mb-8">
          <h1 className="text-3xl font-bold">Defect Management Assistant</h1>

          <div className="flex items-center gap-4">
            <div className="flex items-center gap-4">
                <input
                    type="text"
                    placeholder="Search..."
                    className="bg-gray-800 text-gray-200 px-3 py-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
                <UserButton afterSignOutUrl="/" />
            </div>
          </div>
        </div>
        <header className="mb-10 bg-white p-6 rounded-2xl shadow-sm">
           <h1 className="text-4xl font-bold text-gray-800 mb-4 text-center">
        Welcome to AIgniters Defect Management Assistant
      </h1>
      <p className="text-gray-600 mb-10 text-center text-lg">
        Revolutionizing QA workflows with AI-powered intelligence and automation
      </p>

      {/* Feature / Functionality Cards */}
      <div className="grid md:grid-cols-2 gap-8">
        {/* Intelligent Defect Logging */}
        <div className="p-6 bg-white rounded-2xl shadow hover:shadow-lg transition">
          <h2 className="text-2xl font-semibold mb-2 text-blue-600">
            📝 Intelligent Defect Logging
          </h2>
          <p className="text-gray-600">
            Auto-generate defect descriptions and steps to reproduce from test results, 
            voice instructions, or raw inputs. Automatically attach logs, screenshots, and videos.
          </p>
        </div>

        {/* Duplicate Defect Prevention */}
        <div className="p-6 bg-white rounded-2xl shadow hover:shadow-lg transition">
          <h2 className="text-2xl font-semibold mb-2 text-blue-600">
            🔍 Duplicate Defect Prevention
          </h2>
          <p className="text-gray-600">
            Semantic similarity search prevents duplicate defect entries. Alerts testers if a similar defect already exists.
          </p>
        </div>

        {/* Severity & Priority Automation */}
        <div className="p-6 bg-white rounded-2xl shadow hover:shadow-lg transition">
          <h2 className="text-2xl font-semibold mb-2 text-blue-600">
            🎯 Automated Severity & Priority
          </h2>
          <p className="text-gray-600">
            AI analyzes historical triage data and recommends appropriate severity and priority levels for each defect.
          </p>
        </div>

        {/* Predictive Analytics */}
        <div className="p-6 bg-white rounded-2xl shadow hover:shadow-lg transition">
          <h2 className="text-2xl font-semibold mb-2 text-blue-600">
            📊 Predictive Analytics
          </h2>
          <p className="text-gray-600">
            Predict defect-prone modules, expected defect density, and risk hotspots for upcoming sprints/releases based on historical patterns.
          </p>
        </div>
      </div>
      {/* Footer / Description */}
      <p className="text-sm text-gray-500 mt-6">
        Once uploaded, AI will extract and analyze defect data from your Excel sheet.
      </p>
      <div className="mt-12 w-full">
            <XlsxUpload />
          </div>
        </header>
        

        {/* Buttons */}
        <div className="flex flex-col sm:flex-row gap-6 mb-10 ">
          {/* <div className="mt-12 w-full">
            <XlsxUpload />
          </div> */}
          {/* <button
            onClick={handleCreateDefect}
            className="flex-1 px-8 py-6 bg-gray-600 text-white text-xl font-semibold rounded-2xl shadow-lg hover:bg-blue-700 transition"
          >
            Upload CSV File
          </button>
          <button
            onClick={handleListDefects}
            className="flex-1 px-8 py-6 bg-gray-600 text-white text-xl font-semibold rounded-2xl shadow-lg hover:bg-green-700 transition"
          >
            List of Defects
          </button> */}
        </div>

        {/* Optional defect list */}
        {/* <div className="w-full max-w-3xl">
          <h2 className="text-2xl font-semibold mb-4 text-gray-800">Defect List</h2>
          {defects.length === 0 ? (
            <p className="text-gray-500">No defects recorded yet.</p>
          ) : (
            <ul className="space-y-2">
              {defects.map((defect) => (
                <li
                  key={defect.id}
                  className="p-4 bg-white rounded-xl shadow hover:bg-gray-50 transition"
                >
                  {defect.title}
                </li>
              ))}
            </ul>
          )}
        </div> */}
      </main>
    </div>
  );
};

export default Defects;
