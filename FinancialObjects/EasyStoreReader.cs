//
// EasyStoreReader.cs
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
