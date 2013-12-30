#!/bin/bash
#
# pt -> perfecttrade.sh
#
# Facade script for all PerfectTrade tools.
#
# see "usage" for details.
#
#
# Copyright (C) 2010-2011 Marc Weidler (marc.weidler@web.de)
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

ARGC=$#


###########################################################################
# Usage
#
function usage {
   echo "PerfectTrade - finance stock trading tools"
   echo "Copyright (C) 2011 Marc Weidler (marc.weidler@web.de)"
   echo ""
   echo "PerfectTrade is a command-line based toolset for analyzing and simulating"
   echo "financial stock trading strategies."
   echo ""
   echo "Usage:"
   echo "$0 [command] <options> ..."
   echo "where commands are:"
   echo "  init              initialize quotes"
   echo "  update            update quotes"
   echo "  analyze <engine>  start analyzer engine"
   echo "  simulate          simulate trading strategie"
   echo "  help              print this help"
   echo ""
   echo "See README file for further information."
   echo ""
}


###########################################################################
# Main
#
#
if [ $ARGC -lt 1 ]
then
  usage
  exit 1
fi

case "$1" in
    analyze)
        shift
        mono ~/PerfectTrade/bin/Analyzer.exe $@
        ;;
    simulate)
        shift
        mono ~/PerfectTrade/bin/Simulator.exe $@
        ;;
    update)
        shift
        mono ~/PerfectTrade/bin/QuoteLoader.exe update $@
        ;;
    init)
        shift
        mono ~/PerfectTrade/bin/QuoteLoader.exe init $@
        ;;
    -h|-help|-?|help)
        usage
        ;;
    *)
        echo "$0: unknown command '$1'."
        ;;
esac
