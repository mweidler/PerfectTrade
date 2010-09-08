/*
 * Difference.cs
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
    /// Berechnet die Differenz zweier Datensaetze.
    /// </summary>
    ///
    /// Die Differenz wird wie folgt berechnet:
    /// \f[ Diff = a - b \f]
    public class Difference
    {
        /// <summary>
        /// Berechnet die Differenz und legt das Ergebnis in einem neuen Datencontainer ab.
        /// </summary>
        /// <param name="a">Datensatz a, von dem Datensatz B abgezogen werden soll</param>
        /// <param name="b">Datensatz b, der von Datensatz A abgezogen werden soll</param>
        /// <returns>Neuer DatenContainer mit den Ergebnisdaten</returns>
        public static DataContainer CreateFrom(DataContainer a, DataContainer b)
        {
            DataContainer result = new DataContainer();

            WorkDate workdate = a.OldestDate.Clone();
            for (; workdate <= a.YoungestDate; workdate++)
            {
                if (b.Contains(workdate))
                {
                    result[workdate] = a[workdate] - b[workdate];
                }
            }

            return result;
        }
    }
}
