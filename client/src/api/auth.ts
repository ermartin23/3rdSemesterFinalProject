

const API = import.meta.env.VITE_API_URL;

export async function login(email: string, password: string) {
    const res = await fetch(`${API}/api/auth/login`, {
        method: "POST",
        headers: {"Content-Type": "application/json"},
        body: JSON.stringify({email, password}),
    });
    
    if (!res.ok) {
        const text = await res.text();
        try {
            const json = JSON.parse(text);
            throw new Error(json.message ?? text);
        } catch {
            throw new Error(text);
        }
    }
    
    const data = await res.json();
    console.log("LOGIN RAW RESPONSE", data);

    localStorage.setItem("token", data.token);
    localStorage.setItem("role", data.role);
    localStorage.setItem("userId", data.userId);
    localStorage.setItem("email", data.email);
    
    return data as {token: string; role: "Admin" | "Player"; userId: string; email: string};
}