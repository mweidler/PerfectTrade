//
// QuoteLoaderEngine.cs
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
using FinancialObjects;

namespace QuoteLoader
{
   public abstract class QuoteLoaderEngine
   {
      /// <summary>
      /// Build a URI to download a set of quotes from a public web site.
      /// </summary>
      /// <param name="stock">The stock object that a URI should be build for downloading quotes</param>
      /// <param name="startdate">The date to start updating quotes</param>
      /// <param name="enddate">The date to end updating quotes</param>
      /// <returns>The complete URI that downloaded the quotes from a web site</returns>
      protected abstract string BuildURI(Stock stock, WorkDate startdate, WorkDate enddate);

      /// <summary>
      /// Parse a line of the downloaded quotes web page, parses the quotes data and stores
      /// them into the stock object.
      /// </summary>
      /// <param name="strLine">The data line to be parsed and stored.</param>
      /// <param name="stock">The stock object to be filled with the downloaded data</param>
      /// <returns>True, if the line could be successfully parsed, otherwise false</returns>
      protected abstract bool ParseAndStore(string strLine, Stock stock);

      /// <summary>
      /// Evaluates, if the parsing of the downloaded web page should be started.
      /// This method can be used to skip the header of the html page or other elements
      /// contained in the html page that should be ignored. If you download e.g. a
      /// CSV file, this method can simply return true.
      /// </summary>
      /// <param name="strLine">The data line that should be evaluated.</param>
      /// <returns>True, if the import should start now</returns>
      protected abstract bool EnableImport(string strLine);

      /// <summary>
      /// Evaluates, if the parsing of the downloaded web page should be ended.
      /// This method can be used to skip the footer of the html page or other elements
      /// contained in the html page that should be ignored.  If you download e.g. a
      /// CSV file, this method can simply return false.
      /// </summary>
      /// <param name="strLine">The data line that should be evaluated.</param>
      /// <returns>True, if the import should stopped now</returns>
      protected abstract bool DisableImport(string strLine);


      DBEngine dbengine = DBEngine.GetInstance();

      /// <summary>
      /// Creates a new QuoteLoaderEngine object.
      /// </summary>
      public QuoteLoaderEngine()
      {
      }

      /// <summary>
      /// Initializes a new stock file in the quotes directory.
      /// </summary>
      /// <param name="strISIN">The ISIN of the stock</param>
      public void Init(string strISIN)
      {
         Stock stock = new Stock();

         if (strISIN.Length == 12)
         {
            string strWKN = strISIN.Substring(6, 6);

            stock.ISIN = strISIN;
            stock.WKN = strWKN;

            System.Console.WriteLine("Initializing {0}", strISIN);

            stock.Save(World.GetInstance().QuotesPath + strISIN + ".sto");
         }
      }

      /// <summary>
      /// Updates the stock quotes to the most actual date.
      /// </summary>
      /// <param name="strStockFilename">The stock's path and filename to update</param>
      public void Update(string strStockFilename)
      {
         int nTotalImported = 0;
         int nImported = 0;
         string strWKN = Path.GetFileNameWithoutExtension(strStockFilename);

         Stock stock = dbengine.GetStock(strWKN);
         System.Console.WriteLine("Updating {0}", strWKN);

         do
         {
            nImported = Load(stock);
            nTotalImported += nImported;
         }
         while (nImported > 3);

         System.Console.WriteLine("{0} errors in plausibility check.", stock.CheckPlausibility());
         System.Console.WriteLine("{0} quotes imported.", nTotalImported);
         //System.Console.WriteLine("{0} close gaps filled.", stock.QuotesClose.FillGaps());
         //System.Console.WriteLine("{0} low gaps filled.", stock.QuotesLow.FillGaps());

         if (nTotalImported > 0)
         {
            stock.Save();
         }
      }

      /// <summary>
      /// Download the most recent stock's quote data from a quote server.
      /// It downloads max. 500 quote days. This method should be repeatedly called
      /// until it loads less than 4 quotes.
      /// </summary>
      /// <param name="stock">The stock object to be updated</param>
      /// <returns>The number of imported days</returns>
      public int Load(Stock stock)
      {
         bool bDoImport = false;
         int nImported = 0;

         WebRequest webreq;
         WebResponse webres;

         WorkDate startdate = stock.QuotesClose.YoungestDate.Clone() - 2;
         WorkDate enddate = startdate + 500;
         if (enddate > WorkDate.Today)
         {
            enddate = WorkDate.Today;
         }

         Log.Info("Downloading quotes from " + startdate + " to " + enddate);

         string strURI = BuildURI(stock, startdate, enddate);
         Log.Info(strURI);

         try
         {
            webreq = WebRequest.Create(strURI);
            webres = webreq.GetResponse();
         }
         catch (UriFormatException)
         {
            System.Console.WriteLine("URI invalid. Can not send a web request.");
            return 0;
         }
         catch (WebException)
         {
            System.Console.WriteLine("Can not send a web request. No network?");
            return 0;
         }

         Stream stream = webres.GetResponseStream();
         StreamReader strrdr = new StreamReader(stream);
         string strLine = strrdr.ReadLine();

         while ((strLine = strrdr.ReadLine()) != null)
         {
            if (EnableImport(strLine))
               bDoImport = true;

            if (DisableImport(strLine))
               bDoImport = false;

            if (bDoImport)
            {
               Log.Info(strLine);

               if (ParseAndStore(strLine, stock))
               {
                  nImported++;
               }
            }
         }

         strrdr.Close();
         stream.Close();
         Log.Info("Download and import finished.");

         return nImported;
      }
   }
}
