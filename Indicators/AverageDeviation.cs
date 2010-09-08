/*
 * AverageDeviation.cs
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
    /// </summary>
    public class AverageDeviation
    {
        /// <summary>
        /// Berechnet die durchschnittliche Abweichung in dB% vom Durchschnitt.
        /// </summary>
        /// <param name="source">Daten, von dem die durchschnittliche Abweichung gebildet werden soll</param>
        /// <param name="nAverage">Anzahl der Daten, von dem die durchschnittliche Abweichung gebildet werden soll</param>
        /// <returns>Neuer DatenContainer mit den Ergebnisdaten</returns>
        public static DataContainer CreateFrom(DataContainer source, uint nAverage)
        {
            if (nAverage < 1)
                throw new ArgumentOutOfRangeException("nAverage", nAverage, "Must be greater than zero.");

            DataContainer result = new DataContainer();
            DataContainer relperf = RelativePerformance.CreateFrom(source);
            DataContainer movavg = MovingAverage.CreateFrom(relperf, nAverage);

            if (movavg.Count >= nAverage)
            {
                double dSum = 0;
                WorkDate workdate = movavg.OldestDate.Clone();
                WorkDate historyworkdate = movavg.OldestDate.Clone();

                for (int i = 0; i < nAverage-1; i++, workdate++)
                {
                    dSum += Math.Abs(relperf[workdate] - movavg[workdate]);
                }

                do
                {
                    dSum += Math.Abs(relperf[workdate] - movavg[workdate]);
                    result[workdate] = dSum / nAverage;
                    workdate++;
                    dSum -= Math.Abs(relperf[historyworkdate] - movavg[historyworkdate]);
                    historyworkdate++;
                }
                while (workdate <= source.YoungestDate);
            }

            return result;
        }
    }
}
