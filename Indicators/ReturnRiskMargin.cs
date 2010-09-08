/*
 * RiskReturnRatio.cs
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
    /// Das ReturnRiskRatio ist vergleichbar mit dem SharpeRatio und gibt das Rendite/Risiko-
    /// Verhaeltnis eines Wertpapiers an.
    /// </summary>
    /// Das RRR einer risikolosen Anlage betraegt dessen Zinssatz.
    ///
    /// Das ReturnRiskRatio wird wie folgt berechnet:
    /// \f[ RRR_T = \frac{{Rendite_T}}{1+\sigma_T} \f]
    public class ReturnRiskMargin
    {
        /// <summary>
        /// Berechnet die ReturnRiskRatio und legt das Ergebnis in einem neuen Datencontainer ab.
        /// </summary>
        /// <param name="source">Datensatz, von dem die ReturnRiskRatio gebildet werden soll</param>
        /// <param name="nRange">Anzahl der einzubeziehenden Daten pro Berechnung</param>
        /// <returns>Neuer DatenContainer mit den Ergebnisdaten</returns>
        public static DataContainer CreateFrom(DataContainer source, uint nRange)
        {
            //DataContainer sourceperf   = RelativePerformance.CreateFrom(source);
            DataContainer sourcechange = RelativeChange.CreateFrom(source);
            DataContainer changemovavg = MovingAverage.CreateFrom(sourcechange, nRange);
            DataContainer volatility   = Volatility.CreateFrom(sourcechange, nRange);
            DataContainer result       = new DataContainer();

            //WorkDate historydate = volatility.OldestDate.Clone() - (int)nRange;
            WorkDate workdate = volatility.OldestDate.Clone();
            for (; workdate <= volatility.YoungestDate; workdate++/*, historydate++*/)
            {
                double dReturn = changemovavg[workdate]; //sourceperf[workdate] - sourceperf[historydate];
                double dRisk   = volatility[workdate]; // Standardabweichung
                //result[workdate] = dReturn  / (dRisk + 1.0);
                result[workdate] = dReturn - dRisk;
            }

            return result;
        }
    }
}
