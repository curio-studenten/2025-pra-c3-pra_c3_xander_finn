<?php

namespace App\Http\Controllers;

use App\Models\Team;
use App\Models\GameMatch;
use App\Models\Player;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Hash;
use Illuminate\Support\Str;

class ApiController extends Controller
{
    /**
     * POST /api/register
     * Registreer een nieuwe gebruiker via de API (voor C# WinUI app)
     */
    public function register(Request $request)
    {
        $request->validate([
            'name' => 'required|string|max:255',
            'email' => 'required|email|unique:players,email',
            'password' => 'required|min:6',
        ]);

        $player = Player::create([
            'name' => $request->name,
            'email' => $request->email,
            'password' => Hash::make($request->password),
            'admin' => 0,
            'api_key' => Str::random(32),
        ]);

        return response()->json([
            'success' => true,
            'message' => 'Account aangemaakt!',
            'player' => [
                'id' => $player->id,
                'name' => $player->name,
                'email' => $player->email,
                'api_key' => $player->api_key,
            ],
        ], 201);
    }

    /**
     * POST /api/login
     * Inloggen en API key ophalen (voor C# WinUI app)
     */
    public function login(Request $request)
    {
        $request->validate([
            'email' => 'required|email',
            'password' => 'required',
        ]);

        $player = Player::where('email', $request->email)->first();

        if (!$player || !Hash::check($request->password, $player->password)) {
            return response()->json([
                'success' => false,
                'error' => 'Ongeldige inloggegevens',
            ], 401);
        }

        return response()->json([
            'success' => true,
            'message' => 'Succesvol ingelogd!',
            'player' => [
                'id' => $player->id,
                'name' => $player->name,
                'email' => $player->email,
                'admin' => (bool) $player->admin,
                'api_key' => $player->api_key,
            ],
        ]);
    }

    /**
     * PUT /api/matches/{id}
     * Update de score van een wedstrijd (voor C# WinUI app - alleen admin)
     */
    public function updateMatch(Request $request, $id)
    {
        $apiKey = $request->header('X-API-KEY') ?? $request->query('api_key');

        if (!$apiKey) {
            return response()->json(['error' => 'API key required'], 401);
        }

        $player = Player::where('api_key', $apiKey)->first();

        if (!$player) {
            return response()->json(['error' => 'Invalid API key'], 401);
        }

        if (!$player->admin) {
            return response()->json(['error' => 'Admin rechten vereist'], 403);
        }

        $request->validate([
            'score_team1' => 'required|integer|min:0',
            'score_team2' => 'required|integer|min:0',
        ]);

        $match = GameMatch::find($id);

        if (!$match) {
            return response()->json(['error' => 'Wedstrijd niet gevonden'], 404);
        }

        // Als de wedstrijd al gespeeld was, trek de oude punten er eerst af
        if ($match->played) {
            $this->removePoints($match);
        }

        $match->update([
            'score_team1' => $request->score_team1,
            'score_team2' => $request->score_team2,
            'played' => true,
        ]);

        // Ken punten toe
        $this->assignPoints($match);

        return response()->json([
            'success' => true,
            'message' => 'Score opgeslagen!',
            'match' => [
                'id' => $match->id,
                'team1_name' => $match->team1->name,
                'team2_name' => $match->team2->name,
                'score_team1' => $match->score_team1,
                'score_team2' => $match->score_team2,
                'played' => $match->played,
            ],
        ]);
    }

    private function assignPoints(GameMatch $match)
    {
        $team1 = Team::find($match->team1_id);
        $team2 = Team::find($match->team2_id);

        if ($match->score_team1 > $match->score_team2) {
            $team1->points += 3;
        } elseif ($match->score_team1 < $match->score_team2) {
            $team2->points += 3;
        } else {
            $team1->points += 1;
            $team2->points += 1;
        }

        $team1->save();
        $team2->save();
    }

    private function removePoints(GameMatch $match)
    {
        $team1 = Team::find($match->team1_id);
        $team2 = Team::find($match->team2_id);

        if ($match->score_team1 > $match->score_team2) {
            $team1->points -= 3;
        } elseif ($match->score_team1 < $match->score_team2) {
            $team2->points -= 3;
        } else {
            $team1->points -= 1;
            $team2->points -= 1;
        }

        $team1->save();
        $team2->save();
    }

    public function teams(Request $request)
    {
        $apiKey = $request->header('X-API-KEY') ?? $request->query('api_key');

        if (!$apiKey) {
            return response()->json(['error' => 'API key required'], 401);
        }

        $player = Player::where('api_key', $apiKey)->first();

        if (!$player) {
            return response()->json(['error' => 'Invalid API key'], 401);
        }

        $teams = Team::orderBy('points', 'desc')->get();

        return response()->json($teams);
    }

    public function matches(Request $request)
    {
        $apiKey = $request->header('X-API-KEY') ?? $request->query('api_key');

        if (!$apiKey) {
            return response()->json(['error' => 'API key required'], 401);
        }

        $player = Player::where('api_key', $apiKey)->first();

        if (!$player) {
            return response()->json(['error' => 'Invalid API key'], 401);
        }

        $matches = GameMatch::with(['team1', 'team2'])->orderBy('start_time')->get();

        // Format voor de API
        $formattedMatches = $matches->map(function ($match) {
            return [
                'id' => $match->id,
                'team1_id' => $match->team1_id,
                'team1_name' => $match->team1->name,
                'team2_id' => $match->team2_id,
                'team2_name' => $match->team2->name,
                'score_team1' => $match->score_team1,
                'score_team2' => $match->score_team2,
                'field' => $match->field,
                'start_time' => $match->start_time ? $match->start_time->format('Y-m-d H:i:s') : null,
                'played' => $match->played,
            ];
        });

        return response()->json($formattedMatches);
    }

    public function standings(Request $request)
    {
        $apiKey = $request->header('X-API-KEY') ?? $request->query('api_key');

        if (!$apiKey) {
            return response()->json(['error' => 'API key required'], 401);
        }

        $player = Player::where('api_key', $apiKey)->first();

        if (!$player) {
            return response()->json(['error' => 'Invalid API key'], 401);
        }

        $teams = Team::orderBy('points', 'desc')->get(['id', 'name', 'points']);

        return response()->json($teams);
    }
}
