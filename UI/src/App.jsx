import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Login from "./pages/Login";
import Dashboard from "./pages/Dashboard";
import Reports from "./pages/Defects";
import Settings from "./pages/Settings";
import Home from "./pages/Home";
import { SignedIn, SignedOut, RedirectToSignIn } from "@clerk/clerk-react";
import Signup from "./pages/Signup"; 
import { useEffect } from "react";

function ProtectedRoute({ children }) {
  return (
    <>
      <SignedIn>{children}</SignedIn>
      <SignedOut>
        <RedirectToSignIn />
      </SignedOut>
    </>
  );
}

function App() {

  useEffect(() => {
    const hideSecuredBy = () => {
      document.querySelectorAll('*').forEach(el => {
        // trim() handles any extra spaces/newlines
        if (el.textContent.trim() === 'Secured by' && el.parentElement) {
          console.log('Hiding:', el.textContent);
          el.parentElement.style.display = 'none';
        }
      });
    };

    // Run once on mount
    hideSecuredBy();

    // Also apply opacity to .cl-signIn-root
    const signInRoot = document.querySelector('.cl-signIn-root');
    if (signInRoot) {
      signInRoot.style.opacity = '0.8';
    }


    // Optional: Watch for dynamic changes (e.g. Clerk re-render)
    const observer = new MutationObserver(hideSecuredBy);
    observer.observe(document.body, { childList: true, subtree: true });

    // Cleanup on unmount
    return () => observer.disconnect();
  }, []);

  return (
    <Router>
      <Routes>
        {/* Public Routes */}
        <Route path="/" element={<Home />} />
        <Route path="/sign-up" element={<Signup />} />
        
        <Route path="/login" element={<Login />} />

        {/* Protected Routes */}
        <Route path="/dashboard" element={<ProtectedRoute><Dashboard /></ProtectedRoute>} />
        <Route path="/defects" element={<ProtectedRoute><Reports /></ProtectedRoute>} />
        {/* <Route path="/insights" element={<ProtectedRoute><Insights /></ProtectedRoute>} /> */}
        <Route path="/settings" element={<ProtectedRoute><Settings /></ProtectedRoute>} />
      </Routes>
    </Router>
  )
}

export default App
