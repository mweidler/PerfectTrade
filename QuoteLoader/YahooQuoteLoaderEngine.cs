//
// YahooQuoteLoaderEngine.cs
//
// COPYRIGHT (C) 2011 AND ALL RIGHTS RESERVED BY
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
using System.IO;
using System.Net;
using System.Xml;
using FinancialObjects;

namespace QuoteLoader
{
   public class YahooQuoteLoaderEngine : QuoteLoaderEngine
   {
      protected override string BuildURI(Stock stock, WorkDate startdate, WorkDate enddate)
      {
         //http://ichart.finance.yahoo.com/table.csv?s=D1A9.DE&d=1&e=27&f=2011&g=d&a=4&b=2&c=2007&ignore=.csv

         string strQuoteString = "http://ichart.finance.yahoo.com/table.csv?";
         strQuoteString += "s=" + stock.Symbol;
         strQuoteString += string.Format("&d={0}&e={1}&f={2}", enddate.Month - 1, enddate.Day, enddate.Year);
         strQuoteString += "&g=d";
         strQuoteString += string.Format("&a={0}&b={1}&c={2}&ignore=.csv", startdate.Month - 1, startdate.Day, startdate.Year);

         return strQuoteString;
      }

      protected override bool EnableImport(string strLine)
      {
         return true;
      }

      protected override bool DisableImport(string strLine)
      {
         return false;
      }

      protected override bool ParseAndStore(string input, Stock stock)
      {
         // plausibility check, if the quotes line matches the expectation
         if (input.StartsWith("Date,Open"))
            return false;

         input = input.Replace(',', ';');
         input = input.Replace('.', ',');

         char[] chSplit = { ';' };
         string[] arrTokens = input.Split(chSplit);
         //Date,Open,High,Low,Close,Volume,Adj. Close
         // 0    1    2    3   4    5       6

         DateTime priceDate = new DateTime();
         DateTime.TryParse(arrTokens[0], out priceDate);

         WorkDate workdate = new WorkDate(priceDate);
         double priceLow = Double.Parse(arrTokens[3]);
         double priceClose = Double.Parse(arrTokens[4]);

         stock.QuotesClose[workdate] = priceClose;
         stock.QuotesLow[workdate] = priceLow;

         System.Console.Write(workdate.ToString() + "\r");

         return true;
      }
   }
}
