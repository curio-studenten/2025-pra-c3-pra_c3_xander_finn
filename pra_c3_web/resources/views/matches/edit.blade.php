@extends('layouts.app')

@section('title', 'Score Invoeren - Schoolvoetbal')

@section('content')
<h1 class="mb-2">Score Invoeren</h1>

<div class="card" style="max-width: 500px;">
    <div class="text-center mb-2">
        <h2 style="font-size: 1.5rem;">
            {{ $match->team1->name }} vs {{ $match->team2->name }}
        </h2>
        <p style="color: #666;">
            {{ $match->start_time ? $match->start_time->format('d-m-Y H:i') : 'Geen tijd' }} | Veld {{ $match->field }}
        </p>
        @if($match->played)
            <span class="badge badge-success">Huidige score: {{ $match->score_team1 }} - {{ $match->score_team2 }}</span>
        @endif
    </div>

    <form method="POST" action="/matches/{{ $match->id }}">
        @csrf
        @method('PUT')

        <div class="grid" style="grid-template-columns: 1fr auto 1fr; gap: 1rem; align-items: center;">
            <div class="form-group text-center">
                <label for="score_team1">{{ $match->team1->name }}</label>
                <input type="number" name="score_team1" id="score_team1" class="form-control text-center"
                       value="{{ old('score_team1', $match->score_team1 ?? 0) }}" min="0" required style="font-size: 2rem; text-align: center;">
            </div>

            <div style="font-size: 2rem; color: #666;">-</div>

            <div class="form-group text-center">
                <label for="score_team2">{{ $match->team2->name }}</label>
                <input type="number" name="score_team2" id="score_team2" class="form-control text-center"
                       value="{{ old('score_team2', $match->score_team2 ?? 0) }}" min="0" required style="font-size: 2rem; text-align: center;">
            </div>
        </div>

        <div class="flex gap-1 mt-2" style="justify-content: center;">
            <button type="submit" class="btn btn-success">💾 Score Opslaan</button>
            <a href="/matches" class="btn btn-secondary">Annuleren</a>
        </div>
    </form>
</div>

<div class="card mt-2" style="max-width: 500px;">
    <h4>Punten Toekenning</h4>
    <ul style="margin-left: 1.5rem;">
        <li><strong>Winst:</strong> 3 punten</li>
        <li><strong>Gelijkspel:</strong> 1 punt</li>
        <li><strong>Verlies:</strong> 0 punten</li>
    </ul>
</div>
@endsection
