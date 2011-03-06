//
// ProfitStatistic.cs
//
// COPYRIGHT (C) 2011 AND ALL RIGHTS RESERVED BY
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
   public class ProfitStatistic : IAnalyzerEngine
   {
      #region IAnalyzerEngine Member
      public void Analyze()
      {
         const int nInvestPeriodStep = 5;
         const int nMaxInvestPeriod = 30;

         DBEngine dbengine = DBEngine.GetInstance();

         DataContainer[] dcPerformances = new DataContainer[5];
         for (int i = 0; i < dcPerformances.Length; i++)
         {
            dcPerformances[i] = new DataContainer();
         }

         if (dbengine.Exists("846900") == false)
            return;

         Stock dax = dbengine.GetStock("846900");
         DataContainer quotes = dax.Quotes;

         WorkDate fromDate = quotes.YoungestDate.Clone();
         fromDate.Set(fromDate.Year - 1, fromDate.Month, 1);
         WorkDate endDate = quotes.YoungestDate.Clone() - nMaxInvestPeriod;

         DataContainer dax_ma38 = MovingAverage.CreateFrom(quotes, 38);
         dax_ma38 = dax_ma38.Clone(fromDate);

         DataContainer dax_rel_diff_38 = RelativeDifference.CreateFrom(quotes, dax_ma38);
         dax_rel_diff_38 = dax_rel_diff_38.Clone(fromDate);

         for (; fromDate < endDate; fromDate++)
         {
            for (int nInvestPeriod = 10; nInvestPeriod <= nMaxInvestPeriod; nInvestPeriod += nInvestPeriodStep)
            {
               double dPerf = ((quotes[fromDate + nInvestPeriod] / quotes[fromDate]) - 1) * 100.0;
               dcPerformances[(nInvestPeriod / nInvestPeriodStep) - 2][fromDate] = dPerf;
            }
         }


         Chart chart = new Chart();
         chart.Width = 1500;
         chart.Height = 900;
         chart.Clear();
         chart.SubSectionsX = 8;
         chart.LogScaleY = false;
         chart.Title = dax_rel_diff_38.OldestDate.ToString() + " - " + dax_rel_diff_38.YoungestDate.ToString();
         chart.LabelY = "Performance (%)";
         chart.RightDate = quotes.YoungestDate;
         chart.Add(dcPerformances[0], Chart.LineType.SkyBlue, "10");
         chart.Add(dcPerformances[1], Chart.LineType.SkyBlue, "15");
         chart.Add(dcPerformances[2], Chart.LineType.SkyBlue, "20");
         chart.Add(dcPerformances[3], Chart.LineType.SkyBlue, "25");
         chart.Add(dcPerformances[4], Chart.LineType.SkyBlue, "30");
         chart.Add(dax_rel_diff_38,   Chart.LineType.Red,     "DAX rel. diff. to MA38");
         chart.Create(World.GetInstance().ResultPath + "ProfitStatistik.png");
      }
      #endregion
   }
}
