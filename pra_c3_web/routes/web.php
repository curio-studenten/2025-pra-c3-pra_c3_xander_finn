<?php

use Illuminate\Support\Facades\Route;
use App\Http\Controllers\AuthController;
use App\Http\Controllers\DashboardController;
use App\Http\Controllers\TeamController;
use App\Http\Controllers\MatchController;
use App\Http\Controllers\ApiController;

// Publieke homepage
Route::get('/', function () {
    return view('welcome');
});

// Auth routes
Route::get('/register', [AuthController::class, 'showRegister']);
Route::post('/register', [AuthController::class, 'register']);
Route::get('/login', [AuthController::class, 'showLogin']);
Route::post('/login', [AuthController::class, 'login']);
Route::get('/logout', [AuthController::class, 'logout']);

// Dashboard
Route::get('/dashboard', [DashboardController::class, 'index']);

// Teams
Route::get('/teams', [TeamController::class, 'index']);
Route::get('/teams/create', [TeamController::class, 'create']);
Route::post('/teams', [TeamController::class, 'store']);
Route::get('/teams/{id}', [TeamController::class, 'show']);
Route::get('/teams/{id}/edit', [TeamController::class, 'edit']);
Route::put('/teams/{id}', [TeamController::class, 'update']);
Route::delete('/teams/{id}', [TeamController::class, 'destroy']);
Route::post('/teams/{id}/add-player', [TeamController::class, 'addPlayer']);
Route::delete('/teams/{teamId}/remove-player/{playerId}', [TeamController::class, 'removePlayer']);

// Matches
Route::get('/matches', [MatchController::class, 'index']);
Route::get('/matches/generate', [MatchController::class, 'showGenerate']);
Route::post('/matches/generate', [MatchController::class, 'generate']);
Route::get('/matches/{id}/edit', [MatchController::class, 'edit']);
Route::put('/matches/{id}', [MatchController::class, 'update']);
Route::delete('/matches/{id}', [MatchController::class, 'destroy']);

// API routes (voor C# WinUI gok applicatie)
Route::get('/api/matches.php', [ApiController::class, 'apiMatches']);
Route::get('/api/results.php', [ApiController::class, 'apiResults']);
Route::get('/api/goals.php', [ApiController::class, 'apiGoals']);
