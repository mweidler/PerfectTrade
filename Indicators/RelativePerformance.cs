/*
 * RelativePerformance.cs
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
    /// Berechnet die relative Veraenderung jedes Einzelwertes zum ersten
    /// Element der Datenreihe in dB%.
    /// </summary>
    ///
    /// Dazu wird der Quotient aus jedem Element mit dem ersten Wert gebildet,
    /// logarithmiert und mit 100 multipliziert. Durch Verwendung des
    /// Logarithmus zur Basis 2 resultiert aus einem Veraenderungsfaktor
    /// von 2 (sprich Verdoppelung) ein dB%-Wert von 100.
    /// Eine Halbierung entspricht einem dB%-Wert von -100.
    ///
    /// RelativePerformance wird wie folgt berechnet: Log_2
    /// \f[ 
    ///  x_r = \log{\frac{x_n}{x_(0)}}
    /// \f]
    public class RelativePerformance
    {
        /// <summary>
        /// Berechnet die relative Veraenderung jedes Einzelwertes
        /// zum ersten Element der Datenreihe in dB%.
        /// </summary>
        ///
        /// <param name="source">Daten, von denen die relativen Veraenderungen gebildet werden sollen</param>
        /// <returns>Neuer DatenContainer mit den Ergebnisdaten</returns>
        public static DataContainer CreateFrom(DataContainer source)
        {
            return CreateFrom(source, source.OldestDate);
        }

        public static DataContainer CreateFrom(DataContainer source, WorkDate referenceDate)
        {
            DataContainer result = new DataContainer();
            WorkDate workdate = source.OldestDate.Clone();
            double dReference = source[referenceDate];

            for (; workdate <= source.YoungestDate; workdate++)
            {
                result[workdate] = Math.Log(source[workdate] / dReference, 2.0) * 100.0;
                //result[workdate] = ((source[workdate] / dReference) - 1.0) * 100.0;
            }

            return result;
        }

    }
}
