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
  const email = localStorage.getItem("playerEmail");

  useEffect(() => {
    const saved = localStorage.getItem("boardHistory");
    if (saved) setHistory(JSON.parse(saved));
  }, []);

  return (
    <div className="min-h-screen bg-[#faf6ef]">
      <div className="flex items-center justify-between px-8 py-4 shadow">
        <div className="flex items-center gap-3">
          <img
            src={logo}
            alt="logo"
            style={{ width: "50px", height: "50px" }}
            className="rounded-full"
          />
          <h1 className="text-2xl font-bold text-red-600">
            {email?.split("@")[0]}'s History
          </h1>
        </div>

        <button
          className="btn btn-outline border-red-600 text-red-600"
          onClick={() => navigate("/player-dashboard")}
        >
          Back
        </button>
      </div>

      <div className="max-w-2xl mx-auto mt-10">
        {history.length === 0 ? (
          <p className="text-center text-gray-600 text-lg mt-20">
            No history recorded yet.
          </p>
        ) : (
          history.map((board, idx) => (
            <div
              key={idx}
              className="bg-white rounded-xl shadow p-6 mb-4 flex justify-between"
            >
              <div>
                <p className="font-bold text-red-600">
                  Week {board.week} – 2025
                </p>
                <p>Numbers: {board.numbers.join(", ")}</p>
              </div>
              <p className="font-bold">{board.price} DKK</p>
            </div>
          ))
        )}
      </div>
    </div>
  );
}
