import { Outlet, useNavigate, useLocation } from "react-router-dom";
import {useEffect, useState} from "react";
import Confetti from "react-confetti";
import logo from "../../assets/jerne-if-logo.png";

export default function PlayerLayout() {
    const navigate = useNavigate();
    const location = useLocation();
    const showConfetti = location.pathname === "/player-winners";
    
    const [dimensions, setDimensions] = useState({
        width: window.innerWidth,
        height: window.innerHeight,
    });

    useEffect(() => {
        const handleResize = () =>
            setDimensions({
                width: window.innerWidth,
                height: window.innerHeight,
            });
        
        window.addEventListener("resize", handleResize);
        return () => window.removeEventListener("resize", handleResize);
    }, []);

    useEffect(() => {
        const token = localStorage.getItem("token");
        const role = localStorage.getItem("role");
        if (!token || role !== "Player") {
            navigate("/player-login", { replace: true });
        }
    }, [navigate]);

    return (
        <div className="min-h-screen bg-[#faf6ef] relative">
            {showConfetti && (
                <div className="fixed inset-0 z-0 pointer-events-none">
                    <Confetti
                        width={dimensions.width}
                        height={dimensions.height}
                        numberOfPieces={300}
                        recycle={false}
                        />
                </div>
            )}
            <div className="relative z-10 flex items-center justify-between px-8 py-4 shadow bg-[#faf6ef]">
                <div className="flex items-center gap-3">
                    <img
                        src={logo}
                        alt="Jerne IF"
                        className="rounded-full shadow"
                        style={{ width: "50px", height: "50px", objectFit: "cover" }}
                    />
                    <h1 className="text-3xl font-bold text-red-600 mb-2 flex items-center gap-2">
                        Player Dashboard <span className="text-4xl">🐦</span>
                    </h1>
                </div>

                <button
                    className="btn btn-outline border-red-600 text-red-600 hover:bg-red-50"
                    onClick={() => {
                        localStorage.removeItem("token");
                        localStorage.removeItem("role");
                        localStorage.removeItem("userId");
                        localStorage.removeItem("email");
                        navigate("/player-login", { replace: true });
                    }}
                >
                    Logout
                </button>
            </div>

            <div className="relative z-10 flex justify-center gap-6 mt-6 text-lg">
                <button onClick={() => navigate("/player-dashboard")} className="text-gray-600 hover:text-red-600">
                    Play
                </button>
                <button onClick={() => navigate("/player-history")} className="text-gray-600 hover:text-red-600">
                    View History
                </button>
                <button onClick={() => navigate("/player-winners")} className="text-gray-600 hover:text-red-600">
                    Winning Number
                </button>
                <button onClick={() => navigate("/player-transactions")} className="text-gray-600 hover:text-red-600">
                    Transactions
                </button>
            </div>

            <div className="relative z-10 p-4">
                <Outlet />
            </div>
        </div>
    );
}