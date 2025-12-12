<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Gambler Dashboard - Betting Platform</title>
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
            <h1 style="font-size: 2.5rem;">Gambler Dashboard</h1>
            <p>Welcome back, {{ auth()->user()->name }}! Find exciting games to bet on.</p>
        </div>

        <!-- Stats Cards -->
        <div class="dashboard-grid">
            <div class="stat-card">
                <div class="stat-number">{{ $totalBets ?? 0 }}</div>
                <div class="stat-label">Total Bets Placed</div>
            </div>
            <div class="stat-card">
                <div class="stat-number">{{ $activeBets ?? 0 }}</div>
                <div class="stat-label">Active Bets</div>
            </div>
            <div class="stat-card">
                <div class="stat-number">{{ $wonBets ?? 0 }}</div>
                <div class="stat-label">Bets Won</div>
            </div>
            <div class="stat-card">
                <div class="stat-number">${{ number_format($totalWinnings ?? 0, 2) }}</div>
                <div class="stat-label">Total Winnings</div>
            </div>
        </div>

        <!-- Action Cards -->
        <div class="cards-grid">
            <div class="card">
                <h3>Browse Games</h3>
                <p>Explore active games created by players. Find interesting challenges to bet on.</p>
                <a href="/games/browse" class="btn btn-primary">Browse Games</a>
            </div>

            <div class="card">
                <h3>My Bets</h3>
                <p>Track all your placed bets, their status, and potential winnings.</p>
                <a href="/bets" class="btn btn-secondary">View My Bets</a>
            </div>

            <div class="card">
                <h3>Winnings History</h3>
                <p>Review your betting performance and track your winnings over time.</p>
                <a href="/winnings" class="btn" style="background: linear-gradient(135deg, #43e97b 0%, #38f9d7 100%); box-shadow: 0 4px 15px rgba(67, 233, 123, 0.4);">View Winnings</a>
            </div>
        </div>

        <!-- Available Games -->
        <div class="card" style="margin-top: 3rem;">
            <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem;">
                <h3>Hot Games</h3>
                <a href="/games/browse" style="color: #667eea; text-decoration: none; font-weight: 500;">View All →</a>
            </div>

            @if(isset($availableGames) && count($availableGames) > 0)
                <div class="cards-grid" style="margin: 0;">
                    @foreach($availableGames->take(3) as $game)
                    <div class="card" style="margin: 0; padding: 1.5rem;">
                        <h4 style="margin-bottom: 0.75rem; color: #333;">{{ $game->game_name }}</h4>
                        <p style="color: #666; font-size: 0.9rem; margin-bottom: 1rem;">
                            Created by {{ $game->player->name ?? 'Player' }}
                        </p>

                        <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1rem; font-size: 0.875rem;">
                            <span style="color: #666;">Bets: {{ $game->bets_count ?? 0 }}</span>
                            <span style="padding: 0.25rem 0.75rem; border-radius: 1rem; background: #dcfce7; color: #166534;">
                                {{ ucfirst($game->status) }}
                            </span>
                        </div>

                        <a href="/games/{{ $game->id }}/bet" class="btn btn-primary" style="width: 100%; font-size: 0.875rem;">
                            Place Bet
                        </a>
                    </div>
                    @endforeach
                </div>
            @else
                <div style="text-align: center; padding: 2rem; color: #666;">
                    <div style="font-size: 3rem; margin-bottom: 1rem;"></div>
                    <p>No games available at the moment.</p>
                    <p style="font-size: 0.875rem; margin-top: 0.5rem;">Check back later for new betting opportunities!</p>
                </div>
            @endif
        </div>

        <!-- Recent Bets -->
        <div class="card" style="margin-top: 2rem;">
            <h3 style="margin-bottom: 1.5rem;">Recent Bets</h3>
            @if(isset($recentBets) && count($recentBets) > 0)
                <div style="overflow-x: auto;">
                    <table style="width: 100%; border-collapse: collapse;">
                        <thead>
                            <tr style="border-bottom: 2px solid #e2e8f0;">
                                <th style="padding: 1rem; text-align: left;">Game</th>
                                <th style="padding: 1rem; text-align: left;">Bet Amount</th>
                                <th style="padding: 1rem; text-align: left;">Prediction</th>
                                <th style="padding: 1rem; text-align: left;">Status</th>
                                <th style="padding: 1rem; text-align: left;">Placed</th>
                            </tr>
                        </thead>
                        <tbody>
                            @foreach($recentBets as $bet)
                            <tr style="border-bottom: 1px solid #e2e8f0;">
                                <td style="padding: 1rem; font-weight: 500;">{{ $bet->game->game_name ?? 'Game' }}</td>
                                <td style="padding: 1rem; color: #667eea; font-weight: 500;">${{ number_format($bet->amount, 2) }}</td>
                                <td style="padding: 1rem;">{{ $bet->prediction }}</td>
                                <td style="padding: 1rem;">
                                    <span style="padding: 0.25rem 0.75rem; border-radius: 1rem; font-size: 0.875rem;
                                        background: {{ $bet->status === 'won' ? '#dcfce7; color: #166534' :
                                                     ($bet->status === 'lost' ? '#fee2e2; color: #dc2626' : '#fef3c7; color: #92400e') }};">
                                        {{ ucfirst($bet->status) }}
                                    </span>
                                </td>
                                <td style="padding: 1rem; color: #666;">{{ $bet->created_at->diffForHumans() }}</td>
                            </tr>
                            @endforeach
                        </tbody>
                    </table>
                </div>
            @else
                <div style="text-align: center; padding: 2rem; color: #666;">
                    <div style="font-size: 3rem; margin-bottom: 1rem;"></div>
                    <p>You haven't placed any bets yet.</p>
                    <a href="/games/browse" class="btn btn-primary" style="margin-top: 1rem;">Place Your First Bet</a>
                </div>
            @endif
        </div>
    </main>
</body>
</html>
