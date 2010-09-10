//
// EasyStoreWriter.cs
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
