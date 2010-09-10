//
// MovingAverage.cs
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
   /// Der Begriff "Gleitender Durchschnitt" (MovingAverage) drueckt die beiden wichtigsten
   /// Eigenschaften des Indikators aus.
   /// </summary>
   /// Durchschnitt heiﬂt, dass ueber eine bestimmte Anzahl von Tagen ein Mittelwert der Kurse
   /// gebildet wird. Gleitend drueckt aus, dass die Berechnung mit jedem neuen Kurs um einen
   /// Tag nach vorne verschoben wird, der bis dahin letzte Kurs faellt also aus der Berechnung hinaus.
   /// Der Mittelwert ist "trendfolgend", daher ist der GD der einfachste (und wohl auch wichtigste)
   /// aller Trendfolger.
   ///
   /// Die Mittelwert wird wie folgt berechnet:
   /// \f[ \bar{x} = \frac{1}{n} \sum_{i=1}^n{x_i} = \frac{x_1 + x_2 + \cdots + x_n}{n} \f]
   public class MovingAverage
   {
      /// <summary>
      /// Berechnet den gleitenden Durschschnitt und legt das Ergebnis in einem neuen Datencontainer ab.
      /// </summary>
      /// <param name="source">Daten, von dem der gleitende Durchschnitt gebildet werden soll</param>
      /// <param name="nAverage">Anzahl der Daten, von dem der Durchschnitt gebildet werden soll</param>
      /// <returns>Neuer DatenContainer mit den Ergebnisdaten</returns>
      public static DataContainer CreateFrom(DataContainer source, uint nAverage)
      {
         if (nAverage < 1)
            throw new ArgumentOutOfRangeException("nAverage", nAverage, "Must be greater than zero.");

         DataContainer result = new DataContainer();

         if (source.Count >= nAverage)
         {
            double dSum = 0;
            WorkDate workdate = source.OldestDate.Clone();
            WorkDate historyworkdate = source.OldestDate.Clone();

            for (int i = 0; i < nAverage - 1; i++, workdate++)
            {
               dSum += source[workdate];
            }

            do
            {
               dSum += source[workdate];
               result[workdate] = dSum / nAverage;
               workdate++;
               dSum -= source[historyworkdate];
               historyworkdate++;
            }
            while (workdate <= source.YoungestDate);
         }

         return result;
      }
   }
}
