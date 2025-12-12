<?php

namespace App\Http\Controllers;

use App\Models\Team;
use App\Models\Player;
use Illuminate\Http\Request;

class TeamController extends Controller
{
    public function index()
    {
        $player = Player::find(session('player_id'));
        if (!$player) {
            return redirect('/login');
        }

        $teams = Team::with('creator')->get();
        return view('teams.index', compact('teams', 'player'));
    }

    public function create()
    {
        $player = Player::find(session('player_id'));
        if (!$player) {
            return redirect('/login');
        }

        return view('teams.create', compact('player'));
    }

    public function store(Request $request)
    {
        $player = Player::find(session('player_id'));
        if (!$player) {
            return redirect('/login');
        }

        $request->validate([
            'name' => 'required|string|max:255',
        ]);

        Team::create([
            'name' => $request->name,
            'creator_id' => $player->id,
            'points' => 0,
        ]);

        return redirect('/teams')->with('success', 'Team aangemaakt!');
    }

    public function show($id)
    {
        $player = Player::find(session('player_id'));
        if (!$player) {
            return redirect('/login');
        }

        $team = Team::with(['creator', 'players'])->findOrFail($id);
        $allPlayers = Player::whereNull('team_id')->orWhere('team_id', $id)->get();

        return view('teams.show', compact('team', 'player', 'allPlayers'));
    }

    public function edit($id)
    {
        $player = Player::find(session('player_id'));
        if (!$player) {
            return redirect('/login');
        }

        $team = Team::findOrFail($id);

        // Check of speler de eigenaar is
        if ($team->creator_id != $player->id) {
            return redirect('/teams')->withErrors(['error' => 'Je hebt geen rechten om dit team te bewerken.']);
        }

        return view('teams.edit', compact('team', 'player'));
    }

    public function update(Request $request, $id)
    {
        $player = Player::find(session('player_id'));
        if (!$player) {
            return redirect('/login');
        }

        $team = Team::findOrFail($id);

        // Check of speler de eigenaar is
        if ($team->creator_id != $player->id) {
            return redirect('/teams')->withErrors(['error' => 'Je hebt geen rechten om dit team te bewerken.']);
        }

        $request->validate([
            'name' => 'required|string|max:255',
        ]);

        $team->update([
            'name' => $request->name,
        ]);

        return redirect('/teams/' . $id)->with('success', 'Team bijgewerkt!');
    }

    public function destroy($id)
    {
        $player = Player::find(session('player_id'));
        if (!$player) {
            return redirect('/login');
        }

        $team = Team::findOrFail($id);

        // Check of speler de eigenaar is OF admin
        if ($team->creator_id != $player->id && !$player->admin) {
            return redirect('/teams')->withErrors(['error' => 'Je hebt geen rechten om dit team te verwijderen.']);
        }

        $team->delete();

        return redirect('/teams')->with('success', 'Team verwijderd!');
    }

    public function addPlayer(Request $request, $id)
    {
        $player = Player::find(session('player_id'));
        if (!$player) {
            return redirect('/login');
        }

        $team = Team::findOrFail($id);

        // Check of speler de eigenaar is
        if ($team->creator_id != $player->id) {
            return redirect('/teams/' . $id)->withErrors(['error' => 'Je hebt geen rechten om spelers toe te voegen.']);
        }

        $request->validate([
            'player_id' => 'required|exists:players,id',
        ]);

        $playerToAdd = Player::findOrFail($request->player_id);
        $playerToAdd->update(['team_id' => $id]);

        return redirect('/teams/' . $id)->with('success', 'Speler toegevoegd aan team!');
    }

    public function removePlayer($teamId, $playerId)
    {
        $player = Player::find(session('player_id'));
        if (!$player) {
            return redirect('/login');
        }

        $team = Team::findOrFail($teamId);

        // Check of speler de eigenaar is
        if ($team->creator_id != $player->id) {
            return redirect('/teams/' . $teamId)->withErrors(['error' => 'Je hebt geen rechten om spelers te verwijderen.']);
        }

        $playerToRemove = Player::findOrFail($playerId);
        $playerToRemove->update(['team_id' => null]);

        return redirect('/teams/' . $teamId)->with('success', 'Speler verwijderd uit team!');
    }
}
