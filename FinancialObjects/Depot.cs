//
// Depot.cs
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
   /// Verwaltet eine Liste mit Depot-Positionen.
   /// </summary>
   public class Depot
   {
      private SortedList<string, DepotPosition> m_positions;
      private double m_dCash;
      private double m_dInitialCash;
      private double m_dProvisionRate;
      private uint m_nTrades;

      /// <summary>
      /// Erzeugt ein neues Depot-Objekt und fuehr einen Reset() durch.
      /// </summary>
      public Depot()
      {
         m_positions = new SortedList<string, DepotPosition>();
         Reset();
      }

      /// <summary>
      /// Loescht alle Positionen im Depot und setzt alle
      /// Werte (Cash, InitialCash, Provision, Anzahl der Transaktionen)
      /// auf 0.
      /// </summary>
      public void Reset()
      {
         m_positions.Clear();
         m_dCash = 0;
         m_dInitialCash = 0;
         m_dProvisionRate = 0.0025;
         m_nTrades = 0;
      }

      /// <summary>
      /// Liefert einen Enumerator alle Depot-Positionen
      /// </summary>
      /// <returns></returns>
      public IEnumerator<DepotPosition> GetEnumerator()
      {
         foreach (DepotPosition position in m_positions.Values)
         {
            yield return position;
         }
      }

      /// <summary>
      /// Liefert die Anzahl der Kaeufe.
      /// </summary>
      public uint Trades
      {
         get
         {
            return m_nTrades;
         }
      }

      /// <summary>
      ///
      /// </summary>
      /// <param name="iIndex"></param>
      /// <returns></returns>
      public DepotPosition this[int iIndex]
      {
         get
         {
            return m_positions.Values[iIndex];
         }
      }

      /// <summary>
      /// Liefert die Anzahl der Positionen im Depot
      /// </summary>
      public uint Count
      {
         get
         {
            return (uint)m_positions.Count;
         }
      }

      /// <summary>
      /// Legt die Provisionierung (in Prozent) beim Kauf und Verkauf fest
      /// Beispiel: 0.005 entspricht 0,5% Provision
      /// </summary>
      public double ProvisionRate
      {
         get
         {
            return m_dProvisionRate;
         }
         set
         {
            m_dProvisionRate = value;
         }
      }

      /// <summary>
      /// Liefert und setzt die Menge des Bar-Kapitals
      /// Achtung: ein Setzen des Cash-Wertes macht nur Sinn, wenn
      /// noch keine Positionen im Depot sind. Ansonsten wird die
      /// Depot-Performance verfaelscht.
      /// </summary>
      public double Cash
      {
         get
         {
            return m_dCash;
         }
         set
         {
            m_dCash = value;
            m_dInitialCash = value;
         }
      }

      /// <summary>
      /// Liefert die Summe aus Depotwert und Bar-Kapital.
      /// </summary>
      public double Equity
      {
         get
         {
            return this.Cash + this.Asset;
         }
      }

      /// <summary>
      /// Liefert die aktuelle Depot-Entwicklung in dB-Prozent.
      //  100 dB% bedeutet eine Verdoppelung, -100 dB% eine Halbierung
      /// </summary>
      public double Performance
      {
         get
         {
            return Math.Log(this.Equity / m_dInitialCash, 2.0) * 100.0;
         }

         //get { return (((m_dCash + this.Amount) / m_dInitialCash) - 1.0) * 100.0; }
      }

      /// <summary>
      /// Prueft, ob ein Wertpapier (WKN) im Depot vorhanden ist.
      /// </summary>
      /// <param name="strWKN">Wertpapierkennnummer oder ID</param>
      /// <returns><c>true</c>, wenn das Wertpapier existiert, ansonsten <c>false</c></returns>
      public bool ContainsKey(string strWKN)
      {
         return m_positions.ContainsKey(strWKN);
      }

      /// <summary>
      /// Liefert die Summe der Bewertung aller Depot-Positionen.
      /// </summary>
      public double Asset
      {
         get
         {
            double dDepotAsset = 0.0;

            foreach (DepotPosition position in m_positions.Values)
            {
               dDepotAsset += position.Quantity * position.Price;
            }

            return dDepotAsset;
         }
      }

      /// <summary>
      /// Kauft ein Wertpapier mit den angegebenen Daten.
      /// </summary>
      /// <param name="strWKN">WKN, bzw. ID</param>
      /// <param name="nQuantity">Stueckzahl</param>
      /// <param name="buyDate">Kaufdatum</param>
      /// <param name="dBuyPrice">Kaufpreis</param>
      public void Buy(string strWKN, uint nQuantity, WorkDate buyDate, double dBuyPrice)
      {
         DepotPosition depotposition = null;

         if (m_positions.ContainsKey(strWKN))
         {
            depotposition = m_positions[strWKN];
            uint nNewQuantity = nQuantity + depotposition.Quantity;
            double dNewPrice = (depotposition.Quantity * depotposition.BuyPrice +
                                nQuantity * dBuyPrice) / nNewQuantity;

            depotposition.Quantity = nNewQuantity;
            depotposition.BuyDate = buyDate;
            depotposition.BuyPrice = dNewPrice;
         }
         else
         {
            depotposition = new DepotPosition(strWKN, nQuantity, buyDate, dBuyPrice);
            m_positions.Add(strWKN, depotposition);
         }

         m_dCash -= nQuantity * dBuyPrice * (1.0 + m_dProvisionRate);
         m_nTrades++;
         Log.Info("Buy: " + strWKN + " " + buyDate + " " + nQuantity * dBuyPrice);
      }

      /// <summary>
      /// Verkauft ein Wertpapier mit den angegebenen Daten zum
      /// aktuellen Preis im Depot. Evtl. muss vor dem Verkauf
      /// eine Kursaktualisierung durchgefuehrt werden.
      /// </summary>
      /// <param name="strWKN">WKN, bzw. ID</param>
      /// <param name="nQuantity">Stueckzahl</param>
      /// <param name="sellDate">Verkaufsdatum</param>
      public void Sell(string strWKN, uint nQuantity, WorkDate sellDate)
      {
         if (m_positions.ContainsKey(strWKN))
         {
            DepotPosition depotposition = m_positions[strWKN];

            nQuantity = Math.Min(nQuantity, depotposition.Quantity);
            depotposition.Quantity -= nQuantity;
            m_dCash += nQuantity * depotposition.Price * (1.0 - m_dProvisionRate);
            m_nTrades++;

            if (depotposition.Quantity == 0)
            {
               m_positions.RemoveAt(m_positions.IndexOfKey(strWKN));
            }

            Log.Info("Sell: " + depotposition.WKN + " " + sellDate + " " + nQuantity);
         }
      }

      /// <summary>
      /// Gibt des Inhalt des Depots tabellarisch aus.
      /// </summary>
      /// <param name="today">Aktuelles Datum</param>
      public void Dump(WorkDate today)
      {
         int nPosition = 1;
         Console.Out.WriteLine();
         Console.Out.WriteLine("## Anz.  WKN   Kaufpr. Kaufdatum  Kurs    Age Perf. StopLoss");

         foreach (DepotPosition position in m_positions.Values)
         {
            Console.Out.WriteLine("{0:00} {1:0000} {2} {3:0000.00} {4} {5:0000.00} {6:000} {7:000.0} {8:0000.00}   ",
                                  nPosition,
                                  position.Quantity,
                                  position.WKN,
                                  position.BuyPrice,
                                  position.BuyDate,
                                  position.Price,
                                  position.Age(today),
                                  position.Performance,
                                  position.StopLoss);
            nPosition++;
         }

         for (int i = 10 - nPosition; i > 0; i--)
         {
            Console.Out.WriteLine("                                                        ");
         }

         Console.Out.WriteLine("Trades: {0:00}  Depotwert: {1:n}   Cash: {2:n}   Equity: {3:n}",
                               this.Trades,
                               this.Asset,
                               this.Cash,
                               this.Equity);
      }
   }
}
