//
// WorkDate.cs
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
      private static Hashtable holidays = new Hashtable();
      public static WorkDate MaxDate = new WorkDate(DateTime.MaxValue.Year, DateTime.MaxValue.Month, DateTime.MaxValue.Day);
      public static WorkDate MinDate = new WorkDate(1000,                   DateTime.MinValue.Month, DateTime.MinValue.Day);
      public static WorkDate Today = new WorkDate();

      /// <summary>
      /// Liefert die Anzahl der Werktage pro Jahr zurueck (Konstante)
      /// </summary>
      public static int WorkDaysPerYear
      {
         get
         {
            return 250;
         }
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
         get
         {
            return m_datetime.Day;
         }
      }

      public int Month
      {
         get
         {
            return m_datetime.Month;
         }
      }

      public long Ticks
      {
         get
         {
            return m_datetime.Ticks;
         }
      }

      public int Year
      {
         get
         {
            return m_datetime.Year;
         }
      }

      private void SyncToWorkDate()
      {
         while (isWeekend() || isHoliday())
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

         SyncToWorkDate();
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

         SyncToWorkDate();
      }

      /// <summary>
      /// Validates, if the current date is a holiday
      /// </summary>
      /// <returns>True, if the current date is a holiday, otherwise false</returns>
      private bool isHoliday()
      {
         if (holidays.ContainsKey("01.01." + m_datetime.Year) == false)
         {
            holidays.Add("01.01." + m_datetime.Year, "");
            // Neujahr
            holidays.Add("06.01." + m_datetime.Year, "");
            // Heilige Drei Könige
            holidays.Add("01.05." + m_datetime.Year, "");
            // Tag der Arbeit
            holidays.Add("03.10." + m_datetime.Year, "");
            // Tag der dt. Einheit
            holidays.Add("01.11." + m_datetime.Year, "");
            // Allerheiligen
            holidays.Add("25.12." + m_datetime.Year, "");
            // 1. Weihnachtstag
            holidays.Add("26.12." + m_datetime.Year, "");
            // 2. Weihnachtstag
            WorkDate ostersonntag = GetOsterSonntag(m_datetime.Year);
            holidays.Add(ostersonntag.ToString(), "");
            // Ostersonntag
            holidays.Add((ostersonntag - 2).ToString(), "");
            // Karfreitag
            holidays.Add((ostersonntag + 1).ToString(), "");
            // Ostermontag
            holidays.Add((ostersonntag + 39).ToString(), "");
            // Christi Himmelfahrt
            holidays.Add((ostersonntag + 50).ToString(), "");
            // Pfingstmontag
            holidays.Add((ostersonntag + 60).ToString(), "");
            // Fronleichnam
         }

         return holidays.ContainsKey(m_datetime.ToString());
      }

      /// <summary>
      /// Returns the holiday "Ostersonntag"
      /// </summary>
      /// <param name="Year">Year of the requested holiday</param>
      /// <returns>"Ostersonntag"</returns>
      private WorkDate GetOsterSonntag(int Year)
      {
         int g, h, c, j, l, i;

         g = Year % 19;
         c = Year / 100;
         h = ((c - (c / 4)) - (((8 * c) + 13) / 25) + (19 * g) + 15) % 30;
         i = h - (h / 28) * (1 - (29 / (h + 1)) * ((21 - g) / 11));
         j = (Year + (Year / 4) + i + 2 - c + (c / 4)) % 7;

         l = i - j;
         int month = (int)(3 + ((l + 40) / 44));
         int day = (int)(l + 28 - 31 * (month / 4));

         return new WorkDate(Year, month, day);
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
         while (isWeekend() || isHoliday());
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
         while (isWeekend() || isHoliday());
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

      object ICloneable.Clone()
      {
         return this.Clone();
      }  // this calls the above

      #endregion

      public override string ToString()
      {
         return m_datetime.ToString("dd.MM.yyyy");
      }
   }
}
