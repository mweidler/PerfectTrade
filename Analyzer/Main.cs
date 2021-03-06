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
using System.Collections.Generic;
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
            List<string> strEngines = GetEnginesList();

            if(args.Length == 0)
            {
               System.Console.WriteLine("Usage:");
               System.Console.WriteLine("{0} <Engine>", "Analyzer");
               System.Console.WriteLine("    where <Engine> can be be:");
               System.Console.Write("      all");

               foreach(string strEngine in strEngines)
               {
                  System.Console.Write(",{0}", strEngine);
               }

               System.Console.WriteLine();
            }
            else
            {
               if(args[0] == "all")
               {
                  foreach(string strEngine in strEngines)
                  {
                     DoAnalyze(strEngine);
                  }
               }
               else
               {
                  foreach(string param in args)
                  {
                     DoAnalyze(param);
                  }
               }
            }
         }
         catch(NotSupportedException e)
         {
            System.Console.WriteLine(e.Message);
         }
         catch(Exception e)
         {
            System.Console.WriteLine(e.Message);
            System.Console.WriteLine(e.StackTrace);
         }
      }

      /// <summary>
      /// Execute Ananlyer engine with given name
      /// </summary>
      /// <param name="strEngineName">Name of analyer engine to execute.</param>
      public static void DoAnalyze(string strEngineName)
      {
         System.Console.WriteLine("Analyzing " + strEngineName);
         World.GetInstance().SetWorldPaths(strEngineName);
         IAnalyzerEngine analyzer = InvokeAnalyzerEngine(strEngineName);
         analyzer.Analyze();
      }


      /// <summary>
      /// Search the desired class in the current assembly, instantiate and return it.
      /// </summary>
      /// <param name="strClassName">Name of the class to be searched and instantiated.</param>
      /// <returns>The newly created analyzer engine object.</returns>
      public static IAnalyzerEngine InvokeAnalyzerEngine(string strClassName)
      {
         Assembly assembly = Assembly.GetExecutingAssembly();

         // Walk through each type in the assembly looking for the class
         foreach(Type type in assembly.GetTypes())
         {
            if(type.IsClass == true)
            {
               // e.g. AnalyzerEngine.RelativeStrength
               if(type.FullName.EndsWith("." + strClassName))
               {
                  // create an instance of the object
                  return (IAnalyzerEngine)Activator.CreateInstance(type);
               }
            }
         }

         throw new NotSupportedException("Analyzer engine not found: " + strClassName);
      }


      /// <summary>
      /// Returns available analyzer engines.
      /// </summary>
      public static List<string> GetEnginesList()
      {
         List<string> strEngines = new List<string>();

         Assembly assembly = Assembly.GetExecutingAssembly();
         string strAssemblyName = assembly.FullName.Substring(0, assembly.FullName.IndexOf(','));

         // Walk through each type in the assembly looking for our class
         foreach(Type type in assembly.GetTypes())
         {
            if(type.IsClass == true)
            {
               // e.g. AnalyzerEngine.RelativeStrength
               string strEngineName = type.FullName.Substring(type.FullName.IndexOf('.') + 1);

               if(strAssemblyName.Equals(strEngineName) == false)
               {
                  strEngines.Add(strEngineName);
               }
            }
         }

         return strEngines;
      }
   }
}
