using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using FinancialObjects;
using Indicators;

namespace PerfectTrade
{
	class MainClass
	{
        static void SetWorldPaths(string strApplicationName)
        {
            string strBasePath = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            string strResultPath = strBasePath + "/tradedata/results/" + strApplicationName + "/";
            if (Directory.Exists(strResultPath) == false)
            {
                Directory.CreateDirectory(strResultPath);
            }

            string strDataPath = strBasePath + "/tradedata/data/" + strApplicationName + "/";
            if (Directory.Exists(strDataPath) == false)
            {
                Directory.CreateDirectory(strDataPath);
            }

            World.GetInstance().ResultPath = strResultPath;
            World.GetInstance().DataPath = strDataPath;
            World.GetInstance().QuotesPath = strBasePath + "/tradedata/quotes/";
        }

        public static void Main(string[] args)
        {
            SetWorldPaths("Test");

            DBEngine dbengine = DBEngine.GetInstance();
            if (dbengine.Exists("846900") == false)
                return;

            Stock dax = dbengine.GetStock("846900");
            if (dax.FillGaps() > 0)
            {
                dax.Save(World.GetInstance().QuotesPath + "846900.sto");
            }

            DataContainer quotes = dax.QuotesLow;

            Chart chart = new Chart();
            chart.Add(quotes,1,"Test");
            chart.Create(World.GetInstance().ResultPath + "dax.png");

            DataContainer dax_relperf = RelativePerformance.CreateFrom(quotes, new WorkDate(2008, 4, 21));
            dax_relperf.Save(World.GetInstance().ResultPath + "dax_relperf.csv", ";");
        }
    }
}
