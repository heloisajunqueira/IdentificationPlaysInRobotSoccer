// Class used to read from Robocup log and save interpreted data into csv.
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

#ifndef INCLUDE_READER_H_
#define INCLUDE_READER_H_

// Includes
#include "constants.h"
#include "tools.h"
#include <filesystem>
#include <fstream>
#include <iostream>
#include <string>
#include <vector>

// Class used to Read data from robocup log and rewritte into a more readable csv file.
// Example:
//   Reader reader();
//   reader.readDir("MyPC/Path/To/Data/Root/Dir/To/Filter")
class Reader
{
private:
	// Paths
	// paths are setted and stored to avoid overhead
	std::string gameDir;
	std::string saveDir;
	std::string rcgName;
	std::string rcgPath;
	std::string csvPath;

	// Data
	// data structure used for loading from file to memmory
	std::vector<std::string> csvData;

private:
	// sets all paths a single time before loading and writing
	// Args:
	//	const std::string &rcgPath: Path to the file that should be read.
	//	const std::string &rootDir: Root Dir of the file files folders that should be read.
	// Output:
	//	All paths attributes are setted.
	void setAllPaths(const std::string &rcgPath, const std::string &rootDir);

	// loads data from files and format them in a more readable manner.
	// Output:
	//	csvData attribute is filled with data from files.
	void loadRcg();

	// saves data in to new file.
	// Output:
	//	File is written into disk.
	void saveCsv();

public:
	// Reads all files in a root directory reccursevely and saves the reformatted
	// data in output files.
	// Args:
	//	const std::string &rootDir: the path to the root directory in wich are the files.
	// Output:
	//	All attributes are filled and a bunch of files are written into disk.
	void readDir(const std::string &rootDir = "../data/input");
};

#endif // INCLUDE_READER_H_