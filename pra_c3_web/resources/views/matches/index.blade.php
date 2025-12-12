@extends('layouts.app')

@section('title', 'Wedstrijden - Schoolvoetbal')

@section('content')
<div class="flex flex-between flex-center mb-2">
    <h1>Wedstrijden</h1>
    @if($player->admin)
        <a href="/matches/generate" class="btn btn-success">🔄 Schema Genereren</a>
    @endif
</div>

<div class="card">
    @if($matches->count() > 0)
        <table class="table">
            <thead>
                <tr>
                    <th>Tijd</th>
                    <th>Veld</th>
                    <th>Thuis</th>
                    <th>Uit</th>
                    <th>Score</th>
                    <th>Status</th>
                    @if($player->admin)
                        <th>Actie</th>
                    @endif
                </tr>
            </thead>
            <tbody>
                @foreach($matches as $match)
                    <tr>
                        <td>{{ $match->start_time ? $match->start_time->format('d-m H:i') : '-' }}</td>
                        <td>Veld {{ $match->field }}</td>
                        <td><a href="/teams/{{ $match->team1_id }}">{{ $match->team1->name }}</a></td>
                        <td><a href="/teams/{{ $match->team2_id }}">{{ $match->team2->name }}</a></td>
                        <td>
                            @if($match->played)
                                <strong>{{ $match->score_team1 }} - {{ $match->score_team2 }}</strong>
                            @else
                                <span style="color: #999;">- : -</span>
                            @endif
                        </td>
                        <td>
                            @if($match->played)
                                <span class="badge badge-success">Gespeeld</span>
                            @else
                                <span class="badge badge-warning">Gepland</span>
                            @endif
                        </td>
                        @if($player->admin)
                            <td>
                                <a href="/matches/{{ $match->id }}/edit" class="btn btn-primary btn-sm">Score Invoeren</a>
                            </td>
                        @endif
                    </tr>
                @endforeach
            </tbody>
        </table>
    @else
        <p class="text-center">
            Nog geen wedstrijden gepland.
            @if($player->admin)
                <a href="/matches/generate">Genereer nu een schema!</a>
            @endif
        </p>
    @endif
</div>
@endsection
