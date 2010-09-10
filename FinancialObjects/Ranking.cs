//
// Ranking.cs
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
   public sealed class RankingInfo : IComparable<RankingInfo>
   {
      private readonly string m_strID;
      private readonly double m_dValue;

      public RankingInfo(string strID, double dValue)
      {
         m_strID = strID;
         m_dValue = dValue;
      }

      public string ID
      {
         get
         {
            return m_strID;
         }
      }

      public double Value
      {
         get
         {
            return m_dValue;
         }
      }

      public int CompareTo(RankingInfo theOther)
      {
         return -(this.Value.CompareTo(theOther.Value));
      }
   }

   public class Ranking
   {
      private readonly List<RankingInfo> m_rank;
      private bool m_bAutoSort;
      private bool m_bIsSorted;

      public Ranking()
      {
         m_rank = new List<RankingInfo>();
         m_bAutoSort = true;
         m_bIsSorted = false;
      }

      public void Add(SortedList<string, Instrument> instruments, double dValue)
      {
         foreach (string strID in instruments.Keys)
         {
            m_rank.Add(new RankingInfo(strID, dValue));
         }

         m_bIsSorted = false;

         if (m_bAutoSort)
         {
            Sort();
         }
      }

      public void Add(string strID, double dValue)
      {
         m_rank.Add(new RankingInfo(strID, dValue));
         m_bIsSorted = false;

         if (m_bAutoSort)
         {
            Sort();
         }
      }

      public void Remove(string strID)
      {
         for (int i = 0; i < m_rank.Count; i++)
         {
            if (m_rank[i].ID.Equals(strID))
            {
               m_rank.RemoveAt(i);
               break;
            }
         }
      }


      public void Clear()
      {
         m_rank.Clear();
         m_bIsSorted = false;
      }

      public int Count
      {
         get
         {
            return m_rank.Count;
         }
      }

      public void Sort()
      {
         m_rank.Sort();
         m_bIsSorted = true;
      }

      /// <summary>
      /// Prueft, ob das Wertpapier zu den <c>nTopRange</c> besten Wertpapieren im Ranking gehoehrt.
      /// </summary>
      /// <param name="nTopRange">Top-Bereich</param>
      /// <param name="strID">WKN oder ID</param>
      /// <returns>Liefert true, wenn das Wertpapier zu den <c>nTopRange</c> besten Wertpapieren
      /// im Ranking gehoehrt, ansonsten false.</returns>
      public bool IsTopRanked(uint nTopRange, string strID)
      {
         if (m_bIsSorted == false)
         {
            Sort();
         }

         for (int i = 0; i < nTopRange && i < m_rank.Count; i++)
         {
            if (m_rank[i].ID.Equals(strID))
            {
               return true;
            }
         }

         return false;
      }

      /// <summary>
      /// Prueft, ob das Wertpapier im Ranking vorhanden ist.
      /// </summary>
      /// <param name="strID">WKN oder ID</param>
      /// <returns>Liefert true, wenn das Wertpapier im Ranking vorhanden ist, ansonsten false.</returns>
      public bool Contains(string strID)
      {
         for (int i = 0; i < m_rank.Count; i++)
         {
            if (m_rank[i].ID.Equals(strID))
            {
               return true;
            }
         }

         return false;
      }

      public bool IsSorted
      {
         get
         {
            return m_bIsSorted;
         }
      }

      public bool AutoSort
      {
         get
         {
            return m_bAutoSort;
         }
         set
         {
            m_bAutoSort = value;
         }
      }

      /// <summary>
      /// Liefert einen Enumerator mit den Schluesseln (IDs) aller Ranking-Elemente
      /// </summary>
      /// <returns>Enumerator mit Schluesseln</returns>
      public IEnumerator<string> GetEnumerator()
      {
         foreach (RankingInfo rankinginfo in m_rank)
         {
            yield return rankinginfo.ID;
         }
      }

      public RankingInfo this[int i]
      {
         get
         {
            return m_rank[i];
         }
      }
   }
}
