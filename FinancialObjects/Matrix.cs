//
// Matrix.cs
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

         foreach (string row in m_matrix.Keys)
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
         get
         {
            return m_matrix[strRowID][strColID];
         }
         set
         {
            this.Add(strRowID, strColID, value);
         }
      }

      public double this[string strID]
      {
         get
         {
            return m_matrix[strID][strID];
         }
         set
         {
            this.Add(strID, strID, value);
         }
      }

      /// <summary>
      /// Liefert die Anzahl der Positionen im Depot
      /// </summary>
      public uint Count
      {
         get
         {
            return (uint)m_matrix.Count;
         }
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
