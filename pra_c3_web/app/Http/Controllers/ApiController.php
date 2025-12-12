<?php

namespace App\Http\Controllers;

use App\Models\Team;
use App\Models\GameMatch;
use App\Models\Player;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Hash;
use Illuminate\Support\Str;
use Illuminate\Http\JsonResponse;

class ApiController extends Controller
{
    // ==========================================
    // API ENDPOINTS VOOR C# GOK APPLICATIE
    // ==========================================

    /**
     * GET /api/matches.php - Returns upcoming matches (not yet played) for C# app
     */
    public function apiMatches(): JsonResponse
    {
        // Haal niet-gespeelde wedstrijden uit database
        $matches = GameMatch::with(['team1', 'team2'])
            ->where('played', false)
            ->orderBy('start_time')
            ->get();

        $formatted = $matches->map(function ($match) {
            return [
                'id' => $match->id,
                'team1_id' => $match->team1_id,
                'team1_name' => $match->team1->name,
                'team2_id' => $match->team2_id,
                'team2_name' => $match->team2->name,
            ];
        });

        return response()->json($formatted);
    }

    /**
     * GET /api/results.php - Returns played matches with scores for C# app
     */
    public function apiResults(): JsonResponse
    {
        // Haal gespeelde wedstrijden uit database
        $results = GameMatch::with(['team1', 'team2'])
            ->where('played', true)
            ->orderBy('start_time', 'desc')
            ->get();

        $formatted = $results->map(function ($match) {
            // Bepaal winnaar
            $winnerId = null;
            if ($match->score_team1 > $match->score_team2) {
                $winnerId = $match->team1_id;
            } elseif ($match->score_team2 > $match->score_team1) {
                $winnerId = $match->team2_id;
            }

            return [
                'id' => $match->id,
                'team1_id' => $match->team1_id,
                'team1_name' => $match->team1->name,
                'team1_score' => $match->score_team1,
                'team2_id' => $match->team2_id,
                'team2_name' => $match->team2->name,
                'team2_score' => $match->score_team2,
                'winner_id' => $winnerId,
            ];
        });

        return response()->json($formatted);
    }

    /**
     * GET /api/goals.php?match_id={id} - Returns goals for a specific match
     * Note: Goals worden niet bijgehouden in de huidige database, dus retourneer lege array
     */
    public function apiGoals(Request $request): JsonResponse
    {
        $matchId = $request->query('match_id');

        if (!$matchId) {
            return response()->json(['error' => 'match_id is required'], 400);
        }

        // Goals worden niet bijgehouden in de huidige database
        // Return lege array
        return response()->json([]);
    }
}
