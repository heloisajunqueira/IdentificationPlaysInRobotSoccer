#include "fileFormat.h"

// #fileFormatUserInstancer
fileFormat fileFormatUserInstancer()
{
    fileFormat option;                     // declaring return variable
    std::vector<std::string> alternatives; // declaring auxiliar variable for users alternatives

    // asks user about the folder and file structure tree
    alternatives.reserve(3); //reserve is always more efficient than blind push_backs
    alternatives.push_back("by game");
    alternatives.push_back("by teams");
    alternatives.push_back("single file");
    option.groupBy = multipleAlternativeQuestion("1. How do you wish to group data in files?", alternatives);
    if (option.groupBy == FF_GROUPBY_TEAMS) // if choosed warns user that filtering by team ignores ball data
        std::cout << "    WARNING: Since ball has no team organizing per Team will write ball data in both files, that is duplicating ball data!" << std::endl;

    // asks user about wich filter to use
    alternatives.clear();
    alternatives.push_back("finalization plays");
    alternatives.push_back("finalization plays with only the offensive team");
    option.filterBy = multipleAlternativeQuestion("2. Wich data do you want to gather?", alternatives);

    // asks the user if it should turn on offense side manipulation
    option.halfField = booleanQuestion("3. Do you want to Annalize offenses in a single direction?");

    // if user wants to activate traclus compability other questions are automatically filled
    option.traclus = booleanQuestion("4. Do you want to activate the traclus algorithm compability mode?");
    if (option.traclus)
    {
        option.header = false;
        option.rowId = true;
        option.playLength = true;
        option.team = false;
        option.cycle = false;
        option.player = false;
        option.separator = " ";

        return option;
    }

    //asks user about wich separator to use
    int separator;
    alternatives.clear();
    alternatives.push_back("coma ', '");
    alternatives.push_back("blank space ' '");
    separator = multipleAlternativeQuestion("5. Wich CSV Separator do you want to use?", alternatives);
    if (separator == FF_SEPARATOR_COMMA) //transforms integer user choice in string
        option.separator = ", ";
    if (separator == FF_SEPARATOR_BLANK) //transforms integer user choice in string
        option.separator = " ";

    //asks bunch of questions about file and data formmating
    option.header = booleanQuestion("6. Do you want Headers on the file?");
    option.rowId = booleanQuestion("7. Do you want row IDs on the file?");
    option.cycle = booleanQuestion("8. Do you want a first cycle column on the File?");
    option.team = booleanQuestion("9. Do you want a team column on the File?");
    option.player = booleanQuestion("10. Do you want a player column on the File?");
    option.playLength = booleanQuestion("11. Do you want plays length column on the File?");

    //return fileFormat with all options assigned
    return option;
}