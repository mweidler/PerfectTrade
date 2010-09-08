/*
 * Ranking.cs
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
          get { return m_strID; }
       }

       public double Value
       {
          get { return m_dValue; }
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
           get { return m_rank.Count; }
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
          get { return m_bIsSorted; }
       }

       public bool AutoSort
       {
          get { return m_bAutoSort; }
          set { m_bAutoSort = value; }
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
          get { return m_rank[i]; }
       }
   }
}
