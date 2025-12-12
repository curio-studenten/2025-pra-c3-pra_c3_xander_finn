@extends('layouts.app')

@section('title', 'Nieuw Team - Schoolvoetbal')

@section('content')
<h1 class="mb-2">Nieuw Team Aanmaken</h1>

<div class="card" style="max-width: 500px;">
    <form method="POST" action="/teams">
        @csrf

        <div class="form-group">
            <label for="name">Teamnaam</label>
            <input type="text" name="name" id="name" class="form-control" value="{{ old('name') }}" required autofocus placeholder="Bijv. FC De Kampioenen">
        </div>

        <div class="flex gap-1">
            <button type="submit" class="btn btn-success">Team Aanmaken</button>
            <a href="/teams" class="btn btn-secondary">Annuleren</a>
        </div>
    </form>
</div>
@endsection
