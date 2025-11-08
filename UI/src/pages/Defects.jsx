import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

import { useUser, UserButton } from "@clerk/clerk-react";
// Sidebar items example
const sidebarItems = [
  { name: "Dashboard", link: "/dashboard" },
  { name: "Defect Management", link: "/defects" },
  { name: "Settings", link: "/settings" },
];

const Defects = () => {
  const navigate = useNavigate();

  const [defects, setDefects] = useState([
    { id: 1, title: "UI button misaligned" },
    { id: 2, title: "Crash on login" },
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
          <h1 className="text-3xl font-bold">Defects Management</h1>

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
        <header className="mb-8">
           <p className="text-gray-600 mt-2">
            Track and manage defects efficiently. Create new defects or view existing ones.
          </p>
        </header>
        

        {/* Buttons */}
        <div className="flex flex-col sm:flex-row gap-6 mb-10">
          <button
            onClick={handleCreateDefect}
            className="flex-1 px-8 py-6 bg-blue-600 text-white text-xl font-semibold rounded-2xl shadow-lg hover:bg-blue-700 transition"
          >
            Create a Defect
          </button>
          <button
            onClick={handleListDefects}
            className="flex-1 px-8 py-6 bg-green-600 text-white text-xl font-semibold rounded-2xl shadow-lg hover:bg-green-700 transition"
          >
            List of Defects
          </button>
        </div>

        {/* Optional defect list */}
        <div className="w-full max-w-3xl">
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
        </div>
      </main>
    </div>
  );
};

export default Defects;
