/*
 * PriceOscillator.cs
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
    /// Durch das Bilden des Quotienten schwanken die Indikator-Werte nach oben und unten
    /// offen um die 0. Ein Signal wird durch das Kreuzen der 0 erzeugt.
    /// </summary>
    ///
    /// \f[ PPO = \frac{MovAvg_Fast}{MovAvg_Slow} - 1 \f]
    ///
    /// Ein Wert oberhalb der 0 zeigt an, dass der kuerzere GD ueber dem laengeren liegt
    /// und damit ein Aufwaertstrend im Basistitel vorherrscht. Umgekehrt zeigt ein Wert
    /// unterhalb der 0 an, dass der kuerzere GD unter dem laengeren liegt und damit
    /// ein Abwaertstrend im Basistitel vorherrscht.
    public class PriceOscillator
    {
        ///<summary>
        /// Berechnet den PriceOscillator und legt das Ergebnis in einem neuen Datencontainer ab.
        /// </summary>
        /// <param name="source">Daten, von dem der Indikator gebildet werden soll</param>
        /// <param name="nAverageFast">Anzahl der Daten, des kuerzeren GD</param>
        /// <param name="nAverageSlow">Anzahl der Daten, des laengerem GD</param>
        /// <returns>Neuer DatenContainer mit den Ergebnisdaten</returns>
        public static DataContainer CreateFrom(DataContainer source, uint nAverageFast, uint nAverageSlow)
        {
            if (nAverageFast > nAverageSlow)
                throw (new ArgumentOutOfRangeException());

            DataContainer fast = MovingAverage.CreateFrom(source, nAverageFast);
            DataContainer slow = MovingAverage.CreateFrom(source, nAverageSlow);
            DataContainer result = new DataContainer();

            for (WorkDate workdate = slow.OldestDate.Clone(); workdate <= slow.YoungestDate; workdate++)
            {
                //double dPerf = Math.Log(fast[workdate] / slow[workdate], 2.0) * 100.0;
                double dPerf = (fast[workdate] / slow[workdate]) - 1.0;
                result[workdate] = dPerf;
            }

            return result;
        }
    }
}
