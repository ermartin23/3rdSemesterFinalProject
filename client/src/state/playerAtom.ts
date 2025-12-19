import { atom } from "jotai";
import type { Player } from "../types/player";


export const playersAtom = atom<Player[]>([]);
