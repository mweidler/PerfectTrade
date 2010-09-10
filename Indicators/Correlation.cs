//
// Correlation.cs
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
   /// Der Korrelationskoeffizient (Korrelation) ist ein dimensionsloses Mass fuer den Grad des
   /// linearen Zusammenhangs zwischen zwei Merkmalen.
   /// </summary>
   /// Er kann lediglich Werte zwischen -1 und 1 annehmen.
   ///
   /// Bei einem Wert von +1 (bzw. -1) besteht ein vollstaendig positiver (bzw. negativer)
   /// linearer Zusammenhang zwischen den betrachteten Merkmalen. Wenn der Korrelationskoeffizient
   /// den Wert 0 aufweist, haengen die beiden Merkmale ueberhaupt nicht linear voneinander ab.
   ///
   /// Die Korrelation wird wie folgt berechnet:
   /// \f[ Cor(X,Y) = \frac{COV(X,Y)}{\sigma_X \sigma_Y} \f]
   public class Correlation
   {
      /// <summary>
      /// Berechnet die Korrelation und legt das Ergebnis in einem neuen Datencontainer ab.
      /// </summary>
      /// <param name="a">Datensatz A, zwischen denen die Korrelation gebildet werden soll</param>
      /// <param name="b">Datensatz B, zwischen denen die Korrelation gebildet werden soll</param>
      /// <param name="nRange">Anzahl der einzubeziehenden Daten pro Korrelationsberechnung</param>
      /// <returns>Neuer DatenContainer mit den Ergebnisdaten</returns>
      public static DataContainer CreateFrom(DataContainer a, DataContainer b, uint nRange)
      {
         if (nRange < 1)
            throw new ArgumentOutOfRangeException("Range", nRange, "Must be greater than zero.");

         DataContainer covariance = CoVariance.CreateFrom(a, b, nRange);
         DataContainer sigma_a    = Volatility.CreateFrom(a, nRange);
         DataContainer sigma_b    = Volatility.CreateFrom(b, nRange);
         DataContainer result     = new DataContainer();

         WorkDate workdate = covariance.OldestDate.Clone();

         for (; workdate <= covariance.YoungestDate; workdate++)
         {
            double dCorrelation = covariance[workdate] / (sigma_a[workdate] * sigma_b[workdate]);
            result[workdate] = dCorrelation;
         }

         return result;
      }
   }
}
