import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

interface PlayedBoard {
  week: number;
  numbers: number[];
  price: number;
}

export default function PlayerBalance() {
  const navigate = useNavigate();

  const [email, setEmail] = useState<string | null>(null);
  const [balance, setBalance] = useState<number>(0);
  const [history, setHistory] = useState<PlayedBoard[]>([]);
  const [totalSpent, setTotalSpent] = useState<number>(0);

  useEffect(() => {
    const savedEmail = localStorage.getItem("playerEmail");
    const auth = localStorage.getItem("playerAuthenticated");

    if (!auth || !savedEmail) {
      navigate("/player-login");
      return;
    }

    setEmail(savedEmail);

    const userBalance = Number(localStorage.getItem(`${savedEmail}_balance`) || 0);
    setBalance(userBalance);

    const boards = JSON.parse(localStorage.getItem("boardHistory") || "[]");
    setHistory(boards);

    const spent = boards.reduce((sum: number, b: PlayedBoard) => sum + b.price, 0);
    setTotalSpent(spent);
  }, []);

  const initialAmount = balance + totalSpent;

  if (!email) return null;

  return (
    <div className="min-h-screen bg-[#faf6ef] p-8">
      <h1 className="text-3xl font-bold text-red-600 text-center mb-6">
        Player Balance
      </h1>

      <div className="max-w-2xl mx-auto bg-white shadow-xl rounded-xl p-8">
        <p className="text-lg mb-2">
          <span className="font-bold">Player:</span> {email}
        </p>

        <p className="text-lg">
          <span className="font-bold">Initial Amount:</span> {initialAmount} DKK
        </p>

        <p className="text-lg mt-2">
          <span className="font-bold">Total Spent:</span> {totalSpent} DKK
        </p>

        <p className="text-lg mt-2">
          <span className="font-bold">Current Balance:</span> {balance} DKK
        </p>

        <button
          onClick={() => navigate(-1)}
          className="btn bg-red-600 text-white mt-6 px-6 hover:bg-red-700"
        >
          Back
        </button>
      </div>

      <h2 className="text-2xl font-bold text-red-600 text-center mt-10 mb-4">
        Purchase History
      </h2>

      <div className="max-w-2xl mx-auto bg-white shadow-md rounded-xl p-6">
        {history.length === 0 && (
          <p className="text-center text-gray-500">No boards purchased yet.</p>
        )}

        {history.map((b, idx) => (
          <div key={idx} className="border-b py-3">
            <p className="text-lg font-semibold">Week {b.week}</p>
            <p className="text-gray-700">Numbers: {b.numbers.join(", ")}</p>
            <p className="text-gray-700">Paid: {b.price} DKK</p>
          </div>
        ))}
      </div>
    </div>
  );
}
