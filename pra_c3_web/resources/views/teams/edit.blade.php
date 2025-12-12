@extends('layouts.app')

@section('title', 'Team Bewerken - Schoolvoetbal')

@section('content')
<h1 class="mb-2">Team Bewerken: {{ $team->name }}</h1>

<div class="card" style="max-width: 500px;">
    <form method="POST" action="/teams/{{ $team->id }}">
        @csrf
        @method('PUT')

        <div class="form-group">
            <label for="name">Teamnaam</label>
            <input type="text" name="name" id="name" class="form-control" value="{{ old('name', $team->name) }}" required>
        </div>

        <div class="flex gap-1">
            <button type="submit" class="btn btn-success">Opslaan</button>
            <a href="/teams/{{ $team->id }}" class="btn btn-secondary">Annuleren</a>
        </div>
    </form>
</div>
@endsection
