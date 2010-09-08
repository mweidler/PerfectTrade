/*
 * volatility.cs
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
    /// Die Volatilitaet ist ein Mass für die Streuung der Werte um ihren Mittelwert
    /// und entspricht der Standard-Abweichung
    /// </summary>
    /// 
    /// Sie ist definiert als die positive Quadratwurzel aus der Varianz der Werte und wird mit
    /// \f[ 
    /// \sigma_x = \sqrt{Varianz(X)}
    /// \f]
    /// berechnet, wobei die Varianz
    /// \f[ 
    ///  Varianz(X) = \frac{1}{N-1} \sum_{i=1}^N{(x_i-\bar{x})^2}
    /// \f]
    /// und der Mittelwert
    /// \f[ 
    /// \bar{x} = \frac{1}{N} \sum_{i=1}^N{x_i}
    /// \f]
    /// ist.
    public class Volatility
    {
        /// <summary>
        /// Berechnet die Volatilitaet/Standardabweichung und legt das Ergebnis
        /// in einem neuen Datencontainer ab.
        /// </summary>
        /// <param name="source">Datensatz, von dem die Volatilitaet gebildet werden soll</param>
        /// <param name="nRange">Anzahl der einzubeziehenden Daten pro Berechnung</param>
        /// <returns>Neuer DatenContainer mit den Ergebnisdaten</returns>
        public static DataContainer CreateFrom(DataContainer source, uint nRange)
        {
            if (nRange < 1)
                throw new ArgumentOutOfRangeException("Average", nRange, "Must be greater than zero.");

            DataContainer result = new DataContainer();

            double dSum = 0;
            double dSumSquare = 0;

            WorkDate workdate = source.OldestDate.Clone();
            WorkDate helperworkdate = source.OldestDate.Clone();
            for (int i = 0; i < nRange - 1; i++, workdate++)
            {
                double dValue = source[workdate];
                dSum += dValue;
                dSumSquare += dValue * dValue;
            }

            do
            {
                double dValue = source[workdate];
                dSum += dValue;
                dSumSquare += dValue * dValue;

                double dVariance = ((nRange * dSumSquare) - (dSum * dSum)) / (nRange * (nRange - 1));
                dVariance = Math.Abs(dVariance);
                double dStdDev = Math.Sqrt(dVariance);
                result[workdate] = dStdDev;
                workdate++;

                double dPastValue = source[helperworkdate];
                dSum -= dPastValue;
                dSumSquare -= dPastValue * dPastValue;
                helperworkdate++;

            } while (workdate <= source.YoungestDate);

            return result;
        }
    }
}
