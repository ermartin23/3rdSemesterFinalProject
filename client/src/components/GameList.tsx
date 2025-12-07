import React from "react";
import { useGames } from "../hooks/useGames";

export default function GameList() {
    const { games, loading, error } = useGames();

    if (loading) return <p>Loading games...</p>;
    if (error) return <p>Error: {error}</p>;

    if (games.length === 0) {
        return <p>No games found.</p>;
    }

    return (
        <div style={{ padding: "20px" }}>
            <h2>Games</h2>
            <ul>
                {games.map((g) => (
                    <li key={g.gameid}>
                        <strong>ID:</strong> {g.gameid}<br />
                        <strong>Week:</strong> {g.weekidentity}<br />
                        <strong>Winning Numbers:</strong>{" "}
                        {g.winningnumbers ? g.winningnumbers.join(", ") : "Not set"}
                        <br />
                        <hr />
                    </li>
                ))}
            </ul>
        </div>
    );
}

