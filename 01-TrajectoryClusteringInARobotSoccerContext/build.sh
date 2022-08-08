#!/bin/sh

#get current path
current_path="$PWD"

#get script path
parent_path=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )

#go to script path
cd "$parent_path"

#create a build directory if it doesnt exist in the script parent folder
mkdir -p build

#enters build directory
cd build

#cmake the code (wich should be in the buils script directory)
cmake ..

#compiles the code
make

#go back to the previous current path
cd "$current_path"

