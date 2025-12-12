@extends('layouts.app')

@section('title', 'Registreren - Schoolvoetbal')

@section('content')
<div style="max-width: 400px; margin: 0 auto;">
    <div class="card">
        <h2 class="card-title text-center">Account Aanmaken</h2>

        <form method="POST" action="/register">
            @csrf

            <div class="form-group">
                <label for="name">Naam</label>
                <input type="text" name="name" id="name" class="form-control" value="{{ old('name') }}" required autofocus>
            </div>

            <div class="form-group">
                <label for="email">E-mailadres</label>
                <input type="email" name="email" id="email" class="form-control" value="{{ old('email') }}" required>
            </div>

            <div class="form-group">
                <label for="password">Wachtwoord</label>
                <input type="password" name="password" id="password" class="form-control" required minlength="6">
            </div>

            <div class="form-group">
                <label for="password_confirmation">Wachtwoord bevestigen</label>
                <input type="password" name="password_confirmation" id="password_confirmation" class="form-control" required>
            </div>

            <button type="submit" class="btn btn-success" style="width: 100%;">Registreren</button>
        </form>

        <p class="text-center mt-2">
            Heb je al een account? <a href="/login">Log hier in</a>
        </p>
    </div>
</div>
@endsection
