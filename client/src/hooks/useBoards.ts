import { useAtom } from "jotai";
import { boardsAtom } from "../state/boardsAtom";
import { BoardClient } from "../core/generated-client";
import { baseUrl } from "../core/config";

export function useBoards() {
    const [boards, setBoards] = useAtom(boardsAtom);
    const client = new BoardClient(baseUrl);

    async function loadBoards() {
        const data = await client.getAllBoards();
        setBoards(data);
    }

    // ✅ No playerId here (API contract doesn't accept it)
    async function createBoard(gameId: string, chosenNumbers: number[], repeatingBoardId?: string) {
        const newBoard = await client.createBoard({
            gameId,
            chosenNumbers,
            repeatingBoardId,
        });

        setBoards((prev) => [...prev, newBoard]);
    }

    return {
        boards,
        loadBoards,
        createBoard,
    };
}
