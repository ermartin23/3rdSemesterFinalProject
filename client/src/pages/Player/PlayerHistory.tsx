import { useEffect, useState } from "react";

interface PlayedBoard {
    boardId: string;
    week: number;
    year: number;
    numbers: number[];
    price: number;
    repeatingBoardId?: string | null;
}

export default function PlayerHistory() {
    const [history, setHistory] = useState<PlayedBoard[]>([]);

    useEffect(() => {
        async function loadHistory() {
            const token = localStorage.getItem("token");
            const API_BASE = import.meta.env.VITE_API_URL;

            if (!token) {
                setHistory([]);
                return;
            }

            const res = await fetch(`${API_BASE}/api/boards/me`, {
                headers: { Authorization: `Bearer ${token}` },
            });

            if (!res.ok) {
                console.error("History fetch failed:", await res.text());
                setHistory([]);
                return;
            }

            const data = await res.json();

            const mapped: PlayedBoard[] = data.map((b: any) => ({
                boardId: b.boardId,
                week: b.week,
                year: b.year,
                numbers: b.chosenNumbers ?? [],
                price: Number(b.price ?? 0),
                repeatingBoardId: b.repeatingBoardId ?? null,
            }));

            // Already ordered by backend, but safe:
            mapped.sort((a, b) => {
                const byWeek = (b.year - a.year) || (b.week - a.week);
                return byWeek !== 0 ? byWeek : (b.price - a.price);
            });

            setHistory(mapped);
        }

        loadHistory();
    }, []);
    async function stopRepeating(boardId: string) {
        const ok = window.confirm(
            "Are you sure you want to stop repeating this board? It cannot be undone."
        );
        if (!ok) return;

        const token = localStorage.getItem("token");
        const API_BASE = import.meta.env.VITE_API_URL;

        const res = await fetch(`${API_BASE}/api/repeatingboard/toggle`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`,
            },
            body: JSON.stringify({ boardId, isRepeating: false }),
        });

        if (!res.ok) {
            alert(await res.text());
            return;
        }

        setHistory(prev =>
            prev.map(b => (b.boardId === boardId ? { ...b, repeatingBoardId: null } : b))
        );
    }




    return (
        <div className="min-h-screen bg-[#faf6ef]">
            {/* CONTENT */}
            <div className="max-w-2xl mx-auto mt-10 px-4">
                {history.length === 0 ? (
                    <p className="text-center text-gray-600 text-lg mt-20">
                        No history recorded yet.
                    </p>
                ) : (
                    history.map((board) => (
                        <div
                            key={board.boardId}
                            className="bg-white rounded-xl shadow p-6 mb-4 flex justify-between items-center"
                        >
                            <div>
                                <p className="font-bold text-red-600">
                                    Week {board.week} – {board.year}
                                </p>
                                <p className="text-black">
                                    Numbers played: {board.numbers.join(", ")}
                                </p>
                                {board.repeatingBoardId && (
                                    <div className="mt-2 flex items-center gap-3">
                                        <span className="inline-block px-3 py-1 text-xs font-semibold text-red-600 bg-red-100 rounded-full">
                                        Repeating every week
                                        </span>
                                        <button
                                            className="text-xs font-semibold text-red-600 hover:text-red-700 underline"
                                            onClick={() => stopRepeating(board.boardId)}>
                                            Stop repeating
                                        </button>
                                    </div>
                                )}
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
