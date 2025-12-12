<?php

namespace Database\Seeders;

use App\Models\Player;
use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\Hash;
use Illuminate\Support\Str;

class DatabaseSeeder extends Seeder
{
    /**
     * Seed the application's database.
     */
    public function run(): void
    {
        // Maak admin account aan
        Player::create([
            'name' => 'Admin',
            'email' => 'admin@schoolvoetbal.nl',
            'password' => Hash::make('admin123'),
            'admin' => 1,
            'api_key' => Str::random(32),
        ]);

        // Maak test spelers aan
        Player::create([
            'name' => 'Jan Jansen',
            'email' => 'jan@test.nl',
            'password' => Hash::make('wachtwoord'),
            'admin' => 0,
            'api_key' => Str::random(32),
        ]);

        Player::create([
            'name' => 'Piet Peters',
            'email' => 'piet@test.nl',
            'password' => Hash::make('wachtwoord'),
            'admin' => 0,
            'api_key' => Str::random(32),
        ]);
    }
}
