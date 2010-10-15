//
// EfficientPortfolio.cs
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
using System.IO;
using FinancialObjects;
using Indicators;

namespace Simulator
{
   public class EfficientPortfolio : IRuleEngine
   {
      private RuleEngineInfo m_RuleEngineInfo = new RuleEngineInfo();
      private DataContainer dax_close;
      private DataContainer dax_ma;
      private DataContainer dax_rel_diff;
      private DataContainer buy_events;
      private DataContainer sell_events;

      /// <summary>
      ///
      /// </summary>
      public EfficientPortfolio()
      {
      }

      #region ITradeRule Member
      // Called only once to init trade rule
      public void Setup()
      {
         this.RuleEngineInfo.FromDate = new WorkDate(2009, 10, 1);
         this.RuleEngineInfo.ToDate = new WorkDate(2010, 10, 10);

         this.RuleEngineInfo.Variants.Add("averaging", new int[] { 38 });

         this.RuleEngineInfo.MinimumInvestment = 5000.0;
         this.RuleEngineInfo.TargetPositions = 10;
         this.RuleEngineInfo.MaxLoss = 0.90;

         // Create virtual instuments:
         // Create DAXex from DAX-Index
         DataContainer dax = DBEngine.GetInstance().GetQuotes("846900").Clone();
         DataContainer put = new DataContainer();
         double dRef = dax[dax.OldestDate];

         for (WorkDate workdate = dax.OldestDate.Clone(); workdate <= dax.YoungestDate; workdate++)
         {
            put[workdate] = 100.0 * dRef / dax[workdate];
            dax[workdate] = dax[workdate] / 100.0;
         }

         DBEngine.GetInstance().AddVirtualInvestment("dax_long", "DAX EX", dax);
         DBEngine.GetInstance().AddVirtualInvestment("dax_short", "DAX EX Short", put);

         // Create some fixed growth investment stocks
         DataContainer fixedGrowth = FixedGrowthInvestment.CreateFrom(dax.OldestDate, dax.YoungestDate, 2);
         DBEngine.GetInstance().AddVirtualInvestment("Bond", "Festgeld", fixedGrowth);

         fixedGrowth = FixedGrowthInvestment.CreateFrom(dax.OldestDate, dax.YoungestDate, 10);
         DBEngine.GetInstance().AddVirtualInvestment("TargetPerf", "Soll-Performance", fixedGrowth);
      }

      public void StepDate()
      {
         WorkDate today = RuleEngineInfo.Today;
         today++;
      }

      public RuleEngineInfo RuleEngineInfo
      {
         get { return m_RuleEngineInfo; }
      }

      public bool IsValidVariant()
      {
         return true; // return (variants["fast"] < variants["slow"]);
      }

      /// <summary>
      /// Called each time, the variants have changed.
      /// Attention: May be called by different threads in parallel, dependend
      /// on the number of Cores of the system CPU.
      /// </summary>
      /// <param name="strWKN"></param>
      public void Prepare()
      {
         int nAveraging = this.RuleEngineInfo.Variants["averaging"];

         Stock dax = DBEngine.GetInstance().GetStock("846900");
         this.dax_close = dax.QuotesClose.Clone();
         this.dax_ma = MovingAverage.CreateFrom(dax_close, nAveraging);
         this.dax_rel_diff = RelativeDifference.CreateFrom(dax_close, dax_ma);
         dax_rel_diff = dax_rel_diff.Clone(this.RuleEngineInfo.FromDate);
         dax_ma = dax_ma.Clone(this.RuleEngineInfo.FromDate);
         dax_close = dax_close.Clone(this.RuleEngineInfo.FromDate);
         this.buy_events = new DataContainer();
         this.sell_events = new DataContainer();
      }

      public void Result()
      {
         Chart chart = new Chart();
         chart.Width = 1500;
         chart.Height = 900;
         chart.SubSectionsX = 8;
         chart.LogScaleY = false;
         chart.TicsYInterval = 1;
         chart.Title = dax_rel_diff.OldestDate.ToString() + " - " + dax_rel_diff.YoungestDate.ToString();
         chart.LabelY = "dB%";
         chart.Add(dax_rel_diff, Chart.LineType.Navy, "DAX rel. diff. to MA38");
         chart.Add(buy_events, Chart.LineType.GoLong);
         chart.Add(sell_events, Chart.LineType.GoShort);
         chart.Create(World.GetInstance().ResultPath + "dax_rel_diff.png");
      }

      private void WriteGnuPlotCfg(string strTemplateName, string strTargetName, string strSuffix, Ranking ranklist)
      {
         // TODO

         /*GnuPlotConfigFile gnuplot_p = new GnuPlotConfigFile();
         gnuplot_p.Create(World.GetInstance().DataPath + strTemplateName, World.GetInstance().DataPath + strTargetName);
         gnuplot_p.Options = "using 1:2";

         for (int i = 0; i < ranklist.Count; i++)
         {
            string strName = DBEngine.GetInstance().GetName(ranklist[i].ID);
            gnuplot_p.Write(ranklist[i].ID + strSuffix, strName + "(" + ranklist[i].Value + ")");
         }

         gnuplot_p.Finish();*/
      }

      public void Ranking()
      {
         // do nothing
      }

      public bool SellRule()
      {
         // Verkaufe, wenn RelDiff > 2.
         Depot depot = this.RuleEngineInfo.Depot;
         WorkDate today = RuleEngineInfo.Today;

         if (depot.ContainsKey("846900"))
         {
            if (dax_rel_diff[today] > 3)
            {
               Sell("846900");
               sell_events[today] = dax_rel_diff[today];
               return true;
            }
         }

         return false;
      }

      public bool BuyRule()
      {
         Depot depot = this.RuleEngineInfo.Depot;
         WorkDate today = RuleEngineInfo.Today;

         if (depot.ContainsKey("846900") == false)
         {
            if (dax_rel_diff[today] < -3)
            {
               Buy("846900");
               buy_events[today] = dax_rel_diff[today];
               return true;
            }
         }

         return false;
      }

      public bool Sell(string strWKN)
      {
         // Verkaufe kompletten Bestand
         Depot depot = this.RuleEngineInfo.Depot;
         WorkDate today = this.RuleEngineInfo.Today;
         depot.Sell(strWKN, 100000, today);
         return true;
      }

      public bool Buy(string strWKN)
      {
         WorkDate today = this.RuleEngineInfo.Today;
         Depot depot = this.RuleEngineInfo.Depot;

         double dPrice = DBEngine.GetInstance().GetPrice(strWKN, today);
         int nQuantity = (int)(depot.Cash / dPrice);
         depot.Buy(strWKN, nQuantity, today, dPrice);
         return true;
      }
      #endregion
   }
}
