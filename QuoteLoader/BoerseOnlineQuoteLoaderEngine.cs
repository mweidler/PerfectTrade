//
// BoerseOnlineQuoteLoaderEngine.cs
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

      protected override bool ParseAndStore(string input, Stock stock)
      {
         // plausibility check, if the quotes line can match
         if (input.Contains("<tr><td>&nbsp;") == false)
            return false;

         int i = 0;
         string[] arrTokens = new string[7];
         int iStartIndex = input.IndexOf("<tr><td>&nbsp;");
         string strLine = input.Substring(iStartIndex);

         XmlTextReader reader = new XmlTextReader(new StringReader(strLine));
         reader.WhitespaceHandling = WhitespaceHandling.None;

         while (reader.Read())
         {
            if (reader.NodeType == XmlNodeType.Text)
            {
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

         return true;
      }
   }
}
