#!/bin/bash
#
# pt
#
# Facade script for all PerfectTrade tools.
#
# Usage example:
# pt update
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
   echo "TODO"
   echo ""
   echo "Usage:"
   echo "$0 [command] <options> ..."
   echo "where commands are:"
   echo " analyze <engine>  start analyzer"
   echo " help              print this help"
   echo ""
   echo "Usage example:"
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
        ~/PerfectTrade/bin/Analyzer.exe $@
        ;;
    simulate)
        shift
        ~/PerfectTrade/bin/Simulator.exe $@
        ;;
    update)
        shift
        ~/PerfectTrade/bin/QuoteLoader.exe update $@
        ;;
    init)
        shift
        ~/PerfectTrade/bin/QuoteLoader.exe init $@
        ;;
    publish)
        shift
        ~/PerfectTrade/bin/Publish/$@
        ;;
    -h|-help|-?|help)
        usage
        ;;
    *)
        echo "$0: unknown command '$1'."
        ;;
esac
