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
 * Trainer.cpp
 *
 *  Created on: Feb 25, 2020
 *      Author: Hani Z. Girgis, PhD
 */
#include "SynDataGenerator.h"

SynDataGenerator::SynDataGenerator(string fileName, double t, int threadNumIn, std::vector<int> funIndexList) :DataGenerator(fileName) {
	std::cout << "SYNDataGenerator: " << endl;
	threshold = t;
	this->funIndexList = funIndexList;
	threadNum = threadNumIn;
	fillCompositionList();
	generateData();
}

SynDataGenerator::SynDataGenerator(string dbName, string qryName, double t,
		int threadNumIn, std::vector<int> funIndexList) :
		DataGenerator(dbName, qryName, t) {

	threshold = t;
	this->funIndexList = funIndexList;
	threadNum = threadNumIn;
	fillCompositionList();
	generateData();
}

SynDataGenerator::~SynDataGenerator() {
	if (fTable != nullptr) {
		delete fTable;
	}

	if (lTable != nullptr) {
		delete lTable;
	}

	delete[] compositionList;
}

/**
 * Delete data
 * Must be called when all training is finished
 */
void SynDataGenerator::clearData() {
	delete fTable;
	delete lTable;
	fTable = nullptr;
	lTable = nullptr;
}

void SynDataGenerator::fillCompositionList() {

	int N_DIGITS = 274;
	compositionList = new double[N_DIGITS];


	int seqNum = block->size();

	// Calculate nucleotide composition vector
	for (int i = 0; i < seqNum; i++) {
		compositionList[i]++;
	}

	double total = 0.0;
	for (int i = 0; i < seqNum; i++) {
		total += compositionList[i];
	}
	for (int i = 0; i < seqNum; i++) {
		compositionList[i] /= total;
	}
}

void SynDataGenerator::generateData() {
	// Determine histogram data type
	if (maxLength <= std::numeric_limits<int8_t>::max()) {
		std::cout << "A histogram entry is 8 bits." << std::endl;
		generateDataHelper<int8_t>();
	} else if (maxLength <= std::numeric_limits<int16_t>::max()) {
		std::cout << "A histogram entry is 16 bits." << std::endl;
		generateDataHelper<int16_t>();
	} else if (maxLength <= std::numeric_limits<int32_t>::max()) {
		std::cout << "A histogram entry is 32 bits." << std::endl;
		generateDataHelper<int32_t>();
	} else if (maxLength <= std::numeric_limits<int64_t>::max()) {
		std::cout << "A histogram entry is 64 bits." << std::endl;
		generateDataHelper<int64_t>();
	} else {
		std::cout << "Trainer warning: Overflow is possible however unlikely.";
		std::cout << std::endl;
		std::cout << "A histogram entry is 64 bits." << std::endl;
		generateDataHelper<int64_t>();
	}
}

/**
 * Build data
 * ToDo: Add a sequence vs itself.
 * Do not change 0.99 (to 1.0) in the loop because it would add many of the sequence versus itself.
 */
template<class V>
void SynDataGenerator::generateDataHelper() {
	std::cout << "Generating data." << std::endl;


	bool canGenerateNegatives = !Util::isEqual(threshold, 0.0);

	// Calculate mutation rates
	double m = Parameters::getMinId();

	// A list containing positive mutation rates
	vector<double> pstvRateList;
	for (double i = 0.99; i >= threshold; i -= 0.01) {
		pstvRateList.push_back(1.0 - i);
	}
	pstvRateList.shrink_to_fit();
	int pstvRateSize = pstvRateList.size();
std::cout << " Size of keylist not work: nada 1" << std::endl;
	// A list containing negative mutation rates
	vector<double> ngtvRateList;
	int ngtvRateSize;
	if (canGenerateNegatives) {
		for (double j = threshold - 0.01; j >= m; j -= 0.01) {
			ngtvRateList.push_back(1.0 - j);
		}
		ngtvRateList.shrink_to_fit();
		ngtvRateSize = ngtvRateList.size();
	}
    std::cout << " Size of keylist not work: nada 2 " << std::endl;
	int actual = block->size();
	int desired = Parameters::getBlockSize();
	int copyNum = Parameters::getMutPerTemp() / 2.0;
	if (actual < desired) {
		copyNum = copyNum * desired / (double) actual;
	}

	// Generate mutated sequences from each sequence
    KmerHistogram<uint64_t, V> kTable(k);
    KmerHistogram<uint64_t, uint64_t> monoTable(1);
	// uint8_t keyList[histogramSize * k];

    unsigned int *keyList;
    unsigned int sizeKeyList = histogramSize * k;
    std::cout << " Size of keylist not work: " << sizeKeyList << std::endl;
    //keyList = (unsigned int*)malloc (sizeKeyList * sizeof (unsigned int));
    keyList = (unsigned int*)malloc (sizeKeyList * sizeof (unsigned int));
		// new unsigned int[histogramSize * k]; // Will be deleted at the end of this method
	kTable.getKeysDigitFormat(keyList);
	const int statNum =
			funIndexList.empty() ?
			StatisticInfo::getInstance()->getStatNum() :
			funIndexList.size();
	//cout << " statNum: " << statNum << endl;
	// Initialize Matrixes
	int rowNum = 2.0 * copyNum * actual;
	fTable = new Matrix(rowNum, statNum);
	lTable = new Matrix(rowNum, 1);

	if (!canGenerateNegatives) {
		copyNum *= 2;
	}

	int minBlockSize = Parameters::getMutMinBlockSize();
	int maxBlockSize = Parameters::getMutMaxBlockSize();

	bool isSingle = Parameters::getMutSingle();
	bool isBlock = Parameters::getMutBlock();
	bool isTranslocation = Parameters::getMutTranslocation();
	bool isInversion = Parameters::getMutInverstion();
std::cout << "entry actual: " << actual << std::endl;
//#pragma omp parallel for schedule(static) num_threads(threadNum)
	for (int i = 0; i < actual; i++) {
		//std::cout << "i= " << i << " < " << "actual" << actual << std::endl;
		// Set up a mutator
		cout << " Mutator actual: " << i << endl;
		Mutator mutator(block->at(i).second, maxBlockSize, i, minBlockSize);
		if (isSingle) {
			mutator.enableSinglePoint();
		}
		if (isBlock) {
			mutator.enableBlock();
		}
		if (isTranslocation) {
			mutator.enableTranslocation();
		}
		if (isInversion) {
			mutator.enableInverstion();
		}
		//cout << " build 1 " << endl;
		V *h1 = kTable.build(block->at(i).second);

		//cout << " build 2 " << endl;
		uint64_t *mono1 = monoTable.build(block->at(i).second);

		// Iterate over different mutation rates
		// Balance around threshold
		// Generate positive examples (identity score above the threshold)
	#pragma omp parallel for schedule(static) num_threads(threadNum + 2)
		for (int j = 0; j < copyNum; j++) {
			//cout << " Copy num: " << j << endl;
			//std::cout << "j = " << j << " < " << "copyNum" << copyNum << std::endl;
			double mutRate = pstvRateList[(i * copyNum + j) % pstvRateSize];
			// Add 0.5% identical pairs to the data
			//			if (i % 2500 == 0) {
			//				mutRate = 0.0;
			//			}
			//std::cout << "mutRate = " << mutRate << std::endl;
			auto pPstv = mutator.mutateSequence(mutRate);
			V *h2 = kTable.build(pPstv.first);
			uint64_t *mono2 = monoTable.build(pPstv.first);

			Statistician<V> s(histogramSize, k, h1, h2, mono1, mono2,
					compositionList, keyList);
			vector<double> statList;
			statList.reserve(statNum);
			if (funIndexList.empty()) {
				//std::cout << "IF" << std::endl;
				s.calculateAll(statList);
			} else {
				//std::cout << "ELSE" << std::endl;
				s.calculate(funIndexList, statList);
			}
			int r = canGenerateNegatives ?
					2 * i * copyNum + j : i * copyNum + j;
			//cout << "statList size: " << statList.size() << endl;
			fTable->setRow(r, statList);
			lTable->at(r, 0) = pPstv.second;

			delete pPstv.first;
			delete[] h2;
			delete[] mono2;
		}
		
		// Generate negative examples (identity score below the threshold)
		if (canGenerateNegatives) {
			#pragma omp parallel for schedule(static) num_threads(threadNum + 2)
			for (int j = 0; j < copyNum; j++) {
				//cout << " Copy num2: " << j << endl;
				auto pNgtv = mutator.mutateSequence(
						ngtvRateList[(i * copyNum + j) % ngtvRateSize]);
				V *h2 = kTable.build(pNgtv.first);
				uint64_t *mono2 = monoTable.build(pNgtv.first);

				Statistician<V> s(histogramSize, k, h1, h2, mono1, mono2,
						compositionList, keyList);
				vector<double> statList;
				statList.reserve(statNum);
				if (funIndexList.empty()) {
					s.calculateAll(statList);
				} else {
					s.calculate(funIndexList, statList);
				}
				int r = 2 * i * copyNum + j + copyNum;
				fTable->setRow(r, statList);
				lTable->at(r, 0) = pNgtv.second;

				delete pNgtv.first;
				delete[] h2;
				delete[] mono2;
			}
		}

		delete[] h1;
		delete[] mono1;
	}
	std::cout << "END GENERATE DATA: " << actual << std::endl;
	free(keyList);
}
