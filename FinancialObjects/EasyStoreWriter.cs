/*
 * EasyStoreWriter.cs
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
    /// Strukturierter Datenspeicher fuer String-Basierte Daten.
    /// </summary>
    ///
    /// Daten koennen aehnlich wie in einer XML-Datei gespeichert werden.
    /// Die Struktur der Datei ist aber nicht festgelegt. Die Daten koennen
    /// in Sektionen strukturiert werden.
    /// 
    /// Beispiel:
    /// <code>
    ///     EasyStoreWriter easystore = new EasyStoreWriter();
    ///     easystore.Open(strPathAndName);
    ///     easystore.BeginSection("META");
    ///     easystore.WriteKeyValue("NAME",      this.Name);
    ///     easystore.WriteKeyValue("SHORTNAME", this.ShortName);
    ///     easystore.WriteKeyValue("ISIN",      this.ISIN);
    ///     easystore.WriteKeyValue("WKN",       this.WKN);
    ///     easystore.EndSection();
    ///     easystore.BeginSection("QUOTES");
    ///     foreach (WorkDate workdate in m_quotes.Dates) {
    ///         easystore.WriteKeyValue(workdate.ToString(), m_quotes[workdate].ToString());
    ///     }
    ///     easystore.EndSection();
    ///     easystore.Close();
    /// </code>
    public class EasyStoreWriter
    {
        private StreamWriter sw;
        private bool m_bSection;

        /// <summary>
        /// Legt ein neues Objekt von EasyStoreWriter an.
        /// </summary>
        public EasyStoreWriter()
        {
            m_bSection = false;
        }

        /// <summary>
        /// Oeffnet eine Datei, in dem Daten abgelegt werden koennen.
        /// </summary>
        /// <param name="strPathName">Dateiname mit Pfad auf die zu oeffnende Datei</param>
        public void Open(string strPathName)
        {
            sw = new StreamWriter(strPathName, false, Encoding.ASCII);
            m_bSection = false;
        }

        /// <summary>
        /// Schliesst die Datei.
        /// </summary>
        public void Close()
        {
            sw.Close();
            m_bSection = false;
        }

        /// <summary>
        /// Oeffnet eine neue Sektion in der Datei.
        /// </summary>
        /// Eine Sektion mit beliebigem Namen wird geoeffnet. Eine bereits geoeffnete
        /// Sektion muss vor diesem Aufruf geschlossen werden.
        /// <param name="strSectionName">Name der neuen Sektion</param>
        public void BeginSection(string strSectionName)
        {
            if (m_bSection == false)
            {
                sw.WriteLine("SECTION " + strSectionName);
                m_bSection = true;
            }
            else throw new InvalidOperationException("Section already opened.");
        }

        /// <summary>
        /// Schliesst eine vorher geoeffnete Sektion in der Datei.
        /// </summary>
        /// Eine Sektion muss offen sein bevor sie geschlossen wird.
        /// <param name="strSectionName">Name der neuen Sektion</param>
        public void EndSection()
        {
            if (m_bSection)
            {
                sw.WriteLine("END");
                sw.WriteLine();
                m_bSection = false;
            }
            else throw new InvalidOperationException("Section has not beed opened.");
        }

        /// <summary>
        /// Schreibt ein Schluessel-Werte-Paar in die Datei.
        /// </summary>
        /// Ein Schluessel-Werte-Paar kann an beliebigen Stellen geschrieben werden.
        /// Eine Sektion muss nicht unbedingt geoeffnet sein. Eine Sektion dient nur
        /// zur Strukturierung der Daten.
        /// <param name="strKey">Schluessel des Paares</param>
        /// <param name="strValue">Wert des Paares</param>
        public void WriteKeyValue(string strKey, string strValue)
        {
            sw.WriteLine(strKey + "=" + strValue);
        }
    }
}
