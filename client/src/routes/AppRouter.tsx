import { BrowserRouter, Routes, Route } from "react-router-dom";
import HomePage from "../pages/HomePage";
import AdminLogin from "../pages/Admin/AdminLogin";
import AdminDashboard from "../pages/Admin/AdminDashboard";
import PlayerLogin from "../pages/Player/PlayerLogin";
import PlayerDashboard from "../pages/Player/PlayerDashboard";
import PlayerHistory from "../pages/Player/PlayerHistory";
import PlayerWinners from "../pages/Player/PlayerWinners";
import NotFound from "../pages/NotFound";
import PlayerTransactionsTab from "../pages/Player/PlayerTransactionsTab.tsx";
import AdminTransactionsTab from "../pages/Admin/tabs/AdminTransactionsTab.tsx";

export default function AppRouter() {
    return (
        <BrowserRouter>
            <Routes>

                {/* Main */}
                <Route path="/" element={<HomePage />} />

                {/* Admin */}
                <Route path="/admin-login" element={<AdminLogin />} />
                <Route path="/admin-dashboard" element={<AdminDashboard />} />
                <Route path="/admin-transactions" element={<AdminTransactionsTab />} />

                {/* Player */}
                <Route path="/player-login" element={<PlayerLogin />} />
                <Route path="/player-dashboard" element={<PlayerDashboard />} />
                <Route path="/player-history" element={<PlayerHistory />} />
                <Route path="/player-winners" element={<PlayerWinners />} />
                <Route path="/player-transactions" element={<PlayerTransactionsTab />} />

                {/* CATCH-ALL 404 — MUST BE LAST */}
                <Route path="*" element={<NotFound />} />

            </Routes>
        </BrowserRouter>
    );
}
