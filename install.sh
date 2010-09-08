#!/bin/bash
#
# install.sh
#
# Installs PerfectTrade in the $HOME/PerfectTrade and sets symbolic links
# of Unix-like program names.
#
# Use the following names for the program suite:
#   ql  - Starts the QuoteLoader
#   an  - Starts the Analyzer
#   si  - Starts the Simulator
#   
#
# COPYRIGHT (C) 2010 AND ALL RIGHTS RESERVED BY
# MARC WEIDLER, ULRICHSTR. 12/1, 71672 MARBACH, GERMANY (MARC.WEIDLER@WEB.DE).
#
# ALL RIGHTS RESERVED. THIS PRODUCT AND RELATED DOCUMENTATION ARE PROTECTED BY
# COPYRIGHT RESTRICTING ITS USE, COPYING, DISTRIBUTION, AND DECOMPILATION. NO PART
# OF THIS PRODUCT OR RELATED DOCUMENTATION MAY BE REPRODUCED IN ANY FORM BY ANY
# MEANS WITHOUT PRIOR WRITTEN AUTHORIZATION OF MARC WEIDLER OR HIS PARTNERS, IF ANY.
# UNLESS OTHERWISE ARRANGED, THIRD PARTIES MAY NOT HAVE ACCESS TO THIS PRODUCT OR
# RELATED DOCUMENTATION.
#
# THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW.
# THE COPYRIGHT HOLDERS AND/OR OTHER PARTIES PROVIDE THE PROGRAM "AS IS" WITHOUT
# WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO,
# THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.
# THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU.
# SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY
# SERVICING, REPAIR OR CORRECTION.
#
#set -x

TARGET=Debug

clear
echo "      PerfectTrade installation"
echo "------------------------------------"

if [ -d $HOME/PerfectTrade ]
then
   echo Target directory already exists, good.
else
   echo "Creating target directory $HOME/PerfectTrade"
   mkdir $HOME/PerfectTrade
   echo "Done."
fi

echo "Copying program files..."
cp QuoteLoader/bin/$TARGET/QuoteLoader.exe $HOME/PerfectTrade
cp QuoteLoader/bin/$TARGET/*.dll $HOME/PerfectTrade
echo "Done."

echo "Generating links..."
cd $HOME/PerfectTrade
ln -v -f -s QuoteLoader.exe ql
cd -
echo "Done."

echo "Installation complete."
echo "Use the following names for the program suite"
echo "    ql  - Starts the QuoteLoader"
echo "    an  - Starts the Analyzer"
echo "    si  - Starts the Simulator"

