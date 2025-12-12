<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class Team extends Model
{
    use HasFactory;

    protected $fillable = [
        'name',
        'creator_id',
        'points',
    ];

    public function creator()
    {
        return $this->belongsTo(Player::class, 'creator_id');
    }

    public function players()
    {
        return $this->hasMany(Player::class);
    }

    public function homeMatches()
    {
        return $this->hasMany(GameMatch::class, 'team1_id');
    }

    public function awayMatches()
    {
        return $this->hasMany(GameMatch::class, 'team2_id');
    }
}
