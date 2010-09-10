//
// AverageDeviation.cs
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
   /// </summary>
   public class AverageDeviation
   {
      /// <summary>
      /// Berechnet die durchschnittliche Abweichung in dB% vom Durchschnitt.
      /// </summary>
      /// <param name="source">Daten, von dem die durchschnittliche Abweichung gebildet werden soll</param>
      /// <param name="nAverage">Anzahl der Daten, von dem die durchschnittliche Abweichung gebildet werden soll</param>
      /// <returns>Neuer DatenContainer mit den Ergebnisdaten</returns>
      public static DataContainer CreateFrom(DataContainer source, uint nAverage)
      {
         if (nAverage < 1)
            throw new ArgumentOutOfRangeException("nAverage", nAverage, "Must be greater than zero.");

         DataContainer result = new DataContainer();
         DataContainer relperf = RelativePerformance.CreateFrom(source);
         DataContainer movavg = MovingAverage.CreateFrom(relperf, nAverage);

         if (movavg.Count >= nAverage)
         {
            double dSum = 0;
            WorkDate workdate = movavg.OldestDate.Clone();
            WorkDate historyworkdate = movavg.OldestDate.Clone();

            for (int i = 0; i < nAverage - 1; i++, workdate++)
            {
               dSum += Math.Abs(relperf[workdate] - movavg[workdate]);
            }

            do
            {
               dSum += Math.Abs(relperf[workdate] - movavg[workdate]);
               result[workdate] = dSum / nAverage;
               workdate++;
               dSum -= Math.Abs(relperf[historyworkdate] - movavg[historyworkdate]);
               historyworkdate++;
            }
            while (workdate <= source.YoungestDate);
         }

         return result;
      }
   }
}
