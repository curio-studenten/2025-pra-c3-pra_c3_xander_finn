@extends('layouts.app')

@section('title', 'Teams - Schoolvoetbal')

@section('content')
<div class="flex flex-between flex-center mb-2">
    <h1>Teams</h1>
    <a href="/teams/create" class="btn btn-success">➕ Nieuw Team</a>
</div>

<div class="card">
    @if($teams->count() > 0)
        <table class="table">
            <thead>
                <tr>
                    <th>Naam</th>
                    <th>Eigenaar</th>
                    <th>Punten</th>
                    <th>Spelers</th>
                    <th>Acties</th>
                </tr>
            </thead>
            <tbody>
                @foreach($teams as $team)
                    <tr>
                        <td><a href="/teams/{{ $team->id }}">{{ $team->name }}</a></td>
                        <td>{{ $team->creator->name ?? 'Onbekend' }}</td>
                        <td><strong>{{ $team->points }}</strong></td>
                        <td>{{ $team->players->count() }}</td>
                        <td>
                            <a href="/teams/{{ $team->id }}" class="btn btn-primary btn-sm">Bekijken</a>
                            @if($team->creator_id == $player->id)
                                <a href="/teams/{{ $team->id }}/edit" class="btn btn-secondary btn-sm">Bewerken</a>
                            @endif
                            @if($team->creator_id == $player->id || $player->admin)
                                <form action="/teams/{{ $team->id }}" method="POST" style="display: inline;" onsubmit="return confirm('Weet je zeker dat je dit team wilt verwijderen?')">
                                    @csrf
                                    @method('DELETE')
                                    <button type="submit" class="btn btn-danger btn-sm">Verwijderen</button>
                                </form>
                            @endif
                        </td>
                    </tr>
                @endforeach
            </tbody>
        </table>
    @else
        <p class="text-center">Nog geen teams aangemaakt. <a href="/teams/create">Maak het eerste team aan!</a></p>
    @endif
</div>
@endsection
