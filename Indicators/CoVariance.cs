/*
 * CoVariance.cs
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
    /// Die Kovarianz ist eine Masszahl fuer den Zusammenhang zweier Datenreihen A und B.
    /// </summary>
    ///
    /// <list type="bullets">
    /// <item>Die Kovarianz ist positiv, wenn A und B tendenziell einen gleichsinnigen
    ///   linearen Zusammenhang besitzen, d. h. hohe Werte von A gehen mit hohen Werten
    ///   von B einher und niedrige mit niedrigen.</item>
    /// <item>Die Kovarianz ist hingegen negativ, wenn A und B einen gegensinnigen
    ///   linearen Zusammenhang aufweisen, d. h. hohe Werte der einen Zufallsvariablen
    ///   gehen mit niedrigen Werten der anderen Zufallsvariablen einher.</item>
    /// <item> Ist das Ergebnis 0, so besteht kein Zusammenhang oder ein nicht
    ///   linearer Zusammenhang z.B. eine U-foermige Beziehung zwischen den beiden
    ///   Variablen A und B.</item>
    /// </list>
    /// 
    /// Die Kovarianz gibt zwar die Richtung einer Beziehung zwischen zwei Variablen an,
    /// ueber die Staerke des Zusammenhangs wird aber keine Aussage getroffen.
    /// Um einen Zusammenhang vergleichbar zu machen, muss die Kovarianz standardisiert werden.
    /// Man erhaelt dann den Korrelationskoeffizienten (Korrelation), dessen Masszahl sich
    /// im Intervall von -1 bis +1 bewegt.
    ///
    /// Die Kovarianz wird wie folgt berechnet:
    /// \f[ Cov_{xy} = \frac{1}{n-1}\sum_{i=1}^n{(x_i-\bar{x}) (y_i-\bar{y})} \f]
    /// wobei
    /// \f[ \bar{x} = \frac{1}{n} \sum_{i=1}^n{x_i} \f]
    /// und
    /// \f[ \bar{y} = \frac{1}{n} \sum_{i=1}^n{y_i} \f]
    public class CoVariance
    {
        /// <summary>
        /// Berechnet die Kovrianz und legt das Ergebnis in einem neuen Datencontainer ab.
        /// </summary>
        ///
        /// <param name="a">Datenreihe A</param>
        /// <param name="b">Datenreihe B</param>
        /// <param name="nRange">Anzahl der Daten, von dem die Kovarianz gebildet werden soll</param>
        /// <returns>Neuer DatenContainer mit n Ergebnisdaten</returns>
        public static DataContainer CreateFrom(DataContainer a, DataContainer b, uint nRange)
        {
            if (nRange < 1)
                throw new ArgumentOutOfRangeException("Range", nRange, "Must be greater than zero.");

            DataContainer avg_a  = MovingAverage.CreateFrom(a, nRange);
            DataContainer avg_b  = MovingAverage.CreateFrom(b, nRange);
            DataContainer result = new DataContainer();

            double dSum = 0;
            WorkDate workdate = a.OldestDate.Clone();
            WorkDate historyworkdate = a.OldestDate.Clone();

            for (int i = 0; i < nRange - 1; i++, workdate++)
            {
                dSum += a[workdate] * b[workdate];
            }

            do
            {
                dSum += a[workdate] * b[workdate];
                double dTemp = dSum - (nRange * avg_a[workdate] * avg_b[workdate]);
                result[workdate] = dTemp / (nRange - 1);

                workdate++;
                dSum -= a[historyworkdate] * b[historyworkdate];
                historyworkdate++;
            }
            while (workdate <= a.YoungestDate);

            return result;
        }
    }
}
