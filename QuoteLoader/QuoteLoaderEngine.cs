//
// QuoteLoaderEngine.cs
//
// COPYRIGHT (C) 2010 AND ALL RIGHTS RESERVED BY
// MARC WEIDLER, ULRICHSTR. 12/1, 71672 MARBACH, GERMANY (MARC.WEIDLER@WEB.DE).
//
// ALL RIGHTS RESERVED. THIS PRODUCT AND RELATED DOCUMENTATION ARE PROTECTED BY
// COPYRIGHT RESTRICTING ITS USE, COPYING, DISTRIBUTION, AND DECOMPILATION. NO PART
// OF THIS PRODUCT OR RELATED DOCUMENTATION MAY BE REPRODUCED IN ANY FORM BY ANY
// MEANS WITHOUT PRIOR WRITTEN AUTHORIZATION OF MARC WEIDLER OR HIS PARTNERS, IF ANY.
// UNLESS OTHERWISE ARRANGED, THIRD PARTIES MAY NOT HAVE ACCESS TO THIS PRODUCT OR
// RELATED DOCUMENTATION.
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
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using FinancialObjects;

namespace QuoteLoader
{
   public abstract class QuoteLoaderEngine
   {
      protected abstract string BuildURI(Stock stock, WorkDate startdate, WorkDate enddate);
      protected abstract void ParseAndStore(string strLine, Stock stock);
      protected abstract bool EnableImport(string strLine);
      protected abstract bool DisableImport(string strLine);

      DBEngine dbengine = DBEngine.GetInstance();

      public QuoteLoaderEngine()
      {
      }

      public void Init(string strISIN)
      {
         Stock stock = new Stock();
         
         if (strISIN.Length == 12) {
            string strWKN = strISIN.Substring(6, 6);
            
            stock.ISIN = strISIN;
            stock.WKN = strWKN;
            
            System.Console.WriteLine("Initializing {0}", strISIN);

            stock.Save(World.GetInstance().QuotesPath + strISIN + ".sto");
         }
      }

      public void Update(string strStockFilename)
      {
         string strWKN = Path.GetFileNameWithoutExtension(strStockFilename);
         Stock stock = dbengine.GetStock(strWKN);
         System.Console.WriteLine("Updating {0}", strWKN);
         Read(stock, new WorkDate());
         
         System.Console.WriteLine("{0} close gaps filled.", stock.QuotesClose.FillGaps());
         System.Console.WriteLine("{0} low gaps filled.", stock.QuotesLow.FillGaps());
         stock.Save();
      }

      public void Load(Stock stock, WorkDate workdate, int nHistoricalDays)
      {
         bool bDoImport = false;
         
         Log.Info("Load entered, datetime = " + workdate);
         
         WebRequest webreq;
         WebResponse webres;
         
         string strURI = BuildURI(stock, workdate, workdate - nHistoricalDays);
         Log.Info(strURI);
         
         webreq = WebRequest.Create(strURI);
         webres = webreq.GetResponse();
         
         Stream stream = webres.GetResponseStream();
         StreamReader strrdr = new StreamReader(stream);
         string strLine = strrdr.ReadLine();
         
         while ((strLine = strrdr.ReadLine()) != null) {
            if (EnableImport(strLine))
               bDoImport = true;
            if (DisableImport(strLine))
               bDoImport = false;
            if (bDoImport) {
               Log.Info(strLine);
               ParseAndStore(strLine, stock);
            }
         }
         
         strrdr.Close();
         stream.Close();
         
         Log.Info("Leaving Load");
      }

      public void Read(Stock stock, WorkDate workdate)
      {
         bool bDoImport = false;
         
         string strFilename = "/home/mweidler/dax-kurse.htm";
         
         if (File.Exists(strFilename)) {
            StreamReader re = File.OpenText(strFilename);
            string input = re.ReadLine();
            while ((input = re.ReadLine()) != null) {
               if (EnableImport(input))
                  bDoImport = true;
               if (DisableImport(input))
                  bDoImport = false;
               if (bDoImport)
                  ParseAndStore(input, stock);
            }
            
            re.Close();
         }
      }
   }
}
