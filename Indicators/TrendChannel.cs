/*
 * TrendChannel.cs
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
using FinancialObjects;

namespace Indicators
{
    /*! \brief Trend-Channel indicator
     * 
     * Computes a upper and lower line tangential to the ticker data.
     * The width/range and the end-position (day) can be changed.
     */
    public class TrendChannel : DataContainer
    {
        private enum LimitType
        {
            UPPER = 0,
            LOWER = 1
        };

        private int m_nRange;
        private DataContainer m_LowerLimit;
        private DataContainer m_UpperLimit;
        
        /*! \brief Constructs a new TrendChannel object.
         */
        public TrendChannel() : base()
        {
            m_nRange = 21;
            m_UpperLimit = new DataContainer();
            m_LowerLimit = new DataContainer();
        }

        public int Range
        {
            get { return m_nRange; }
        }

        public string GetName()
        {
            return "TrendChannel (" + m_nRange + ")";
        }

        public DataContainer UpperLimit
        {
            get { return m_UpperLimit; }
        }

        public DataContainer LowerLimit
        {
            get { return m_LowerLimit; }
        }

        public void CreateFrom(DataContainer source, WorkDate today)
        {
            Console.Out.WriteLine("TrendChannel CreateFrom called\n");
            this.Clear();
            m_UpperLimit.Clear();
            m_LowerLimit.Clear();

            for (WorkDate date = new WorkDate(today); date > today-300/*source.GetOldestDate()+m_nRange*/; date--)
            {
                trendLine(m_UpperLimit, source, date, LimitType.UPPER);
                trendLine(m_LowerLimit, source, date, LimitType.LOWER);

                Console.Out.WriteLine(date);
            }

            for (WorkDate date = new WorkDate(m_UpperLimit.OldestDate); date <= m_UpperLimit.YoungestDate; date++)
            {
                if (m_LowerLimit.Contains(date))
                {
                    this[date] = (m_UpperLimit[date] + m_LowerLimit[date])/2.0;
                }
            }
        }

        /*
         * Berechnet die obere oder untere Trendlinie eines Charts.
         */
        private void trendLine(DataContainer target, DataContainer source, WorkDate today, LimitType limit)
        {
            WorkDate fromDate = new WorkDate(today);
            fromDate -= m_nRange;

            WorkDate bestLeft = new WorkDate(fromDate);
            WorkDate bestRight = new WorkDate(fromDate);

            for (; fromDate < today; fromDate++ )
            {
                WorkDate scanDate = new WorkDate(today);
                scanDate -= m_nRange / 2;

                for (; scanDate < today; scanDate++ )
                {
                    if (isTangential(source, fromDate, scanDate, limit))
                    {
                        // laenger, als der bereits gefundene
                        // UND Linkes und rechtes Ende der Gerade muss in jeweils einer haelfte liegen
                        if (scanDate - fromDate > bestRight - bestLeft)
                        {
                            bestLeft = new WorkDate(fromDate);
                            bestRight = new WorkDate(scanDate);
                        }
                    }
                }
            }

            double dm = (double)(source[bestRight] - source[bestLeft]) / (double)(bestRight - bestLeft);

            target[today] = source[bestLeft] + (double)(dm * (today - bestLeft));

            /*fromDate = new WorkDate(today);
            fromDate -= m_nRange;

            for (; fromDate < today; fromDate++ )
            {
                target[fromDate] = source[bestLeft] + (double)(dm * (fromDate - bestLeft));
            }*/
        }

        /*
         * Prüft, ob die Gerade X1/X2 keinen anderen Punkt im Chart
         * innerhalb der vorgegebenen Grenzen schneidet.
         * Berührungen sind erlaubt!
         */
        private bool isTangential(DataContainer source, WorkDate left, WorkDate right, LimitType limit)
        {
            double iY1 = source[left];
            double iY2 = source[right];

            double dm = (double)(iY2 - iY1) / (double)(right - left);

            for (WorkDate i = new WorkDate(left+1); i < right; i++)
            {
                double iDiff = source[i] - source[left] - (double)(dm * (i - left));
                if ( (limit == LimitType.LOWER && iDiff <= 0) ||
                     (limit == LimitType.UPPER && iDiff >= 0))
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}
