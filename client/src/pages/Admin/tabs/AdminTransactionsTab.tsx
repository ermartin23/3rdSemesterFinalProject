import {useEffect, useState} from "react";

const TRANSACTION_API = `${import.meta.env.VITE_API_URL}/api/Transaction`;
const TRANSACTION_PAGE_SIZE = 10;

interface Transaction {
    transactionId: string;
    playerId: string;
    amount: number;
    mobilePayTransactionNumber: string;
    status: string;
    createdAt: string;
}

export default function TransactionsTab() {
    const [transactions, setTransactions] = useState<Transaction[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [actionLoadingId, setActionLoadingId] = useState<string | null>(null);
    
    const [historyPage, setHistoryPage] = useState(1);
    
    useEffect(() => {
        async function loadTransactions() {
            setLoading(true);
            setError(null);
            
            try {
                const res = await fetch(TRANSACTION_API, {
                    method: "GET",
                    headers: {Accept: "application/json"},
                });
                
                if (!res.ok) {
                    throw new Error(`Failed to load transactions (${res.status})`);
                }
                
                const data: Transaction[] = await res.json();
                setTransactions(data);
            } catch (err: any) {
                console.error(err);
                setError(err.message ?? "Failed to load transactions");
            } finally {
                setLoading(false);
            }
        }
        loadTransactions();
    }, []);
    
    const pending = transactions.filter(
        (t) => t.status?.toLowerCase() === "pending"
    );
    const processed = transactions.filter(
        (t) => t.status?.toLowerCase() !== "pending"
    );
    
    const totalHistoryPages = Math.max(
        1,
        Math.ceil(processed.length / TRANSACTION_PAGE_SIZE)
    );
    
    const historyStartIndex = (historyPage - 1) * TRANSACTION_PAGE_SIZE;
    const paginatedHistory = processed.slice(
        historyStartIndex,
        historyPage * TRANSACTION_PAGE_SIZE
    );
    
    function formatDate(iso: string) {
        if (!iso) return "-";
        try {
            const date = new Date(iso);
            return date.toLocaleString("da-DK", {
                year: "numeric",
                month: "2-digit",
                day: "2-digit",
                hour: "2-digit",
                minute: "2-digit",
            });
        } catch {
            return iso;
        }
    }

    function updateTransactionStatusLocally(
        id: string,
        newStatus: string
    ) {
        setTransactions((prev) =>
            prev.map((t) =>
                t.transactionId === id ? { ...t, status: newStatus } : t
            )
        );
    }
    
    async function handleApprove(id: string) {
        setActionLoadingId(id);
        try {
            const res = await fetch(`${TRANSACTION_API}/${id}/approve`, {
                method: "POST",
            });
            
            if (!res.ok) {
                throw new Error(`Failed to approve transaction (${res.status})`);
            }
            
            updateTransactionStatusLocally(id, "approved");
        } catch (error: any) {
            console.error(error);
            alert(error.message ?? "Failed to approve transaction");
        } finally {
            setActionLoadingId(null);
        }
    }
    
    async function handleReject(id: string) {
        setActionLoadingId(id);
        try {
            const res = await fetch(`${TRANSACTION_API}/${id}/reject`, {
                method: "POST",
            });
            
            if (!res.ok) {
                throw new Error(`Failed to reject transaction (${res.status})`);
            }
            
            updateTransactionStatusLocally(id, "declined");
        } catch (error: any) {
            console.error(error);
            alert(error.message ?? "Failed to reject transaction");
        } finally {
            setActionLoadingId(null);
        }
    }

    if (loading) {
        return (
            <div className="max-w-5xl mx-auto mt-10 text-center text-gray-600">
                Loading transactions...
            </div>
        );
    }

    if (error) {
        return (
            <div className="max-w-5xl mx-auto mt-10 text-center text-red-600">
                Failed to load transactions: {error}
            </div>
        );
    }

    return (
        <div className="max-w-5xl mx-auto mt-10 space-y-10">
            {/* Pending transactions */}
            <section>
                <div className="flex justify-between items-center mb-4">
                    <h2 className="text-xl font-bold text-red-600">
                        Pending Transactions
                    </h2>
                </div>

                {pending.length === 0 ? (
                    <div className="bg-white border rounded-xl shadow p-6 text-center text-gray-600">
                        No pending transactions
                    </div>
                ) : (
                    <div className="bg-white border rounded-xl shadow overflow-hidden">
                        <table className="min-w-full text-sm">
                            <thead className="bg-gray-100 text-left">
                            <tr>
                                <th className="px-4 py-2">Created</th>
                                <th className="px-4 py-2">Player</th>
                                <th className="px-4 py-2">Amount</th>
                                <th className="px-4 py-2">MobilePay #</th>
                                <th className="px-4 py-2">Status</th>
                                <th className="px-4 py-2 text-right">
                                    Actions
                                </th>
                            </tr>
                            </thead>
                            <tbody>
                            {pending.map((t) => (
                                <tr
                                    key={t.transactionId}
                                    className="border-t"
                                >
                                    <td className="px-4 py-2">
                                        {formatDate(t.createdAt)}
                                    </td>
                                    <td className="px-4 py-2">
                                            <span className="font-mono text-xs">
                                                {t.playerId}
                                            </span>
                                    </td>
                                    <td className="px-4 py-2">
                                        {t.amount} kr
                                    </td>
                                    <td className="px-4 py-2">
                                        {t.mobilePayTransactionNumber}
                                    </td>
                                    <td className="px-4 py-2 capitalize">
                                        {t.status}
                                    </td>
                                    <td className="px-4 py-2 text-right space-x-2">
                                        <button
                                            onClick={() =>
                                                handleApprove(
                                                    t.transactionId
                                                )
                                            }
                                            disabled={
                                                actionLoadingId ===
                                                t.transactionId
                                            }
                                            className="btn btn-sm bg-green-600 text-white hover:bg-green-700 disabled:opacity-50"
                                        >
                                            Approve
                                        </button>
                                        <button
                                            onClick={() =>
                                                handleReject(
                                                    t.transactionId
                                                )
                                            }
                                            disabled={
                                                actionLoadingId ===
                                                t.transactionId
                                            }
                                            className="btn btn-sm bg-red-600 text-white hover:bg-red-700 disabled:opacity-50"
                                        >
                                            Reject
                                        </button>
                                    </td>
                                </tr>
                            ))}
                            </tbody>
                        </table>
                    </div>
                )}
            </section>

            {/* Transaction history */}
            <section>
                <h2 className="text-xl font-bold text-red-600 mb-4">
                    Transaction History
                </h2>

                {processed.length === 0 ? (
                    <div className="bg-white border rounded-xl shadow p-6 text-center text-gray-600">
                        No processed transactions yet
                    </div>
                ) : (
                    <>
                        <div className="bg-white border rounded-xl shadow overflow-hidden">
                            <table className="min-w-full text-sm">
                                <thead className="bg-gray-100 text-left">
                                <tr>
                                    <th className="px-4 py-2">Created</th>
                                    <th className="px-4 py-2">Player</th>
                                    <th className="px-4 py-2">Amount</th>
                                    <th className="px-4 py-2">
                                        MobilePay #
                                    </th>
                                    <th className="px-4 py-2">Status</th>
                                </tr>
                                </thead>
                                <tbody>
                                {paginatedHistory.map((t) => (
                                    <tr
                                        key={t.transactionId}
                                        className="border-t"
                                    >
                                        <td className="px-4 py-2">
                                            {formatDate(t.createdAt)}
                                        </td>
                                        <td className="px-4 py-2">
                                                <span className="font-mono text-xs">
                                                    {t.playerId}
                                                </span>
                                        </td>
                                        <td className="px-4 py-2">
                                            {t.amount} kr
                                        </td>
                                        <td className="px-4 py-2">
                                            {
                                                t.mobilePayTransactionNumber
                                            }
                                        </td>
                                        <td className="px-4 py-2 capitalize">
                                            {t.status}
                                        </td>
                                    </tr>
                                ))}
                                </tbody>
                            </table>
                        </div>

                        {/* Pagination controls */}
                        <div className="flex items-center justify-between mt-4 text-sm">
                            <span className="text-gray-600">
                                Page {historyPage} of {totalHistoryPages}
                            </span>
                            <div className="space-x-2">
                                <button
                                    onClick={() =>
                                        setHistoryPage((p) =>
                                            Math.max(1, p - 1)
                                        )
                                    }
                                    disabled={historyPage === 1}
                                    className="btn btn-sm disabled:opacity-50"
                                >
                                    Previous
                                </button>
                                <button
                                    onClick={() =>
                                        setHistoryPage((p) =>
                                            Math.min(
                                                totalHistoryPages,
                                                p + 1
                                            )
                                        )
                                    }
                                    disabled={
                                        historyPage === totalHistoryPages
                                    }
                                    className="btn btn-sm disabled:opacity-50"
                                >
                                    Next
                                </button>
                            </div>
                        </div>
                    </>
                )}
            </section>
        </div>
    );
}