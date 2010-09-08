//
// <filename>.cs
// 
// (C)OPYRIGHT 2007 BY MARC WEIDLER, ULRICHSTR. 12/1, 71672 MARBACH, GERMANY.
// 
// All rights reserved. This product and related documentation are protected by
// copyright restricting its use, copying, distribution, and decompilation. No part
// of this product or related documentation may be reproduced in any form by any
// means without prior written authorization of Marc Weidler or his partners, if any.
// Unless otherwise arranged, third parties may not have access to this product or 
// related documentation.
// 
// THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW.
// THE COPYRIGHT HOLDERS AND/OR OTHER PARTIES PROVIDE THE PROGRAM "AS IS" WITHOUT
// WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO,
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.
// THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU.
// SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY
// SERVICING, REPAIR OR CORRECTION.
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
        }

        static void SetWorldPaths(string strApplicationName)
        {
            string strBasePath = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            
            string strResultPath = strBasePath + "/tradedata/results/" + strApplicationName + "/";
            if (Directory.Exists(strResultPath) == false) {
                Directory.CreateDirectory(strResultPath);
            }
            
            string strDataPath = strBasePath + "/tradedata/data/" + strApplicationName + "/";
            if (Directory.Exists(strDataPath) == false) {
                Directory.CreateDirectory(strDataPath);
            }
            
            World.GetInstance().ResultPath = strResultPath;
            World.GetInstance().DataPath = strDataPath;
            World.GetInstance().QuotesPath = strBasePath + "/tradedata/quotes/";
        }

        #region IAnalyzerEngine Member
        public void Setup()
        {
            SetWorldPaths("RelativeStrength");

            DBEngine dbengine = DBEngine.GetInstance();
            if (dbengine.Exists("846900") == false)
                return;

            Chart chart = new Chart();
            chart.AutoDeleteTempFiles = false;
            chart.Width = 1500;
            chart.Height = 400;
            Stock dax = dbengine.GetStock("846900");
            DataContainer quotes = dax.Quotes;
            DataContainer dax_ma38 = MovingAverage.CreateFrom(quotes, 38);
            DataContainer dax_ma200 = MovingAverage.CreateFrom(quotes, 200);

            WorkDate startDate = quotes.YoungestDate.Clone();
            startDate.Set(startDate.Year - 5, startDate.Month, 1);

            DataContainer dax_ranged = quotes.Clone(startDate);


            chart.Clear();
            chart.SubSectionsX = 6;
            chart.Title = dax_ranged.OldestDate.ToString() + " - " + dax_ranged.YoungestDate.ToString();
            chart.LabelY = "Punkte";
            chart.Add(dax_ranged, 1, "DAX");
            chart.Add(dax_ma38, 2, "DAX (ma38)");
            chart.Add(dax_ma200, 3, "DAX (ma200)");
            chart.Create(World.GetInstance().ResultPath + "dax.png");

            DataContainer dax_diff_ma38 = Difference.CreateFrom(quotes, dax_ma38).Clone(startDate);
            DataContainer dax_diff_ma200 = Difference.CreateFrom(quotes, dax_ma200);

            DataContainer dax_rel_diff_38 = RelativeDifference.CreateFrom(quotes, dax_ma38);
            DataContainer dax_rel_diff_200 = RelativeDifference.CreateFrom(quotes, dax_ma200);
            dax_rel_diff_38 = dax_rel_diff_38.Clone(startDate);
            chart.Clear();
            chart.LabelY = "dB%";
            chart.Add(dax_rel_diff_38, 2, "DAX (rel diff 38)");
            chart.Add(dax_rel_diff_200, 3, "DAX (rel diff 200)");
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
            chart.Add(dax_diff_ma200, 3, "DAX rel. ma200");
            chart.Create(World.GetInstance().ResultPath + "dax_relperf.png");

/*            chart.Clear();
            chart.Title = "DAX";
            chart.LogScaleY = true;
            //chart.LabelsY = labels;
            chart.LabelY = "Punkte";
            chart.TicsYInterval = 500;
            
            chart.Add(dax_ranged, "DAX(Performance)");
            chart.Add(dax_ma38, "Moving Average (38)");
            chart.Add(dax_ma200, "Moving Average (200)");
            chart.Create(World.GetInstance().ResultPath + "dax.png");*/
        }
        #endregion
    }
}

