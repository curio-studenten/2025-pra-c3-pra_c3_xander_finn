<?php

namespace App\Http\Controllers;

use App\Models\Team;
use App\Models\Player;
use Illuminate\Http\Request;

class DashboardController extends Controller
{
    public function index()
    {
        $player = Player::find(session('player_id'));

        if (!$player) {
            return redirect('/login');
        }

        // Top 5 teams gesorteerd op punten
        $topTeams = Team::orderBy('points', 'desc')->limit(5)->get();

        return view('dashboard.index', compact('player', 'topTeams'));
    }
}
