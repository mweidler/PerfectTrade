/*
 * Matrix.cs
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

namespace FinancialObjects
{
    /// <summary>
    /// </summary>
    public class Matrix
    {
        private SortedList<string, SortedList<string, double>> m_matrix;

        /// <summary>
        /// Erzeugt ein neues Matrix-Objekt und fuehr einen Reset() durch.
        /// </summary>
        public Matrix()
        {
            m_matrix = new SortedList<string, SortedList<string, double>>();
            Reset();
        }

        /// <summary>
        /// Loescht alle Positionen der Matrix.
        /// </summary>
        public void Reset()
        {
            m_matrix.Clear();
        }

        public void RemoveKey(string strRowID)
        {
            m_matrix.Remove(strRowID);
        }

        public void SearchMaximum(out string strRowID, out string strColID)
        {
            string maxrow = null;
            string maxcol = null;

            if (m_matrix.Keys.Count == 1)
            {
                strRowID = m_matrix.Keys[0];
                strColID = m_matrix.Keys[0];
                return;
            }

            foreach ( string row in m_matrix.Keys )
            {
                foreach (string col in m_matrix[row].Keys)
                {
                    if (row.Equals(col) == false)
                    {
                        if (maxrow == null || (m_matrix[row][col] > m_matrix[maxrow][maxcol]))
                        {
                            maxrow = row;
                            maxcol = col;
                        }
                    }
                }
            }

            strRowID = maxrow;
            strColID = maxcol;
        }

        public void Add(string strRowID, string strColID, double dValue)
        {
           if (m_matrix.ContainsKey(strRowID) == false)
           {
              m_matrix[strRowID] = new SortedList<string, double>();
           }

           m_matrix[strRowID][strColID] = dValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strRowID"></param>
        /// <param name="strColID"></param>
        /// <returns></returns>
        public double this[string strRowID, string strColID]
        {
            get { return m_matrix[strRowID][strColID]; }
            set { this.Add(strRowID,strColID, value); }
        }

        public double this[string strID]
        {
            get { return m_matrix[strID][strID]; }
            set { this.Add(strID, strID, value); }
        }

        /// <summary>
        /// Liefert die Anzahl der Positionen im Depot
        /// </summary>
        public uint Count
        {
            get { return (uint)m_matrix.Count; }
        }

        /// <summary>
        /// Prueft, ob ein Wertpapier (WKN) in der Matrix vorhanden ist.
        /// </summary>
        /// <param name="strWKN">Wertpapierkennnummer oder ID</param>
        /// <returns><c>true</c>, wenn das Wertpapier existiert, ansonsten <c>false</c></returns>
        public bool ContainsKey(string strWKN)
        {
            return m_matrix.ContainsKey(strWKN);
        }
    }
}
