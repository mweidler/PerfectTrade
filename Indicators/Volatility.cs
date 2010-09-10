//
// Volatility.cs
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
   /// Die Volatilitaet ist ein Mass für die Streuung der Werte um ihren Mittelwert
   /// und entspricht der Standard-Abweichung
   /// </summary>
   ///
   /// Sie ist definiert als die positive Quadratwurzel aus der Varianz der Werte und wird mit
   /// \f[
   /// \sigma_x = \sqrt{Varianz(X)}
   /// \f]
   /// berechnet, wobei die Varianz
   /// \f[
   ///  Varianz(X) = \frac{1}{N-1} \sum_{i=1}^N{(x_i-\bar{x})^2}
   /// \f]
   /// und der Mittelwert
   /// \f[
   /// \bar{x} = \frac{1}{N} \sum_{i=1}^N{x_i}
   /// \f]
   /// ist.
   public class Volatility
   {
      /// <summary>
      /// Berechnet die Volatilitaet/Standardabweichung und legt das Ergebnis
      /// in einem neuen Datencontainer ab.
      /// </summary>
      /// <param name="source">Datensatz, von dem die Volatilitaet gebildet werden soll</param>
      /// <param name="nRange">Anzahl der einzubeziehenden Daten pro Berechnung</param>
      /// <returns>Neuer DatenContainer mit den Ergebnisdaten</returns>
      public static DataContainer CreateFrom(DataContainer source, uint nRange)
      {
         if (nRange < 1)
            throw new ArgumentOutOfRangeException("Average", nRange, "Must be greater than zero.");

         DataContainer result = new DataContainer();

         double dSum = 0;
         double dSumSquare = 0;

         WorkDate workdate = source.OldestDate.Clone();
         WorkDate helperworkdate = source.OldestDate.Clone();

         for (int i = 0; i < nRange - 1; i++, workdate++)
         {
            double dValue = source[workdate];
            dSum += dValue;
            dSumSquare += dValue * dValue;
         }

         do
         {
            double dValue = source[workdate];
            dSum += dValue;
            dSumSquare += dValue * dValue;

            double dVariance = ((nRange * dSumSquare) - (dSum * dSum)) / (nRange * (nRange - 1));
            dVariance = Math.Abs(dVariance);
            double dStdDev = Math.Sqrt(dVariance);
            result[workdate] = dStdDev;
            workdate++;

            double dPastValue = source[helperworkdate];
            dSum -= dPastValue;
            dSumSquare -= dPastValue * dPastValue;
            helperworkdate++;

         }
         while (workdate <= source.YoungestDate);

         return result;
      }
   }
}
