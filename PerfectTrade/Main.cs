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
         DataContainer quotes = dax.QuotesLow;

         Chart chart = new Chart();
         chart.Add(quotes, Chart.LineType.Red, "Test");
         chart.Create(World.GetInstance().ResultPath + "dax.png");

         DataContainer dax_relperf = RelativePerformance.CreateFrom(quotes, new WorkDate(2008, 4, 21));
         dax_relperf.Save(World.GetInstance().ResultPath + "dax_relperf.csv", ";");
      }
   }
}
