import { useState } from "react";
import { useNavigate } from "react-router-dom";
import logo from "../../assets/jerne-if-logo.png";
import {login} from "../../api/auth.ts";

export default function PlayerLogin() {
    const navigate = useNavigate();
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState<string | null>(null);

    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        setError(null);
        
        try {
            const data = await login(email, password);
            
            console.log("Login response:", data);
            
            if (!data.token) {
                setError("Login succeeded, but no token was returned");
                return;
            }
            
            if (data.role !== "Player") {
                setError("This account is not a player");
                return;
            }
            
            console.log("Navigating to Player Dashboard");
            navigate("/player-dashboard");
        } catch (err: any) {
            console.error("Login failed:", err);
            setError(err.message ?? "Login failed");
        }
        console.log("Current path:", window.location.pathname);
    }

    return (
        <div className="min-h-screen bg-[#faf6ef] flex flex-col items-center justify-center px-4">
            <img
                src={logo}
                alt="Jerne IF"
                className="rounded-full shadow mb-6"
                style={{ width: "110px", height: "110px", objectFit: "cover" }}
            />

            <h1 className="text-3xl font-bold text-red-600 mb-2">Player Login</h1>
            <p className="text-gray-600 mb-8 text-center">
                Enter your email and password
            </p>

            <form
                onSubmit={handleSubmit}
                className="bg-white p-6 rounded-xl shadow w-full max-w-md"
            >
                <label className="block text-gray-700 font-semibold mb-2">Email</label>
                <input
                    type="email"
                    className="input input-bordered w-full mb-4"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                />

                <label className="block text-gray-700 font-semibold mb-2">Password</label>
                <input
                    type="password"
                    className="input input-bordered w-full mb-5"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                />

                <button
                    type="submit"
                    className="btn w-full bg-red-600 text-white hover:bg-red-700"
                >
                    Login
                </button>

                {error && (
                    <p className="text-red-600 text-sm mt-4 text-center">{error}</p>
                )}
            </form>

            <button
                onClick={() => navigate("/")}
                className="btn btn-outline mt-6 px-6 border-red-600 text-red-600 hover:bg-red-50"
            >
                Back to Home
            </button>

            <p className="text-gray-500 text-xs mt-8">Jerne IF Klub</p>
        </div>
    );
}
