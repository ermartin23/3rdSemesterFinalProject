import type {ProblemDetails} from "../core/problemdetails.ts";
import { toast } from "react-hot-toast";

export const customFetch = {
    fetch(url: RequestInfo, init?: RequestInit): Promise<Response> {
        const token = localStorage.getItem('jwt');
        const headers = new Headers(init?.headers ?? {});
        
        if (token) {
            headers.set('Authorization', `Bearer ${token}`);
        }
        
        return fetch(url, {
            ...init,
            headers,
        }).then(async (response) => {
            if (!response.ok) {
                const errorClone = response.clone();
                const problemDetails = (await errorClone.json()) as ProblemDetails;
                console.log(problemDetails);
                toast(problemDetails.title);
            }
            return response;
        });
    }
};