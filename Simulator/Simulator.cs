//
// Simulator.cs
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
using System.Reflection;
using System.Threading;
using System.Text;
using System.IO;
using System.ComponentModel;
using FinancialObjects;
using Indicators;

namespace Simulator
{
   class Simulator
   {
      private IRuleEngine m_TradeRule = null;
      private string m_strResultPath;
      private string m_strDataPath;
      private string m_strQuotesPath;
      //private Thread[] threads = null;

      static void Main(string[] args)
      {
         //try
         {
            Simulator simulator = new Simulator("EfficientPortfolio");
            simulator.Simulate();
         }
         /*catch (Exception e)
         {
             System.Console.WriteLine(e.Message);
             System.Console.WriteLine(e.StackTrace);
         }*/
      }

      public IRuleEngine InvokeTradeRule(string strClassName)
      {
         Assembly assembly = Assembly.GetExecutingAssembly();

         // Walk through each type in the assembly looking for our class
         foreach (Type type in assembly.GetTypes())
         {
            if (type.IsClass == true)
            {
               if (type.FullName.EndsWith("." + strClassName))  // e.g. RuleEngines.TradeRule
               {
                  // create an instance of the object
                  return (IRuleEngine)Activator.CreateInstance(type);
               }
            }
         }

         throw(new System.Exception("Rule engine not found!"));
      }

      public Simulator(string strRuleEngine)
      {
         m_strDataPath = "c:/work/TradeTools/Data/";
         m_strDataPath += strRuleEngine + "/";

         m_strResultPath = "c:/work/TradeTools/Results/";
         m_strResultPath += strRuleEngine + "/";

         if (Directory.Exists(m_strResultPath) == false)
         {
            Directory.CreateDirectory(m_strResultPath);
         }

         m_strQuotesPath = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/TradeTools/";

         if (Directory.Exists(m_strQuotesPath) == false)
         {
            Directory.CreateDirectory(m_strQuotesPath);
         }

         World.GetInstance().DataPath = m_strDataPath;
         World.GetInstance().ResultPath = m_strResultPath;
         World.GetInstance().QuotesPath = m_strQuotesPath;

         m_TradeRule = InvokeTradeRule(strRuleEngine);
         //int nThreads = System.Environment.ProcessorCount;
         //threads = new Thread[nThreads];
      }

      public void Simulate()
      {
         Console.Clear();
         CreateVirtualInstruments();
         //ReadInstrumentList();
         m_TradeRule.Setup();

         StreamWriter sw = new StreamWriter(m_strResultPath + "results.csv", false);
         sw.AutoFlush = true;
         SimulateVariants(sw);
         sw.Close();

         CallGnuPlot();
      }

      public void SimulateVariants(StreamWriter sw)
      {
         bool bPrintResultHeaders = true;

         do
         {
            if (m_TradeRule.IsValidVariant())
            {
               PrintInfo();
               double dPerf = SimulatePerformance();

               if (bPrintResultHeaders)
               {
                  foreach (string strVariantKey in m_TradeRule.RuleEngineInfo.Variants.ItemKeys)
                  {
                     sw.Write(strVariantKey + ";");
                  }

                  sw.WriteLine("Log-Perf.;Perf.-Factor;Trades");
                  bPrintResultHeaders = false;
               }

               foreach (string strVariantKey in m_TradeRule.RuleEngineInfo.Variants.ItemKeys)
               {
                  int nVariantValue = m_TradeRule.RuleEngineInfo.Variants[strVariantKey];
                  sw.Write(nVariantValue + ";");
               }

               sw.Write(dPerf + ";" + Math.Pow(2.0, dPerf / 100.0));
               sw.Write(";" + m_TradeRule.RuleEngineInfo.Depot.Trades);
               sw.WriteLine();
            }

            m_TradeRule.RuleEngineInfo.Variants.Next();

         }
         while (m_TradeRule.RuleEngineInfo.Variants.HasMoreIterations);
      }

      private double SimulatePerformance()
      {
         DataContainer dcPerformance = new DataContainer();
         DataContainer dcDepotPositions = new DataContainer();
         DataContainer dcInvestmentRate = new DataContainer();

         //World world = World.GetInstance();
         Depot depot = m_TradeRule.RuleEngineInfo.Depot;

         depot.Reset();
         depot.Cash = 100000;
         m_TradeRule.Prepare();
         m_TradeRule.RuleEngineInfo.Today = m_TradeRule.RuleEngineInfo.FromDate;

         while (m_TradeRule.RuleEngineInfo.Today <= m_TradeRule.RuleEngineInfo.ToDate)
         {
            UpdateDepotPositions(); PrintInfo();
            m_TradeRule.Ranking();
            m_TradeRule.SellRule(); PrintInfo();
            m_TradeRule.BuyRule();

            dcPerformance[m_TradeRule.RuleEngineInfo.Today] = depot.Performance;
            dcDepotPositions[m_TradeRule.RuleEngineInfo.Today] = depot.Count;
            dcInvestmentRate[m_TradeRule.RuleEngineInfo.Today] = 100.0 * depot.Asset / depot.Equity;
            PrintInfo();

            m_TradeRule.StepDate();
         }

         dcPerformance.Save(m_strResultPath + "performance" + ".dat");
         dcDepotPositions.Save(m_strResultPath + "positions" + ".dat");
         dcInvestmentRate.Save(m_strResultPath + "investmentrate" + ".dat");

         return depot.Performance;
      }

      public void PrintInfo()
      {
         WorkDate today = m_TradeRule.RuleEngineInfo.Today;
         Depot depot = m_TradeRule.RuleEngineInfo.Depot;

         Console.SetCursorPosition(0, 1);
         Console.Out.WriteLine("Simulation von: " + m_TradeRule.RuleEngineInfo.FromDate + " bis " + m_TradeRule.RuleEngineInfo.ToDate + " Heute: " + today + " Compl.: " + m_TradeRule.RuleEngineInfo.Variants.Completeness + "   ");
         depot.Dump(today);
         Console.Out.WriteLine("Variante: ");

         foreach (string strVariantKey in m_TradeRule.RuleEngineInfo.Variants.ItemKeys)
         {
            Console.Out.WriteLine(strVariantKey + "=" + m_TradeRule.RuleEngineInfo.Variants[strVariantKey] + "    ");
         }

         Console.Out.WriteLine("Performance: " + depot.Performance + " dB%    ");
      }

      void UpdateDepotPositions()
      {
         WorkDate today = m_TradeRule.RuleEngineInfo.Today;
         Depot depot = m_TradeRule.RuleEngineInfo.Depot;

         foreach (DepotPosition position in depot)
         {
            double dPrice = DBEngine.GetInstance().GetPrice(position.WKN, today);
            position.Price = dPrice;
         }
      }

      private void CreateVirtualInstruments()
      {
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
         DataContainer fixedGrowth = FixedGrowthInvestment.CreateFrom(dax.OldestDate, dax.YoungestDate, 4);
         DBEngine.GetInstance().AddVirtualInvestment("fix_04", "Sparbuch", fixedGrowth);

         fixedGrowth = FixedGrowthInvestment.CreateFrom(dax.OldestDate, dax.YoungestDate, 15);
         DBEngine.GetInstance().AddVirtualInvestment("fix_15", "Zielperformance", fixedGrowth);

         fixedGrowth = FixedGrowthInvestment.CreateFrom(dax.OldestDate, dax.YoungestDate, 0);
         DBEngine.GetInstance().AddVirtualInvestment("fix_00", "KeineAnlage", fixedGrowth);
      }

      /*public void ReadInstrumentList()
      {
         DBEngine dbengine = DBEngine.GetInstance();
         SortedList<string, Instrument> instruments = World.GetInstance().Instruments;

         if (File.Exists(m_strDataPath + "Instruments.txt"))
         {
            StreamReader sr = new StreamReader(m_strDataPath + "Instruments.txt");

            while (sr.Peek() >= 0)
            {
               string strLine = sr.ReadLine();

               if (strLine.StartsWith("#") == false)
               {
                  if (strLine.Length >= 6)
                  {
                     string strWKN = strLine.Substring(0, 6);

                     if (dbengine.Exists(strWKN))
                     {
                        Instrument instrument = new Instrument();
                        instrument.ID = strWKN;
                        instrument.Name = dbengine.GetName(strWKN);
                        instruments.Add(strWKN, instrument);
                     }
                  }
               }
            }

            sr.Close();
         }
      }
       */
      public void CallGnuPlot()
      {
         string[] strGPLFiles = Directory.GetFiles(m_strResultPath, "*.gpl");

         foreach (string strGPLPathName in strGPLFiles)
         {
            string strGPLName = strGPLPathName.Substring(strGPLPathName.LastIndexOf("/") + 1);

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.WorkingDirectory = m_strResultPath;
            p.StartInfo.FileName = "pgnuplot";
            p.StartInfo.Arguments = strGPLName;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.WaitForExit();
            p.Close();
         }
      }
   }
}