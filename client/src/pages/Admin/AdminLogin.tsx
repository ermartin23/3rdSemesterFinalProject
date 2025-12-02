import { useState } from "react";

export default function AdminLogin() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();

    // Fake Login to presentation (we can change later)
    if (email === "admin@admin.com" && password === "admin123") {
      localStorage.setItem("adminAuthenticated", "true");
      window.location.href = "/admin-dashboard";
    } else {
      setError("Invalid email or password.");
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

        <form onSubmit={handleSubmit} className="space-y-5">

          <input
            type="email"
            className="input input-bordered w-full"
            placeholder="Email"
            onChange={(e) => setEmail(e.target.value)}
            required
          />

          <input
            type="password"
            className="input input-bordered w-full"
            placeholder="Password"
            onChange={(e) => setPassword(e.target.value)}
            required
          />

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


