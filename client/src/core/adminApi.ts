import { api } from "./api";

export type AdminCreateRequestDto = {
    name: string;
    email: string;
    phone: string;
    password: string;
};

export type AdminUpdateRequestDto = {
    name: string;
    email: string;
    phone: string;
};

export type AdminResponseDto = {
    adminId: string;
    name: string;
    email: string;
    phone: string;
};

export function getAdmins() {
    return api<AdminResponseDto[]>("/api/admins");
}

export function createAdmin(dto: AdminCreateRequestDto) {
    return api<AdminResponseDto>("/api/admins", {
        method: "POST",
        body: JSON.stringify(dto),
    });
}

export function updateAdmin(id: string, dto: AdminUpdateRequestDto) {
    return api<AdminResponseDto>(`/api/admins/${id}`, {
        method: "PUT",
        body: JSON.stringify(dto),
    });
}

export function deleteAdmin(id: string) {
    return api<void>(`/api/admins/${id}`, {
        method: "DELETE",
    });
}
