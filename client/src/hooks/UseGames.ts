import { useEffect, useState } from "react";

export interface Game {
    gameid: string;
    weekidentity: string;
    winningnumbers: number[] | null;
    createdat: string;
    cutofftime: string;
    isOpen: boolean;
}

export function useGames() {
    const [games, setGames] = useState<Game[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        async function fetchGames() {
            try {
                const res = await fetch("http://localhost:5239/api/games");

                if (!res.ok) {
                    throw new Error("Failed to fetch games");
                }

                const data = await res.json();
                setGames(data);
            } catch (err: any) {
                setError(err.message);
            } finally {
                setLoading(false);
            }
        }

        fetchGames();
    }, []);

    return { games, loading, error };
}
