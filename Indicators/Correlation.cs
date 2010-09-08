/*
 * Correlation.cs
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
    /// Der Korrelationskoeffizient (Korrelation) ist ein dimensionsloses Mass fuer den Grad des
    /// linearen Zusammenhangs zwischen zwei Merkmalen.
    /// </summary>
    /// Er kann lediglich Werte zwischen -1 und 1 annehmen.
    /// 
    /// Bei einem Wert von +1 (bzw. -1) besteht ein vollstaendig positiver (bzw. negativer)
    /// linearer Zusammenhang zwischen den betrachteten Merkmalen. Wenn der Korrelationskoeffizient
    /// den Wert 0 aufweist, haengen die beiden Merkmale ueberhaupt nicht linear voneinander ab.
    /// 
    /// Die Korrelation wird wie folgt berechnet:
    /// \f[ Cor(X,Y) = \frac{COV(X,Y)}{\sigma_X \sigma_Y} \f]
    public class Correlation
    {
        /// <summary>
        /// Berechnet die Korrelation und legt das Ergebnis in einem neuen Datencontainer ab.
        /// </summary>
        /// <param name="a">Datensatz A, zwischen denen die Korrelation gebildet werden soll</param>
        /// <param name="b">Datensatz B, zwischen denen die Korrelation gebildet werden soll</param>
        /// <param name="nRange">Anzahl der einzubeziehenden Daten pro Korrelationsberechnung</param>
        /// <returns>Neuer DatenContainer mit den Ergebnisdaten</returns>
        public static DataContainer CreateFrom(DataContainer a, DataContainer b, uint nRange)
        {
            if (nRange < 1)
                throw new ArgumentOutOfRangeException("Range", nRange, "Must be greater than zero.");

            DataContainer covariance = CoVariance.CreateFrom(a, b, nRange);
            DataContainer sigma_a    = Volatility.CreateFrom(a, nRange);
            DataContainer sigma_b    = Volatility.CreateFrom(b, nRange);
            DataContainer result     = new DataContainer();

            WorkDate workdate = covariance.OldestDate.Clone();

            for (; workdate <= covariance.YoungestDate; workdate++)
            {
                double dCorrelation = covariance[workdate] / (sigma_a[workdate] * sigma_b[workdate]);
                result[workdate] = dCorrelation;
            }

            return result;
        }
    }
}
