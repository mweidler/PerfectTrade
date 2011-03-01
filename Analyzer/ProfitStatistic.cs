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

         DataContainer buy_events = new DataContainer();
         DataContainer sell_events = new DataContainer();
         DataContainer[] dcPerformances = new DataContainer[5];

         for (int i = 0; i < dcPerformances.Length; i++)
         {
            dcPerformances[i] = new DataContainer();
         }

         if (dbengine.Exists("846900") == false)
            return;

         Stock dax = dbengine.GetStock("846900");
         dax.CheckPlausibility();
         DataContainer quotes = dax.Quotes;

         buy_events[new WorkDate(2011, 1, 3)] = 1;
         buy_events[new WorkDate(2011, 1, 14)] = 1;
         buy_events[new WorkDate(2011, 1, 23)] = 1;

         sell_events[new WorkDate(2011, 2, 8)] = 1;

         WorkDate fromDate = quotes.YoungestDate.Clone();
         fromDate.Set(fromDate.Year - 1, fromDate.Month, 1);
         WorkDate endDate = quotes.YoungestDate.Clone() - nMaxInvestPeriod;

         dcPerformances[0][quotes.YoungestDate] = 0;
         dcPerformances[0].FillGaps();

         for (; fromDate < endDate; fromDate++)
         {
            for (int nInvestPeriod = 10; nInvestPeriod <= nMaxInvestPeriod; nInvestPeriod += nInvestPeriodStep)
            {
               double dPerf = ((quotes[fromDate + nInvestPeriod] / quotes[fromDate]) - 1) * 100.0;
               dcPerformances[(nInvestPeriod / nInvestPeriodStep) - 2][fromDate] = dPerf;
            }
         }

         Chart chart = new Chart();
         chart.Width = 1000;
         chart.Height = 500;
         chart.Clear();
         chart.SubSectionsX = 8;
         chart.LogScaleY = false;
         chart.Title = dcPerformances[0].OldestDate.ToString() + " - " + dcPerformances[0].YoungestDate.ToString();
         chart.LabelY = "Performance (%)";
         chart.Add(dcPerformances[0], Chart.LineType.SeaGreen, "10");
         chart.Add(dcPerformances[1], Chart.LineType.Navy,    "15");
         chart.Add(dcPerformances[2], Chart.LineType.Orange,  "20");
         chart.Add(dcPerformances[3], Chart.LineType.Purple,  "25");
         chart.Add(dcPerformances[4], Chart.LineType.Red,  "30");
         chart.Add(buy_events, Chart.LineType.GoLong);
         chart.Add(sell_events, Chart.LineType.GoShort);
         chart.Create(World.GetInstance().ResultPath + "ProfitStatistik.png");
      }
      #endregion
   }
}
