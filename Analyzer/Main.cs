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
using System.IO;
using System.Reflection;
using System.ComponentModel;
using FinancialObjects;
using Indicators;

namespace Analyzer
{
   class Analyzer
   {
      public static void Main(string[] args)
      {
         try
         {
            if (args.Length > 0)
            {
               World.GetInstance().SetWorldPaths(args[0]);
               IAnalyzerEngine analyzer = InvokeAnalyzerEngine(args[0]);
               analyzer.Analyze();
            }
            else
            {
               DumpEngines();
            }
         }
         catch (NotSupportedException e)
         {
            System.Console.WriteLine(e.Message);
         }
         catch (Exception e)
         {
            System.Console.WriteLine(e.Message);
            System.Console.WriteLine(e.StackTrace);
         }
      }

      public static IAnalyzerEngine InvokeAnalyzerEngine(string strClassName)
      {
         Assembly assembly = Assembly.GetExecutingAssembly();

         // Walk through each type in the assembly looking for our class
         foreach (Type type in assembly.GetTypes())
         {
            if (type.IsClass == true)
            {
               // e.g. AnalyzerEngine.RelativeStrength
               if (type.FullName.EndsWith("." + strClassName))
               {
                  // create an instance of the object
                  return (IAnalyzerEngine)Activator.CreateInstance(type);
               }
            }
         }

         throw(new System.NotSupportedException("Analyzer engine not found: " + strClassName));
      }

      public static void DumpEngines()
      {
         int nEngine = 1;

         Assembly assembly = Assembly.GetExecutingAssembly();
         string strAssemblyName = assembly.FullName.Substring(0, assembly.FullName.IndexOf(','));

         System.Console.WriteLine("Possible analyzer engines:");

         // Walk through each type in the assembly looking for our class
         foreach (Type type in assembly.GetTypes())
         {
            if (type.IsClass == true)
            {
               // e.g. AnalyzerEngine.RelativeStrength
               string strEngineName = type.FullName.Substring(type.FullName.IndexOf('.') + 1);

               if (strAssemblyName.Equals(strEngineName) == false)
               {
                  System.Console.WriteLine("{0: 0}) " + strEngineName, nEngine);
                  nEngine++;
               }
            }
         }
      }
   }
}
