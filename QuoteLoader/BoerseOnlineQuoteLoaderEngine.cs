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
    public class BoerseOnlineQuoteLoaderEngine : QuoteLoaderEngine
    {
        public BoerseOnlineQuoteLoaderEngine()
        {
        }

        protected override string BuildURI(Stock stock, WorkDate workdate)
        {
            string strQuoteString = "http://ichart.yahoo.com/table.csv?s=";
            strQuoteString += stock.ISIN;
            strQuoteString += "&d=" + (workdate.Month-1);
            strQuoteString += "&e=" + workdate.Day;
            strQuoteString += "&f=" + workdate.Year;
            strQuoteString += "&g=d&a=8&b=3&c=2000&ignore=.csv";
            return strQuoteString;
        }

        protected override void ParseAndStore(string input, Stock stock)
        {
            input = input.Replace(',', ';');
            input = input.Replace('.', ',');

            //char[] chSplit = { ';' };
            //string[] arrTokens = input.Split(chSplit);
            //Date,Open,High,Low,Close,Volume,Adj. Close
            // 0    1    2    3   4    5       6

            //DataContainer quotes = stock.Quotes;
            //WorkDate priceDate = new WorkDate();
            /*DateTime.TryParse(arrTokens[0], out priceDate);

            quote.Date = priceDate;
            quote.Open = Double.Parse(arrTokens[1]);
            quote.High = Double.Parse(arrTokens[2]);
            quote.Low = Double.Parse(arrTokens[3]);
            quote.Close = Double.Parse(arrTokens[4]);
            quote.Volume = Double.Parse(arrTokens[5]);
            quoteList.Add(quote);*/
        }
    }
}
