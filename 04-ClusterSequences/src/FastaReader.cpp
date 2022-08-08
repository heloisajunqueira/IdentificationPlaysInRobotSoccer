/*
 Identity 2.0 calculates DNA sequence identity scores rapidly without alignment.

 Copyright (C) 2020-2022 Hani Z. Girgis, PhD

 Academic use: Affero General Public License version 1.

 Any restrictions to use for-profit or non-academics: Alternative commercial license is needed.

 This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

 Please contact Dr. Hani Z. Girgis (hzgirgis@buffalo.edu) if you need more information.
 */

/*
 * FastaReader.cpp
 *
 *  Created on: Nov 2, 2019
 *      Author: Hani Zakaria Girgis, PhD
 *
 *
 *  Notes:
 *  + A sequence of all unknown nucleotides or aa's is excluded.
 *  + A sequence is converted to upper case.
 */

#include "FastaReader.h"

FastaReader::FastaReader(std::string fileNameIn, int blockSizeIn,
		long int currentPosIn, int maxLenIn) {
	std::cout << "in FastaReader: " << endl;
	fileName = fileNameIn;
	blockSize = blockSizeIn;

	unknown = Parameters::getUnknown();

	isDone = false;
	currentPos = currentPosIn;
	in = std::ifstream(fileName.c_str());
	if (!in.good()) {
		std::cerr << "Cannot open file: " << fileName << std::endl;
		throw std::exception();
	}

	in.seekg(currentPos);
	if (in.rdstate() & (in.badbit | in.eofbit | in.failbit)) {
		std::cerr << "FastaReader Error: " << std::endl;
		std::cerr << "Cannot move to position: " << currentPos << std::endl;
		throw std::exception();
	}

	maxLen = maxLenIn;

	/*for (int i = 0; i < 128; i++) {
		codeMap[i] = NOT;
	}

	codeMap['A'] = 'A';
	codeMap['C'] = 'C';
	codeMap['G'] = 'G';
	codeMap['T'] = 'T';
	codeMap['N'] = 'N';
	codeMap['R'] = 'G';
	codeMap['Y'] = 'C';
	codeMap['M'] = 'A';
	codeMap['K'] = 'T';
	codeMap['S'] = 'G';
	codeMap['W'] = 'T';
	codeMap['H'] = 'C';
	codeMap['B'] = 'T';
	codeMap['V'] = 'A';
	codeMap['D'] = 'T';
	// Added on 11/19/2020 to enable reading multi-sequence alignments
	codeMap['-'] = '-';*/
}

FastaReader::~FastaReader() {
	in.close();
}

/**
 * Utility function
 */
void FastaReader::deleteBlock(Block *block) {
	for (auto p : *block) {
		delete p.first;
		delete p.second;
	}

	block->clear();
	// Added on 4/25/2020.
	delete block;
}
vector<int> convertLine(string line) {
	int segEnd = line.size();
	int j=0,i=0, count = 0;
	vector<int> line2;
	while(j<segEnd)
	        {
	        	if(line[j] == ',')
	        	{
	        		if(count == 1){ //pega 1 digito
						char aux = (int) line[j-1] - 48;
						line2.push_back(aux);

						count = 0;
						i++;
						j++;
	        		} else if(count == 2){ // pega 2 digitos
						char aux1 = (int) line[j-1] - 48;
						char aux2 = (int) line[j-2] - 48;
						line2.push_back(10*aux2 + aux1);

						count = 0;
						i++;
						j++;

	        		} else if(count==3){ //pega 3 digitos
						char aux1 = (int) line[j-1] - 48;
						char aux2 = (int) line[j-2] - 48;
						char aux3 = (int) line[j-3] - 48;
						line2.push_back(100*aux3 + 10*aux2+ aux1);

						count = 0;
						i++;
						j++;

					} else if(count==4){ //pega 4 digitos
						char aux1 = (int) line[j-1] - 48;
						char aux2 = (int) line[j-2] - 48;
						char aux3 = (int) line[j-3] - 48;
						char aux4 = (int) line[j-4] - 48;
						line2.push_back(1000*aux4 + 100*aux3 + 10*aux2+ aux1);

						count = 0;
						i++;
						j++;
					}

	        	}
	        	else if(count ==0 || count == 1 || count==2 || count==3 ) {
	        		count++;
	        		j++;
	        	}
	      }
	return line2;
}

Block* FastaReader::read() {
	std::cout << "FastaReader::read(): " << endl;
	if (!isStillReading()) {
		std::cerr << "Cannot call read on a done FastaReader object.";
		std::cerr << std::endl;
		throw std::exception();
	}

	bool isFirst = true;
	//string *base = new string("");
	string *info;
	Block *b = new Block();
	vector<int> line2;
	vector<int>* base = new vector<int>();
	b->reserve(blockSize);
	int counter = 0;
	while (in.good() && counter < blockSize) {
		string line;
		getline(in, line);
		int len = line.length();

		if (line[len - 1] == '\r') {
			len--;
			line.pop_back();
		}

		if (line[0] == '>') {
			if (!isFirst) {
				if (line2.size() > maxLen) {
					maxLen = line2.size();
				}

				//if (isAllInvalid(base)) {
					//delete info;
					//delete base;
				//} else {
					//line2->shrink_to_fit();
				b->push_back(make_pair(info, base));
				counter++;
				//}

				if (counter < blockSize) {
					info = new string(line);
					base = new vector<int>();
					base->reserve(maxLen);
				} else {
					// Go back one line
					in.seekg(-1 - len, in.cur);
				}
			} else {
				info = new string(line);
				isFirst = false;
			}
		} else {
			// Convert non-traditional bases to traditional ones

			//line2->append(line);
			line2 = convertLine(line);
;
			//for(int t = 0 ; t < line2.size(); t++)
			//{
				//cout << (int)line2[t] << ";";
				//base->push_back(line2[i]);
			//}

			for(int z = 0 ; z < line2.size(); z++)
			{
				base->push_back(line2[z]);
			}
		}
	}
	// Record current position
	currentPos = in.tellg();

	// The file ended and the last sequence has not been added to
	// the block
	if (!in.good()) {

		b->push_back(make_pair(info, base));
	}
	isDone = true;
	//b->shrink_to_fit();

	return b;
}

/**
 * Check if the sequence consists of all unknown nucleotides or aa's
 */
bool FastaReader::isAllInvalid(const string *base) {
	bool r = true;
	const char *arr = base->c_str();
	int len = base->length();
	for (int i = 0; i < len; i++) {
		if (arr[i] != unknown) {
			r = false;
			break;
		}
	}

	return r;
}

bool FastaReader::isStillReading() {
	return !isDone;
}

long int FastaReader::getCurrentPos() {
	return currentPos;
}

int FastaReader::getMaxLen() {
	return maxLen;
}

/**
 * Start reading from the beginning of a file
 */
void FastaReader::restart() {
	currentPos = 0;
	isDone = false;
	in.clear();
	in.seekg(currentPos);
}

void FastaReader::setBlockSize(int newBlockSize) {
	blockSize = newBlockSize;
}
