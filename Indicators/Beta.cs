//
// Beta.cs
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
