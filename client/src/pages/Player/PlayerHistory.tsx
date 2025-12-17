import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import logo from "../../assets/jerne-if-logo.png";

interface PlayedBoard {
    week: number;
    numbers: number[];
    price: number;
}

export default function PlayerHistory() {
    const navigate = useNavigate();
    const [history, setHistory] = useState<PlayedBoard[]>([]);

    useEffect(() => {
        const raw = localStorage.getItem("boardHistory");

        if (!raw) {
            setHistory([]);
            return;
        }

        try {
            const parsed: PlayedBoard[] = JSON.parse(raw);

            if (!Array.isArray(parsed)) {
                setHistory([]);
                return;
            }

            // Newest first
            const sorted = [...parsed].sort((a, b) => b.week - a.week);
            setHistory(sorted);
        } catch {
            setHistory([]);
        }
    }, []);

    return (
        <div className="min-h-screen bg-[#faf6ef]">
            {/* HEADER */}
            <div className="flex items-center justify-between px-8 py-4 shadow bg-[#faf6ef]">
                <div className="flex items-center gap-3">
                    <img
                        src={logo}
                        alt="Jerne IF"
                        className="w-[50px] h-[50px] rounded-full"
                    />
                    <h1 className="text-2xl font-bold text-red-600">
                        Your History
                    </h1>
                </div>

                <button
                    className="btn btn-outline border-red-600 text-red-600"
                    onClick={() => navigate("/player-dashboard")}
                >
                    Back
                </button>
            </div>

            {/* CONTENT */}
            <div className="max-w-2xl mx-auto mt-10 px-4">
                {history.length === 0 ? (
                    <p className="text-center text-gray-600 text-lg mt-20">
                        No history recorded yet.
                    </p>
                ) : (
                    history.map((board, idx) => (
                        <div
                            key={`${board.week}-${idx}`}
                            className="bg-white rounded-xl shadow p-6 mb-4 flex justify-between items-center"
                        >
                            <div>
                                <p className="font-bold text-red-600">
                                    Week {board.week} – 2025
                                </p>
                                <p className="text-black">
                                    Numbers played: {board.numbers.join(", ")}
                                </p>
                            </div>

                            <p className="text-black font-bold text-lg">
                                {board.price} DKK
                            </p>
                        </div>
                    ))
                )}
            </div>
        </div>
    );
}
