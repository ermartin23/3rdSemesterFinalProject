import { BrowserRouter, Routes, Route } from "react-router-dom";
import HomePage from "../pages/HomePage";
import AdminLogin from "../pages/Admin/AdminLogin";
import AdminDashboard from "../pages/Admin/AdminDashboard";
import PlayerLogin from "../pages/Player/PlayerLogin.tsx";
import PlayerDashboard from "../pages/Player/PlayerDashboard";
import PlayerHistory from "../pages/Player/PlayerHistory";
import PlayerWinners from "../pages/Player/PlayerWinners";




export default function AppRouter() {
  return (
    <BrowserRouter>
      <Routes>

        <Route path="/" element={<HomePage />} />
        <Route path="/admin-login" element={<AdminLogin />} />
        <Route path="/admin-dashboard" element={<AdminDashboard />} />
        <Route path="/player-login" element={<PlayerLogin />} />
        <Route path="/player-dashboard" element={<PlayerDashboard />} />
        <Route path="/player-history" element={<PlayerHistory />} />
          <Route path="/player-winners" element={<PlayerWinners />} />
          
        



        {/* ⬇ Player*/}
        <Route path="/player-login" element={<PlayerLogin />} />

      </Routes>
    </BrowserRouter>
  );
}
