import React, { useState, useEffect } from "react";
import logo from "../../assets/jerne-if-logo.png";
import type { JSX } from "react/jsx-runtime";
import TransactionsTab from "./tabs/TransactionsTab.tsx";

interface Player {
    playerId: string;
    name: string;
    email: string;
    phone: string;
    active: boolean;
    balance: number;
}

interface Game {
    id: string;
    week: number;
    deadline: string;
    winningNumbers: number[];
    revenue: number;
}

export default function AdminDashboard() {
    const [activeTab, setActiveTab] = useState<"players" | "transactions" | "games">("players");

    return (
        <div className="min-h-screen bg-[#faf6ef]">
            <div className="flex items-center justify-between px-8 py-4 bg-[#faf6ef] shadow-sm">
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

            <div className="flex justify-center mt-6">
                <div className="tabs tabs-boxed bg-[#f7f2e9]">
                    <a className={`tab ${activeTab === "players" ? "tab-active" : ""}`} onClick={() => setActiveTab("players")}>Players</a>
                    <a className={`tab ${activeTab === "transactions" ? "tab-active" : ""}`} onClick={() => setActiveTab("transactions")}>Transactions</a>
                    <a className={`tab ${activeTab === "games" ? "tab-active" : ""}`} onClick={() => setActiveTab("games")}>Games</a>
                </div>
            </div>

            {activeTab === "players" && <PlayersTab />}
            {activeTab === "transactions" && <TransactionsTab />}
            {activeTab === "games" && <GamesTab />}
        </div>
    );
}

function PlayersTab() {
    const [players, setPlayers] = useState<Player[]>([]);
    const [selectedPlayer, setSelectedPlayer] = useState<Player | null>(null);
    const [showAddModal, setShowAddModal] = useState(false);
    const [showEditModal, setShowEditModal] = useState(false);
    const [showDeleteModal, setShowDeleteModal] = useState(false);

    const API = "http://127.0.0.1:5239/api/players";

    useEffect(() => {
        fetch(API)
            .then(res => res.json())
            .then(data => setPlayers(data));
    }, []);

    async function handleAddPlayer(e: React.FormEvent) {
        e.preventDefault();
        const form = e.currentTarget as HTMLFormElement;
        const data = new FormData(form);

        const newPlayer = {
            name: String(data.get("name")),
            email: String(data.get("email")),
            phone: String(data.get("phone")),
            active: data.get("active") === "on"
        };

        const res = await fetch(API, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(newPlayer)
        });

        const created = await res.json();
        setPlayers(prev => [...prev, created]);

        setShowAddModal(false);
        form.reset();
    }

    async function handleEditPlayer(e: React.FormEvent) {
        e.preventDefault();
        if (!selectedPlayer) return;

        const form = e.currentTarget as HTMLFormElement;
        const data = new FormData(form);

        const updated = {
            playerId: selectedPlayer.playerId,
            name: String(data.get("name")),
            email: String(data.get("email")),
            phone: String(data.get("phone")),
            active: data.get("active") === "on"
        };

        const res = await fetch(`${API}/${selectedPlayer.playerId}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(updated)
        });

        const saved = await res.json();

        setPlayers(prev => prev.map(p => (p.playerId === saved.playerId ? saved : p)));
        setShowEditModal(false);
    }

    async function handleDeletePlayer() {
        if (!selectedPlayer) return;

        await fetch(`${API}/${selectedPlayer.playerId}`, { method: "DELETE" });

        setPlayers(prev => prev.filter(p => p.playerId !== selectedPlayer.playerId));
        setShowDeleteModal(false);
    }

    return (
        <div className="max-w-5xl mx-auto mt-10">
            <div className="flex justify-between items-center mb-4">
                <h2 className="text-xl font-bold text-red-600">Player Management</h2>
                <button onClick={() => setShowAddModal(true)} className="btn bg-red-600 text-white hover:bg-red-700 px-6 py-3 text-lg rounded-lg">
                    + Add Player
                </button>
            </div>

            <div className="bg-white border rounded-xl shadow p-6 text-gray-600">
                {players.length === 0 ? (
                    <p className="text-center">No players registered yet.</p>
                ) : (
                    <ul className="space-y-3">
                        {players.map(player => (
                            <li key={player.playerId} className="p-4 bg-[#faf6ef] rounded-xl shadow flex justify-between items-center">
                                <span className="flex flex-col">
                                    <strong>{player.name}</strong>
                                    <span>{player.email}</span>
                                    <span>{player.phone}</span>
                                    <span>Balance: {player.balance ?? 0} DKK</span>
                                    <span className={`mt-1 inline-block px-2 py-1 rounded text-xs font-semibold ${player.active ? "bg-green-200 text-green-800" : "bg-red-200 text-red-800"}`}>
                                        {player.active ? "Active" : "Inactive"}
                                    </span>
                                </span>

                                <div className="flex gap-3">
                                    <button className="btn btn-sm btn-outline border-blue-600 text-blue-600" onClick={() => { setSelectedPlayer(player); setShowEditModal(true); }}>
                                        Edit
                                    </button>
                                    <button className="btn btn-sm btn-outline border-red-600 text-red-600" onClick={() => { setSelectedPlayer(player); setShowDeleteModal(true); }}>
                                        Delete
                                    </button>
                                </div>
                            </li>
                        ))}
                    </ul>
                )}
            </div>

            {showAddModal && (
                <Modal onClose={() => setShowAddModal(false)}>
                    <h3 className="font-bold text-lg text-red-600 mb-4">Add New Player</h3>
                    <form onSubmit={handleAddPlayer} className="space-y-4">
                        <Input name="name" label="Full Name" required />
                        <Input name="email" label="Email" type="email" required />
                        <Input name="phone" label="Phone" />
                        <Toggle name="active" label="Active Player" />
                        <button className="btn bg-red-600 text-white hover:bg-red-700 px-6 py-3 text-lg rounded-lg">+ Add Player</button>
                    </form>
                </Modal>
            )}

            {showEditModal && selectedPlayer && (
                <Modal onClose={() => setShowEditModal(false)}>
                    <h3 className="font-bold text-lg text-blue-600 mb-4">Edit Player</h3>
                    <form onSubmit={handleEditPlayer} className="space-y-4">
                        <Input name="name" label="Full Name" defaultValue={selectedPlayer.name} required />
                        <Input name="email" label="Email" type="email" defaultValue={selectedPlayer.email} required />
                        <Input name="phone" label="Phone" defaultValue={selectedPlayer.phone} />
                        <Toggle name="active" label="Active Player" defaultChecked={selectedPlayer.active} />
                        <button className="btn bg-blue-600 text-white hover:bg-blue-700 w-full mt-2">Save Changes</button>
                    </form>
                </Modal>
            )}

            {showDeleteModal && selectedPlayer && (
                <Modal onClose={() => setShowDeleteModal(false)}>
                    <h3 className="text-lg font-bold text-red-600 mb-4">Delete Player</h3>
                    <p className="mb-6">Are you sure you want to delete <strong>{selectedPlayer.name}</strong>?</p>

                    <div className="flex justify-end gap-3">
                        <button className="btn" onClick={() => setShowDeleteModal(false)}>Cancel</button>
                        <button className="btn bg-red-600 text-white hover:bg-red-700" onClick={handleDeletePlayer}>Delete</button>
                    </div>
                </Modal>
            )}
        </div>
    );
}

function GamesTab() {
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
                winningNumbers: selected.filter(x => x !== n)
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

        setHistory(prev => [...prev, activeGame]);

        setActiveGame({
            id: crypto.randomUUID(),
            week: activeGame.week + 1,
            deadline: "—",
            winningNumbers: [],
            revenue: 0
        });
    }

    return (
        <div className="max-w-5xl mx-auto mt-10">
            <div className="p-6 border border-red-300 rounded-xl bg-white shadow">
                <div className="flex items-center gap-3 mb-4">
                    <span className="font-bold text-lg text-red-600">Active Game – Week {activeGame.week}</span>
                    <span className="px-2 py-1 bg-red-500 text-white rounded text-sm">Active</span>
                </div>

                <p className="text-gray-600 mb-3">Deadline: <strong>{activeGame.deadline}</strong></p>

                <div className="bg-[#f7ead4] p-4 rounded-lg mb-6">
                    <p>Current Boards: <strong>0</strong></p>
                    <p>Total Revenue: <strong>0 DKK</strong></p>
                </div>

                <div className="p-4 bg-[#fff4ef] border rounded-xl mb-6">
                    <p className="font-bold mb-4">Select 3 Winning Numbers</p>

                    <div className="grid grid-cols-8 gap-3">
                        {Array.from({ length: 16 }, (_, i) => i + 1).map(num => (
                            <button
                                key={num}
                                onClick={() => toggleNumber(num)}
                                className={`p-4 rounded-xl text-center border ${
                                    activeGame.winningNumbers.includes(num)
                                        ? "bg-red-500 text-white border-red-700"
                                        : "bg-[#f7ead4] border-[#e8d8bd] hover:bg-[#f3e3c7]"
                                }`}
                            >
                                {num}
                            </button>
                        ))}
                    </div>

                    <p className="mt-3 text-sm text-gray-600">Selected: {activeGame.winningNumbers.length}/3</p>

                    <button
                        disabled={activeGame.winningNumbers.length !== 3}
                        onClick={closeGame}
                        className="btn mt-4 bg-red-600 text-white hover:bg-red-700 disabled:opacity-40"
                    >
                        Close Game & Set Winners
                    </button>
                </div>
            </div>

            <div className="mt-10 mb-20">
                <h2 className="text-xl font-bold text-red-600 mb-4">Game History</h2>

                <div className="bg-white border rounded-xl shadow p-6">
                    {history.length === 0 ? (
                        <p className="text-center text-gray-600">No closed games yet</p>
                    ) : (
                        <ul className="space-y-3">
                            {history.map(g => (
                                <li key={g.id} className="p-4 rounded-xl bg-[#faf6ef] shadow flex justify-between items-center">
                                    <span>
                                        <strong>Week {g.week}</strong> — Winners: {g.winningNumbers.join(", ")}
                                    </span>
                                    <span className="text-gray-600">{g.revenue} DKK</span>
                                </li>
                            ))}
                        </ul>
                    )}
                </div>
            </div>
        </div>
    );
}

function Modal({ children, onClose }: { children: React.ReactNode; onClose: () => void }) {
    return (
        <>
            <div className="fixed inset-0 bg-black bg-opacity-50 backdrop-blur-sm z-40" onClick={onClose}></div>

            <div className="fixed inset-0 flex items-center justify-center z-50">
                <div className="bg-white p-8 rounded-xl shadow-xl w-full max-w-md relative">
                    <button className="absolute top-3 right-3 text-gray-500 hover:text-red-600" onClick={onClose}>✕</button>
                    {children}
                </div>
            </div>
        </>
    );
}

function Input({
                   name,
                   label,
                   type = "text",
                   defaultValue,
                   required
               }: {
    name: string;
    label: string;
    type?: string;
    defaultValue?: string;
    required?: boolean;
}): JSX.Element {
    return (
        <div>
            <label className="block mb-1 text-sm font-medium">{label}</label>
            <input
                name={name}
                type={type}
                defaultValue={defaultValue}
                required={required}
                className="input input-bordered w-full"
            />
        </div>
    );
}

function Toggle({
                    name,
                    label,
                    defaultChecked
                }: {
    name: string;
    label: string;
    defaultChecked?: boolean;
}): JSX.Element {
    return (
        <label className="flex items-center gap-2">
            <input type="checkbox" name={name} className="toggle" defaultChecked={defaultChecked} />
            <span>{label}</span>
        </label>
    );
}

function getWeekNumber(date: Date) {
    const d = new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));
    const dayNum = d.getUTCDay() || 7;
    d.setUTCDate(d.getUTCDate() + 4 - dayNum);
    const yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
    return Math.ceil(((d.getTime() - yearStart.getTime()) / 86400000 + 1) / 7);
}