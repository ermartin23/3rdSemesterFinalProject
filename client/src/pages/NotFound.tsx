import { useNavigate } from "react-router-dom";

export default function NotFound() {
    const navigate = useNavigate();

    return (
        <div className="min-h-screen bg-[#faf6ef] flex flex-col items-center justify-center text-center px-6">

            {/* BIG BIRD ICON */}
            <div className="text-8xl mb-4">🐦</div>

            <h1 className="text-4xl font-bold text-red-600 mb-4">
                Oops! Something went wrong.
            </h1>

            <p className="text-lg text-gray-700 mb-8 max-w-md">
                The page you were trying to reach doesn’t exist or may have been moved.
            </p>

            <button
                onClick={() => navigate("/")}
                className="btn bg-red-600 text-white px-6 py-3 rounded-lg hover:bg-red-700"
            >
                ← Go Back Home
            </button>
        </div>
    );
}
