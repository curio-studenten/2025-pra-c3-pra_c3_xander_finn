<?php

namespace App\Http\Controllers;

use App\Models\GameMatch;
use App\Models\Team;
use App\Models\Player;
use Illuminate\Http\Request;
use Carbon\Carbon;

class MatchController extends Controller
{
    public function index()
    {
        $player = Player::find(session('player_id'));
        if (!$player) {
            return redirect('/login');
        }

        $matches = GameMatch::with(['team1', 'team2'])->orderBy('start_time')->get();
        return view('matches.index', compact('matches', 'player'));
    }

    public function showGenerate()
    {
        $player = Player::find(session('player_id'));
        if (!$player || !$player->admin) {
            return redirect('/dashboard')->withErrors(['error' => 'Alleen beheerders kunnen wedstrijden genereren.']);
        }

        $teams = Team::all();
        return view('matches.generate', compact('player', 'teams'));
    }

    public function generate(Request $request)
    {
        $player = Player::find(session('player_id'));
        if (!$player || !$player->admin) {
            return redirect('/dashboard')->withErrors(['error' => 'Alleen beheerders kunnen wedstrijden genereren.']);
        }

        $request->validate([
            'fields' => 'required|integer|min:1|max:10',
            'match_duration' => 'required|integer|min:5|max:120',
            'break_between' => 'required|integer|min:0|max:60',
            'start_time' => 'required|date',
        ]);

        // Verwijder bestaande wedstrijden
        GameMatch::query()->delete();

        // Reset alle team punten
        Team::query()->update(['points' => 0]);

        $teams = Team::all();
        $numFields = $request->fields;
        $matchDuration = $request->match_duration;
        $breakBetween = $request->break_between;
        $startTime = Carbon::parse($request->start_time);

        $currentField = 1;
        $currentTime = $startTime->copy();
        $matchesOnCurrentTime = 0;

        // Genereer halve competitie: elk team speelt 1x tegen elk ander team
        foreach ($teams as $team1) {
            foreach ($teams as $team2) {
                // Voorkom dat een team tegen zichzelf speelt
                if ($team1->id >= $team2->id) {
                    continue;
                }

                GameMatch::create([
                    'team1_id' => $team1->id,
                    'team2_id' => $team2->id,
                    'field' => $currentField,
                    'start_time' => $currentTime,
                    'played' => false,
                ]);

                $matchesOnCurrentTime++;
                $currentField++;

                // Als alle velden bezet zijn, ga naar volgende tijdslot
                if ($currentField > $numFields) {
                    $currentField = 1;
                    $currentTime->addMinutes($matchDuration + $breakBetween);
                    $matchesOnCurrentTime = 0;
                }
            }
        }

        return redirect('/matches')->with('success', 'Wedstrijdschema gegenereerd!');
    }

    public function edit($id)
    {
        $player = Player::find(session('player_id'));
        if (!$player || !$player->admin) {
            return redirect('/matches')->withErrors(['error' => 'Alleen beheerders kunnen scores invoeren.']);
        }

        $match = GameMatch::with(['team1', 'team2'])->findOrFail($id);
        return view('matches.edit', compact('match', 'player'));
    }

    public function update(Request $request, $id)
    {
        $player = Player::find(session('player_id'));
        if (!$player || !$player->admin) {
            return redirect('/matches')->withErrors(['error' => 'Alleen beheerders kunnen scores invoeren.']);
        }

        $request->validate([
            'score_team1' => 'required|integer|min:0',
            'score_team2' => 'required|integer|min:0',
        ]);

        $match = GameMatch::findOrFail($id);

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

        return redirect('/matches')->with('success', 'Score opgeslagen!');
    }

    private function assignPoints(GameMatch $match)
    {
        $team1 = Team::find($match->team1_id);
        $team2 = Team::find($match->team2_id);

        if ($match->score_team1 > $match->score_team2) {
            // Team 1 wint
            $team1->points += 3;
        } elseif ($match->score_team1 < $match->score_team2) {
            // Team 2 wint
            $team2->points += 3;
        } else {
            // Gelijkspel
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

    public function destroy($id)
    {
        $player = Player::find(session('player_id'));
        if (!$player || !$player->admin) {
            return redirect('/matches')->withErrors(['error' => 'Alleen beheerders kunnen wedstrijden verwijderen.']);
        }

        $match = GameMatch::findOrFail($id);

        // Als de wedstrijd al gespeeld was, trek de punten er eerst af
        if ($match->played) {
            $this->removePoints($match);
        }

        $match->delete();

        return redirect('/matches')->with('success', 'Wedstrijd verwijderd!');
    }
}
