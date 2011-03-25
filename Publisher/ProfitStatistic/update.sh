#!/bin/bash
#
# update.sh
#
# Download actual quotes, run analyzer and upload result data to a web server via ftp.
#
# Usage example:
# update.sh
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

# Check login parameters
if [[ "$PT_FTP_USER" == "" ]] || [[ "$PT_FTP_PASSWD" == "" ]]
then
   echo "Please set environment variables PT_FTP_USER and PT_FTP_PASSWD."
   exit 1;
fi

#
# Execute script only at 3am. Otherwise exit.
#
if [[ `date +%H` -gt 3 ]]
then
  echo "Script will only run at 3am."
  exit;
fi

#
# 1. Update quotes
#
~/projects/PerfectTrade/bin/Debug/QuoteLoader.exe update

#
# 2. Run analyzer
#
~/projects/PerfectTrade/bin/Debug/Analyzer.exe
cp ~/tradedata/results/ProfitStatistic/ProfitStatistik.png .


#
# 3. Update website
#
ftp -n ftp.marcweidler.de <<SCRIPT
user $PT_FTP_USER $PT_FTP_PASSWD
binary
put index.html
put ProfitStatistik.png
quit
SCRIPT

#
# 4. Cleanup
#
rm ProfitStatistik.png

#
# 5. Shutdown machine
#
sync
sleep 5
shutdown -h now