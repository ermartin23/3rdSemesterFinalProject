import { useState } from "react";
import logo from "../../assets/jerne-if-logo.png";

interface Game {
  id: string;
  week: number;
  deadline: string;
  winningNumbers: number[];
  revenue: number;
}

export default function GamesTab() {
  const currentWeek = getWeekNumber(new Date());

  const [activeGame, setActiveGame] = useState<Game>({
    id: crypto.randomUUID(),
    week: currentWeek,
    deadline: "29.11.2025 - 17:00",
    winningNumbers: [],
    revenue: 0
  });

  const [history, setHistory] = useState<Game[]>([]);

  function toggleNumber(n: number) {
    const selected = activeGame.winningNumbers;

    if (selected.includes(n)) {
      setActiveGame({
        ...activeGame,
        winningNumbers: selected.filter((x) => x !== n)
      });
      return;
    }

    if (selected.length === 3) return;

    setActiveGame({
      ...activeGame,
      winningNumbers: [...selected, n]
    });
  }

  function closeGame() {
    if (activeGame.winningNumbers.length !== 3) return;

    setHistory((prev) => [...prev, activeGame]);

    setActiveGame({
      id: crypto.randomUUID(),
      week: activeGame.week + 1,
      deadline: "—",
      winningNumbers: [],
      revenue: 0
    });
  }

  return (
    <div className="min-h-screen bg-[#faf6ef] p-6">

      {/* Header with Logo */}
      <div className="flex items-center justify-between px-6 py-4 bg-[#faf6ef] shadow-sm mb-8">

        <div className="flex items-center gap-3">
          <img
            src={logo}
            alt="Jerne IF"
            className="rounded-full shadow"
            style={{ width: "50px", height: "50px", objectFit: "cover" }}
          />
          <h1 className="text-2xl font-bold text-red-600">Admin Dashboard</h1>
        </div>

        <div className="flex items-center gap-4">
          <span className="text-gray-600">Logged in as Administrator</span>
          <button
            className="btn btn-outline border-red-600 text-red-600 hover:bg-red-50"
            onClick={() => {
              localStorage.removeItem("adminAuthenticated");
              window.location.href = "/admin-login";
            }}
          >
            Logout
          </button>
        </div>
      </div>

      {/* Active Game */}
      <div className="max-w-5xl mx-auto p-6 border border-red-300 rounded-xl bg-white shadow">

        <div className="flex items-center gap-3 mb-4">
          <span className="font-bold text-lg text-red-600">
            Active Game – Week {activeGame.week}
          </span>
          <span className="px-2 py-1 bg-red-500 text-white rounded text-sm">
            Active
          </span>
        </div>

        <p className="text-gray-600 mb-3">
          Deadline: <strong>{activeGame.deadline}</strong>
        </p>

        <div className="bg-[#f7ead4] p-4 rounded-lg mb-6">
          <p>Current Boards: <strong>0</strong></p>
          <p>Total Revenue: <strong>0 DKK</strong></p>
        </div>

        <div className="p-4 bg-[#fff4ef] border rounded-xl mb-6">
          <p className="font-bold mb-4">Select 3 Winning Numbers</p>

          <div className="grid grid-cols-8 gap-3">
            {Array.from({ length: 16 }, (_, i) => i + 1).map((num) => {
              const selected = activeGame.winningNumbers.includes(num);
              return (
                <button
                  key={num}
                  onClick={() => toggleNumber(num)}
                  className={`p-4 rounded-xl text-center border ${
                    selected
                      ? "bg-red-500 text-white border-red-700"
                      : "bg-[#f7ead4] border-[#e8d8bd] hover:bg-[#f3e3c7]"
                  }`}
                >
                  {num}
                </button>
              );
            })}
          </div>

          <p className="mt-3 text-sm text-gray-600">
            Selected: {activeGame.winningNumbers.length}/3
          </p>

          <button
            disabled={activeGame.winningNumbers.length !== 3}
            onClick={closeGame}
            className="btn mt-4 bg-red-600 text-white hover:bg-red-700 disabled:opacity-40"
          >
            Close Game & Set Winners
          </button>
        </div>
      </div>

      {/* Game History */}
      <div className="max-w-5xl mx-auto mt-10 mb-20">
        <h2 className="text-xl font-bold text-red-600 mb-4">Game History</h2>

        <div className="bg-white border rounded-xl shadow p-6">
          {history.length === 0 ? (
            <p className="text-center text-gray-600">No closed games yet</p>
          ) : (
            <ul className="space-y-3">
              {history.map((game) => (
                <li
                  key={game.id}
                  className="p-4 rounded-xl bg-[#faf6ef] shadow flex justify-between items-center"
                >
                  <span>
                    <strong>Week {game.week}</strong> — Winners:{" "}
                    {game.winningNumbers.join(", ")}
                  </span>
                  <span className="text-gray-600">{game.revenue} DKK</span>
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>
    </div>
  );
}

function getWeekNumber(date: Date) {
  const d = new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));
  const dayNum = d.getUTCDay() || 7;
  d.setUTCDate(d.getUTCDate() + 4 - dayNum);
  const yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
  return Math.ceil(((d.getTime() - yearStart.getTime()) / 86400000 + 1) / 7);
}
