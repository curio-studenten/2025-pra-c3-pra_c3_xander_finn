@extends('layouts.app')

@section('title', 'Welkom - Schoolvoetbal')

@section('content')
<div class="text-center">
    <div class="card">
        <h1 style="font-size: 2.5rem; color: #2c3e50; margin-bottom: 1rem;">Welkom bij Schoolvoetbal</h1>
        <p style="font-size: 1.2rem; color: #7f8c8d; margin-bottom: 2rem;">
            Het platform voor het organiseren van schoolvoetbaltoernooien
        </p>

        <div class="grid grid-2" style="max-width: 600px; margin: 0 auto;">
            <div class="card" style="background: linear-gradient(135deg, #3498db, #2980b9); color: white;">
                <h3>Teams Beheren</h3>
                <p>Maak teams aan en voeg spelers toe</p>
            </div>
            <div class="card" style="background: linear-gradient(135deg, #27ae60, #219a52); color: white;">
                <h3>Wedstrijden Plannen</h3>
                <p>Genereer automatisch wedstrijdschema's</p>
            </div>
            <div class="card" style="background: linear-gradient(135deg, #e74c3c, #c0392b); color: white;">
                <h3>Scores Bijhouden</h3>
                <p>Voer uitslagen in en bekijk de stand</p>
            </div>
            <div class="card" style="background: linear-gradient(135deg, #9b59b6, #8e44ad); color: white;">
                <h3>API Toegang</h3>
                <p>Koppel met externe applicaties</p>
            </div>
        </div>

        <div class="mt-2">
            <a href="/register" class="btn btn-primary" style="font-size: 1.2rem; padding: 0.75rem 2rem;">Registreer Nu</a>
            <a href="/login" class="btn btn-secondary" style="font-size: 1.2rem; padding: 0.75rem 2rem;">Inloggen</a>
        </div>
    </div>
</div>
@endsection
