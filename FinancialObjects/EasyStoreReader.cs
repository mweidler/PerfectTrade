/*
 * EasyStoreReader.cs
 *
 * (C)OPYRIGHT 2007 BY MARC WEIDLER, ULRICHSTR. 12/1, 71672 MARBACH, GERMANY.
 *
 * All rights reserved. This product and related documentation are protected
 * by copyright restricting its use, copying, distribution, and decompilation.
 * No part of this product or related documentation may be reproduced in any
 * form by any means without prior written authorization of Marc Weidler or
 * his partners, if any. Unless otherwise arranged, third parties may not
 * have access to this product or related documentation.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FinancialObjects
{
    /// <summary>
    /// Ermoeglicht das Lesen aus einem strukturierter Datenspeicher fuer String-Basierte Daten.
    /// </summary>
    ///
    /// Daten koennen aehnlich wie in einer XML-Datei gelesen werden.
    /// Die Struktur der Datei ist aber nicht festgelegt. Die Daten koennen
    /// in Sektionen strukturiert werden.
    public class EasyStoreReader
    {
        private StreamReader sr;
        private bool m_bSection;

        /// <summary>
        /// Legt ein neues Objekt von EasyStoreReader an.
        /// </summary>
        public EasyStoreReader()
        {
            m_bSection = false;
        }

        /// <summary>
        /// Oeffnet eine Datei, aus der Daten gelesen werden sollen.
        /// </summary>
        /// <param name="strPathName">Dateiname mit Pfad auf die zu oeffnende Datei</param>
        public void Open(string strPath)
        {
            sr = new StreamReader(strPath);
            m_bSection = false;
        }

        /// <summary>
        /// Schliesst die Datei.
        /// </summary>
        public void Close()
        {
            sr.Close();
            m_bSection = false;
        }

	/// <summary>
        /// Springt zur naechsten Sektion und liefert dessen Namen oder
        ///  null bei Dateiende zurueck.
        /// </summary>
        /// <returns>Name der naechsten Sektion oder null bei Dateiende</returns>
        public string GetNextSection()
        {
            string strLine;
            do
            {
                strLine = sr.ReadLine();
                if (strLine == null) return null;
            }
            while (strLine.StartsWith("SECTION") == false);

            m_bSection = true;
            return strLine.Substring(8);
        }

	/// <summary>
        /// Liefert das naechste Schluessel-Werte-Paar in der Datei.
        /// </summary>
        /// <param name="strKey">Out-Parameter des Schluessels</param>
        /// <param name="strValue">Out-Parameter des Wertes</param>
        /// <returns>true, wenn strKey und strValue korrekt gefuellt wurden, oder
        ///          false bei Datei- oder Sektion-Ende</returns>
        public bool GetNextKeyValue(out string strKey, out string strValue)
        {
            if (m_bSection == false)
            {
                strKey = "";
                strValue = "";
                return false;
            }

            string strLine = sr.ReadLine();
            if (strLine.Equals("END"))
            {
                strKey = "";
                strValue = "";
                m_bSection = false;
                return false;
            }

            strKey = strLine.Substring(0, strLine.IndexOf('='));
            strValue = strLine.Substring(strLine.IndexOf('=') + 1);
            return true;
        }
    }
}
