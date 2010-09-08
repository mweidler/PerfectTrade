/*
 * WorkDate.cs
 *
 * (C)OPYRIGHT 2007 BY MARC WEIDLER, ULRICHSTR. 12/1, 71672 MARBACH, GERMANY.
 *
 * All rights reserved. This product and related documentation are protected
 * by copyright restricting its use, copying, distribution, and decompilation.
 * No part of this product or related documentation may be reproduced in any
 * form by any means without prior written authorization of Marc Weidler or
 * his partners, if any. Unless otherwise arranged, third parties may not
 * have access to this product or related documentation.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace FinancialObjects
{
    /// <summary>
    /// Klasse zur Verarbeitung eines Datums.
    /// </summary>
    /// WorkDate stellt sicher, dass immer nur Wochentage verarbeitet werden, d.h.
    /// wenn das Datum auf ein Wochenende (Samstag, Sonntag) faellt wird
    /// 1. Bei der Initialisierung der naechste Wochentag eingestellt
    /// 2. Bei Addition wird der naechste Wochentag eingestellt
    /// 3. Bei Subtraktion wird der vorhergehende Wochentag eingestellt
    public class WorkDate : IComparable<WorkDate>, ICloneable
    {
        private DateTime m_datetime;
        public static WorkDate MaxDate = new WorkDate(DateTime.MaxValue.Year, DateTime.MaxValue.Month, DateTime.MaxValue.Day);
        public static WorkDate MinDate = new WorkDate(DateTime.MinValue.Year, DateTime.MinValue.Month, DateTime.MinValue.Day);
        public static WorkDate Today = new WorkDate();

        /// <summary>
        /// Liefert die Anzahl der Werktage pro Jahr zurueck (Konstante)
        /// </summary>
        public static int WorkDaysPerYear
        {
            get { return 250; }
        }

        /// <summary>
        /// Erstellt ein neues Datums-Objekt mit dem heutigen Datum.
        /// </summary>
        public WorkDate()
        {
            m_datetime = DateTime.Today;
            SyncToWorkDate();
        }

        /// <summary>
        /// Erstellt ein neues Datums-Objekt.
        /// </summary>
        /// <param name="workdate">WorkDate, dessen Datum zur Initialisierung übernommen werden soll.</param>
        public WorkDate(WorkDate workdate)
        {
            m_datetime = new DateTime(workdate.Year, workdate.Month, workdate.Day);
            SyncToWorkDate();
        }

        /// <summary>
        /// Erstellt ein neues Datums-Objekt.
        /// </summary>
        /// <param name="datetime">DateTime, dessen Datum zur Initialisierung übernommen werden soll.</param>
        public WorkDate(DateTime datetime)
        {
            m_datetime = new DateTime(datetime.Ticks);
            SyncToWorkDate();
        }

        /// <summary>
        /// Erstellt ein neues Datums-Objekt.
        /// </summary>
        public WorkDate(int iYear, int iMonth, int iDay)
        {
            m_datetime = new DateTime(iYear, iMonth, iDay);
            SyncToWorkDate();
        }

        public void Set(int iYear, int iMonth, int iDay)
        {
            m_datetime = new DateTime(iYear, iMonth, iDay);
            SyncToWorkDate();
        }

        public int Day
        {
            get { return m_datetime.Day; }
        }

        public int Month
        {
            get { return m_datetime.Month; }
        }

        public long Ticks
        {
            get { return m_datetime.Ticks; }
        }

        public int Year
        {
            get { return m_datetime.Year; }
        }

        private void SyncToWorkDate()
        {
            while (isWeekend())
            {
                m_datetime = m_datetime.AddDays(1);
            }
        }

        /*************************
         * Arithmetic
         ************************/
        public static WorkDate operator -(WorkDate datetime, int nDays)
        {
            WorkDate newDate = new WorkDate(datetime);
            for (int i = 0; i < nDays; i++)
            {
                newDate.PreviousDay();
            }
            return newDate;
        }

        public static WorkDate operator --(WorkDate datetime)
        {
            datetime.PreviousDay();
            return datetime;
        }

        public static WorkDate operator +(WorkDate datetime, int nDays)
        {
            WorkDate newDate = new WorkDate(datetime);
            for (int i = 0; i < nDays; i++)
            {
                newDate.NextDay();
            }
            return newDate;
        }

        public static WorkDate operator ++(WorkDate datetime)
        {
            datetime.NextDay();
            return datetime;
        }

        /*************************
         * Skip
         ************************/
        /// <summary>
        /// Setzt das Datum auf den ersten Werktag des naechsten Monats.
        /// </summary>
        public void NextMonth()
        {
            int nCurrentMonth = this.Month;
            
            do
            {
                NextDay();
            }
            while (nCurrentMonth == this.Month);
        }

        /// <summary>
        /// Setzt das Datum auf den nachfolgenden Montag.
        /// </summary>
        public void NextWeek()
        {
            do
            {
                NextDay();
            }
            while (m_datetime.DayOfWeek != DayOfWeek.Monday);
        }

        /*************************
         * Duration
         ************************/
        /// <summary>
        /// Returns the number of days between the two workdates.
        /// </summary>
        /// <param name="theone">The older date</param>
        /// <param name="theother">The younger date</param>
        /// <returns></returns>
        public static long operator -(WorkDate theone, WorkDate theother)
        {
            return (theone.Ticks - theother.Ticks) / TimeSpan.TicksPerDay;
        }

        /*************************
         * Comparison
         ************************/
        public static bool operator <(WorkDate datetime1, WorkDate datetime2)
        {
            return datetime1.Ticks < datetime2.Ticks;
        }

        public static bool operator <=(WorkDate datetime1, WorkDate datetime2)
        {
            return datetime1.Ticks <= datetime2.Ticks;
        }

        public static bool operator >(WorkDate datetime1, WorkDate datetime2)
        {
            return datetime1.Ticks > datetime2.Ticks;
        }

        public static bool operator >=(WorkDate datetime1, WorkDate datetime2)
        {
            return datetime1.Ticks >= datetime2.Ticks;
        }


        /// <summary>
        /// Setzt das Datum auf den naechsten Werktag.
        /// </summary>
        private void NextDay()
        {
            do
            {
                m_datetime = m_datetime.AddDays(1);
            }
            while (isWeekend());
        }

        /// <summary>
        /// Setzt das Datum auf den vorhergehenden Werktag.
        /// </summary>
        private void PreviousDay()
        {
            do
            {
                m_datetime = m_datetime.AddDays(-1);
            }
            while (isWeekend());
        }

        private bool isWeekend()
        {
            return m_datetime.DayOfWeek == System.DayOfWeek.Saturday ||
                   m_datetime.DayOfWeek == System.DayOfWeek.Sunday;
        }

        #region IComparable<WorkDate> Member

        public int CompareTo(WorkDate other)
        {
            return this.Ticks.CompareTo(other.Ticks);

            /*if (this.Ticks < other.Ticks) return -1;
            else if (this.Ticks > other.Ticks) return 1;
            else return 0;*/
        }

        #endregion

        #region ICloneable Member

        public WorkDate Clone()
        {
            return new WorkDate(this);
        }

        object ICloneable.Clone() { return this.Clone(); }  // this calls the above

        #endregion

        public override string ToString()
        {
            return m_datetime.ToString("dd.MM.yyyy");
        }
    }
}
