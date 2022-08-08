//TO DO
//mkdir data before run
//files per team

#include "Reader.h"
#include "Filter.h"
#include "fileFormat.h"

int main(int argc, char *argv[])
{

	// user chooses output format
	fileFormat format = fileFormatUserInstancer();

	// read rcg
	Reader reader;
	reader.readDir();

	// filter data
	Filter filter(format);
	filter.filterDir();

	// transform output into traclus compatible
	if (format.traclus)
		traclusDirCompatibility(filter.getOutputRootDir());
}
