import React from "react";
import { SignUp } from "@clerk/clerk-react";

export default function Signup() {
  return (
    <div className="flex items-center justify-center h-screen bg-gray-950 text-white">
      <div className="bg-gray-900 p-8 rounded-2xl shadow-xl border border-gray-800">
        <SignUp signInUrl="/" />
      </div>
    </div>
  );
}
