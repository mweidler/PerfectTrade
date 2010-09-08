/*
 * DataContainer.cs
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
            get { return m_data.Count; }
        }

        /// <summary>
        /// Liefert das aelteste Datum
        /// </summary>
        public WorkDate OldestDate
        {
            get { return m_MinDate; }
        }


        /// <summary>
        /// Liefert das neueste/juengste Datum
        /// </summary>
        public WorkDate YoungestDate
        {
            get { return m_MaxDate; }
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

            for (WorkDate keyDate = new WorkDate(fromDate); keyDate <= toDate; keyDate++) {
                if (this.Contains(keyDate)) {
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
            get { return m_data.Keys; }
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
