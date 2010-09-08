/*
 * DepotPosition.cs
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
            get { return m_strWKN; }
        }

        /// <summary>
        /// Stueckzahl
        /// </summary>
        public uint Quantity
        {
            get { return m_nQuantity; }
            set { m_nQuantity = value; }
        }

        /// <summary>
        /// Kaufdatum
        /// </summary>
        public WorkDate BuyDate
        {
            get { return m_BuyDate; }
            set { m_BuyDate = new WorkDate(value); }
        }

        /// <summary>
        /// Kaufpreis (Kurs) pro Stueck
        /// </summary>
        public double BuyPrice
        {
            get { return m_dBuyPrice; }
            set { m_dBuyPrice = value; }
        }

        /// <summary>
        /// Stop-Loss-wert
        /// </summary>
        public double StopLoss
        {
            get { return m_dStopLoss; }
            set { m_dStopLoss = value; }
        }

        /// <summary>
        /// Aktueller Kurs (Preis)
        /// </summary>
        public double Price
        {
            get { return m_dPrice; }
            set { m_dPrice = value; }
        }

        /// <summary>
        /// Liefert die aktuelle Positions-entwicklung in dB-Prozent.
        //  100 dB% bedeutet eine Verdoppelung, -100 dB% eine Halbierung
        /// </summary>
        public double Performance
        {
            get {
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
