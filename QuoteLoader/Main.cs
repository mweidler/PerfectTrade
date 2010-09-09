//
// Main.cs
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
using System.Text;
using System.IO;
using FinancialObjects;

namespace QuoteLoader
{
   class MainClass
   {
      public static void Usage()
      {
         System.Console.WriteLine("QuoteLoader <command> <parameter>\n" +
                                  "\n" +
                                  "<command>:\n" +
                                  "    init    initialize the given wkn number\n" +
                                  "    update  update wkn quotes\n" + "\n");
      }

      public static void Main(string[] args)
      {
         string strBasePath = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
         World.GetInstance().QuotesPath = strBasePath + "/tradedata/quotes/";

         if (args.Length < 1) {
            Usage();
            return;
         }

         try {

            QuoteLoaderEngine loaderEngine = new BoerseOnlineQuoteLoaderEngine();

            if (args[0].Equals("init")) {
               string strISIN = args[1];
               loaderEngine.Init(strISIN);
            } else if (args[0].Equals("update")) {
               string[] strStockFilenames = Directory.GetFiles(World.GetInstance().QuotesPath, "*.sto");
               foreach (string strStockFilename in strStockFilenames) {
                  loaderEngine.Update(strStockFilename);
               }
            } else {
               System.Console.WriteLine("Unknown command {0}.", args[0]);
            }

         } catch (Exception e) {
            System.Console.WriteLine(e.Message);
            System.Console.WriteLine(e.StackTrace);
         }
      }
   }
}
