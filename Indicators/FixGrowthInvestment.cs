/*
 * FixedGrowthInvestment.cs
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
    /// <summary>
    /// Berechnet eine Performance-Entwicklung einer festverzinslichen Anlage
    /// im angegebenen Zeitraum und mit dem gewuenschten Zinssatz.
    /// </summary>
    public class FixedGrowthInvestment
    {
        /// <summary>
        /// Fuehrt eine Zinseszinsberechnung auf der Basis des angegebenen
        /// Prozentsatzes und einem Startkapital von 100.0 aus.
        /// Die Berechnung wird für jeden Arbeitstag (und Feiertage), jedoch
        /// nicht fuer Wochenenden durchgefuehrt.
        /// </summary>
        /// <param name="fromDate">Beginn des Berechnungszeitraumes</param>
        /// <param name="toDate">Ende des Berechnungszeitraumes</param>
        /// <param name="dYearGrowth">Jahres-Performance in Prozent, z.B. 3.45</param>
        public static DataContainer CreateFrom(WorkDate fromDate, WorkDate toDate, double dYearGrowth)
        {
            DataContainer result = new DataContainer();

            dYearGrowth /= 100.0;
            double dPerformanceFactor = Math.Pow(1.0 + dYearGrowth, 1.0 / WorkDate.WorkDaysPerYear);
            double dEquity = 100.0;

            for (WorkDate workdate = fromDate.Clone(); workdate <= toDate; workdate++)
            {
                result[workdate] = dEquity;
                dEquity *= dPerformanceFactor;
            }

            return result;
        }
    }
}
