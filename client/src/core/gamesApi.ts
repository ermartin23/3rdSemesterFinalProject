import { api } from "./api";

export type GameResponseDto = {
    gameid: string;
    weekidentity: string;
    createdat: string;
    cutofftime: string;
    winningnumbers: number[] | null;
    isOpen: boolean;

    cutoffUtc: string;
    canSetWinnersNow: boolean;
};

export type GameSetWinnersDto = {
    winningNumbers: number[];
};

export type GameBoardSummaryDto = {
    boardId: string;
    playerId: string;
    chosenNumbers: number[];
    price: number;
    isWinningBoard: boolean;
};

export type GamePlayerBoardsDto = {
    playerId: string;
    name: string;
    phone: string;
    email: string;
    active: boolean;
    boards: GameBoardSummaryDto[];
};

export type GameDetailsResponseDto = {
    gameId: string;
    weekIdentity: string;
    createdAt: string;
    cutoffTime: string;
    winningNumbers: number[] | null;
    isOpen: boolean;
    totalWinningBoards: number;
    players: GamePlayerBoardsDto[];
};

export function getGames() {
    return api<GameResponseDto[]>("/api/games");
}

export function getGameDetails(gameId: string) {
    return api<GameDetailsResponseDto>(`/api/games/${gameId}/details`);
}

export function setGameWinners(gameId: string, dto: GameSetWinnersDto) {
    return api<GameResponseDto>(`/api/games/${gameId}/winners`, {
        method: "POST",
        body: JSON.stringify(dto),
    });
}
