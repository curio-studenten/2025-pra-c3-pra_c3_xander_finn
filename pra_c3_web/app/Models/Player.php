<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Database\Eloquent\Model;

class Player extends Model
{
    use HasFactory;

    protected $fillable = [
        'name',
        'email',
        'password',
        'admin',
        'team_id',
        'api_key',
    ];

    protected $hidden = [
        'password',
    ];

    protected $casts = [
        'admin' => 'boolean',
    ];

    public function team()
    {
        return $this->belongsTo(Team::class);
    }

    public function createdTeams()
    {
        return $this->hasMany(Team::class, 'creator_id');
    }
}
