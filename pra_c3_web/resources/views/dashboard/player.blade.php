<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Player Dashboard - Betting Platform</title>
    <link rel="stylesheet" href="{{ asset('build/assets/app-BcrHRb7D.css') }}">
</head>
<body>
    <!-- Header -->
    <header class="header">
        <div class="header-content">
            <a href="/" class="logo">Betting Platform</a>
            <nav class="nav-links">
                <a href="http://localhost/2025-pra-c3-pra_c3_xander_finn/pra_c3_web/public/dashboard">Dashboard</a>
                <form method="POST" action="http://localhost/2025-pra-c3-pra_c3_xander_finn/pra_c3_web/public/logout" style="display: inline;">
                    @csrf
                    <button type="submit" style="background: none; border: none; color: white; padding: 0.5rem 1rem; border-radius: 0.5rem; cursor: pointer; transition: all 0.3s ease; background: rgba(255, 255, 255, 0.1);">
                        Logout
                    </button>
                </form>
            </nav>
        </div>
    </header>

    <!-- Main Content -->
    <main class="main-content">
        <!-- Welcome Section -->
        <div class="hero" style="margin-bottom: 3rem;">
            <h1 style="font-size: 2.5rem;">Player Dashboard</h1>
            <p>Welcome back, {{ auth()->user()->name }}! Create and manage your games.</p>
        </div>

        <!-- Stats Cards -->
        <div class="dashboard-grid">
            <div class="stat-card">
                <div class="stat-number">{{ $totalGames ?? 0 }}</div>
                <div class="stat-label">Total Games Created</div>
            </div>
            <div class="stat-card">
                <div class="stat-number">{{ $activeGames ?? 0 }}</div>
                <div class="stat-label">Active Games</div>
            </div>
            <div class="stat-card">
                <div class="stat-number">{{ $totalBets ?? 0 }}</div>
                <div class="stat-label">Bets Placed on Your Games</div>
            </div>
            <div class="stat-card">
                <div class="stat-number">${{ number_format($totalEarnings ?? 0, 2) }}</div>
                <div class="stat-label">Total Earnings</div>
            </div>
        </div>

        <!-- Action Cards -->
        <div class="cards-grid">
            <div class="card">
                <h3>Create New Game</h3>
                <p>Set up a new game for others to bet on. Define the rules, duration, and winning conditions.</p>
                <a href="/games/create" class="btn btn-primary">Create Game</a>
            </div>

            <div class="card">
                <h3>My Games</h3>
                <p>View and manage all your created games. See betting activity and update game statuses.</p>
                <a href="/games" class="btn btn-secondary">View My Games</a>
            </div>

            <div class="card">
                <h3>Earnings Report</h3>
                <p>Track your earnings, view payout history, and analyze your game performance.</p>
                <a href="/earnings" class="btn" style="background: linear-gradient(135deg, #43e97b 0%, #38f9d7 100%); box-shadow: 0 4px 15px rgba(67, 233, 123, 0.4);">View Earnings</a>
            </div>
        </div>

        <!-- Recent Games -->
        <div class="card" style="margin-top: 3rem;">
            <h3 style="margin-bottom: 1.5rem;">Recent Games</h3>
            @if(isset($recentGames) && count($recentGames) > 0)
                <div style="overflow-x: auto;">
                    <table style="width: 100%; border-collapse: collapse;">
                        <thead>
                            <tr style="border-bottom: 2px solid #e2e8f0;">
                                <th style="padding: 1rem; text-align: left;">Game Name</th>
                                <th style="padding: 1rem; text-align: left;">Status</th>
                                <th style="padding: 1rem; text-align: left;">Bets</th>
                                <th style="padding: 1rem; text-align: left;">Created</th>
                            </tr>
                        </thead>
                        <tbody>
                            @foreach($recentGames as $game)
                            <tr style="border-bottom: 1px solid #e2e8f0;">
                                <td style="padding: 1rem; font-weight: 500;">{{ $game->game_name }}</td>
                                <td style="padding: 1rem;">
                                    <span style="padding: 0.25rem 0.75rem; border-radius: 1rem; font-size: 0.875rem;
                                        background: {{ $game->status === 'active' ? '#dcfce7; color: #166534' :
                                                     ($game->status === 'completed' ? '#dbeafe; color: #1d4ed8' : '#fef3c7; color: #92400e') }};">
                                        {{ ucfirst($game->status) }}
                                    </span>
                                </td>
                                <td style="padding: 1rem;">{{ $game->bets_count ?? 0 }}</td>
                                <td style="padding: 1rem; color: #666;">{{ $game->created_at->diffForHumans() }}</td>
                            </tr>
                            @endforeach
                        </tbody>
                    </table>
                </div>
            @else
                <div style="text-align: center; padding: 2rem; color: #666;">
                    <div style="font-size: 3rem; margin-bottom: 1rem;"></div>
                    <p>You haven't created any games yet.</p>
                    <a href="/games/create" class="btn btn-primary" style="margin-top: 1rem;">Create Your First Game</a>
                </div>
            @endif
        </div>
    </main>
</body>
</html>
