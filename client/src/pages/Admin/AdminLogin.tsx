import React, { useState, useEffect } from "react";
import {useNavigate} from "react-router-dom";
import {login} from "../../api/auth.ts";
import PasswordInput from "../../components/PasswordInput";


export default function AdminLogin() {
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  useEffect(() => {
    const token = localStorage.getItem("token");
    const role = localStorage.getItem("role");
    
    if (token && role === "Admin") {
      navigate("/admin-dashboard", { replace: true });
      return;
    }
    
    if (token && role === "Player") {
      localStorage.removeItem("token");
      localStorage.removeItem("role");
      localStorage.removeItem("userId");
      localStorage.removeItem("email");
    }
  }, [navigate]);
  
  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError("");
    
    try {
      const data = await login(email, password);
      
      if (data.role !== "Admin") {
        setError("This account is not an administrator");
        return;
      }
      
      navigate("/admin-dashboard");
    } catch (err: any) {
      setError(err.message ?? "Login failed");
    }
  }

  return (
    <div className="min-h-screen bg-[#faf6ef] flex justify-center items-center px-4">
      <div className="bg-white border rounded-xl shadow p-8 w-full max-w-md">

        <h1 className="text-3xl font-bold text-red-600 text-center mb-6">
          Admin Login
        </h1>

        {error && (
          <div className="text-red-600 text-center mb-4">{error}</div>
        )}

        <form onSubmit={handleSubmit} className="space-y-4 text-black">
          <label className="block text-gray-700 font-semibold">Email</label>
          <input
              type="email"
              className="input input-bordered w-full bg-gray-300"
              placeholder=" Email"
              onChange={(e) => setEmail(e.target.value)}
              required
          />
          <PasswordInput className="bg-gray-300 text-black" value={password} onChange={setPassword} />

          <button
              type="submit"
              className="btn bg-red-600 text-white hover:bg-red-700 w-full"
          >
            Login
          </button>
        </form>

        <div className="text-center mt-6">
          <a href="/" className="text-gray-600 hover:underline">
            ← Back to Home
          </a>
        </div>

      </div>
    </div>
  );
}


