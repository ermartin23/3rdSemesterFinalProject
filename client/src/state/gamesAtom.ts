import { atom } from "jotai";
import type {Game} from "../types/game";

export const gamesAtom = atom<Game[]>([]);
