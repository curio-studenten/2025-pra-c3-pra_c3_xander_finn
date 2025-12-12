<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class GameMatch extends Model
{
    use HasFactory;

    protected $table = 'matches';

    protected $fillable = [
        'team1_id',
        'team2_id',
        'score_team1',
        'score_team2',
        'field',
        'start_time',
        'played',
    ];

    protected $casts = [
        'start_time' => 'datetime',
        'played' => 'boolean',
    ];

    public function team1()
    {
        return $this->belongsTo(Team::class, 'team1_id');
    }

    public function team2()
    {
        return $this->belongsTo(Team::class, 'team2_id');
    }
}
