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
      private Ranking m_ranking = new Ranking();
      //Difference sellsig = new Difference();
      //Difference buysig = new Difference();

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
         this.RuleEngineInfo.FromDate = new WorkDate(2006, 1, 1);
         this.RuleEngineInfo.ToDate = new WorkDate(2006, 12, 30);

         this.RuleEngineInfo.Variants.Add("averaging", new int[] { 63 });

         this.RuleEngineInfo.MinimumInvestment = 5000.0;
         this.RuleEngineInfo.TargetPositions = 10;
         this.RuleEngineInfo.MaxLoss = 0.90;
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
         //RuleEngineInfo.Depot.Cash = 100000;   //    100000 is default
      }

      private Ranking FilterByReturn(Ranking ranking, WorkDate today, int nRange)
      {
         Ranking result = new Ranking();
         WorkDate pastdate = today.Clone() - (int)(nRange);

         DataContainer fixrateinvest = DBEngine.GetInstance().GetQuotes("fix_04").Clone(pastdate, today);
         DataContainer relperffix = RelativePerformance.CreateFrom(fixrateinvest);

         /* Ermittle fuer jedes Wertpapier die durchschnittliche (erwartete) Tagesrendite
          * (Return) und dessen Volatilitaet (Standardabweichung, Risk).
          * Fuer jedes Risk wird die Rendite des Wertpapieres gespeichert.
          * Die Daten werden automatisch sortiert (SortedList).
          */
         foreach (string strWKN in ranking)
         {
            DataContainer quotes = DBEngine.GetInstance().GetQuotes(strWKN).Clone(pastdate, today);
            DataContainer relperf = RelativePerformance.CreateFrom(quotes);

            if (relperf[today] > relperffix[today])
            {
               result.Add(strWKN, relperf[today]);
            }
         }

         string strDataPath = World.GetInstance().DataPath + "returnrisk/";
         int i = 1;

         foreach (string strWKN in ranking)
         {
            StreamWriter sw = new StreamWriter(strDataPath + strWKN, true, Encoding.ASCII);

            if (result.Contains(strWKN))
            {
               sw.WriteLine(today + " " + i);
            }
            else
            {
               sw.WriteLine(today + " " + 0);
            }

            sw.Close();
            i++;
         }

         return result;
      }

      private Ranking FilterByReturnRisk(Ranking ranking, WorkDate today, int nRange)
      {
         Ranking  result = new Ranking();
         WorkDate pastdate = today.Clone() - (int)(3 * nRange);
         SortedList<double, SortedList<double, string>> riskreturn =
            new SortedList<double, SortedList<double, string>>();

         /* Ermittle fuer jedes Wertpapier die durchschnittliche (erwartete) Tagesrendite
          * (Return) und dessen Volatilitaet (Standardabweichung, Risk).
          * Fuer jedes Risk wird die Rendite des Wertpapieres gespeichert.
          * Die Daten werden automatisch sortiert (SortedList).
          */
         foreach (string strWKN in ranking)
         {
            DataContainer quotes       = DBEngine.GetInstance().GetQuotes(strWKN).Clone(pastdate, today);
            //DataContainer movavg       = MovingAverage.CreateFrom(quotes, nRange);
            DataContainer relchange    = RelativeChange.CreateFrom(quotes);
            DataContainer changemovavg = MovingAverage.CreateFrom(relchange, nRange);
            DataContainer volatility   = Volatility.CreateFrom(relchange, nRange);

            double dReturn = changemovavg[today];
            double dRisk = volatility[today];

            if (riskreturn.ContainsKey(dRisk) == false)
            {
               riskreturn.Add(dRisk, new SortedList<double, string>());
            }

            riskreturn[dRisk][dReturn] = strWKN;
         }

         /* Als Ergebnis werden alle Wertpapiere ausgesucht, deren
          * Renditen maximal für das entsprechende Risiko sind.
          * Weiterhin muss die Rendite zu höheren Risiken steigen, begonnen
          * mit der risikolosen Rendite des festverzinslichen Wertpapieres.
          * Als Resultat sind alle Wertpapiere auf der Effizientlinie.
          */
         DataContainer fixrateinvest = DBEngine.GetInstance().GetQuotes("fix_04").Clone(pastdate, today);
         DataContainer relchangefix  = RelativeChange.CreateFrom(fixrateinvest);
         DataContainer fixmovavg     = MovingAverage.CreateFrom(relchangefix, nRange);
         double   dPreviousMaxReturn = fixmovavg[today];

         foreach (double dRisk in riskreturn.Keys)
         {
            SortedList<double, string> returns = riskreturn[dRisk];
            int nCount = returns.Count;
            double dReturn = returns.Keys[nCount - 1];
            string strWKN = returns.Values[nCount - 1];

            if (dReturn > dPreviousMaxReturn)
            {
               result.Add(strWKN, dReturn);
               dPreviousMaxReturn = dReturn;
            }
         }

         string strDataPath = World.GetInstance().DataPath + "returnrisk/";
         int i = 1;

         foreach (string strWKN in ranking)
         {
            StreamWriter sw = new StreamWriter(strDataPath + strWKN, true, Encoding.ASCII);

            if (result.Contains(strWKN))
            {
               sw.WriteLine(today + " " + i);
            }
            else
            {
               sw.WriteLine(today + " " + 0);
            }

            sw.Close();
            i++;
         }

         return result;
      }

      private void WriteReturnRiskTable(Ranking ranking, WorkDate today, int nRange, string strFilename)
      {
         WorkDate pastdate = today.Clone() - (int)nRange;
         StreamWriter sw = new StreamWriter(strFilename, false, Encoding.ASCII);

         foreach (string strWKN in ranking)
         {
            DataContainer quotes = DBEngine.GetInstance().GetQuotes(strWKN).Clone(pastdate, today);
            DataContainer relchange = RelativeChange.CreateFrom(quotes);
            DataContainer changemovavg = MovingAverage.CreateFrom(relchange, nRange);
            DataContainer volatility = Volatility.CreateFrom(relchange, nRange);

            double dReturn = changemovavg[today];
            double dRisk = volatility[today];
            sw.WriteLine(dRisk + " " + dReturn);
         }

         sw.Close();
      }

      private void WriteReturnRiskTableForGnuPlot(Ranking ranking, WorkDate today, int nRange, string strPath)
      {
         WorkDate pastdate = today.Clone() - (int)nRange;
         WriteGnuPlotCfg("DrawRR.tpl", "DrawRR.gpl", "", ranking);
         int i = 1;

         foreach (string strWKN in ranking)
         {
            StreamWriter sw = new StreamWriter(strPath + strWKN, true, Encoding.ASCII);

            DataContainer quotes = DBEngine.GetInstance().GetQuotes(strWKN).Clone(pastdate, today);
            DataContainer relchange = RelativeChange.CreateFrom(quotes);
            DataContainer changemovavg = MovingAverage.CreateFrom(relchange, nRange);
            DataContainer volatility = Volatility.CreateFrom(relchange, nRange);

            double dReturn = changemovavg[today];
            double dRisk = volatility[today];

            if (dReturn > 0.02)
            {
               sw.WriteLine(today + " " + dRisk + " " + i);
            }
            else
            {
               sw.WriteLine(today + " " + dRisk + " " + 0); // dReturn
            }

            i++;
            sw.Close();
         }
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

      private Ranking FilterByCorrelation(Ranking ranking, WorkDate today, int nRange)
      {
         Ranking newranking = new Ranking();

         string strPath = World.GetInstance().DataPath + "cormatrix.dat";
         StreamWriter sw = new StreamWriter(strPath, false, Encoding.ASCII);

         Matrix matrix = new Matrix();
         WorkDate pastdate = (today.Clone()) - (int)nRange;

         foreach (string strWKNRow in ranking)
         {
            DataContainer datacontainerRow = DBEngine.GetInstance().GetQuotes(strWKNRow);
            datacontainerRow = datacontainerRow.Clone(pastdate, today);
            datacontainerRow = RelativeChange.CreateFrom(datacontainerRow);
            datacontainerRow.Save(World.GetInstance().DataPath + strWKNRow + ".dat");

            foreach (string strWKNCol in ranking)
            {
               DataContainer datacontainerCol = DBEngine.GetInstance().GetQuotes(strWKNCol);
               datacontainerCol = datacontainerCol.Clone(pastdate, today);
               datacontainerCol = RelativeChange.CreateFrom(datacontainerCol);
               DataContainer corr = Correlation.CreateFrom(datacontainerCol, datacontainerRow, nRange);

               matrix[strWKNRow, strWKNCol] = corr[today];
               sw.WriteLine(strWKNRow + " " + strWKNCol + " " + corr[today]);

               if (strWKNCol.Equals(strWKNRow))
                  break;
            }
         }

         sw.Close();

         while (matrix.Count > this.RuleEngineInfo.TargetPositions)
         {
            string maxrow, maxcol;
            matrix.SearchMaximum(out maxrow, out maxcol);
            matrix.RemoveKey(maxrow);
         }

         while (matrix.Count > 0)
         {
            string maxrow, maxcol;
            matrix.SearchMaximum(out maxrow, out maxcol);
            matrix.RemoveKey(maxrow);
            newranking.Add(maxrow, 10 - matrix.Count);
         }

         return newranking;
      }

      public void Ranking()
      {
         //SortedList<string, Instrument> instruments = World.GetInstance().Instruments;
         //string strDataPath = World.GetInstance().DataPath;

         int nAveraging = this.RuleEngineInfo.Variants["averaging"];
         WorkDate today = this.RuleEngineInfo.Today;
         WorkDate from = today - (int)nAveraging;

         m_ranking.Clear();
         //m_ranking.Add(instruments, 0.0);

         //WriteReturnRiskTable(m_ranking, today, nAveraging, strDataPath + "returnrisk_org.dat");
         Ranking filtered = FilterByReturn(m_ranking, today, nAveraging);
         //WriteReturnRiskTable(filtered, today, nAveraging, strDataPath + "returnrisk_rrf.dat");
         //WriteReturnRiskTableForGnuPlot(m_ranking, today, nAveraging, strDataPath + "returnrisk/");
         filtered = FilterByCorrelation(filtered, today, nAveraging);
         //WriteReturnRiskTable(filtered, today, nAveraging, strDataPath + "returnrisk_cof.dat");
         m_ranking = filtered;

         foreach (string strWKN in filtered)
         {
            DataContainer datacontainer = DBEngine.GetInstance().GetQuotes(strWKN);
            datacontainer = datacontainer.Clone(from, today);
            DataContainer datachange = RelativeChange.CreateFrom(datacontainer);
            DataContainer dataperf = RelativePerformance.CreateFrom(datacontainer);
            DataContainer volatility = Volatility.CreateFrom(datachange, nAveraging);
            DataContainer returnrisk = ReturnRiskMargin.CreateFrom(datacontainer, nAveraging);
            datachange.Save(World.GetInstance().DataPath + strWKN + "_c.dat");
            dataperf.Save(World.GetInstance().DataPath + strWKN + "_p.dat");
            volatility.Save(World.GetInstance().DataPath + strWKN + "_v.dat");
            returnrisk.Save(World.GetInstance().DataPath + strWKN + "_r.dat");
         }

         WriteGnuPlotCfg("Draw_c.tpl", "Draw_c.gpl", "_c.dat", filtered);
         WriteGnuPlotCfg("Draw_p.tpl", "Draw_p.gpl", "_p.dat", filtered);
         WriteGnuPlotCfg("Draw_v.tpl", "Draw_v.gpl", "_v.dat", filtered);
         WriteGnuPlotCfg("Draw_r.tpl", "Draw_r.gpl", "_r.dat", filtered);

         DataContainer reference = DBEngine.GetInstance().GetQuotes("846900");
         reference = reference.Clone(from, today);
         //DataContainer relreference = RelativeChange.CreateFrom(reference);
         DataContainer relperformance = RelativePerformance.CreateFrom(reference);
         relperformance.Save(World.GetInstance().DataPath + "846900" + ".dat");
      }

      public void SellRule()
      {
         // Verkaufe die Werte, die sich derzeit nicht in der Top10 befinden.
         Depot depot = this.RuleEngineInfo.Depot;

         for (int i = 0; i < depot.Count; i++)
         {
            if (m_ranking.IsTopRanked(this.RuleEngineInfo.TargetPositions, depot[i].WKN) == false)
            {
               Sell(depot[i].WKN);
               i = -1; // Restart
            }
         }
      }

      public void BuyRule()
      {
         Depot depot = this.RuleEngineInfo.Depot;

         // Kaufe die besten Wertpapiere im Ranking
         for (int i = 0; i < m_ranking.Count && i < this.RuleEngineInfo.TargetPositions; i++)
         {
            if (depot.ContainsKey(m_ranking[i].ID) == false)
            {
               Buy(m_ranking[i].ID);
            }
         }
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
         double dMinInvestment = this.RuleEngineInfo.MinimumInvestment;
         WorkDate today = this.RuleEngineInfo.Today;
         Depot depot = this.RuleEngineInfo.Depot;

         // ...buy this WKN if enough cash exists
         double dAmount = depot.Equity / this.RuleEngineInfo.TargetPositions;
         dAmount = Math.Max(dAmount, dMinInvestment);
         dAmount = Math.Min(dAmount, depot.Cash);

         if (dAmount >= dMinInvestment)
         {
            dAmount *= (1.0 - depot.ProvisionRate);
            double dPrice = DBEngine.GetInstance().GetPrice(strWKN, today);
            int nQuantity = (int)(dAmount / dPrice);

            depot.Buy(strWKN, nQuantity, today, dPrice);
            return true;
         }

         return false;
      }
      #endregion
   }
}
