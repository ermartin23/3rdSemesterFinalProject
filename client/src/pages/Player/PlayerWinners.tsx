import { useEffect, useState } from "react";

type WinnerResponse = {week: number; year: number; numbers: number[]};

export default function PlayerWinners() {
    const API_BASE = import.meta.env.VITE_API_URL;

    const [week, setWeek] = useState<number | null>(null);
    const [year, setYear] = useState<number | null>(null);
    const [numbers, setNumbers] = useState<number[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        async function loadWinningNumbers() {
            try {
                const token = localStorage.getItem("token");
                const res = await fetch(`${API_BASE}/api/games/latest-winning-numbers`, {
                    headers: {Authorization: `Bearer ${token}`},
                });
                const text = await res.text();
                console.log("Raw response", text);

                if (!res.ok) throw new Error("Error fetching winning numbers");
                
                const data: WinnerResponse = JSON.parse(text);
                setWeek(data.week);
                setYear(data.year);
                setNumbers(data.numbers);
            } catch (e) {
                console.error(e);
            } finally {
                setLoading(false);
            }
        }
        loadWinningNumbers();
    }, [API_BASE]);
    
    const hasData = week !== null && year !== null && numbers.length > 0;

    return (
        <div className="px-4 py-10 flex items-center justify-center">
            <div className="bg-white rounded-xl shadow-xl border border-red-300 p-10 w-full max-w-xl text-center">
                <h1 className="text-3xl font-bold text-red-600 mb-2">🎉 Winning Number 🎉</h1>

                {loading ? (
                    <p className="text-gray-600 mb-6">Loading…</p>
                ) : hasData ? (
                    <>
                        <p className="text-lg text-gray-600 mb-6">Week {week} — {year}</p>

                        <h2 className="text-black text-xl font-semibold mb-4">The winning numbers are:</h2>

                        <div className="flex flex-wrap justify-center gap-4 mb-6">
                            {numbers.map((n) => (
                                <div key={n} className="w-20 h-20 flex items-center justify-center rounded-xl bg-red-400 text-white text-3xl font-bold shadow-lg">
                                    {n}
                                </div>
                            ))}
                        </div>

                        <p className="text-black text-lg font-medium mb-6">
                            Congratulations to all winners! 🏆✨
                        </p>
                    </>
                ) : (
                    <>
                        <p className="text-gray-600 mb-6">No winner has been drawn yet 😅</p>
                    </>
                )}
            </div>
        </div>
    );
}