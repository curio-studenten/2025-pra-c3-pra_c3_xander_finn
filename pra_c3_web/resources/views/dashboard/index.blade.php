@extends('layouts.app')

@section('title', 'Dashboard - Schoolvoetbal')

@section('content')
<h1 class="mb-2">Welkom, {{ $player->name }}!</h1>

@if($player->admin)
    <span class="badge badge-admin mb-2">Administrator</span>
@endif

<div class="grid grid-2">
    <div class="card">
        <h3 class="card-title">Top 5 Teams</h3>
        @if($topTeams->count() > 0)
            <table class="table">
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Team</th>
                        <th>Punten</th>
                    </tr>
                </thead>
                <tbody>
                    @foreach($topTeams as $index => $team)
                        <tr>
                            <td>{{ $index + 1 }}</td>
                            <td><a href="/teams/{{ $team->id }}">{{ $team->name }}</a></td>
                            <td><strong>{{ $team->points }}</strong></td>
                        </tr>
                    @endforeach
                </tbody>
            </table>
        @else
            <p>Nog geen teams aangemaakt.</p>
        @endif
        <a href="/teams" class="btn btn-primary mt-2">Alle Teams Bekijken</a>
    </div>

    <div class="card">
        <h3 class="card-title">Snelle Acties</h3>
        <div class="flex" style="flex-direction: column; gap: 0.5rem;">
            <a href="/teams/create" class="btn btn-success"> Nieuw Team Aanmaken</a>
            <a href="/matches" class="btn btn-primary"> Wedstrijden Bekijken</a>
            @if($player->admin)
                <a href="/matches/generate" class="btn btn-secondary"> Wedstrijdschema Genereren</a>
            @endif
        </div>
    </div>

    <div class="card">
        <h3 class="card-title"> Mijn Gegevens</h3>
        <p><strong>Naam:</strong> {{ $player->name }}</p>
        <p><strong>Email:</strong> {{ $player->email }}</p>
        <p><strong>Team:</strong> {{ $player->team ? $player->team->name : 'Geen team' }}</p>
    </div>

    @if($player->admin)
    <div class="card" style="border-left: 4px solid #9b59b6;">
        <h3 class="card-title"> Beheerder Functies</h3>
        <p>Als beheerder kun je:</p>
        <ul style="margin-left: 1.5rem; margin-bottom: 1rem;">
            <li>Alle teams verwijderen</li>
            <li>Wedstrijdschema's genereren</li>
            <li>Scores invoeren</li>
        </ul>
        <a href="/matches/generate" class="btn btn-primary">Naar Schema Generator</a>
    </div>
    @endif
</div>
@endsection
