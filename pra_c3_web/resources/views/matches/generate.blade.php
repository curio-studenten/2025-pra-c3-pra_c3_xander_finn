@extends('layouts.app')

@section('title', 'Schema Genereren - Schoolvoetbal')

@section('content')
<h1 class="mb-2">Wedstrijdschema Genereren</h1>

@if($teams->count() < 2)
    <div class="alert alert-danger">
        Je hebt minimaal 2 teams nodig om een wedstrijdschema te genereren.
        <a href="/teams/create">Maak eerst meer teams aan.</a>
    </div>
@else
    <div class="card" style="max-width: 600px;">
        <div class="alert alert-danger" style="background-color: #fff3cd; color: #856404; border-color: #ffeeba;">
            <strong>Let op!</strong> Het genereren van een nieuw schema verwijdert alle bestaande wedstrijden en reset alle teampunten naar 0.
        </div>

        <form method="POST" action="/matches/generate">
            @csrf

            <div class="form-group">
                <label for="fields">Aantal Velden</label>
                <select name="fields" id="fields" class="form-control" required>
                    @for($i = 1; $i <= 5; $i++)
                        <option value="{{ $i }}">{{ $i }} {{ $i == 1 ? 'veld' : 'velden' }}</option>
                    @endfor
                </select>
                <small style="color: #666;">Hoeveel wedstrijden kunnen tegelijk gespeeld worden?</small>
            </div>

            <div class="form-group">
                <label for="match_duration">Wedstrijdduur (minuten)</label>
                <input type="number" name="match_duration" id="match_duration" class="form-control" value="15" min="5" max="120" required>
            </div>

            <div class="form-group">
                <label for="break_between">Pauze tussen wedstrijden (minuten)</label>
                <input type="number" name="break_between" id="break_between" class="form-control" value="5" min="0" max="60" required>
            </div>

            <div class="form-group">
                <label for="start_time">Starttijd toernooi</label>
                <input type="datetime-local" name="start_time" id="start_time" class="form-control" value="{{ date('Y-m-d\TH:i') }}" required>
            </div>

            <div class="card" style="background-color: #f8f9fa; margin-bottom: 1rem;">
                <h4>Deelnemende Teams ({{ $teams->count() }})</h4>
                <ul style="margin-left: 1.5rem;">
                    @foreach($teams as $team)
                        <li>{{ $team->name }}</li>
                    @endforeach
                </ul>
                <p style="margin-top: 0.5rem; color: #666;">
                    <strong>Totaal aantal wedstrijden:</strong> {{ ($teams->count() * ($teams->count() - 1)) / 2 }} (halve competitie)
                </p>
            </div>

            <div class="flex gap-1">
                <button type="submit" class="btn btn-success" onclick="return confirm('Weet je zeker dat je een nieuw schema wilt genereren? Alle bestaande wedstrijden worden verwijderd!')">
                    🔄 Schema Genereren
                </button>
                <a href="/matches" class="btn btn-secondary">Annuleren</a>
            </div>
        </form>
    </div>
@endif
@endsection
