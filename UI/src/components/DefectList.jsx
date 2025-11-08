// src/pages/DefectList.jsx
import React from "react";

const DefectList = ({ defects }) => {
  return (
    <div className="max-w-4xl mx-auto p-6">
      <h2 className="text-2xl font-bold mb-4">Defect List</h2>
      {defects?.length === 0 ? (
        <p>No defects logged yet.</p>
      ) : (
        <table className="w-full border-collapse border">
          <thead>
            <tr className="bg-gray-100">
              <th className="border px-4 py-2">Title</th>
              <th className="border px-4 py-2">Description</th>
              <th className="border px-4 py-2">Severity</th>
              <th className="border px-4 py-2">Priority</th>
            </tr>
          </thead>
          <tbody>
            {defects?.map((defect, index) => (
              <tr key={index} className="hover:bg-gray-50">
                <td className="border px-4 py-2">{defect?.title}</td>
                <td className="border px-4 py-2">{defect?.description}</td>
                <td className="border px-4 py-2">{defect?.severity}</td>
                <td className="border px-4 py-2">{defect?.priority}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
};

export default DefectList;
