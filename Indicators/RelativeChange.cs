/*
 * RelativeChange.cs
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
    /// Berechnet die relative Veraenderung jedes Einzelwertes zu
    /// seinem Vorgaenger in dB%.
    /// </summary>
    /// Dazu wird der Quotient aus vorangegangenem und aktuellen Wert
    /// logarithmiert und mit 100 multipliziert. Durch Verwendung des
    /// Logarithmus zur Basis 2 resultiert aus einem Veraenderungsfaktor
    /// von 2 (sprich Verdoppelung) ein dB%-Wert von 100.
    /// Eine Halbierung entspricht einem dB%-Wert von -100.
    ///
    /// RelativeChange wird wie folgt berechnet:
    /// \f[ x_r = 100 \cdot \log_2{\frac{x_n}{ x_{(n-1)}}} \f]
    ///
    /// Folgende Tabelle veranschaulicht den Zusammenhang:
    /// <code>
    /// Quot.	dB_2    dB_2% 	  % (verwendet Basis 2 des Log)
    /// 10,00	3,32	332,19	900
    ///  9,00	3,17	316,99	800
    ///  8,00	3,00	300,00	700
    ///  7,00	2,81	280,74	600
    ///  6,00	2,58	258,50	500
    ///  5,00	2,32	232,19	400
    ///  4,00	2,00	200,00	300
    ///  3,00	1,58	158,50	200
    ///  2,90	1,54	153,61	190
    ///  2,80	1,49	148,54	180
    ///  2,70	1,43	143,30	170
    ///  2,60	1,38	137,85	160
    ///  2,50	1,32	132,19	150
    ///  2,40	1,26	126,30	140
    ///  2,30	1,20	120,16	130
    ///  2,20	1,14	113,75	120
    ///  2,10	1,07	107,04	110
    ///  2,00	1,00	100,00	100
    ///  1,50	0,58	058,50	 50
    ///  1,40	0,49	048,54	 40
    ///  1,30	0,38	 37,85 	 30
    ///  1,20	0,26	 26,30	 20
    ///  1,10	0,14	 13,75	 10
    ///  1,00	0,00	  0,00	  0
    ///  0,90	-0,15	-15,20	-10
    ///  0,80	-0,32	-32,19	-20
    ///  0,70	-0,51	-51,46	-30
    ///  0,60	-0,74	-73,70	-40
    ///  0,50	-1,00	-100,00	-50
    ///  0,40	-1,32	-132,19	-60
    ///  0,30	-1,74	-173,70	-70
    ///  0,25	-2,00	-200,00	-75
    ///  0,13	-3,00	-300,00	-87,5
    /// </code>
    public class RelativeChange
    {
        /// <summary>
        /// Berechnet die relative Veraenderung jedes Einzelwertes zu seinem Vorgaenger in dB%.
        /// </summary>
        /// <param name="source">Daten, von dem die logarithmierten Kursrenditen gebildet werden soll</param>
        /// <returns>Neuer DatenContainer mit den Ergebnisdaten</returns>
        public static DataContainer CreateFrom(DataContainer source)
        {
            return CreateFrom(source, 1);
        }

        public static DataContainer CreateFrom(DataContainer source, int nRange)
        {
            DataContainer result = new DataContainer();

            WorkDate workdate = source.OldestDate.Clone() + nRange;
            for (; workdate <= source.YoungestDate; workdate++)
            {
                double dPerf = Math.Log(source[workdate] / source[workdate-nRange], 2.0) * 100.0;
                //double dPerf = ((source[workdate] / source[workdate - 1]) - 1.0) * 100.0;
                result[workdate] = dPerf;
            }
            
            return result;
        }
    }
}
