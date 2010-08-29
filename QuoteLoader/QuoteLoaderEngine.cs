//
// <filename>.cs
//
// (C)OPYRIGHT 2010 BY MARC WEIDLER, ULRICHSTR. 12/1, 71672 MARBACH, GERMANY.
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
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using FinancialObjects;

namespace QuoteLoader
{
    public abstract class QuoteLoaderEngine
    {
        protected abstract string BuildURI(Stock stock, WorkDate workdate);
        protected abstract void ParseAndStore(string input, Stock stock);

        DBEngine dbengine = DBEngine.GetInstance();

        public QuoteLoaderEngine()
        {
        }

        protected bool DownloadQuote(String strURI, String strTargetFile)
        {
            WebRequest webreq;
            WebResponse webres;

            try {
                webreq = WebRequest.Create(strURI);
                webres = webreq.GetResponse();
            } catch (Exception exc) {
                Log.Info(exc.ToString());
                return false;
            }


            Stream stream = webres.GetResponseStream();
            StreamReader strrdr = new StreamReader(stream);
            string strLine;

            StreamWriter sw = new StreamWriter(strTargetFile, true);

            while ((strLine = strrdr.ReadLine()) != null) {
                sw.WriteLine(strLine);
            }

            stream.Close();
            sw.Close();

            return true;
        }

        public void Load(string strWKN, WorkDate workdate)
        {
            Stock stock = dbengine.GetStock(strWKN);

            Log.Info("Load entered, datetime = " + workdate);

            WebRequest webreq;
            WebResponse webres;

            string strURI = BuildURI(stock, workdate);
            Log.Info(strURI);

            webreq = WebRequest.Create(strURI);
            webres = webreq.GetResponse();

            Stream stream = webres.GetResponseStream();
            StreamReader strrdr = new StreamReader(stream);
            string strLine = strrdr.ReadLine();

            while ((strLine = strrdr.ReadLine()) != null) {
                Log.Info(strLine);
                ParseAndStore(strLine, stock);
            }

            strrdr.Close();
            stream.Close();

            Log.Info("Leaving Load");

        }

        public void Read(string strWKN, WorkDate workdate)
        {
          Stock stock = dbengine.GetStock(strWKN);

          string strFilename = "~/adidas_werte.html";

            if (File.Exists(strFilename))
            {
                StreamReader re = File.OpenText(strFilename);
                string input = re.ReadLine();
                while ((input = re.ReadLine()) != null)
                {
                    BeginParsing
                    ParseAndStore(input, stock);
                    EndParsing
                }

                re.Close();
            }
        }
    }
}