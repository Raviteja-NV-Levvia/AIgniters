import React, { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { SignIn, useAuth } from "@clerk/clerk-react";

export default function Login() {
  const { isSignedIn } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    if (isSignedIn) {
      navigate("/dashboard");
    }
  }, [isSignedIn, navigate]);

  return (
    <main className="relative w-full h-screen overflow-hidden bg-gray-900 flex justify-center items-center">
      <div className="bg-gray-900 p-8 rounded-2xl shadow-xl border border-gray-800">
        <SignIn signUpUrl="/sign-up" />
      </div>
    </main>
  );
}
