import { api } from "./api";

export type PlayerCreateRequestDto = {
    name: string;
    email: string;
    phone: string;
    password: string;
};

export type PlayerUpdateRequestDto = {
    name: string;
    email: string;
    phone: string;
    password?: string; // optional on update
};

export type PlayerResponseDto = {
    playerId: string;
    name: string;
    email: string;
    phone: string;
    active: boolean;
    balance?: number;
};

export function getPlayers() {
    return api<PlayerResponseDto[]>("/api/players");
}

export function createPlayer(dto: PlayerCreateRequestDto) {
    return api<PlayerResponseDto>("/api/players", {
        method: "POST",
        body: JSON.stringify(dto),
    });
}

export function updatePlayer(id: string, dto: PlayerUpdateRequestDto) {
    return api<PlayerResponseDto>(`/api/players/${id}`, {
        method: "PUT",
        body: JSON.stringify(dto),
    });
}

export function deletePlayer(id: string) {
    return api<void>(`/api/players/${id}`, { method: "DELETE" });
}



