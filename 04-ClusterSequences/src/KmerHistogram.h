/*
 * KmerHistogram.h
 *
 *  Created on: Jul 25, 2012
 *  	Modified on: 12/21/2019
 *      Author: Hani Zakaria Girgis, PhD - NCBI/NLM/NIH
 */

#ifndef KMERHISTOGRAM_H_
#define KMERHISTOGRAM_H_

#include <string>
#include <vector>
// Review these include statement
#include <stdio.h>
#include <math.h>
#include <iostream>
#include <tuple>
#include <iomanip>

 #include "Parameters.h"

using namespace std;

template<class I, class V>
class KmerHistogram {

protected:
	/* Fields */
	//static const int maxKeyLength = 15;
	const int k;
	unsigned int maxTableSize;
	int N_DIGITS = 274; // change double mMinusOne[749];

private:
	// [4^0, 4^1, ... , 4^(k-1)]
	//I bases[maxKeyLength];
	//I mMinusOne[4];
	//I bases[];
	//I mMinusOne[];
	unsigned int *bases;
	unsigned int *mMinusOne;
	int digitList['T' + 1];

public:
	/* Methods */
	KmerHistogram(int);
	virtual ~KmerHistogram();

	double hash(const int *);
	double hash(const int *, int);
	void hash(const int *, int, int, vector<double>*);
	V* build(vector<int>*);

	void getKeys(const int *);
	void getKeysDigitFormat(unsigned int keyList[]);
	void printTable(V*);
	void printPythonFormat(V*);

	int getK();
	I getMaxTableSize();



};

#include "KmerHistogram.cpp"

#endif /* KMERHISTOGRAM_H_ */
