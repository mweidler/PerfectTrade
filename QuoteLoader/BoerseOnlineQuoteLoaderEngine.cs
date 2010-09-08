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
using System.Xml;
using FinancialObjects;

namespace QuoteLoader
{
    public class BoerseOnlineQuoteLoaderEngine : QuoteLoaderEngine
    {
        protected override string BuildURI(Stock stock, WorkDate startdate, WorkDate enddate)
        {
            //www.boerse-online.de/kurse-tools/historische-kurse?do=historie
            //&isin=DE0005003404&land=276&boerse=XETRA
            //&starttag=01&startmonat=01&startjahr=1998
            //&endtag=12&endmonat=08&endjahr=2010
            //&x=17&y=9

            string strQuoteString = "www.boerse-online.de/kurse-tools/historische-kurse?do=historie";
            strQuoteString += "&isin=" + stock.ISIN;
            strQuoteString += "&land=276&boerse=XETRA";
            strQuoteString += "&starttag=" + startdate.Day;
            strQuoteString += "&startmonat" + startdate.Month;
            strQuoteString += "&startjahr=" + startdate.Year;
            strQuoteString += "&endtag=" + enddate.Day;
            strQuoteString += "&endmonat" + enddate.Month;
            strQuoteString += "&endjahr=" + enddate.Year;
            strQuoteString += "&x=17&y=9";
            return strQuoteString;
        }

        protected override bool EnableImport(string strLine)
        {
            if (strLine.StartsWith("<h2>Historische Kursdaten"))
                return true;

            return false;
        }

        protected override bool DisableImport(string strLine)
        {
            if (strLine.Contains("<!-- EINBINDUNG ENDE -->"))
                return true;

            return false;
        }

        protected override void ParseAndStore(string input, Stock stock)
        {
            // plausibility check, if the quotes line can match
            if (input.Contains("<tr><td>&nbsp;") == false)
                return;

            int i = 0;
            string[] arrTokens = new string[7];
            int iStartIndex = input.IndexOf("<tr><td>&nbsp;");
            string strLine = input.Substring(iStartIndex);

            XmlTextReader reader = new XmlTextReader(new StringReader(strLine));
            reader.WhitespaceHandling = WhitespaceHandling.None;

            while (reader.Read()) {
                if (reader.NodeType == XmlNodeType.Text) {
                    arrTokens[i] = reader.Value;
                    i++;
                }
            }

            reader.Close();

            //Date,Open,Close,High,Low,misc
            // 0    1     2    3    4    5

            DateTime priceDate = new DateTime();
            DateTime.TryParse(arrTokens[0], out priceDate);
            WorkDate workdate = new WorkDate(priceDate);
            double priceClose = Double.Parse(arrTokens[2]);
            double priceLow = Double.Parse(arrTokens[4]);

            stock.QuotesClose[workdate] = priceClose;
            stock.QuotesLow[workdate] = priceLow;

            System.Console.Write(workdate.ToString() + "\r");
        }
    }
}
