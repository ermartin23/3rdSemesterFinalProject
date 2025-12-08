import { useLocation, useNavigate } from "react-router-dom";
import Confetti from "react-confetti";
import { useEffect, useState } from "react";

export default function PlayerWinners() {
    const navigate = useNavigate();
    const location = useLocation();

    // Data passed from previous page
    const { week, numbers } = location.state || { week: null, numbers: [] };

    // Detect screen for confetti
    const [dimensions, setDimensions] = useState({
        width: window.innerWidth,
        height: window.innerHeight,
    });

    useEffect(() => {
        const handleResize = () =>
            setDimensions({ width: window.innerWidth, height: window.innerHeight });

        window.addEventListener("resize", handleResize);
        return () => window.removeEventListener("resize", handleResize);
    }, []);

    return (
        <div className="min-h-screen bg-[#faf6ef] flex flex-col items-center justify-center relative overflow-hidden">

            {/* Confetti */}
            <Confetti
                width={dimensions.width}
                height={dimensions.height}
                numberOfPieces={300}
                recycle={false}
            />

            {/* Card */}
            <div className="bg-white rounded-xl shadow-xl border border-red-300 p-10 w-full max-w-xl text-center relative z-10">

                <h1 className="text-3xl font-bold text-red-600 mb-2">
                    🎉 Winning Number 🎉
                </h1>

                <p className="text-lg text-gray-600 mb-6">
                    Week {week} — 2025
                </p>

                <h2 className="text-xl font-semibold mb-4">
                    The winning numbers are:
                </h2>

                {/* Numbers */}
                <div className="flex justify-center gap-6 mb-6">
                    {numbers?.map((n: number) => (
                        <div
                            key={n}
                            className="w-20 h-20 flex items-center justify-center rounded-xl bg-red-400 text-white text-3xl font-bold shadow-lg"
                        >
                            {n}
                        </div>
                    ))}
                </div>

                <p className="text-lg font-medium mb-6">
                    Congratulations to all winners! 🏆
                </p>

                <button
                    className="btn bg-red-600 text-white hover:bg-red-700 px-6 py-2 rounded-lg"
                    onClick={() => navigate("/player-history")}
                >
                    ← Back to History
                </button>
            </div>
        </div>
    );
}
