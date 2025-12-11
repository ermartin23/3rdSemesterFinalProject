import { atom } from "jotai";
import { Board } from "../types/board";

export const boardsAtom = atom<Board[]>([]);

export const winningBoardsAtom = atom((get) =>
    get(boardsAtom).filter((b) => b.iswinningboard)
);
