import React, { useState } from "react";
import { motion } from "framer-motion";

import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from "recharts";
import { Link, useLocation } from "react-router-dom";
import { useUser, UserButton } from "@clerk/clerk-react";

const data = [
  { name: "Jan", defects: 12 },
  { name: "Feb", defects: 18 },
  { name: "Mar", defects: 15 },
  { name: "Apr", defects: 22 },
  { name: "May", defects: 28 },
  { name: "Jun", defects: 25 },
];

export default function Dashboard() {
  const [isCollapsed, setIsCollapsed] = useState(false);
  const location = useLocation();
  const { user } = useUser(); // Clerk hook
  const fullName = user?.fullName || "User";
  const profileImage = user?.profileImageUrl;

  const navItems = [
    { name: "Dashboard", icon: "🏠", path: "/dashboard" },
    { name: "Defect", icon: "📊", path: "/defects" },
    { name: "Settings", icon: "⚙️", path: "/settings" },
  ];

  return (
    <div className="flex h-screen bg-gray-950 text-gray-100">
      {/* Sidebar */}
      <motion.aside
        animate={{ width: isCollapsed ? "80px" : "240px" }}
        transition={{ duration: 0.3 }}
        className="bg-gray-900 flex flex-col p-5 border-r border-gray-800"
      >
        <div className="flex items-center justify-between mb-8">
          {!isCollapsed && (
            <h2 className="text-2xl font-bold text-blue-400">DefectIQ</h2>
          )}
          <button
            onClick={() => setIsCollapsed(!isCollapsed)}
            className="text-gray-400 hover:text-white"
            title="Toggle Sidebar"
          >
            {isCollapsed ? "➡️" : "⬅️"}
          </button>
        </div>

        <nav className="flex flex-col gap-3">
          {navItems.map((item, i) => (
            <Link
              key={i}
              to={item.path}
              className={`flex items-center gap-3 p-3 rounded-lg transition-all duration-200 ${
                location.pathname === item.path
                  ? "bg-blue-600 text-white"
                  : "text-gray-400 hover:bg-gray-800 hover:text-white"
              }`}
            >
              <span className="text-lg">{item.icon}</span>
              {!isCollapsed && <span>{item.name}</span>}
            </Link>
          ))}
        </nav>

        <div className="mt-auto text-xs text-gray-500 text-center pt-6">
          {!isCollapsed && "© 2025 DefectIQ"}
        </div>
      </motion.aside>

      {/* Main Content */}
      <div className="flex-1 p-8 overflow-y-auto">
        {/* Top Bar */}
        <div className="flex items-center justify-between mb-8">
          <h1 className="text-3xl font-bold">Dashboard Overview</h1>

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

        {/* Summary Cards */}
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
          {[
            { title: "Total defects", value: "128", change: "+12%" },
            { title: "New Defects", value: "34", change: "+8%" },
            { title: "AI Predicted Severity", value: "9 High", change: "-3%" },
            { title: "Duplicate Detected", value: "3", change: "+3%" },
          ].map((card, i) => (
            <motion.div
              key={i}
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: i * 0.1 }}
              className="bg-gray-900 rounded-2xl p-5 shadow-lg border border-gray-800 hover:border-blue-600 transition"
            >
              <h3 className="text-gray-400">{card.title}</h3>
              <p className="text-3xl font-bold mt-2 text-white">
                {card.value}
              </p>
              <p
                className={`text-sm mt-1 ${
                  card.change.includes("-") ? "text-red-400" : "text-green-400"
                }`}
              >
                {card.change}
              </p>
            </motion.div>
          ))}
        </div>

        {/* Chart */}
        <div className="bg-gray-900 p-6 rounded-2xl shadow-lg border border-gray-800 mb-8">
          <h2 className="text-xl font-semibold mb-4 text-gray-200">
            Monthly Defect Activity
          </h2>
          <ResponsiveContainer width="100%" height={300}>
            <LineChart data={data}>
              <CartesianGrid strokeDasharray="3 3" stroke="#333" />
              <XAxis dataKey="name" stroke="#888" />
              <YAxis stroke="#888" />
              <Tooltip />
              <Line
                type="monotone"
                dataKey="defects"
                stroke="#3b82f6"
                strokeWidth={2}
                dot={{ r: 4 }}
              />
            </LineChart>
          </ResponsiveContainer>
        </div>
      </div>
    </div>
  );
}
