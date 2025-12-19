import logo from "../assets/jerne-if-logo.png";
import {Link} from "react-router";

export default function HomePage() {
  return (
    <div className="min-h-screen bg-[#faf6ef] flex justify-center items-start py-10 px-4">
      {}
      <div className="w-full max-w-4xl flex flex-col items-center">

        {}
        <img
          src={logo}
          alt="Jerne IF"
          style={{ width: "120px", height: "120px", objectFit: "cover" }}
          className="rounded-full shadow mb-4"
        />

        {}
        <h1 className="text-4xl font-bold text-red-600 mb-2">
          Dead Pigeons <span className="inline-block">🐦</span>
        </h1>

        <p className="text-center max-w-2xl text-gray-700 mb-10">
          Support Jerne IF through our weekly lottery game! Pick your numbers and win prizes.
        </p>

        {}
        <div className="grid grid-cols-1 md:grid-cols-2 gap-8 w-full">

          {}
          <div className="bg-white border rounded-xl shadow p-6">
            <h2 className="text-xl font-semibold text-red-600 mb-4">How to Play</h2>
            <ul className="list-disc list-inside space-y-2 text-gray-700 leading-relaxed">
              <li>Choose 5–8 numbers from 1–16 on your board</li>
              <li>3 winning numbers are drawn each week</li>
              <li>Match all 3 numbers to win a share of the prize pool</li>
              <li>70% of revenue goes to prizes, 30% supports Jerne IF</li>
            </ul>
          </div>

          {}
          <div className="bg-white border rounded-xl shadow p-6">
            <h2 className="text-xl font-semibold text-red-600 mb-4">Pricing</h2>

            <div className="space-y-3">
              <div className="flex justify-between bg-[#f8ecd9] p-3 rounded-md">
                <span className="text-black">5 numbers</span>
                <span className="font-bold text-red-600">20 DKK</span>
              </div>
              <div className="flex justify-between bg-[#f8ecd9] p-3 rounded-md">
                <span className="text-black">6 numbers</span>
                <span className="font-bold text-red-600">40 DKK</span>
              </div>
              <div className="flex justify-between bg-[#f8ecd9] p-3 rounded-md">
                <span className="text-black">7 numbers</span>
                <span className="font-bold text-red-600">80 DKK</span>
              </div>
              <div className="flex justify-between bg-[#f8ecd9] p-3 rounded-md">
                <span className="text-black">8 numbers</span>
                <span className="font-bold text-red-600">160 DKK</span>
              </div>
            </div>
          </div>
        </div>

        {}
        <div className="flex gap-4 mt-10">
          <Link to="/admin-login">
            <button className="btn bg-red-600 text-white hover:bg-red-700 px-8">
              Admin Login
            </button>
          </Link>

          <Link to="/player-login">
            <button className="btn btn-outline border-red-600 text-red-600 hover:bg-red-50 px-8">
              Player Login
            </button>
          </Link>
        </div>

        <p className="text-gray-500 text-sm mt-10">
          Created by CodeBusters
        </p>

      </div>
    </div>
  );
}
