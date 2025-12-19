import { atom } from "jotai";
import type { Board } from "../core/generated-client";

export const boardsAtom = atom<Board[]>([]);

export const winningBoardsAtom = atom((get) =>
    get(boardsAtom).filter((b) => b.iswinningboard)
);
