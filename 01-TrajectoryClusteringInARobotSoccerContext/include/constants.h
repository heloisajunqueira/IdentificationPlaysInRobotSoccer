// Constants Used throuhg code
// Copyright (C) 2020  Arthur Demarchi

// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

#ifndef INCLUDE_CONSTANTS_H_
#define INCLUDE_CONSTANTS_H_

//defining maximum game length
//6000 cycles for normal games
//4000 cycles for prorrogation
#define MAX_NUMBER_OF_CYCLES 10000

//defining play maximum length
//based purely on empirical observations
//of how long a play takes to happen
#define MAX_PLAY_LENGTH 150

//Constants based on 2D Soccer Simulation Manual used by Robocup
#define PLAYER_BODY 0.085
#define PLAYER_SIZE 0.3
#define PLAYER_AREA (PLAYER_BODY + PLAYER_SIZE)
#define KICK_MARGIN 0.7
#define ERROR_MARGIN 1.05
#define KICKABLE_AREA (PLAYER_AREA + KICK_MARGIN) * ERROR_MARGIN
#define PLAYER_MOVEMENT 1.2 * ERROR_MARGIN
#define DASH_DISTANCE PLAYER_MOVEMENT * 2
#define REACTION_TIME 1.5
#define REACTION 2 * (KICKABLE_AREA) / REACTION_TIME
#define INTERACTION_TRESHOLD 0.4

//after a play is finished there a number of cycles
//between the finalization of play and the referee
//playmode message, wich makes evaluating last
//team on ball hard. This constant determines
//at wich how many cycles in message delay are
//acceptable.
#define CHANGE_PLAYMODE_DELAY 5

//defining boolean for each team
#define LEFT_TEAM 0
#define RIGHT_TEAM 1
#define BALL_TEAM -1

#endif // INCLUDE_CONSTANTS_H_
