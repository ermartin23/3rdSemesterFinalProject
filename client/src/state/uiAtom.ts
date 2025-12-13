import { atom } from "jotai";

export const loadingAtom = atom(false);
export const errorAtom = atom<string | null>(null);
export const successAtom = atom<string | null>(null);
