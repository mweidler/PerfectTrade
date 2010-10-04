//
// RelativeStrength.cs
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
using System.IO;
using FinancialObjects;
using Indicators;

namespace Analyzer
{
   public class RelativeStrength : IAnalyzerEngine
   {
      public RelativeStrength()
      {
         // empty
      }

      #region IAnalyzerEngine Member
      public string GetApplicationName()
      {
         return "RelativeStrength";
      }

      public void Analyze()
      {
         DBEngine dbengine = DBEngine.GetInstance();

         if (dbengine.Exists("846900") == false)
            return;

         Chart chart = new Chart();
         chart.AutoDeleteTempFiles = false;
         chart.Width = 1500;
         chart.Height = 900;
         Stock dax = dbengine.GetStock("846900");
         DataContainer quotes = dax.Quotes;
         DataContainer dax_ma38 = MovingAverage.CreateFrom(quotes, 38);

         WorkDate startDate = quotes.YoungestDate.Clone();
         startDate.Set(startDate.Year - 20, startDate.Month, 1);

         DataContainer dax_ranged = quotes.Clone(startDate);


         chart.Clear();
         chart.SubSectionsX = 6;
         chart.LogScaleY = true;
         chart.TicsYInterval = 1000;
         chart.Title = dax_ranged.OldestDate.ToString() + " - " + dax_ranged.YoungestDate.ToString();
         chart.LabelY = "Punkte";
         chart.Add(dax_ranged, 1, "DAX");
         chart.Add(dax_ma38, 2, "DAX (ma38)");
         chart.Create(World.GetInstance().ResultPath + "dax.png");

         DataContainer dax_diff_ma38 = Difference.CreateFrom(quotes, dax_ma38).Clone(startDate);

         DataContainer dax_rel_diff_38 = RelativeDifference.CreateFrom(quotes, dax_ma38);
         dax_rel_diff_38 = dax_rel_diff_38.Clone(startDate);
         chart.Clear();
         chart.Title = dax_ranged.OldestDate.ToString() + " - " + dax_ranged.YoungestDate.ToString();
         chart.LabelY = "dB%";
         chart.Add(dax_rel_diff_38, 2, "DAX (rel diff 38)");
         chart.Create(World.GetInstance().ResultPath + "dax_rel_diff_38.png");

         DataContainer dax_relperf = RelativePerformance.CreateFrom(quotes, startDate);
         dax_relperf = dax_relperf.Clone(startDate);

         chart.Clear();
         chart.SubSectionsX = 2;
         chart.Title = dax_diff_ma38.OldestDate.ToString() + " - " + dax_diff_ma38.YoungestDate.ToString();
         chart.LabelY = "Abstand zum Durchschnitt";
         //chart.Add(dax_ranged, 1, "DAX (Performance)");
         //chart.Add(dax_ma38, 2, "DAX (ma38)");
         //chart.Add(dax_ma200, 3, "DAX (ma200)");
         chart.Add(dax_diff_ma38, 2, "DAX rel. ma38");
         chart.Create(World.GetInstance().ResultPath + "dax_relperf.png");
      }
      #endregion
   }
}
