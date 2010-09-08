/*
 * Variants.cs
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
using System.Threading;

namespace FinancialObjects
{
    /// <summary>
    /// Verwaltet eine einzelne Variante.
    /// Eine variante kann durch alle moeglichen Werte iterieren.
    /// </summary>
    class VariantsItem
    {
        private string m_strName;
        private int[] m_items;
        private int m_iCurrent;

        /// <summary>
        /// Legt ein neues Objekt mit Namen und moeglichen Werten an
        /// </summary>
        /// Eine variante kann durch alle moeglichen Werte iterieren.
        public VariantsItem(string strName, int[] items)
        {
            m_strName = strName;
            m_items = items;
            m_iCurrent = 0;
        }

        /// <summary>
        /// Liefert den aktuellen Wert der Variante
        /// </summary>
        public int Current
        {
            get { return m_items[m_iCurrent]; }
        }

        /// <summary>
        /// Stellt den naechsten moeglichen Wert der Variante ein oder beginnt von vorne.
        /// </summary>
        /// <returns><c>true</c>, wenn wieder von vorne angefangen wird, ansonsten <c>false</c></returns>
        public bool Next()
        {
            m_iCurrent++;

            if (m_iCurrent >= this.Size)
            {
                m_iCurrent = 0;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Liefert die Anzahl der moeglichen Werte zureuck.
        /// </summary>
        public uint Size
        {
            get { return (uint)m_items.Length; }
        }

        /// <summary>
        /// Liefert den Namen der Variante zureuck.
        /// </summary>
        public string Name
        {
            get { return m_strName; }
        }
    }


    /// <summary>
    /// Verwaltet eine Liste von Varianten
    /// </summary>
    ///
    /// Beispiel:
    /// <code>
    /// Variants variants = Variants.GetInstance();
    /// variants.Add("test1", 2,8,2);
    /// variants.Add("test2", new int[] { 6, 9, 11 });
    /// while (variants.HasMoreIterations)
    /// {
    ///     Console.Out.WriteLine("test1: " + variants["test1"] + " test2: " + variants["test2"] + " compl: " + variants.Completeness);
    ///     variants.Next();
    /// }
    /// 
    /// The first added variant name will be iterated first.
    /// Output: 2 6
    ///         4 6
    ///         6 6
    ///         8 6
    ///         2 9
    ///         4 9
    ///         ...
    /// </code>         
    public class Variants
    {
        // global variant items storage
        private static SortedList<string, VariantsItem> m_slVariants = new SortedList<string, VariantsItem>();
        private static List<VariantsItem> m_lVariants = new List<VariantsItem>();
        private static uint m_nIterations;
        private static uint m_nTotalIterations;
        
        // local variant items storage
        private SortedList<string, int> m_slValues = new SortedList<string, int>();

        private static Object thislock = new Object();

        // !!!
        // ACHTUNG: Kein Singleton hieraus machen! Jeder "Thread" braucht seine locale Variante!
        // !!!

        public Variants()
        {
            // nothing to do
        }

        public void Clear()
        {
            m_slVariants.Clear();
            m_lVariants.Clear();
            m_slValues.Clear();
            m_nIterations = 0;
            m_nTotalIterations = 0;
        }

        public IEnumerable<string> ItemKeys
        {
            get
            {
                foreach (VariantsItem item in m_lVariants)
                {
                    yield return item.Name;
                }
            }
        }

        public void Add(string strName, int nFrom, int nTo, int nStep)
        {
            if ( nFrom > nTo )
                throw new Exception("'From' argument must be lower than 'To' argument.");

            int nItems = ((nTo - nFrom) / nStep) + 1;
            int[] items = new int[nItems];
            int i = 0;

            for (int iValue = nFrom; iValue <= nTo; iValue += nStep, i++)
            {
                items[i] = iValue;
            }

            if (i != nItems)
                throw new Exception("Implementation error");

            this.Add(strName, items);
        }

        public void Add(string strName, int[] items)
        {
            if (items.Length < 1)
                throw new ArgumentException("There must be at least 1 item to add.", "items");

            VariantsItem item = new VariantsItem(strName, items);
            m_slVariants.Add(strName, item );
            m_lVariants.Add(item);
            m_slValues[strName] = items[0];
            m_nIterations = 1;
            m_nTotalIterations = Math.Max(m_nTotalIterations, 1) * (uint)items.Length;
        }

        public int this[string strName]
        {
            get { return m_slValues[strName]; }
        }

        public void Next()
        {
            lock (thislock)
            {
                foreach (VariantsItem currentItem in m_lVariants)
                {
                    if (currentItem.Next() == false) // no overflow
                    {
                        m_slValues[currentItem.Name] = currentItem.Current;
                        break;
                    }

                    m_slValues[currentItem.Name] = currentItem.Current;
                }

                m_nIterations++;
            }
        }

        public bool HasMoreIterations
        {
            get { return m_nIterations <= m_nTotalIterations; }
        }

        public uint TotalIterations
        {
            get { return m_nTotalIterations; }
        }

        public uint Iterations
        {
            get { return m_nIterations; }
        }

        /// <summary>
        /// Liefert den Fertigstellungsgrad in Prozent zwischen 0 und 100
        /// </summary>
        public uint Completeness
        {
            get { return (100 * m_nIterations) / Math.Max(m_nTotalIterations,1); }
        }
    }
}
