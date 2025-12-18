import { useState, useEffect } from "react";
import Modal from "../../../components/Modal";
import Input from "../../../components/Input";
import Toggle from "../../../components/Toggle";
import PasswordInput from "../../../components/PasswordInput";
import { getPlayers, createPlayer, updatePlayer, deletePlayer } from "../../../core/playersApi";


interface Player {
    playerId: string;
    name: string;
    email: string;
    phone: string;
    active: boolean;
}

export default function PlayersTab() {
    const API_BASE = import.meta.env.VITE_API_URL;

    const [players, setPlayers] = useState<Player[]>([]);
    const [selectedPlayer, setSelectedPlayer] = useState<Player | null>(null);

    const [showAddModal, setShowAddModal] = useState(false);
    const [showEditModal, setShowEditModal] = useState(false);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [password, setPassword] = useState("");
    const [balances, setBalances] = useState<Record<string, number>>({});


    useEffect(() => {
        getPlayers()
            .then(setPlayers)
            .catch((err) => {
                console.error(err);
                setPlayers([]);
            });
    }, []);

    useEffect(() => {
        if (players.length === 0) return;
        
        players.forEach((player) => {
            loadBalanceForPlayer(player.playerId).catch(console.error);
        });
    }, [players]);
    
    async function handleAddPlayer(e: React.FormEvent) {
        e.preventDefault();
        const form = e.currentTarget as HTMLFormElement;
        const data = new FormData(form);

        const newPlayer = {
            name: String(data.get("name") ?? ""),
            email: String(data.get("email") ?? ""),
            phone: String(data.get("phone") ?? ""),
            password, // required by backend
            active: data.get("active") === "true",
        };

        try {
            const created = await createPlayer(newPlayer);
            setPlayers((prev) => [...prev, created]);
            setPassword("");
            setShowAddModal(false);
            form.reset();
        } catch (err: any) {
            alert(err.message ?? "Failed to create player");
        }
    }
    
    async function handleEditPlayer(e: React.FormEvent) {
        e.preventDefault();
        if (!selectedPlayer) return;

        const form = e.currentTarget as HTMLFormElement;
        const data = new FormData(form);

        const updatedPlayer = {
            name: String(data.get("name") ?? ""),
            email: String(data.get("email") ?? ""),
            phone: String(data.get("phone") ?? ""),
            active: data.get("active") === "on"
        };

        try {
            const saved = await updatePlayer(selectedPlayer.playerId, updatedPlayer);
            setPlayers((prev) => prev.map((p) => (p.playerId === saved.playerId ? saved : p)));
            setShowEditModal(false);
        } catch (err: any) {
            alert(err.message ?? "Failed to update player");
        }
    }
    
    async function handleDeletePlayer() {
        if (!selectedPlayer) return;

        try {
            await deletePlayer(selectedPlayer.playerId);
            setPlayers((prev) => prev.filter((p) => p.playerId !== selectedPlayer.playerId));
            setShowDeleteModal(false);
        } catch (err: any) {
            alert(err.message ?? "Failed to delete player");
        }
    }
    
    async function loadBalanceForPlayer(playerId: string) {
        const token = localStorage.getItem("token");
        const res = await fetch(`${API_BASE}/api/Transaction/admin/player/${playerId}/balance`, {
            headers: { Authorization: `Bearer ${token}` },
        });
        if (!res.ok) throw new Error("Failed to load balance");
        const b = await res.json();
        setBalances(prev => ({ ...prev, [playerId]: b }));
    }

    return (
        <div className="max-w-4xl mx-auto mt-10">
            <div className="flex justify-between items-center mb-4">
                <h2 className="text-xl font-bold text-red-600">Player Management</h2>

                <button
                    className="btn bg-red-600 text-white hover:bg-red-700 px-6"
                    onClick={() => setShowAddModal(true)}
                >
                    + Add Player
                </button>
            </div>

            <div className="bg-white shadow rounded-xl p-6">
                {players.length === 0 ? (
                    <p className="text-black">No players registered yet.</p>
                ) : (
                    <ul className="text-black space-y-4">
                        {players.map((player) => (
                            <li
                                key={player.playerId}
                                className="p-4 bg-[#faf6ef] rounded-xl shadow flex justify-between"
                            >
                                <div>
                                    <strong>{player.name}</strong>
                                    <p>{player.email}</p>
                                    <p>{player.phone}</p>
                                    <p>Balance: {balances[player.playerId] ?? "-"} DKK</p>
                                    <span
                                        className={`inline-block mt-2 px-2 py-1 text-xs rounded ${
                                            player.active
                                                ? "bg-green-200 text-green-800"
                                                : "bg-red-200 text-red-800"
                                        }`}
                                    >
                                        {player.active ? "Active" : "Inactive"}
                                    </span>
                                </div>

                                <div className="flex gap-3">
                                    <button
                                        className="btn btn-outline border-blue-600 text-blue-600"
                                        onClick={() => {
                                            setSelectedPlayer(player);
                                            setShowEditModal(true);
                                        }}
                                    >
                                        Edit
                                    </button>

                                    <button
                                        className="btn btn-outline border-red-600 text-red-600"
                                        onClick={() => {
                                            setSelectedPlayer(player);
                                            setShowDeleteModal(true);
                                        }}
                                    >
                                        Delete
                                    </button>
                                </div>
                            </li>
                        ))}
                    </ul>
                )}
            </div>

            {}
            {showAddModal && (
                <Modal onClose={() => setShowAddModal(false)}>
                    <h3 className="text-lg font-bold mb-3 text-red-600">Add New Player</h3>

                    <form onSubmit={handleAddPlayer} className="space-y-4 text-black">
                        <Input name="name" label="Full Name" required className="bg-gray-300"/>
                        <Input name="email" label="Email" type="email" required className="bg-gray-300"/>
                        <Input name="phone" label="Phone" required className="bg-gray-300"/>
                        <PasswordInput
                            className="bg-gray-300"
                            value={password}
                            onChange={setPassword}
                        />
                        <Toggle name="active" label="Active Player"/>

                        <button className="btn bg-red-600 text-white hover:bg-red-700 w-full mt-4">
                            Add Player
                        </button>
                    </form>
                </Modal>
            )}

            {showEditModal && selectedPlayer && (
                <Modal onClose={() => setShowEditModal(false)}>
                    <h3 className="text-lg font-bold mb-3 text-blue-600">
                        Edit Player
                    </h3>

                    <form onSubmit={handleEditPlayer} className="space-y-4 text-black">
                        <Input name="name" defaultValue={selectedPlayer.name} label={""} className="bg-gray-300"/>
                        <Input name="email" defaultValue={selectedPlayer.email} label={""} className="bg-gray-300"/>
                        <Input name="phone" defaultValue={selectedPlayer.phone} label={""} className="bg-gray-300"/>
                        <Toggle
                            name="active"
                            label="Active Player"
                            defaultChecked={selectedPlayer.active}
                        />

                        <button className="btn bg-blue-600 text-white hover:bg-blue-700 w-full">
                            Save Changes
                        </button>
                    </form>
                </Modal>
            )}

            {showDeleteModal && selectedPlayer && (
                <Modal onClose={() => setShowDeleteModal(false)}>
                    <h3 className="font-bold text-lg text-red-600">Delete Player</h3>
                    <p className="mb-4 text-black">
                        Are you sure you want to delete{" "}
                        <strong>{selectedPlayer.name}</strong>?
                    </p>

                    <div className="flex justify-end gap-4">
                        <button
                            className="btn text-black"
                            onClick={() => setShowDeleteModal(false)}
                        >
                            Cancel
                        </button>
                        <button
                            className="btn bg-red-600 text-white hover:bg-red-700"
                            onClick={handleDeletePlayer}
                        >
                            Delete
                        </button>
                    </div>
                </Modal>
            )}
        </div>
    );
}
