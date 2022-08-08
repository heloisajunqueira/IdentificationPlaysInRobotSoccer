// struct with attributes about how to format software output.
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

#ifndef INCLUDE_FILEFORMAT_H_
#define INCLUDE_FILEFORMAT_H_

// Includes.
#include <iostream>
#include <vector>
#include "tools.h"

// Defines for the multi optional attributes
#define FF_GROUPBY_GAME 0
#define FF_GROUPBY_TEAMS 1
#define FF_GROUPBY_SINGLE 2
#define FF_FILTERBY_FINAL 0
#define FF_FILTERBY_FINAL_O 1
#define FF_SEPARATOR_COMMA 0
#define FF_SEPARATOR_BLANK 1

// struct with attributes about how to format software output.
struct fileFormat
{
    int groupBy;           // Determines how output file tree will be structured.
    int filterBy;          // Determines wich data will be save into files.
    bool header;           // Determines if tables will have header.
    bool rowId;            // Determines if tables will have ids on rows.
    bool playLength;       // Determines if tables will have a play length column
    bool team;             // Determines if tables will have a team column.
    bool cycle;            // Determines if tables will have a cycle column.
    bool player;           // Determines if tables will habe a player column.
    bool traclus;          // Determines if should use traclus compatible format
    std::string separator; // The string separator that will be used for tables
    bool halfField;        // Determines if data will suffer transformations to be placed
                           // as if plays were happening from the same side of field.
};

// A Terminal interface to gather values from user input
// and assign them to a fileFormat struct returning it.
//  Return:
//      fileFormat: Struct created with values gather from user input.
//  Output:
//      A fileFormat struct will be created, this will be a endeless
//      loop until the user can create the scruct successfully.
//  Example:
//      fileFormatUserInstancer();
fileFormat fileFormatUserInstancer();

#endif // INCLUDE_FILEFORMAT_H_