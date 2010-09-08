/*
 * MovingAverage.cs
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
    /// Der Begriff "Gleitender Durchschnitt" (MovingAverage) drueckt die beiden wichtigsten
    /// Eigenschaften des Indikators aus.
    /// </summary>
    /// Durchschnitt heiﬂt, dass ueber eine bestimmte Anzahl von Tagen ein Mittelwert der Kurse
    /// gebildet wird. Gleitend drueckt aus, dass die Berechnung mit jedem neuen Kurs um einen
    /// Tag nach vorne verschoben wird, der bis dahin letzte Kurs faellt also aus der Berechnung hinaus.
    /// Der Mittelwert ist "trendfolgend", daher ist der GD der einfachste (und wohl auch wichtigste)
    /// aller Trendfolger.
    ///
    /// Die Mittelwert wird wie folgt berechnet:
    /// \f[ \bar{x} = \frac{1}{n} \sum_{i=1}^n{x_i} = \frac{x_1 + x_2 + \cdots + x_n}{n} \f]
    public class MovingAverage
    {
        /// <summary>
        /// Berechnet den gleitenden Durschschnitt und legt das Ergebnis in einem neuen Datencontainer ab.
        /// </summary>
        /// <param name="source">Daten, von dem der gleitende Durchschnitt gebildet werden soll</param>
        /// <param name="nAverage">Anzahl der Daten, von dem der Durchschnitt gebildet werden soll</param>
        /// <returns>Neuer DatenContainer mit den Ergebnisdaten</returns>
        public static DataContainer CreateFrom(DataContainer source, uint nAverage)
        {
            if (nAverage < 1)
                throw new ArgumentOutOfRangeException("nAverage", nAverage, "Must be greater than zero.");

            DataContainer result = new DataContainer();

            if (source.Count >= nAverage)
            {
                double dSum = 0;
                WorkDate workdate = source.OldestDate.Clone();
                WorkDate historyworkdate = source.OldestDate.Clone();

                for (int i = 0; i < nAverage-1; i++, workdate++)
                {
                    dSum += source[workdate];
                }

                do
                {
                    dSum += source[workdate];
                    result[workdate] = dSum / nAverage;
                    workdate++;
                    dSum -= source[historyworkdate];
                    historyworkdate++;
                }
                while (workdate <= source.YoungestDate);
            }

            return result;
        }
    }
}
