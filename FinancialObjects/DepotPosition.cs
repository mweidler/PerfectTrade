//
// DepotPosition.cs
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

namespace FinancialObjects
{
   /// <summary>
   /// Stellt eine Depot-Positionen dar.
   /// </summary>
   public class DepotPosition
   {
      private readonly string m_strWKN;
      private uint     m_nQuantity;
      private WorkDate m_BuyDate;
      private double m_dBuyPrice;
      private double m_dStopLoss;
      private double m_dPrice;

      /// <summary>
      /// Erzeugt eine neue Dpotposition
      /// </summary>
      public DepotPosition(string strWKN, uint nQuantity, WorkDate buyDate, double dBuyPrice)
      {
         m_strWKN      = strWKN;
         this.Quantity = nQuantity;
         this.BuyDate  = buyDate;
         this.BuyPrice = dBuyPrice;
         this.StopLoss = 0.0;
         this.Price    = dBuyPrice;
      }

      /// <summary>
      /// Wertpapierkennnummer oder ID
      /// </summary>
      public string WKN
      {
         get
         {
            return m_strWKN;
         }
      }

      /// <summary>
      /// Stueckzahl
      /// </summary>
      public uint Quantity
      {
         get
         {
            return m_nQuantity;
         }
         set
         {
            m_nQuantity = value;
         }
      }

      /// <summary>
      /// Kaufdatum
      /// </summary>
      public WorkDate BuyDate
      {
         get
         {
            return m_BuyDate;
         }
         set
         {
            m_BuyDate = new WorkDate(value);
         }
      }

      /// <summary>
      /// Kaufpreis (Kurs) pro Stueck
      /// </summary>
      public double BuyPrice
      {
         get
         {
            return m_dBuyPrice;
         }
         set
         {
            m_dBuyPrice = value;
         }
      }

      /// <summary>
      /// Stop-Loss-wert
      /// </summary>
      public double StopLoss
      {
         get
         {
            return m_dStopLoss;
         }
         set
         {
            m_dStopLoss = value;
         }
      }

      /// <summary>
      /// Aktueller Kurs (Preis)
      /// </summary>
      public double Price
      {
         get
         {
            return m_dPrice;
         }
         set
         {
            m_dPrice = value;
         }
      }

      /// <summary>
      /// Liefert die aktuelle Positions-entwicklung in dB-Prozent.
      //  100 dB% bedeutet eine Verdoppelung, -100 dB% eine Halbierung
      /// </summary>
      public double Performance
      {
         get
         {
            return Math.Log(m_dPrice / m_dBuyPrice, 2.0) * 100.0;
            //return ((m_dPrice / m_dBuyPrice) - 1.0) * 100.0;
         }
      }

      /// <summary>
      /// Berechnet die Dauer der aktuellen Anlage in Tagen.
      /// Bei Zukaeufen wird ab dem letzten Kaufdatum gezaehlt.
      /// </summary>
      /// <param name="workdate">Aktuelles Datum</param>
      /// <returns>Dauer der Anlage in Tagen</returns>
      public long Age(WorkDate workdate)
      {
         return workdate - m_BuyDate;
      }
   }
}
