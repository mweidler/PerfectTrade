//
// Calendar.cs
//
// COPYRIGHT (C) 2010 AND ALL RIGHTS RESERVED BY
// MARC WEIDLER, ULRICHSTR. 12/1, 71672 MARBACH, GERMANY (MARC.WEIDLER@WEB.DE).
//
// ALL RIGHTS RESERVED. THIS SOFTWARE AND RELATED DOCUMENTATION ARE PROTECTED BY
// COPYRIGHT RESTRICTING ITS USE, COPYING, DISTRIBUTION, AND DECOMPILATION. NO PART
// OF THIS PRODUCT OR RELATED DOCUMENTATION MAY BE REPRODUCED IN ANY FORM BY ANY
// MEANS WITHOUT PRIOR WRITTEN AUTHORIZATION OF MARC WEIDLER OR HIS PARTNERS, IF ANY.
// UNLESS OTHERWISE ARRANGED, THIRD PARTIES MAY NOT HAVE ACCESS TO THIS PRODUCT OR
// RELATED DOCUMENTATION. SEE LICENSE FILE FOR DETAILS.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
// IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
// INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
// BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE
// OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
// OF THE POSSIBILITY OF SUCH DAMAGE. THE ENTIRE RISK AS TO THE QUALITY AND
// PERFORMANCE OF THE PROGRAM IS WITH YOU. SHOULD THE PROGRAM PROVE DEFECTIVE,
// YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.
//

using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialObjects
{
    public class Calendar
    {
        private int m_nDay, m_nMonth, m_nYear;

        public Calendar()
        {
            Set(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
        }

        public Calendar(Calendar calendar)
        {
            Set(calendar.m_nYear, calendar.m_nMonth, calendar.m_nDay);
        }

        public Calendar(int nYear, int nMonth, int nDay)
        {
            Set(nYear, nMonth, nDay);
        }

        public void Set(int nYear, int nMonth, int nDay)
        {
            m_nDay = nDay;
            m_nMonth = nMonth;
            m_nYear = nYear;
        }

        /// <summary>
        /// Liefert den Wochentag des Datums.
        /// </summary>
        public System.DayOfWeek getDayOfWeek()
        {
            int[] dayCount = { 0, 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334 };
            System.DayOfWeek[] weekdays = { System.DayOfWeek.Sunday, System.DayOfWeek.Monday,
                                            System.DayOfWeek.Tuesday, System.DayOfWeek.Wednesday,
                                            System.DayOfWeek.Thursday, System.DayOfWeek.Friday,
                                            System.DayOfWeek.Saturday };

            if (m_nYear <= 1900)
                throw new ArgumentOutOfRangeException("Year", m_nYear, "Year must be greater or equal to 1900.");

            int nYears = m_nYear - 1900;
            int nLeapDays = nYears / 4;
            int nDays = nYears * 365 + nLeapDays + dayCount[m_nMonth] + m_nDay;
            int nWeekDay = nDays % 7;

            return weekdays[nWeekDay];  // 0=Sunday
        }

        /// <summary>
        /// Ermittelt, ob das aktuelle Datum in einem Schaltjahr liegt.
        ///
        /// Ab 1600: alle Jahre, die durch 4 teilbar sind, außer den
        /// vollen Jahrhunderten, es sei denn, sie sind durch 400 teilbar.
        /// </summary>
        public bool isLeapYear()
        {
            if (m_nYear <= 1900)
                throw new ArgumentOutOfRangeException("Year", m_nYear, "Year must be greater or equal to 1900.");

            if (m_nYear % 4 == 0)
            {
                if (m_nYear % 100 == 0)
                {
                    return (m_nYear % 400 == 0);
                }

                return true;
            }

            return false;
        }

        /*private int DaysPerMonth()
        {
            int[] iaDaysPerMonth = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            if (m_nMonth == 2 && isLeapYear())
            {
                return 29;
            }

            return iaDaysPerMonth[m_nMonth];
        }*/
    }
}
