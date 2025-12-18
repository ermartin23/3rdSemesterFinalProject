import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

const API_BASE = import.meta.env.VITE_API_URL;
const TRANSACTION_CREATE_API = `${API_BASE}/api/Transaction/player/createtransaction`;
const TRANSACTION_LIST_API = `${API_BASE}/api/Transaction/player/transactions`;

interface PlayerTransaction {
    transactionId: string;
    amount: number;
    status: string;
    createdat: string;
}

export default function PlayerTransactionsTab() {
    const navigate = useNavigate();
    
    const [amount, setAmount] = useState<string>("");
    const [mobilePayTransactionNumber, setMobilePayTransactionNumber] = useState("");
    const [loading, setLoading] = useState(false);
    const [message, setMessage] = useState<string | null>(null);
    const [messageType, setMessageType] = useState<"success" | "error" | null>(null);
    const [transactions, setTransactions] = useState<PlayerTransaction[]>([]);
    const [txLoading, setTxLoading] = useState(false);
    
    useEffect(() => {
        const token = localStorage.getItem("token");
        if (!token) {
            navigate("/player-login");
        }
    }, [navigate]);

    useEffect(() => {
        loadMyTransactions();
    }, []);
    
    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        setMessageType(null);
        setMessage(null);
        
        const token = localStorage.getItem("token");
        if (!token) {
            setMessageType("error");
            setMessage("Not logged in");
            return;
        }
        
        const parsedAmunt = Number(amount.replace(",", "."));
        if (!Number.isFinite(parsedAmunt) || parsedAmunt <= 0) {
            setMessageType("error");
            setMessage("Please enter valid amount greater than 0");
            return;
        }
        
        if (!mobilePayTransactionNumber.trim()) {
            setMessageType("error");
            setMessage("Please enter valid MobilePay transaction number");
            return;
        }
        
        setLoading(true);
        try {
            const res = await fetch(TRANSACTION_CREATE_API, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Accept: "application/json",
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({
                    amount: parsedAmunt,
                    mobilePayTransactionNumber: mobilePayTransactionNumber.trim(),
                }),
            });
            
            if (!res.ok) {
                const text = await res.text().catch(() => "");
                throw new Error(text || `Failed to submit payment request: ${res.status}`);
            }
            
            setAmount("");
            setMobilePayTransactionNumber("");
            setMessageType("success");
            setMessage("Payment submitted! It will be pending until an administrator approves it");
            await loadMyTransactions();
        } catch (error: any) {
            console.error(error);
            setMessageType("error");
            setMessage(error.message ?? "Failed to submit payment request");
        } finally {
            setLoading(false);
        }
    }
    
    function formatDate(iso: string) {
        const date = new Date(iso);
        return date.toLocaleString(undefined, {
            year: "numeric",
            month: "2-digit",
            day: "2-digit",
            hour: "2-digit",
            minute: "2-digit",
        });
    }
    
    async function loadMyTransactions() {
        const token = localStorage.getItem("token");
        if (!token) return;
        
        setTxLoading(true);
        try {
            const res = await fetch(TRANSACTION_LIST_API, {
                headers: {Authorization: `Bearer ${token}`},
            });
            
            if (!res.ok) throw new Error(`Failed to load transactions: ${res.status}`);
            
            const data = await res.json();
            setTransactions(data);
        } catch (error) {
            console.error(error);
        } finally {
            setTxLoading(false);
        }
    }

    return (
        <div className="min-h-screen bg-[#faf6ef] p-6">
            <div className="max-w-xl mx-auto bg-white border rounded-xl shadow p-6">
                <div className="flex items-center justify-between mb-6">
                    <h1 className="text-2xl font-bold text-red-600">Deposit via MobilePay</h1>
                    <button className="text-red-600" onClick={() => navigate("/player-dashboard")}>
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
                            placeholder="e.g. 12345678910"
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

                {message && <p className={`mt-4 text-sm ${
                messageType === "success" ? "text-green-700" : "text-red-700"}`}>
                    {message}</p>}

                <div className="mt-8">
                    <div className="flex items-center justify-between mb-3">
                        <h2 className="text-lg font-semibold text-black">Your transactions</h2>
                        <button className="text-black" type="button" onClick={loadMyTransactions}>
                            Refresh
                        </button>
                    </div>

                    {txLoading ? (
                        <p className="text-sm text-gray-600">Loading…</p>
                    ) : transactions.length === 0 ? (
                        <p className="text-sm text-gray-600">No transactions yet.</p>
                    ) : (
                        <div className="space-y-2">
                            {transactions.map((t) => {
                                const status = (t.status ?? "").toLowerCase();

                                const badgeClass =
                                    status === "approved"
                                        ? "px-3 py-1 rounded-full text-green-700 bg-green-100"
                                        : status === "rejected" || status === "declined"
                                            ? "px-3 py-1 rounded-full text-red-700 bg-red-100"
                                            : "px-3 py-1 rounded-full text-orange-700 bg-orange-100";

                                const label =
                                    status === "approved" ? "Approved" :
                                        (status === "rejected" || status === "declined") ? "Rejected" :
                                            "Pending";

                                return (
                                    <div key={t.transactionId} className="flex items-center justify-between bg-gray-50 p-3 rounded-lg">
                                        <div className="text-black">
                                            <div className="font-semibold">{t.amount} DKK</div>
                                            <div className="text-xs text-gray-600">{formatDate(t.createdat)}</div>
                                        </div>

                                        <span className={badgeClass}>{label}</span>
                                    </div>
                                );
                            })}
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}