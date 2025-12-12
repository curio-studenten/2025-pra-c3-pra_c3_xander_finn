@extends('layouts.app')

@section('title', 'Inloggen - Schoolvoetbal')

@section('content')
<div style="max-width: 400px; margin: 0 auto;">
    <div class="card">
        <h2 class="card-title text-center">Inloggen</h2>
        
        <form method="POST" action="/login">
            @csrf
            
            <div class="form-group">
                <label for="email">E-mailadres</label>
                <input type="email" name="email" id="email" class="form-control" value="{{ old('email') }}" required autofocus>
            </div>
            
            <div class="form-group">
                <label for="password">Wachtwoord</label>
                <input type="password" name="password" id="password" class="form-control" required>
            </div>
            
            <button type="submit" class="btn btn-primary" style="width: 100%;">Inloggen</button>
        </form>
        
        <p class="text-center mt-2">
            Nog geen account? <a href="/register">Registreer hier</a>
        </p>
    </div>
</div>
@endsection
