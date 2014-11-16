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
#LOGNAME=`date +%F-%H-%M`
#exec &>~/log$LOGNAME.txt

PT_FTP_USER=$1
PT_FTP_PASSWD=$2

echo "Update-Script started at `date`"

# Check login parameters
if [[ "$PT_FTP_USER" == "" ]] || [[ "$PT_FTP_PASSWD" == "" ]]
then
   echo "Please set environment variables PT_FTP_USER and PT_FTP_PASSWD."
   exit 1;
fi

#
# Sleep 10 seconds to wait for system to be ready.
#
sleep 10
date

#
# Execute script only at 4am. Otherwise exit.
#
if [[ `date +%k` -ne 4 ]]
then
  echo "Script will only run at 4am."
  exit;
fi

#
# 1. Update quotes
#
pt update

#
# 2. Run analyzer
#
pt analyze all

#
# 3. Update website
#
ftp -n ftp.marcweidler.de <<SCRIPT
user $PT_FTP_USER $PT_FTP_PASSWD
binary
lcd ~/PerfectTrade/Results/ProfitStatistic
put index.html
put DAXProfitStatistik.svg
put DAXOverview.svg
put Nasdaq100ProfitStatistik.svg
put Nasdaq100Overview.svg
quit
SCRIPT

#
# 4. Shutdown machine
#
sync
sleep 5
sudo shutdown -h +2 &

echo "Update-Script ended at `date`"
