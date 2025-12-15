import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

const API_BASE = import.meta.env.VITE_API_URL;
const TRANSACTION_API = `${API_BASE}/api/transaction`;

export default function PlayerTransactionsTab() {
    const navigate = useNavigate();
    
    const [playerId, setPlayerId] = useState<string | null>(null);
    const [amount, setAmount] = useState<string>("");
    const [mobilePayTransactionNumber, setMobilePayTransactionNumber] = useState("");
    const [loading, setLoading] = useState(false);
    const [message, setMessage] = useState<string | null>(null);
    
    useEffect(() => {
        const auth = localStorage.getItem("playerAuthenticated");
        if (!auth) {
            navigate("/player/login");
            return;
        }
        
        const id = localStorage.getItem("playerId");
        setPlayerId(id);
    }, [navigate]);
    
    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        setMessage(null);
        
        if (!playerId) {
            setMessage("Missing player id");
            return;
        }
        
        const parsedAmount = Number(amount.replace(",", "."));
        if (!Number.isFinite(parsedAmount) || parsedAmount <= 0) {
            setMessage("Please enter a valid amount greater than 0");
            return;
        }
        
        if (!mobilePayTransactionNumber.trim) {
            setMessage("Please enter a valid MobilePay transaction number");
            return;
        }
        
        setLoading(true);
        try {
            const res = await fetch(
                `${TRANSACTION_API}/player/${playerId}/transaction`,
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        Accept: "application/json",
                    },
                    body: JSON.stringify({
                        amount: parsedAmount,
                        mobilePayTransactionNumber: mobilePayTransactionNumber.trim(),
                    }),
                }
            );
            
            if (!res.ok) {
                const text = await res.text().catch(() => "");
                throw new Error(text || `Failed to submit payment request: ${res.status}`);
            }
            
            setAmount("");
            setMobilePayTransactionNumber("");
            setMessage("Payment submitted! It will be pending until an admin approves it");
        } catch (error: any) {
            console.error(error);
            setMessage(error.message ?? "Failed to submit payment request");
        } finally {
            setLoading(false);
        }
    }

    return (
        <div className="min-h-screen bg-[#faf6ef] p-6">
            <div className="max-w-xl mx-auto bg-white border rounded-xl shadow p-6">
                <div className="flex items-center justify-between mb-6">
                    <h1 className="text-2xl font-bold text-red-600">Deposit via MobilePay</h1>
                    <button className="btn btn-sm text-black" onClick={() => navigate("/player-dashboard")}>
                        Back
                    </button>
                </div>

                <form onSubmit={handleSubmit} className="space-y-4">
                    <div>
                        <label className="block mb-1 text-sm text-black font-medium">Amount (DKK)</label>
                        <input
                            className="input input-bordered w-full"
                            value={amount}
                            onChange={(e) => setAmount(e.target.value)}
                            placeholder="e.g. 50"
                            inputMode="decimal"
                            required
                        />
                    </div>

                    <div>
                        <label className="block mb-1 text-sm text-black font-medium">MobilePay transaction number</label>
                        <input
                            className="input input-bordered w-full"
                            value={mobilePayTransactionNumber}
                            onChange={(e) => setMobilePayTransactionNumber(e.target.value)}
                            placeholder="e.g. 1234567890"
                            required
                        />
                    </div>

                    <button
                        className="btn bg-red-600 text-white hover:bg-red-700 w-full disabled:opacity-50"
                        disabled={loading}
                        type="submit"
                    >
                        {loading ? "Submitting..." : "Submit payment"}
                    </button>
                </form>

                {message && <p className="mt-4 text-sm text-white-700">{message}</p>}
            </div>
        </div>
    );
}