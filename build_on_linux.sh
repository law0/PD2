#!/bin/bash

./cmake_generic.sh build -DCMAKE_INSTALL_PREFIX="$(pwd)/install" -DURHO3D_SAMPLES=0 -DURHO3D_C++11=1 -DURHO3_LIB_TYPE=SHARED -DURHO3D_HOME="/home/law/bin/Urho3D/build/install"
cd build
make
cd -
