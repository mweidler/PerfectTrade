//
// PriceOscillator.cs
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
   /// Durch das Bilden des Quotienten schwanken die Indikator-Werte nach oben und unten
   /// offen um die 0. Ein Signal wird durch das Kreuzen der 0 erzeugt.
   /// </summary>
   ///
   /// \f[ PPO = \frac{MovAvg_Fast}{MovAvg_Slow} - 1 \f]
   ///
   /// Ein Wert oberhalb der 0 zeigt an, dass der kuerzere GD ueber dem laengeren liegt
   /// und damit ein Aufwaertstrend im Basistitel vorherrscht. Umgekehrt zeigt ein Wert
   /// unterhalb der 0 an, dass der kuerzere GD unter dem laengeren liegt und damit
   /// ein Abwaertstrend im Basistitel vorherrscht.
   public class PriceOscillator
   {
      ///<summary>
      /// Berechnet den PriceOscillator und legt das Ergebnis in einem neuen Datencontainer ab.
      /// </summary>
      /// <param name="source">Daten, von dem der Indikator gebildet werden soll</param>
      /// <param name="nAverageFast">Anzahl der Daten, des kuerzeren GD</param>
      /// <param name="nAverageSlow">Anzahl der Daten, des laengerem GD</param>
      /// <returns>Neuer DatenContainer mit den Ergebnisdaten</returns>
      public static DataContainer CreateFrom(DataContainer source, int nAverageFast, int nAverageSlow)
      {
         if (nAverageFast > nAverageSlow)
            throw(new ArgumentOutOfRangeException());

         DataContainer fast = MovingAverage.CreateFrom(source, nAverageFast);
         DataContainer slow = MovingAverage.CreateFrom(source, nAverageSlow);
         DataContainer result = new DataContainer();

         for (WorkDate workdate = slow.OldestDate.Clone(); workdate <= slow.YoungestDate; workdate++)
         {
            //double dPerf = Math.Log(fast[workdate] / slow[workdate], 2.0) * 100.0;
            double dPerf = (fast[workdate] / slow[workdate]) - 1.0;
            result[workdate] = dPerf;
         }

         return result;
      }
   }
}
