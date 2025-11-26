import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import logo from "../../assets/jerne-if-logo.png";

export default function PlayerLogin() {
  const navigate = useNavigate();
  const [email, setEmail] = useState("");

  // Seed demo players ONCE
  useEffect(() => {
    if (!localStorage.getItem("seededPlayers")) {
      const demoPlayers = [
        { email: "test@demo.com", balance: 200 },
        { email: "katja@example.com", balance: 200 },
        { email: "laura@example.com", balance: 200 },
        { email: "emre@example.com", balance: 200 },
      ];

      demoPlayers.forEach((p) => {
        localStorage.setItem(`${p.email}_balance`, String(p.balance));
      });

      localStorage.setItem("seededPlayers", "true");
    }
  }, []);

  function handleLogin(e: React.FormEvent) {
    e.preventDefault();

    const emailLower = email.toLowerCase();

    // Check if this player exists in LocalStorage
    const balance = localStorage.getItem(`${emailLower}_balance`);

    if (balance) {
      localStorage.setItem("playerAuthenticated", "true");
      localStorage.setItem("playerEmail", emailLower);
      navigate("/player-dashboard");
      return;
    }

    alert(
      "This email is not registered.\n\nTry one of these:\n" +
      "• test@demo.com\n" +
      "• katja@example.com\n" +
      "• laura@example.com\n" +
      "• emre@example.com"
    );
  }

  return (
    <div className="min-h-screen bg-[#faf6ef] flex flex-col items-center justify-center px-4">

      <img
        src={logo}
        alt="Jerne IF"
        className="rounded-full shadow mb-6"
        style={{ width: "110px", height: "110px", objectFit: "cover" }}
      />

      <h1 className="text-3xl font-bold text-red-600 mb-2">Player Login</h1>
      <p className="text-gray-600 mb-8 text-center">
        Enter your email to access your account
      </p>

      <form
        onSubmit={handleLogin}
        className="bg-white p-6 rounded-xl shadow w-full max-w-md"
      >
        <label className="block text-gray-700 font-semibold mb-2">Email</label>

        <input
          type="email"
          className="input input-bordered w-full mb-5"
          placeholder="test@demo.com"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />

        <button
          type="submit"
          className="btn w-full bg-red-600 text-white hover:bg-red-700"
        >
          Login
        </button>
      </form>

      <button
        onClick={() => navigate("/")}
        className="btn btn-outline mt-6 px-6 border-red-600 text-red-600 hover:bg-red-50"
      >
        Back to Home
      </button>

      <p className="text-gray-500 text-xs mt-8">
        Jerne If Klub
      </p>
    </div>
  );
}
