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
using System.Reflection;
using System.Threading;
using System.Text;
using System.IO;
using System.ComponentModel;
using FinancialObjects;

namespace QuoteLoader
{
    class MainClass
    {
        static void SetWorldPaths(string strApplicationName)
        {
            string strBasePath = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            string strResultPath = strBasePath + "/tradedata/results/" + strApplicationName + "/";
            if (Directory.Exists(strResultPath) == false) {
                Directory.CreateDirectory(strResultPath);
            }

            string strDataPath = strBasePath + "/tradedata/data/" + strApplicationName + "/";
            if (Directory.Exists(strDataPath) == false) {
                Directory.CreateDirectory(strDataPath);
            }

            World.GetInstance().ResultPath = strResultPath;
            World.GetInstance().DataPath = strDataPath;
            World.GetInstance().QuotesPath = strBasePath + "/tradedata/quotes/";
        }

        public static void Main(string[] args)
        {
            SetWorldPaths("RelativeStrength");

            try {
                QuoteLoaderEngine loaderEngine = new BoerseOnlineQuoteLoaderEngine();

                loaderEngine.Read("486217", new WorkDate());

            }catch (Exception e) {
                System.Console.WriteLine(e.Message);
                System.Console.WriteLine(e.StackTrace);
            }
        }
    }
}




