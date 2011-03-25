//
// PropabilityTrader.cs
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
   public class PropabilityTrader : IRuleEngine
   {
      private RuleEngineInfo m_RuleEngineInfo = new RuleEngineInfo();
      private DataContainer dax_close;
      private DataContainer dax_etf;
      private DataContainer dax_ma;
      private DataContainer dax_rel_diff;
      private DataContainer buy_events;
      private DataContainer sell_events;
      private DataContainer buy_events_dax;
      private DataContainer sell_events_dax;


      /// <summary>
      ///
      /// </summary>
      public PropabilityTrader()
      {
      }

      #region ITradeRule Member
      // Called only once to init trade rule
      public void Setup()
      {
         DataContainer dax = DBEngine.GetInstance().GetQuotes("846900");
         this.RuleEngineInfo.FromDate = new WorkDate(2001, 1, 1);
         this.RuleEngineInfo.ToDate = dax.YoungestDate.Clone();

         this.RuleEngineInfo.Variants.Add("averaging", new int[] { 38 });

         this.RuleEngineInfo.MinimumInvestment = 5000.0;
         this.RuleEngineInfo.TargetPositions = 10;
         this.RuleEngineInfo.MaxLoss = 0.90;

         this.RuleEngineInfo.Depot.DefaultTrailingGap = 5;

         // Create virtual instuments:
         // Create DAXex from DAX-Index
         dax_close = dax.Clone();
         dax_etf = dax_close.Clone();
         dax_etf.Scale(0.01);
         DBEngine.GetInstance().AddVirtualInvestment("dbx1da", "DAX EX", dax_etf);
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

         dax_ma = MovingAverage.CreateFrom(dax_close, nAveraging);
         dax_ma = dax_ma.Clone(this.RuleEngineInfo.FromDate);

         dax_rel_diff = RelativeDifference.CreateFrom(dax_close, dax_ma);
         dax_rel_diff = dax_rel_diff.Clone(this.RuleEngineInfo.FromDate);

         this.buy_events = new DataContainer();
         this.sell_events = new DataContainer();
         this.buy_events_dax = new DataContainer();
         this.sell_events_dax = new DataContainer();
      }

      public void Result()
      {
         Chart chart = new Chart();
         chart.Width = 6000;
         chart.Height = 900;

         chart.SubSectionsX = 8;
         chart.LogScaleY = true;
         chart.TicsYInterval = 100;
         chart.Title = dax_close.OldestDate.ToString() + " - " + dax_close.YoungestDate.ToString();
         chart.LabelY = "Punkte (log.)";
         chart.Add(dax_close, Chart.LineType.Navy, "DAX");
         chart.Add(dax_ma, Chart.LineType.SeaGreen, "Moving Average (38)");
         chart.Add(buy_events_dax, Chart.LineType.GoLong);
         chart.Add(sell_events_dax, Chart.LineType.GoShort);
         chart.LeftDate = dax_ma.OldestDate;
         chart.Create(World.GetInstance().ResultPath + "dax.png");

         chart.Clear();
         chart.LogScaleY = false;
         chart.TicsYInterval = 1;
         chart.Title = dax_rel_diff.OldestDate.ToString() + " - " + dax_rel_diff.YoungestDate.ToString();
         chart.LabelY = "dB%";
         chart.Add(dax_rel_diff, Chart.LineType.Navy, "DAX rel. diff. to MA38");
         chart.Add(buy_events, Chart.LineType.GoLong);
         chart.Add(sell_events, Chart.LineType.GoShort);
         chart.Create(World.GetInstance().ResultPath + "dax_rel_diff.png");
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

         if (depot.Contains("dbx1da"))
         {
            if (dax_rel_diff[today] > 0)
            {
               if (depot[0].TrailingGap > 2)
                  depot[0].TrailingGap -= 0.2;
            }

            if (depot[0].StopLoss > depot[0].Price)
            {
               Sell("dbx1da");
               sell_events[today] = dax_rel_diff[today];
               sell_events_dax[today] = dax_close[today];
               return true;
            }
         }

         return false;
      }

      public bool BuyRule()
      {
         //Depot depot = this.RuleEngineInfo.Depot;
         WorkDate today = RuleEngineInfo.Today;

         Console.WriteLine("{0} {1}", today, dax_rel_diff[today]);

         if (dax_rel_diff[today] < -5 &&
             dax_rel_diff[today - 1] < dax_rel_diff[today]
            )
         {
            if (Buy("dbx1da"))
            {
               buy_events[today] = dax_rel_diff[today];
               buy_events_dax[today] = dax_close[today];
            }
            return true;
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
         int nQuantity = (int)(depot.Cash * 0.5 / dPrice);
         if (nQuantity > 100)
         {
            depot.Buy(strWKN, nQuantity, today, dPrice);
            return true;
         }

         return false;
      }
      #endregion
   }
}
