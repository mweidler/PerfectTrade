#!/bin/bash
#
# install.sh
#
# Installs PerfectTrade in the $HOME/PerfectTrade and sets symbolic links
# of Unix-like program names.
#
#
# COPYRIGHT (C) 2011 AND ALL RIGHTS RESERVED BY
# MARC WEIDLER, ULRICHSTR. 12/1, 71672 MARBACH, GERMANY (MARC.WEIDLER@WEB.DE).
#
# ALL RIGHTS RESERVED. THIS SOFTWARE AND RELATED DOCUMENTATION ARE PROTECTED BY
# COPYRIGHT RESTRICTING ITS USE, COPYING, DISTRIBUTION, AND DECOMPILATION. NO PART
# OF THIS PRODUCT OR RELATED DOCUMENTATION MAY BE REPRODUCED IN ANY FORM BY ANY
# MEANS WITHOUT PRIOR WRITTEN AUTHORIZATION OF MARC WEIDLER OR HIS PARTNERS, IF ANY.
# UNLESS OTHERWISE ARRANGED, THIRD PARTIES MAY NOT HAVE ACCESS TO THIS PRODUCT OR
# RELATED DOCUMENTATION. SEE LICENSE FILE FOR DETAILS.
#
# THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
# ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
# WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
# IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
# INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
# BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
# DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
# LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE
# OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
# OF THE POSSIBILITY OF SUCH DAMAGE. THE ENTIRE RISK AS TO THE QUALITY AND
# PERFORMANCE OF THE PROGRAM IS WITH YOU. SHOULD THE PROGRAM PROVE DEFECTIVE,
# YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.
#
#set -x

#
# Check mono installation
#
MONOVER=`mono --version | grep version | awk -F" " '{print $5}'`
MONOVER1=`echo $MONOVER | awk -F"." '{print $1}'`
MONOVER2=`echo $MONOVER | awk -F"." '{print $2}'`
if [[ "$MONOVER" == "" ]]
then
   echo "No mono runtime installed."
   exit 1
else
   if [[ $MONOVER1 < 2 ]] || [[ $MONOVER2 < 6 ]]
   then
     echo "PerfectTrade needs Mono 2.6 or higher. Installed is version $MONOVER."
     exit 1
   fi
fi

#
# Create directories, if not already there
#
mkdir -p $HOME/PerfectTrade/bin
mkdir -p $HOME/bin

#
# Install a binary tool
#
function installexe {
   echo "... installing $1 to $HOME/PerfectTrade/bin"
   if [[ -e ./$1/bin/Debug/$1.exe ]]
   then
     cp -f ./$1/bin/Debug/$1.exe $HOME/PerfectTrade/bin
   fi

   if [[ -e ./$1/bin/Release/$1.exe ]]
   then
     cp -u ./$1/bin/Release/$1.exe $HOME/PerfectTrade/bin
   fi
}

#
# Install a dll
#
function installdll {
   echo "... installing $1 to $HOME/PerfectTrade/bin"
   if [[ -e ./Analyzer/bin/Debug/$1.dll  ]]
   then
     cp -f ./Analyzer/bin/Debug/$1.dll $HOME/PerfectTrade/bin
   fi

   if [[ -e ./Analyzer/bin/Release/$1.dll  ]]
   then
     cp -u ./Analyzer/bin/Release/$1.dll $HOME/PerfectTrade/bin
   fi
}

#
# Install exes
#
installexe Analyzer
installexe Simulator
installexe QuoteLoader

#
# Install DLLs
#
installdll FinancialObjects
installdll Indicators

#
# Install facade frontend
#
cp -f perfecttrade.sh $HOME/bin
rm -f $HOME/bin/pt
ln -s perfecttrade.sh $HOME/bin/pt

echo "Installation in '$HOME/bin' and '$HOME/PerfectTrade/bin' done."

