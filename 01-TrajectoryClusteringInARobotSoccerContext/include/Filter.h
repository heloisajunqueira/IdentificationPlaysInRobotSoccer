// Class used to filter read/Interpreted data written by 'Reader' class
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

#ifndef INCLUDE_FILTER_H_
#define INCLUDE_FILTER_H_

// Includes.
#include <vector>
#include <fstream>
#include <string>
#include <iostream>
#include <math.h>
#include "tools.h"
#include "constants.h"
#include "fileFormat.h"

// Class used to filter read/Interpreted data written by 'Reader' class.
// Data is read from disk, 'Reader' is not needed to be instanciated or
// referenced as longs as its output is on Disk on the path used by this class.
// It has a root Dir from wich it will be read all files, and can output data
// in two different files for each file it read.
// Example:
//   fileFormat format = fileFormatUserInstancer();
//   Filter filter(format);
//   filter.filterDir("MyPC/Path/To/Data/Root/Dir/To/Filter")
class Filter
{
private:
	// Output format Attribute.
	fileFormat outputFormat; //Sets output data file tree and content

	// System Paths related Attributes.
	// 1. Input data root dir.
	// 2. Current Input data dir.
	// 3. Current Input data path.
	// 4. Current input data file name.
	// 5. Current Output Dir.
	// 6. Current Output file name.
	// 7. CUrrent Second output file name.
	std::string rootDir;
	std::string inputDir;
	std::string inputPath;
	std::string inputName;
	std::string outputDir;
	std::string outputPath;
	std::string outputSecondPath;

	// Data Tables Attributes.
	// 1. data: The raw data loaded from a csv file plus infered info.
	// 2. plays: set of plays inits, ends and their offesinve teams.
	// 3. filtered: Data filtered using said infered info and plays boundaries.
	// 4. paths: Sets of Path took by each player during each play.
	std::vector<std::vector<float>> data;
	std::vector<std::vector<int>> plays;
	std::vector<std::vector<float>> filtered;
	std::vector<std::vector<float>> paths;

	// Based on output format attribute, a root input directory and a input file path
	// set all system paths for output and input in the class inner attributes
	// output format attribute should be already setted prior use of this function.
	//  Args:
	//		const std::string &inputPath: path for the current file input.
	//		const std::string &rootDir: Path for the root dir of input files.
	//  Output:
	//      All inner attributes related to system paths should be setted.
	void setPaths(const std::string &inputPath, const std::string &rootDir);

	// sub module of setPaths to set the data input paths attributes.
	// Args:
	//	const std::string &inputPath: path for the current file input.
	// Output:
	//	Input path inner attribute should be setted.
	void setInputPath(const std::string &inputPat);

	// sub module of setPaths to set the data output path for a single file.
	// Output:
	//	output data path inner attributes should be setted.
	void setSinglePath();

	// sub module of setPaths to set the data output path for game file structure tree.
	// Output:
	//	output data path inner attributes should be setted.
	void setPerGamePath();

	// sub module of setPaths to set the data output path for team file structure tree.
	// Output:
	//	output data path inner attributes should be setted.
	void setPerTeamPath();

	// Using Inner Path attributes reads data from disk and saves t
	// into the 'data' attribute.
	// Output:
	//	Attribute data will be filled with read data.
	//	Columns that reference ball possession will be fillled with zero.
	void loadData();

	// sub module of loadData to transform the strings interpreted as 'playmode'
	// to a integer for a simplified sotorage of information into the data attribute.
	// Args:
	//	const std::string &playmode: playmode string to be converted to integer.
	// return:
	//	int: integer obtained from playmode to integer table.
	int playmodeToInt(const std::string &playmode);

	// Using a set a rules and the filled data attributes
	// generate data about ball possession. This data will be
	// save on the data attribute it self, in columns that
	// were left empty by loadData.
	// Output:
	//	data columns that are about ball possession will be correctly refiled.
	void evalHold();

	// Using data attributes set the plays attribute
	// each play is composed by a init cycle, a
	// end cycle and the info about whose the offensive team
	// Output:
	//  Fills the inner attribute Plays.
	void setPlays();

	// sub module of setPlays used to discover if data line is the end of play.
	// returns true only for one line of each cycle (cycles have 23 data lines).
	// Args:
	//	int i: index of a data line.
	// return:
	//	bool: true when the data line in question is the end of a play.
	bool isAnEnd(int i);

	// sub module of setPlays used to discover the begin of play based ont
	// its end and data attribute.
	// Args:
	//	int i: index of a dataline that is a end of play.
	// return:
	//	int: dataline index that is the begining of the play with end in dataline index 'i.
	int lookForBegin(int i);

	// sub module of setPlays used to discover the offenssive team of
	// a play.
	// Args:
	//	int playmode: a integer that refers the final playmode of that play.
	// return:
	//	int: 0 for left team, 1 for right team.
	int getAttacker(int playmdoe);

	// Uses data attribute and plays attribute  to fill
	// the fltered attribute, wich is the same as data
	// but only for the plays selected.
	// Output:
	//	Inner attribute filtered is setted.
	void setFiltered();

	// User the filtered attribute to create and fill the
	// paths attribute.
	// Output:
	//	inner attribute paths is filled.
	void createPaths();

	// Saves the paths attribute data into one or two output files.
	// It uses paths, plays and the system paths attributes.
	// Output:
	//	one or two files will be written into disk.
	void saveFile();

	// sub module of saveFile that is used to write a header into a file
	// it also checks if file is empty before doind so.
	// Output:
	//	Header is written into a file.
	void writeHeaderIfEmpty();

	// sub module of saveFile that gets from plays wich is the
	// offessinve team in the path that is being written.
	// Args:
	//	unsigned int i: index of path that is being written
	// return;
	//	bool: false for left team, true for right team.
	bool getPathOffensiveTeam(unsigned int i);

	// sub module of saveFile that saves the actual data of the
	// paths into the file.
	// Output:
	//	Major part of paths data is written into the file.
	void writeBody();

public:
	// Default constructor cant be used.
	// all attributes and all methods, but one, are private
	// because they need to be executed in that certain order and the
	// single public method ensures that.
	// Since everything is private a default constructor has no point
	Filter() = delete;

	// Constructor has a initializer list to set output format
	// Args:
	//	fileFormat format: the struct fileFormat used to determines the output format.
	// output:
	//	class is instanciated.
	Filter(fileFormat format) : outputFormat(format){};

	// getter for outputRootDir, wich ironically its not an attribute
	// from this Class, but can be easily infered from input rootDir
	// Return:
	//	std::string: root dir in wich all output tree is.
	// Example:
	//	filter.ggetOutputRootDir();
	std::string getOutputRootDir() { return rootDir.substr(0, rootDir.rfind("/", rootDir.size() - 2)) + "/filtered"; }

	// Filters all files in a root directory reccursevely and saves the filteres
	// data in output files.
	// Avoid reutilizing in the same directory without clearing output, it append instead
	// of overwritten.
	// Args:
	//	const std::string &rootDir: the path to the root directory in wich are the files.
	// Output:
	//	All attributes are filled and a bunch of files are written into disk.
	// Example:
	//	filter.filterDir("Path/TO/DATA/FILES")
	void filterDir(const std::string &rootDir = "../data/output/unfiltered");
};

#endif // INCLUDE_FILTER_H_