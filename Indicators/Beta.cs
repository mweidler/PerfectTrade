/*
 * Beta.cs
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
    /// Der Betafaktor \f$ \beta \f$ stellt in den auf dem Capital Asset Pricing Model (CAPM)
    /// aufbauenden finanzwissenschaftlichen Theorien die Kennzahl fuer das mit einer
    /// Investitions- oder Finanzierungsmassnahme uebernommene systematische Risiko
    /// (auch Marktrisiko genannt) dar.
    /// </summary>
    ///
    /// Beta wird wie folgt berechnet:
    /// \f[ \beta = \frac{COV(r_i, r_M)}{\sigma^2_M} \f]
    /// 
    /// Mit dem Betafaktor lassen sich drei Gruppen von Wertpapieren bilden:
    /// <list type="number">
    /// <item>\f$ \beta > 1 \f$ bedeutet: das Wertpapier bewegt sich in groesseren Schwankungen als der Gesamtmarkt</item>
    /// <item>\f$ \beta = 1 \f$ bedeutet: das Wertpapier bewegt sich gleich dem Gesamtmarkt</item>
    /// <item>\f$ \beta < 1 \f$ bedeutet: das Wertpapier bewegt sich weniger stark als der Gesamtmarkt.</item>
    /// </list>
    public class Beta
    {
        /// <summary>
        /// Berechnet Beta und legt das Ergebnis in einem neuen Datencontainer ab.
        /// </summary>
        /// <param name="source">Datensatz, von dem Beta gebildet werden soll</param>
        /// <param name="reference">Referenz, zu dem Beta gebildet werden soll</param>
        /// <param name="nRange">Anzahl der einzubeziehenden Daten pro Beta-Berechnung</param>
        /// <returns>Neuer DatenContainer mit den Ergebnisdaten</returns>
        public static DataContainer CreateFrom(DataContainer source, DataContainer reference, uint nRange)
        {
            if (nRange < 1)
                throw new ArgumentOutOfRangeException("Range", nRange, "Must be greater than zero.");

            DataContainer covariance = CoVariance.CreateFrom(source, reference, nRange);
            DataContainer sigma      = Volatility.CreateFrom(reference, nRange);
            DataContainer result     = new DataContainer();

            WorkDate workdate = covariance.OldestDate.Clone();
            for (; workdate <= covariance.YoungestDate; workdate++)
            {
                double dSigma = sigma[workdate];
                double dBeta = covariance[workdate] / (dSigma * dSigma);
                result[workdate] = dBeta;
            }

            return result;
        }
    }
}
