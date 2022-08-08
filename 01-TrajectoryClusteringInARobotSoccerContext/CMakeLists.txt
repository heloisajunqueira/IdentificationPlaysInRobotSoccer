#defining minimum cmake version
cmake_minimum_required(VERSION 3.5)

#defining project name
project(Project)

#defining flags and output path
#c++17 is needed due to filesystem librarie
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_EXTENSIONS OFF)

set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wall -Werror")
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY "${PROJECT_SOURCE_DIR}/bin")

#defining src and header directories variables
set(source_dir "${PROJECT_SOURCE_DIR}/src")
set(include_dir "${PROJECT_SOURCE_DIR}/include")

#include headers directory into relative path
include_directories (${include_dir})  

#gathers files from src int variable
file(GLOB source_files "${source_dir}/*.cpp")

#choosing build type(debug vs release), if debug define 'DEBUG' for source code
option(DEBUG "Use debug" OFF)
if(DEBUG)
  MESSAGE(STATUS "COMPILING FOR DEBUG")
  add_definitions(-DDEBUG)
  set(CMAKE_BUILD_TYPE Debug)
else()
  MESSAGE(STATUS "COMPILING FOR RELEASE")
  set(CMAKE_BUILD_TYPE Release)
endif()

#compile src files
add_executable(app.bin ${source_files})
target_link_libraries(app.bin stdc++fs)
