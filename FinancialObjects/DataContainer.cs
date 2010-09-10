//
// DataContainer.cs
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
using System.IO;

namespace FinancialObjects
{
   /// <summary>
   /// Verwaltet eine Liste mit Key-Value-Eintraegen:
   /// Key:   WorkDate
   /// Value: double
   /// Wird z.B. zur Speicherung von Kursdaten, aber auch fuer
   /// Indikatordaten verwendet.
   /// </summary>
   public class DataContainer
   {
      private SortedList<WorkDate, double> m_data;
      private WorkDate m_MinDate;
      private WorkDate m_MaxDate;

      /// <summary>
      /// Erzeugt einen neuen Datenkontainer
      /// </summary>
      public DataContainer()
      {
         m_data = new SortedList<WorkDate, double>();
         Clear();
      }

      /// <summary>
      /// Liefert die Anzahl der Key-Value-Paare
      /// </summary>
      public int Count
      {
         get
         {
            return m_data.Count;
         }
      }

      /// <summary>
      /// Liefert das aelteste Datum
      /// </summary>
      public WorkDate OldestDate
      {
         get
         {
            return m_MinDate;
         }
      }


      /// <summary>
      /// Liefert das neueste/juengste Datum
      /// </summary>
      public WorkDate YoungestDate
      {
         get
         {
            return m_MaxDate;
         }
      }


      /// <summary>
      /// Prueft, ob ein Datum (Key) im Container vorhanden ist
      /// </summary>
      /// <returns><c>true</c>, wenn das Datum existiert, ansonsten <c>false</c></returns>
      public bool Contains(WorkDate workdate)
      {
         return m_data.ContainsKey(workdate);
      }

      /// <summary>
      /// Entfernt alle Key-Value-Elemente aus dem Container
      /// </summary>
      public void Clear()
      {
         m_data.Clear();
         m_MinDate = WorkDate.MaxDate;
         m_MaxDate = WorkDate.MinDate;
      }

      /// <summary>
      /// Setzt alle Value-Elemente auf 0.
      /// Alle Key-Value-Paare bleiben jedoch erhalten.
      /// </summary>
      public void Zero()
      {
         for (WorkDate keyDate = new WorkDate(m_MinDate); keyDate <= m_MaxDate; keyDate++)
         {
            if (this.Contains(keyDate))
            {
               m_data[keyDate] = 0;
            }
         }
      }

      /// <summary>
      /// Fuellt alle Luecken mit dem letzten gueltigen Wert auf.
      /// </summary>
      /// <returns>Anzahl der Luecken, die aufgefuellt wurden.
      public int FillGaps()
      {
         double dOldValue = 0;
         int nFilledGaps = 0;

         for (WorkDate keyDate = new WorkDate(m_MinDate); keyDate <= m_MaxDate; keyDate++)
         {
            if (this.Contains(keyDate))
            {
               dOldValue = this[keyDate];
            }
            else
            {
               this[keyDate] = dOldValue;
               nFilledGaps++;
            }
         }

         return nFilledGaps;
      }

      /// <summary>
      /// Greift auf ein Key-Value-Paar zu und ermoeglicht:
      /// 1. Lesen eines Values mit dem angegebenen Key (Datum)
      /// 2. Ueberschreiben eines vorhandenen Values mit dem angegebenen Key (Datum)
      /// 3. Hinzufuegen eines neuen Key-Value-Paares
      /// </summary>
      ///
      /// <code>
      ///       DataContainer quotes = new DataContainer();
      ///       WorkDate datum1 = new WorkDate(2005,1,1);
      ///       quotes[datum1] = 45.7;          // fuegt ein neues Element ein
      ///       quotes[datum1] = 12.1;          // veraendert ein vorhandenes Element
      ///       double dValue  = quotes[datum]; // Liest den Wert am Datum
      /// </code>
      public double this[WorkDate workdate]
      {
         get
         {
            return m_data[workdate];
         }

         set
         {
            WorkDate myworkdate = new WorkDate(workdate);
            m_data[myworkdate] = value;

            if (myworkdate < m_MinDate) m_MinDate = myworkdate;

            if (myworkdate > m_MaxDate) m_MaxDate = myworkdate;
         }
      }

      /// <summary>
      /// Liefert einen neuen DataContainer, der inhaltlich die
      /// gleichen Key-Value-Paare enhaelt, wie der aktuelle DataContainer.
      /// Alle Element-Objekte (was auch immer) wird neu erzeugt.
      /// </summary>
      public DataContainer Clone(WorkDate fromDate, WorkDate toDate)
      {
         DataContainer dcTarget = new DataContainer();

         for (WorkDate keyDate = new WorkDate(fromDate); keyDate <= toDate; keyDate++)
         {
            if (this.Contains(keyDate))
            {
               dcTarget[keyDate] = this[keyDate];
            }
         }

         return dcTarget;
      }

      /// <summary>
      /// Liefert einen neuen DataContainer, der inhaltlich die
      /// gleichen Key-Value-Paare enhaelt, wie der aktuelle DataContainer.
      /// Alle Element-Objekte (was auch immer) wird neu erzeugt.
      /// </summary>
      public DataContainer Clone(WorkDate fromDate)
      {
         DataContainer dcTarget = new DataContainer();
         WorkDate toDate = this.YoungestDate;

         for (WorkDate keyDate = new WorkDate(fromDate); keyDate <= toDate; keyDate++)
         {
            if (this.Contains(keyDate))
            {
               dcTarget[keyDate] = this[keyDate];
            }
         }

         return dcTarget;
      }


      /// <summary>
      /// Liefert einen neuen DataContainer, der inhaltlich die
      /// gleichen Key-Value-Paare enhaelt, wie der aktuelle DataContainer.
      /// Alle Element-Objekte (was auch immer) wird neu erzeugt.
      /// </summary>
      public DataContainer Clone()
      {
         DataContainer dcTarget = new DataContainer();

         foreach (WorkDate keyDate in m_data.Keys)
         {
            dcTarget[keyDate] = this[keyDate];
         }

         return dcTarget;
      }

      static public void Copy(DataContainer source, DataContainer target)
      {
         target.Clear();

         foreach (WorkDate keyDate in source.m_data.Keys)
         {
            target[keyDate] = source[keyDate];
         }
      }

      public IList<WorkDate> Dates
      {
         get
         {
            return m_data.Keys;
         }
      }

      /// <summary>
      /// Laedt den Inhalt eines DataContainers aus einer Datei.
      /// </summary>
      /// <param name="strFilename">Dateiname der zu lesenden Datei</param>
      public int Load(string strFilename)
      {
         char[] chSplit = { ' ' };
         string[] arrTokens = null;
         DateTime priceDate = new DateTime();
         Clear();

         if (File.Exists(strFilename))
         {
            StreamReader sr = new StreamReader(strFilename);

            while (sr.Peek() >= 0)
            {
               string strLine = sr.ReadLine();
               arrTokens = strLine.Split(chSplit);
               DateTime.TryParse(arrTokens[0], out priceDate);
               double dValue = Double.Parse(arrTokens[1]);
               WorkDate workdate = new WorkDate(priceDate.Year, priceDate.Month, priceDate.Day);
               this[workdate] = dValue;
            }

            sr.Close();
         }

         return this.Count;
      }

      /// <summary>
      /// Speichert den Inhalt eines DataContainers in eine Datei.
      /// </summary>
      /// <param name="strFilename">Dateiname der zu speichernden Datei</param>
      public void Save(string strFilename)
      {
         Save(strFilename, " ");
      }

      /// <summary>
      /// Speichert den Inhalt eines DataContainers in eine Datei.
      /// </summary>
      /// <param name="strFilename">Dateiname der zu speichernden Datei</param>
      /// <param name="strSeparator">Trenmnzeichen zwischen Zeilenelementen</param>
      public void Save(string strFilename, string strSeparator)
      {
         StreamWriter sw = new StreamWriter(strFilename, false);

         foreach (WorkDate workdate in m_data.Keys)
         {
            sw.WriteLine(workdate.ToString() + strSeparator + this[workdate]);
         }

         sw.Close();
      }

   }
}
