//
// ReturnRiskMargin.cs
//
// COPYRIGHT (C) 2010 AND ALL RIGHTS RESERVED BY
// MARC WEIDLER, ULRICHSTR. 12/1, 71672 MARBACH, GERMANY (MARC.WEIDLER@WEB.DE).
//
// ALL RIGHTS RESERVED. THIS SOFTWARE AND RELATED DOCUMENTATION ARE PROTECTED BY
// COPYRIGHT RESTRICTING ITS USE, COPYING, DISTRIBUTION, AND DECOMPILATION. NO PART
// OF THIS PRODUCT OR RELATED DOCUMENTATION MAY BE REPRODUCED IN ANY FORM BY ANY
// MEANS WITHOUT PRIOR WRITTEN AUTHORIZATION OF MARC WEIDLER OR HIS PARTNERS, IF ANY.
// UNLESS OTHERWISE ARRANGED, THIRD PARTIES MAY NOT HAVE ACCESS TO THIS PRODUCT OR
// RELATED DOCUMENTATION. SEE LICENSE FILE FOR DETAILS.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
// IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
// INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
// BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE
// OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
// OF THE POSSIBILITY OF SUCH DAMAGE. THE ENTIRE RISK AS TO THE QUALITY AND
// PERFORMANCE OF THE PROGRAM IS WITH YOU. SHOULD THE PROGRAM PROVE DEFECTIVE,
// YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.
//

using System;
using System.Collections.Generic;
using System.Text;
using FinancialObjects;

namespace Indicators
{
    /// <summary>
    /// Das ReturnRiskMargin ist vergleichbar mit dem SharpeRatio und gibt das Rendite/Risiko-
    /// Verhaeltnis eines Wertpapiers an.
    /// </summary>
    /// Das RRR einer risikolosen Anlage betraegt dessen Zinssatz.
    ///
    /// Das ReturnRiskMargin wird wie folgt berechnet:
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
