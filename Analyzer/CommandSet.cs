// 
// <filename>.cs
// 
// (C)OPYRIGHT 2007 BY MARC WEIDLER, ULRICHSTR. 12/1, 71672 MARBACH, GERMANY.
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
namespace Analyzer
{
#if TESTXX
    public class CommandSet
    {
       public string original;
       public string m_strTarget;
       public string m_strCommand;
       public string[] parameters;

       public CommandSet(string target, string command, string[] parameters)
       {
          this.target = target;
          this.command = command;
          this.parameters = parameters;
       }

       public void AddParameter(string parameter)
       {
          parameters.Add(parameter);
       }

       public string OriginalLine
       {
          get{ value = original; }

          set{
           original = value;
          }
       }

       public string Command(string parameter)
       {
          get { return m_strCommand; }
          set { m_strCommand = value; }

          get{
             value = command;
          }

          set{
           command = parameter;
          }
       }

       public int NumberOfParameters
       {
         get{
           value = parameters.size;
         }
       }
    }
#endif
}

