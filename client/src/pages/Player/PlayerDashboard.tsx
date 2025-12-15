import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import logo from "../../assets/jerne-if-logo.png";

interface PlayedBoard {
    week: number;
    numbers: number[];
    price: number;
}

export default function PlayerDashboard() {
    const navigate = useNavigate();

    const [email, setEmail] = useState<string | null>(null);
    const [balance, setBalance] = useState<number>(0);
    const [selectedNumbers, setSelectedNumbers] = useState<number[]>([]);
    const [currentWeek, setCurrentWeek] = useState<number>(0);
    const [lastBoard, setLastBoard] = useState<PlayedBoard | null>(null);

    useEffect(() => {
        const auth = localStorage.getItem("playerAuthenticated");
        const savedEmail = localStorage.getItem("playerEmail");
        const savedBoard = localStorage.getItem("lastBoard");

        if (!auth || !savedEmail) {
            navigate("/player-login");
            return;
        }

        setEmail(savedEmail);

        const perUser = localStorage.getItem(`${savedEmail}_balance`);
        const global = localStorage.getItem("playerBalance");

        const finalBalance =
            perUser !== null
                ? Number(perUser)
                : global !== null
                    ? Number(global)
                    : 200;

        setBalance(finalBalance);

        if (savedBoard) {
            setLastBoard(JSON.parse(savedBoard));
        }

        const week = getWeekNumber(new Date());
        setCurrentWeek(week);
    }, []);

    const prices: Record<number, number> = {
        5: 20,
        6: 40,
        7: 80,
        8: 160,
    };

    const price = prices[selectedNumbers.length as keyof typeof prices] || 0;

    function toggleNumber(num: number) {
        if (selectedNumbers.includes(num)) {
            setSelectedNumbers(selectedNumbers.filter((n) => n !== num));
            return;
        }
        if (selectedNumbers.length >= 8) return;
        setSelectedNumbers([...selectedNumbers, num]);
    }

    function handlePlay() {
        if (selectedNumbers.length < 5) {
            alert("You must choose at least 5 numbers.");
            return;
        }

        if (balance < price) {
            alert("Not enough balance to play this board.");
            return;
        }

        const newBalance = balance - price;
        setBalance(newBalance);

        if (email) {
            localStorage.setItem(`${email}_balance`, String(newBalance));
        }
        localStorage.setItem("playerBalance", String(newBalance));

        const board: PlayedBoard = {
            week: currentWeek,
            numbers: selectedNumbers,
            price,
        };

        localStorage.setItem("lastBoard", JSON.stringify(board));
        setLastBoard(board);

        const previous = JSON.parse(localStorage.getItem("boardHistory") || "[]");
        localStorage.setItem("boardHistory", JSON.stringify([...previous, board]));

        alert("Board submitted successfully!");
    }

    if (!email) return null;

    return (
        <div className="min-h-screen bg-[#faf6ef]">
            {/* HEADER */}
            <div className="flex items-center justify-between px-8 py-4 shadow bg-[#faf6ef]">
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
                        localStorage.removeItem("playerAuthenticated");
                        localStorage.removeItem("playerEmail");
                        navigate("/player-login");
                    }}
                >
                    Logout
                </button>
            </div>

            {/* NAV BUTTONS */}
            <div className="flex justify-center gap-6 mt-6 text-lg">
                <button
                    onClick={() => navigate("/player-dashboard")}
                    className="text-red-600 font-semibold underline"
                >
                    Play
                </button>

                <button
                    onClick={() => navigate("/player-history")}
                    className="text-gray-600 hover:text-red-600"
                >
                    View History
                </button>

                {/*  NEW BUTTON: Winning Number */}
                <button
                    onClick={() =>
                        navigate("/player-winners", {
                            state: {
                                week: currentWeek,
                                numbers: [4, 9, 12]   // TEMPORARY → placeholder until backend is connected
                            }
                        })
                    }
                    className="text-gray-600 hover:text-red-600"
                >
                    Winning Number
                </button>
                
                <button
                    onClick={() => navigate("/player-balance")}
                    className="text-gray-600 hover:text-red-600"
                >
                    Balance
                </button>

                <button
                    onClick={() => navigate("/player-transactions")}
                    className="text-gray-600 hover:text-red-600"
                >
                    Transactions
                </button>

                <button
                    onClick={() => navigate(-1)}
                    className="text-gray-600 hover:text-red-600"
                >
                    Back
                </button>
            </div>

            {/* USER INFO */}
            <div className="text-center mt-8">
                <h2 className="text-3xl font-bold text-red-600">
                    Welcome {email.split("@")[0]}!
                </h2>
                <p className="text-gray-600 mt-2">
                    Balance: <span className="font-bold">{balance} DKK</span>
                </p>
            </div>

            <h3 className="text-center text-xl font-semibold mt-8">
                Current Game – Week {currentWeek} – 2025
            </h3>

            {/* NUMBER GRID */}
            <div className="grid grid-cols-4 gap-4 max-w-xl mx-auto mt-8">
                {Array.from({ length: 16 }, (_, i) => i + 1).map((num) => {
                    const isSelected = selectedNumbers.includes(num);
                    return (
                        <button
                            key={num}
                            onClick={() => toggleNumber(num)}
                            className={`p-6 border rounded-xl text-lg ${
                                isSelected
                                    ? "bg-orange-400 text-white"
                                    : "bg-[#e8dfcf]"
                            }`}
                        >
                            {num}
                        </button>
                    );
                })}
            </div>

            <p className="text-center text-lg font-semibold mt-6">Price: {price} DKK</p>

            <div className="text-center mt-4">
                <button
                    onClick={handlePlay}
                    className="btn bg-red-600 text-white hover:bg-red-700 px-10"
                >
                    Play
                </button>
            </div>

            {/* LAST PLAYED BOARD – NEW WIDE RECTANGLE */}
            {lastBoard && (
                <div className="mt-12 flex justify-center">
                    <div className="bg-white rounded-xl shadow-xl w-full max-w-2xl p-10 text-center">
                        <h3 className="text-xl font-bold text-red-600 mb-4">
                            Last Played Board
                        </h3>

                        <p className="text-lg">Week: {lastBoard.week} — 2025</p>
                        <p className="text-lg mt-1">
                            Numbers: {lastBoard.numbers.join(", ")}
                        </p>
                        <p className="text-lg mt-1">Price Paid: {lastBoard.price} DKK</p>

                        <button
                            className="btn bg-red-600 text-white mt-6 px-8 py-2 hover:bg-red-700"
                            onClick={() => setSelectedNumbers(lastBoard.numbers)}
                        >
                            Repeat This Week
                        </button>
                    </div>
                </div>
            )}

            <p className="text-center text-gray-500 text-xs mt-6">
                This board is valid for the current week
            </p>
        </div>
    );
}

function getWeekNumber(date: Date) {
    const start = new Date(date.getFullYear(), 0, 1);
    const diff =
        (date.getTime() - start.getTime()) / (1000 * 60 * 60 * 24);
    return Math.ceil((diff + start.getDay() + 1) / 7);
}
