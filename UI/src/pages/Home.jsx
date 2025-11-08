import React, { useState, useRef, useEffect } from "react";
import Spline from "@splinetool/react-spline";
import { motion } from "framer-motion";
import { useNavigate } from "react-router-dom";
import ambientSound from "../assets/cinematic-ambient.mp3";
import { SignIn, SignUp, useAuth } from "@clerk/clerk-react";
import Login from "./Login.jsx";

export default function Dashboards() {
  const navigate = useNavigate();
  const [isLoaded, setIsLoaded] = useState(false);
  const [isPlaying, setIsPlaying] = useState(false);
  const audioRef = useRef(null);
  const fadeInterval = useRef(null);
  const { isSignedIn } = useAuth();

   const handleStart = () => navigate("/login")
  // Fade in/out helper
  const fadeAudio = (fadeIn = true) => {
    if (!audioRef.current) return;
    const audio = audioRef.current;
    const step = 0.02; // volume step per tick
    const delay = 80; // ms between steps

    clearInterval(fadeInterval.current);

    if (fadeIn) {
      audio.volume = 0;
      audio.play();
      fadeInterval.current = setInterval(() => {
        if (audio.volume < 0.8) {
          audio.volume = Math.min(audio.volume + step, 0.8);
        } else {
          clearInterval(fadeInterval.current);
        }
      }, delay);
    } else {
      fadeInterval.current = setInterval(() => {
        if (audio.volume > 0) {
          audio.volume = Math.max(audio.volume - step, 0);
        } else {
          clearInterval(fadeInterval.current);
          audio.pause();
        }
      }, delay);
    }
  };

  const toggleAudio = () => {
    if (isPlaying) {
      fadeAudio(false);
    } else {
      fadeAudio(true);
    }
    setIsPlaying(!isPlaying);
  };

  useEffect(() => {
    return () => clearInterval(fadeInterval.current);
  }, []);

  return (
    <>
      <main className="relative w-full h-screen overflow-hidden bg-gray-900">
          {/* Background Audio */}
          <audio ref={audioRef} src={ambientSound} loop preload="auto" />

          {/* 3D Scene */}
          <Spline
            scene="https://prod.spline.design/kjyNRHP7nG4WW96G/scene.splinecode"
            onLoad={() => {
              setIsLoaded(true);
              fadeAudio(true); // gentle fade-in after load
              setIsPlaying(true);
            }}
          />

          {/* Loading Overlay */}
          {!isLoaded && (
            <motion.div
              initial={{ opacity: 1 }}
              animate={{ opacity: 0 }}
              transition={{ delay: 1, duration: 0.8 }}
              className="absolute inset-0 flex flex-col items-center justify-center bg-gray-900 z-20"
            >
              <div className="w-12 h-12 border-4 border-blue-500 border-t-transparent rounded-full animate-spin mb-4"></div>
              <p className="text-gray-300 text-lg font-medium animate-pulse">
                Initializing AI systems…
              </p>
            </motion.div>
          )}

          {/* Overlay UI */}
          {isLoaded && (
            <motion.div
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 1 }}
              className="absolute inset-0 flex flex-col items-center justify-center text-center z-10"
            >
              <motion.h1
                initial={{ opacity: 0, y: -40 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 1 }}
                className="text-6xl font-extrabold text-white drop-shadow-lg"
              >
                AI-powered <br/>
                Defect Management Assistent
              </motion.h1>

              <motion.p
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: 0.3, duration: 1 }}
                className="text-xl text-gray-300 mt-4 mb-8"
              >
                AI That Finds Defects Before They Find You
              </motion.p>

              <div className="flex items-center gap-4">
                {!isSignedIn && <motion.button
                  whileHover={{ scale: 1.1 }}
                  whileTap={{ scale: 0.95 }}
                  onClick={handleStart}
                  className="bg-gray-600 hover:bg-blue-700 text-white px-6 py-3 rounded-xl text-lg font-semibold shadow-md transition"
                >
                  🚀 Login Here
                </motion.button>}


                {isSignedIn && <motion.button
                  whileHover={{ scale: 1.1 }}
                  whileTap={{ scale: 0.95 }}
                  onClick={handleStart}
                  className="bg-gray-600 hover:bg-blue-700 text-white px-6 py-3 rounded-xl text-lg font-semibold shadow-md transition"
                >
                  🚀 Welcome, Let's go to the Dashboard
                </motion.button>}

                <button
                  onClick={toggleAudio}
                  className="text-gray-400 hover:text-white text-xl transition"
                  title={isPlaying ? "Mute Sound" : "Play Sound"}
                >
                  {isPlaying ? "🔊" : "🔈"}
                </button>
              </div>

            </motion.div>
          )}

          {/* Gradient overlay */}
          <div className="absolute inset-0 bg-gradient-to-t from-black/70 via-transparent to-transparent z-0"></div>
      </main>
    </>

  );
}
