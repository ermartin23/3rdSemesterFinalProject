import { useEffect, useMemo, useState } from "react";
import {
    getGames,
    setGameWinners,
    getGameDetails,
    type GameResponseDto,
    type GameDetailsResponseDto,
} from "../../../core/gamesApi";

export default function GamesTab() {
    const [games, setGames] = useState<GameResponseDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [selected, setSelected] = useState<number[]>([]);
    const [error, setError] = useState<string | null>(null);
    const [saving, setSaving] = useState(false);

    // ✅ details of the currently active game (for totals)
    const [activeDetails, setActiveDetails] = useState<GameDetailsResponseDto | null>(null);

    const [expandedGameId, setExpandedGameId] = useState<string | null>(null);
    const [detailsCache, setDetailsCache] = useState<Record<string, GameDetailsResponseDto>>({});
    const [detailsLoadingId, setDetailsLoadingId] = useState<string | null>(null);

    async function load() {
        setError(null);
        setLoading(true);
        try {
            const all = await getGames();
            setGames(all);
        } catch (e: any) {
            setError(e.message ?? "Failed to load games");
        } finally {
            setLoading(false);
        }
    }

    // load list once
    useEffect(() => {
        load();
    }, []);

    const activeGame = useMemo(() => {
        return (
            games
                .filter((g) => g.isOpen)
                .sort((a, b) => b.weekidentity.localeCompare(a.weekidentity))[0] ?? null
        );
    }, [games]);

    const history = useMemo(() => {
        return games
            .filter((g) => !g.isOpen)
            .sort((a, b) => b.weekidentity.localeCompare(a.weekidentity));
    }, [games]);

    // ✅ load details whenever active game changes
    useEffect(() => {
        async function loadDetails() {
            if (!activeGame) {
                setActiveDetails(null);
                return;
            }

            try {
                const details = await getGameDetails(activeGame.gameid);
                setActiveDetails(details);
            } catch {
                // don’t crash UI if details fails
                setActiveDetails(null);
            }
        }

        loadDetails();
    }, [activeGame?.gameid]);

    // ✅ compute totals from details
    const currentBoards = useMemo(() => {
        if (!activeDetails) return 0;
        return activeDetails.players.reduce((sum, p) => sum + p.boards.length, 0);
    }, [activeDetails]);

    const totalRevenue = useMemo(() => {
        if (!activeDetails) return 0;
        return activeDetails.players.reduce(
            (sum, p) => sum + p.boards.reduce((s, b) => s + (b.price ?? 0), 0),
            0
        );
    }, [activeDetails]);

    // ✅ IMPORTANT: define this AFTER activeGame exists
    const canSetNow = activeGame?.canSetWinnersNow ?? false;

    function toggleNumber(n: number) {
        if (!canSetNow) return;

        setSelected((prev) => {
            if (prev.includes(n)) return prev.filter((x) => x !== n);
            if (prev.length === 3) return prev;
            return [...prev, n];
        });
    }

    async function closeGame() {
        if (!activeGame) return;
        if (!canSetNow) return;
        if (selected.length !== 3) return;

        setSaving(true);
        setError(null);
        try {
            await setGameWinners(activeGame.gameid, { winningNumbers: selected });
            setSelected([]);
            await load(); // refresh list (and details will refresh via useEffect)
        } catch (e: any) {
            setError(e.message ?? "Failed to close game");
        } finally {
            setSaving(false);
        }
    }

    async function toggleHistoryDetails(gameId: string) {
        // collapse if same item clicked
        if (expandedGameId === gameId) {
            setExpandedGameId(null);
            return;
        }

        setExpandedGameId(gameId);

        // already loaded -> don't refetch
        if (detailsCache[gameId]) return;

        setDetailsLoadingId(gameId);
        try {
            const details = await getGameDetails(gameId);
            setDetailsCache(prev => ({ ...prev, [gameId]: details }));
        } catch (e: any) {
            setError(e.message ?? "Failed to load game details");
        } finally {
            setDetailsLoadingId(null);
        }
    }

    if (loading) return <div className="p-6">Loading...</div>;

    return (
        <div className="min-h-screen bg-[#faf6ef] p-6">
            <div className="max-w-5xl mx-auto">
                {error && (
                    <div className="mb-4 p-3 rounded border bg-white text-red-600">
                        {error}
                    </div>
                )}

                {/* ACTIVE GAME */}
                <div className="p-6 border border-red-300 rounded-xl bg-white shadow">
                    <div className="flex items-center gap-3 mb-4">
                        <span className="font-bold text-lg text-red-600">Active Game</span>
                        <span className="px-2 py-1 bg-red-500 text-white rounded text-sm">
            Active
          </span>
                    </div>

                    {!activeGame ? (
                        <p className="text-gray-600">No active game found.</p>
                    ) : (
                        <>
                            <p className="text-gray-600 mb-3">
                                Week: <strong>{formatWeek(activeGame.weekidentity)}</strong>
                            </p>

                            {/* ✅ real totals */}
                            <div className="bg-[#f7ead4] p-4 rounded-lg mb-6 text-black">
                                <p>
                                    Current Boards: <strong>{currentBoards}</strong>
                                </p>
                                <p>
                                    Total Revenue: <strong>{totalRevenue} DKK</strong>
                                </p>
                            </div>

                            {!canSetNow && (
                                <div className="mb-4 text-sm text-gray-600">
                                    You can set winners after{" "}
                                    <strong>{formatDkDateTime(activeGame.cutoffUtc)}</strong> (DK
                                    time).
                                </div>
                            )}

                            <div className="p-4 bg-[#fff4ef] border rounded-xl mb-6 text-black">
                                <p className="font-bold mb-4">Select 3 Winning Numbers</p>

                                <div className="grid grid-cols-8 gap-3">
                                    {Array.from({ length: 16 }, (_, i) => i + 1).map((num) => {
                                        const isSel = selected.includes(num);

                                        return (
                                            <button
                                                key={num}
                                                disabled={!canSetNow}
                                                onClick={() => toggleNumber(num)}
                                                className={`p-4 rounded-xl text-center border ${
                                                    isSel
                                                        ? "bg-red-500 text-white border-red-700"
                                                        : "bg-[#f7ead4] border-[#e8d8bd] hover:bg-[#f3e3c7]"
                                                } ${!canSetNow ? "opacity-50 cursor-not-allowed" : ""}`}
                                            >
                                                {num}
                                            </button>
                                        );
                                    })}
                                </div>

                                <p className="mt-3 text-sm text-gray-600">
                                    Selected: {selected.length}/3
                                </p>

                                <button
                                    disabled={saving || !canSetNow || selected.length !== 3}
                                    onClick={closeGame}
                                    className="btn mt-4 bg-red-600 text-white hover:bg-red-700 disabled:opacity-40"
                                >
                                    {saving ? "Saving..." : "Close Game & Set Winners"}
                                </button>
                            </div>
                        </>
                    )}
                </div>

                {/* GAME HISTORY */}
                <div className="mt-10 mb-20">
                    <h2 className="text-xl font-bold text-red-600 mb-4">Game History</h2>

                    <div className="bg-white border rounded-xl shadow p-6">
                        {history.length === 0 ? (
                            <p className="text-center text-gray-600">No closed games yet</p>
                        ) : (
                            <ul className="space-y-3">
                                {history.map((g) => {
                                    const isExpanded = expandedGameId === g.gameid;
                                    const details = detailsCache[g.gameid];
                                    const isLoading = detailsLoadingId === g.gameid;

                                    const totalBoards = details
                                        ? details.players.reduce((sum, p) => sum + p.boards.length, 0)
                                        : 0;

                                    const totalWinningBoards = details?.totalWinningBoards ?? 0;

                                    return (
                                        <li
                                            key={g.gameid}
                                            className="rounded-xl bg-white shadow border border-red-200"
                                        >
                                            <button
                                                type="button"
                                                onClick={() => toggleHistoryDetails(g.gameid)}
                                                className="w-full p-4 flex justify-between items-center text-left bg-white rounded-xl text-gray-900 hover:bg-gray-50"
                                            >
                      <span className="text-gray-900">
                        <strong className="text-red-600">
                          {formatWeek(g.weekidentity)}
                        </strong>
                        <span className="text-gray-700"> — Winners: </span>
                        <span className="text-gray-900 font-semibold">
                          {g.winningnumbers?.join(", ")}
                        </span>
                      </span>

                                                <span className="text-gray-700">
                        {isExpanded ? "▲" : "▼"}
                      </span>
                                            </button>

                                            {isExpanded && (
                                                <div className="px-4 pb-4">
                                                    {isLoading && (
                                                        <p className="text-sm text-gray-700">
                                                            Loading details...
                                                        </p>
                                                    )}

                                                    {!isLoading && details && (
                                                        <div className="bg-[#fff4ef] border border-red-200 rounded-xl p-4 text-gray-900">
                                                            <div className="flex gap-6 text-sm text-gray-800 mb-3">
                                                                <div>
                                                                    Total boards: <strong>{totalBoards}</strong>
                                                                </div>
                                                                <div>
                                                                    Total winning boards:{" "}
                                                                    <strong>{totalWinningBoards}</strong>
                                                                </div>
                                                            </div>

                                                            {details.players.length === 0 ? (
                                                                <p className="text-sm text-gray-700">
                                                                    No boards for this game.
                                                                </p>
                                                            ) : (
                                                                <div className="space-y-3">
                                                                    {details.players.map((p) => (
                                                                        <div
                                                                            key={p.playerId}
                                                                            className="border border-red-200 bg-white rounded-lg p-3"
                                                                        >
                                                                            <div className="font-semibold mb-2 text-gray-900">
                                                                                {p.name}
                                                                            </div>

                                                                            <ul className="space-y-1 text-sm">
                                                                                {p.boards.map((b) => (
                                                                                    <li
                                                                                        key={b.boardId}
                                                                                        className="flex justify-between text-gray-800"
                                                                                    >
                                          <span>
                                            Board: [{b.chosenNumbers.join(", ")}
                                              ] — {b.price} DKK
                                          </span>

                                                                                        <span
                                                                                            className={
                                                                                                b.isWinningBoard
                                                                                                    ? "text-green-600 font-semibold"
                                                                                                    : "text-gray-600"
                                                                                            }
                                                                                        >
                                            {b.isWinningBoard ? "WIN" : "—"}
                                          </span>
                                                                                    </li>
                                                                                ))}
                                                                            </ul>
                                                                        </div>
                                                                    ))}
                                                                </div>
                                                            )}
                                                        </div>
                                                    )}

                                                    {!isLoading && !details && (
                                                        <p className="text-sm text-gray-700">
                                                            No details loaded.
                                                        </p>
                                                    )}
                                                </div>
                                            )}
                                        </li>
                                    );
                                })}
                            </ul>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
}

function formatWeek(weekIdentityIso: string) {
    const date = new Date(weekIdentityIso);
    const weekNumber = getISOWeek(date);
    const dateText = date.toLocaleDateString("da-DK");
    return `Week ${weekNumber} – ${dateText}`;
}

function getISOWeek(date: Date) {
    const d = new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));
    const dayNum = d.getUTCDay() || 7;
    d.setUTCDate(d.getUTCDate() + 4 - dayNum);
    const yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
    return Math.ceil((((d.getTime() - yearStart.getTime()) / 86400000) + 1) / 7);
}

function formatDkDateTime(utcIso: string) {
    return new Date(utcIso).toLocaleString("da-DK", { timeZone: "Europe/Copenhagen" });
}
