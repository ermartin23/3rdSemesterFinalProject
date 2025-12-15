import { useState } from "react";
import logo from "../../assets/jerne-if-logo.png";
import PlayersTab from "./tabs/PlayersTab";
import GamesTab from "./tabs/GamesTab";
import TransactionsTab from "./tabs/AdminTransactionsTab.tsx";

export default function AdminDashboard() {
    const [activeTab, setActiveTab] =
        useState<"players" | "transactions" | "games">("players");

    // @ts-ignore
    return (
        <div className="min-h-screen bg-[#faf6ef]">
            {/* HEADER */}
            <div className="flex items-center justify-between px-8 py-4 bg-[#faf6ef] shadow-sm">
                <div className="flex items-center gap-3">
                    <img
                        src={logo}
                        alt="Jerne IF"
                        className="rounded-full shadow"
                        style={{ width: "50px", height: "50px", objectFit: "cover" }}
                    />
                    <h1 className="text-2xl font-bold text-red-600">Admin Dashboard</h1>
                </div>

                <div className="flex items-center gap-4">
                    <span className="text-gray-600">Logged in as Administrator</span>

                    <button
                        className="btn btn-outline border-red-600 text-red-600 hover:bg-red-50"
                        onClick={() => {
                            localStorage.removeItem("adminAuthenticated");
                            window.location.href = "/admin-login";
                        }}
                    >
                        Logout
                    </button>
                </div>
            </div>

            {/* TAB BAR */}
            <div className="flex justify-center mt-6">
                <div className="tabs tabs-boxed bg-[#f7f2e9]">
                    <a
                        className={`tab ${
                            activeTab === "players" ? "tab-active" : ""
                        }`}
                        onClick={() => setActiveTab("players")}
                    >
                        Players
                    </a>

                    <a
                        className={`tab ${
                            activeTab === "transactions" ? "tab-active" : ""
                        }`}
                        onClick={() => setActiveTab("transactions")}
                    >
                        Transactions
                    </a>

                    <a
                        className={`tab ${activeTab === "games" ? "tab-active" : ""}`}
                        onClick={() => setActiveTab("games")}
                    >
                        Games
                    </a>
                </div>
            </div>

            {/* RENDER TABS */}
            <div className="p-4">
                {activeTab === "players" && <PlayersTab />}
                {activeTab === "games" && <GamesTab />}
                {activeTab === "transactions" && <TransactionsTab />}
            </div>
        </div>
    );
}