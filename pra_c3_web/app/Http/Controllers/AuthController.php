<?php

namespace App\Http\Controllers;

use App\Models\Player;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Hash;
use Illuminate\Support\Str;

class AuthController extends Controller
{
    public function showRegister()
    {
        return view('auth.register');
    }

    public function register(Request $request)
    {
        $request->validate([
            'name' => 'required|string|max:255',
            'email' => 'required|email|unique:players,email',
            'password' => 'required|min:6|confirmed',
        ]);

        $player = Player::create([
            'name' => $request->name,
            'email' => $request->email,
            'password' => Hash::make($request->password),
            'admin' => 0,
            'api_key' => Str::random(32),
        ]);

        session(['player_id' => $player->id]);

        return redirect('/dashboard')->with('success', 'Account aangemaakt!');
    }

    public function showLogin()
    {
        return view('auth.login');
    }

    public function login(Request $request)
    {
        $request->validate([
            'email' => 'required|email',
            'password' => 'required',
        ]);

        $player = Player::where('email', $request->email)->first();

        if ($player && Hash::check($request->password, $player->password)) {
            session(['player_id' => $player->id]);
            return redirect('/dashboard')->with('success', 'Succesvol ingelogd!');
        }

        return back()->withErrors(['email' => 'Ongeldige inloggegevens.']);
    }

    public function logout()
    {
        session()->forget('player_id');
        return redirect('/')->with('success', 'Uitgelogd!');
    }
}
