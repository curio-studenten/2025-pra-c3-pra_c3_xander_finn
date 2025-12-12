@extends('layouts.app')

@section('title', $team->name . ' - Schoolvoetbal')

@section('content')
<div class="flex flex-between flex-center mb-2">
    <h1>{{ $team->name }}</h1>
    <div class="flex gap-1">
        @if($team->creator_id == $player->id)
            <a href="/teams/{{ $team->id }}/edit" class="btn btn-secondary">✏️ Bewerken</a>
        @endif
        <a href="/teams" class="btn btn-primary">← Terug naar Teams</a>
    </div>
</div>

<div class="grid grid-2">
    <div class="card">
        <h3 class="card-title">📊 Team Info</h3>
        <p><strong>Naam:</strong> {{ $team->name }}</p>
        <p><strong>Eigenaar:</strong> {{ $team->creator->name ?? 'Onbekend' }}</p>
        <p><strong>Punten:</strong> <span style="font-size: 1.5rem; color: #27ae60;">{{ $team->points }}</span></p>
        <p><strong>Aangemaakt:</strong> {{ $team->created_at->format('d-m-Y H:i') }}</p>
    </div>

    <div class="card">
        <h3 class="card-title">👥 Spelers ({{ $team->players->count() }})</h3>
        @if($team->players->count() > 0)
            <ul style="list-style: none;">
                @foreach($team->players as $teamPlayer)
                    <li class="flex flex-between flex-center" style="padding: 0.5rem 0; border-bottom: 1px solid #eee;">
                        <span>
                            {{ $teamPlayer->name }}
                            @if($teamPlayer->admin)
                                <span class="badge badge-admin">Admin</span>
                            @endif
                        </span>
                        @if($team->creator_id == $player->id && $teamPlayer->id != $team->creator_id)
                            <form action="/teams/{{ $team->id }}/remove-player/{{ $teamPlayer->id }}" method="POST" onsubmit="return confirm('Weet je zeker dat je deze speler wilt verwijderen uit het team?')">
                                @csrf
                                @method('DELETE')
                                <button type="submit" class="btn btn-danger btn-sm">Verwijderen</button>
                            </form>
                        @endif
                    </li>
                @endforeach
            </ul>
        @else
            <p>Nog geen spelers in dit team.</p>
        @endif

        @if($team->creator_id == $player->id)
            <hr style="margin: 1rem 0;">
            <h4>Speler Toevoegen</h4>
            <form method="POST" action="/teams/{{ $team->id }}/add-player" class="flex gap-1 mt-1">
                @csrf
                <select name="player_id" class="form-control" required>
                    <option value="">Selecteer een speler...</option>
                    @foreach($allPlayers as $availablePlayer)
                        @if($availablePlayer->team_id != $team->id)
                            <option value="{{ $availablePlayer->id }}">{{ $availablePlayer->name }} ({{ $availablePlayer->email }})</option>
                        @endif
                    @endforeach
                </select>
                <button type="submit" class="btn btn-success">Toevoegen</button>
            </form>
        @endif
    </div>
</div>
@endsection
