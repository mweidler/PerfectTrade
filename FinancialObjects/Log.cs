// 
// Log.cs
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
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FinancialObjects
{
    /// <summary>
    /// Stellt grundlegende Logging-Funktionalitaet zur Verfuegung.
    /// Die Log-Daten werden in eine Datei im Benutzerverzeichnis abgelegt.
    /// </summary>
    public static class Log
    {
        private static string m_path = null;

        /// <summary>
        /// Schreibt Informationen ins Logfile
        /// </summary>
        /// <param name="strLogText">Die zu schreibende Information</param>
        public static void Info(string strLogText)
        {
            if (m_path == null)
            {
                m_path = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                m_path += "/PerfectTrade.log";
            }

            //Console.WriteLine(strLogText);

            StreamWriter sw = new StreamWriter(m_path, true);
            sw.WriteLine(strLogText);
            sw.Close();
        }
    }
}
