import { useAtom } from "jotai";
import { gamesAtom } from "../state/gamesAtom";
import { GameClient } from "../core/generated-client";
import { baseUrl } from "../core/config";


export function useGame() {
    const [games, setGames] = useAtom(gamesAtom);
    const client = new GameClient(baseUrl);

    async function loadGames() {
        const data = await client.getAll();
        setGames(data);
    }

    async function createGame(weekidentity: string) {
        const newGame = await client.create({ weekidentity });
        setGames((prev) => [...prev, newGame]);
    }

    return {
        games,
        loadGames,
        createGame,
    };
}
