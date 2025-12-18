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
import PlayerBalance from "../pages/Player/PlayerBalance.tsx";
import PlayerLayout from "../pages/Player/PlayerLayout.tsx";

export default function AppRouter() {
    return (
        <BrowserRouter>
            <Routes>

                {}
                <Route path="/" element={<HomePage />} />

                {}
                <Route path="/admin-login" element={<AdminLogin />} />
                <Route path="/admin-dashboard" element={<AdminDashboard />} />
                <Route path="/admin-transactions" element={<AdminTransactionsTab />} />

                {}
                <Route path="/player-login" element={<PlayerLogin />} />
                <Route element={<PlayerLayout />}>
                    <Route path="/player-dashboard" element={<PlayerDashboard />} />
                    <Route path="/player-history" element={<PlayerHistory />} />
                    <Route path="/player-winners" element={<PlayerWinners />} />
                    <Route path="/player-balance" element={<PlayerBalance />} />
                    <Route path="/player-transactions" element={<PlayerTransactionsTab />} />
                </Route>

                {}
                <Route path="*" element={<NotFound />} />

            </Routes>
        </BrowserRouter>
    );
}
