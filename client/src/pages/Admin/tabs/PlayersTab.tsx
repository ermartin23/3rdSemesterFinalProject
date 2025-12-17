import { useState, useEffect } from "react";
import Modal from "../../../components/Modal";
import Input from "../../../components/Input";
import Toggle from "../../../components/Toggle";
import PasswordInput from "../../../components/PasswordInput";



interface Player {
    playerId: string;
    name: string;
    email: string;
    phone: string;
    active: boolean;
    balance: number;
}

export default function PlayersTab() {
    const API = "http://127.0.0.1:5239/api/players";

    const [players, setPlayers] = useState<Player[]>([]);
    const [selectedPlayer, setSelectedPlayer] = useState<Player | null>(null);

    const [showAddModal, setShowAddModal] = useState(false);
    const [showEditModal, setShowEditModal] = useState(false);
    const [showDeleteModal, setShowDeleteModal] = useState(false);

    useEffect(() => {
        async function loadPlayers() {
            const token = localStorage.getItem("token");
            
            const res = await fetch(API, {
                headers: {
                    Accept: "application/json",
                    Authorization: `Bearer ${token}`,
                },
            });
            
            if (!res.ok) {
                const text = await res.text().catch(() => "");
                throw new Error(text || `Failed to load players (${res.status})`);
            }
            
            const data = await res.json();
            setPlayers(data);
        }
        
        loadPlayers().catch((err) => {
            console.error(err);
            setPlayers([]);
        });
    }, []);

    // ADD PLAYER
    async function handleAddPlayer(e: React.FormEvent) {
        e.preventDefault();
        const form = e.currentTarget as HTMLFormElement;
        const data = new FormData(form);

        const newPlayer = {
            name: data.get("name"),
            email: data.get("email"),
            phone: data.get("phone"),
            active: data.get("active") === "on",
            password: data.get("password") // required by backend
        };

        const res = await fetch(API, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(newPlayer),
        });

        const created = await res.json();
        setPlayers((prev) => [...prev, created]);

        setShowAddModal(false);
        form.reset();
    }

    // EDIT PLAYER
    async function handleEditPlayer(e: React.FormEvent) {
        e.preventDefault();
        if (!selectedPlayer) return;

        const form = e.currentTarget as HTMLFormElement;
        const data = new FormData(form);

        const updatedPlayer = {
            name: data.get("name"),
            email: data.get("email"),
            phone: data.get("phone"),
            active: data.get("active") === "on"
        };

        const res = await fetch(`${API}/${selectedPlayer.playerId}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(updatedPlayer),
        });

        const saved = await res.json();
        setPlayers((prev) =>
            prev.map((p) => (p.playerId === saved.playerId ? saved : p))
        );

        setShowEditModal(false);
    }

    // DELETE PLAYER
    async function handleDeletePlayer() {
        if (!selectedPlayer) return;

        await fetch(`${API}/${selectedPlayer.playerId}`, {
            method: "DELETE",
        });

        setPlayers((prev) =>
            prev.filter((p) => p.playerId !== selectedPlayer.playerId)
        );

        setShowDeleteModal(false);
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
                                    <p>Balance: {player.balance ?? 0} DKK</p>
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

            {/* ADD PLAYER MODAL */}
            {showAddModal && (
                <Modal onClose={() => setShowAddModal(false)}>
                    <h3 className="text-lg font-bold mb-3 text-red-600">Add New Player</h3>

                    <form onSubmit={handleAddPlayer} className="space-y-4 text-black">
                        <Input name="name" label="Full Name" required className="bg-gray-300"/>
                        <Input name="email" label="Email" type="email" required className="bg-gray-300"/>
                        <Input name="phone" label="Phone" required className="bg-gray-300"/>
                        <PasswordInput className="bg-gray-300"/>
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
