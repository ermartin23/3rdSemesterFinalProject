import { atom } from "jotai";
import { Player } from "../types/player";

export const playersAtom = atom<Player[]>([]);
