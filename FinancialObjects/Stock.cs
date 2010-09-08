/*
 * Stock.cs
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
    /// Verwaltet Informationen zu einem Wertpapier,
    /// z.B. Name und Kurse
    /// </summary>
    public class Stock
    {
        private string   m_strName;
        private string   m_strShortName;
        private string   m_strISIN;
        private string   m_strWKN;
        private double   m_dBonusLevel;
        private double   m_dCap;
        private double   m_dKnockOut;
        private WorkDate m_ExpireDate;
        private double   m_dRatio;
        private readonly DataContainer m_QuotesClose;
        private readonly DataContainer m_QuotesLow;
        private string   m_strFilename;

        /// <summary>
        /// Erzeugt eine neues Wertpapier-Objekt
        /// </summary>
        public Stock()
        {
            m_QuotesClose = new DataContainer();
            m_QuotesLow   = new DataContainer();
            m_ExpireDate  = new WorkDate();
            Clear();
        }


        public void Clear()
        {
            m_strName = "n.a";
            m_strShortName = "n.a";
            m_strISIN = "n.a";
            m_strWKN = "n.a";
            m_dBonusLevel = 0;
            m_dCap = 0;
            m_dKnockOut = 0;
            m_dRatio = 1.0;
            m_ExpireDate.Set(9999, 12, 31);
            m_strFilename = null;
           m_QuotesClose.Clear();
           m_QuotesLow.Clear();
       }

        public int FillGaps()
        {
            int nFilledGaps = m_QuotesClose.FillGaps();
            nFilledGaps +=    m_QuotesLow.FillGaps();

            return nFilledGaps;
        }

        /// <summary>
        /// Liefert und setzt den Namen
        /// </summary>
        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        /// <summary>
        /// Liefert und setzt den Kurz-Namen
        /// </summary>
        public string ShortName
        {
            get { return m_strShortName; }
            set { m_strShortName = value; }
        }

        /// <summary>
        /// Liefert und setzt die ISIN
        /// </summary>
        public string ISIN
        {
            get { return m_strISIN; }
            set { m_strISIN = value; }
        }

        /// <summary>
        /// Liefert und setzt die WKN
        /// </summary>
        public string WKN
        {
            get { return m_strWKN; }
            set { m_strWKN = value; }
        }

        /// <summary>
        /// Liefert und setzt den BonusLevel
        /// </summary>
        public double BonusLevel
        {
            get { return m_dBonusLevel; }
            set { m_dBonusLevel = value; }
        }

        /// <summary>
        /// Liefert und setzt den Cap-Level
        /// </summary>
        public double Cap
        {
            get { return m_dCap; }
            set { m_dCap = value; }
        }

        /// <summary>
        /// Liefert und setzt den KnockOut
        /// </summary>
        public double KnockOut
        {
            get { return m_dKnockOut; }
            set { m_dKnockOut = value; }
        }

        /// <summary>
        /// Liefert und setzt den Faelligkeitstermin
        /// </summary>
        public WorkDate Expire
        {
            get { return m_ExpireDate; }
            set { m_ExpireDate = value; }
        }

        /// <summary>
        /// Liefert und setzt den Ratio/Bezugsverhaeltnis
        /// </summary>
        public double Ratio
        {
            get { return m_dRatio; }
            set { m_dRatio = value; }
        }

        /// <summary>
        /// Liefert den DatenContainer der Kursdaten
        /// </summary>
        public DataContainer Quotes
        {
            get { return m_QuotesClose; }
        }

        /// <summary>
        /// Liefert den DatenContainer der Kursdaten (Schlusskurse)
        /// </summary>
        public DataContainer QuotesClose
        {
            get { return m_QuotesClose; }
        }

        /// <summary>
        /// Liefert den DatenContainer der Kursdaten (Low-Kurse)
        /// </summary>
        public DataContainer QuotesLow
        {
            get { return m_QuotesLow; }
        }

        /// <summary>
        /// Laedt Wertpapierdaten aus einer Datei
        /// </summary>
        /// <param name="strPathAndName">Dateiname der zu lesenden Datei</param>
        public void Load(string strPathAndName)
        {
            Clear();

            EasyStoreReader easystore = new EasyStoreReader();
            easystore.Open(strPathAndName);

            string strSection;
            while ((strSection = easystore.GetNextSection()) != null)
            {
                if (strSection.Equals("META"))
               {
                    LoadMetaData(easystore);
                }

               if (strSection.Equals("QUOTES") || strSection.Equals("CLOSE"))
               {
                    LoadQuotes(easystore, m_QuotesClose);
                }

               if (strSection.Equals("LOW"))
               {
                    LoadQuotes(easystore, m_QuotesLow);
                }
            }
 
            easystore.Close();

            m_strFilename = (string)strPathAndName.Clone();
        }

        /// <summary>
        /// Laedt Meta-Daten aus der Datei
        /// </summary>
        private void LoadMetaData(EasyStoreReader easystore)
        {
            DateTime termDate = new DateTime();
            string strKey, strValue;
            while (easystore.GetNextKeyValue(out strKey, out strValue))
            {
               if (strKey.Equals("NAME"))       this.Name = strValue;
               if (strKey.Equals("SHORTNAME"))  this.ShortName = strValue;
               if (strKey.Equals("ISIN"))       this.ISIN = strValue;
               if (strKey.Equals("WKN"))        this.WKN = strValue;
               if (strKey.Equals("BONUSLEVEL")) this.m_dBonusLevel = Double.Parse(strValue);
               if (strKey.Equals("CAP"))        this.m_dCap = Double.Parse(strValue);
               if (strKey.Equals("KNOCKOUT"))   this.m_dKnockOut = Double.Parse(strValue);
               if (strKey.Equals("EXPIRE"))
               {
                   DateTime.TryParse(strValue, out termDate);
                   this.m_ExpireDate.Set(termDate.Year, termDate.Month, termDate.Day);
               }
               if (strKey.Equals("RATIO"))      this.m_dRatio = Double.Parse(strValue);
            }
        }

        /// <summary>
        /// Laedt Kursdaten aus der Datei
        /// </summary>
        private void LoadQuotes(EasyStoreReader easystore, DataContainer quotes)
        {
            DateTime priceDate = new DateTime();
            string strKey, strValue;
            while (easystore.GetNextKeyValue(out strKey, out strValue))
            {
               DateTime.TryParse(strKey, out priceDate);
               WorkDate workdate = new WorkDate(priceDate.Year, priceDate.Month, priceDate.Day);
               quotes[workdate] = Double.Parse(strValue);
            }
        }

        public void Save()
        {
            if (m_strFilename != null)
                Save(m_strFilename);
            else
                throw new FileNotFoundException("Missing load");
        }

        /// <summary>
        /// Speichert Wertpapierdaten in eine Datei
        /// </summary>
        /// <param name="strPathAndName">Dateiname der zu schreibenden Datei</param>
        public void Save(string strPathAndName)
        {
            EasyStoreWriter easystore = new EasyStoreWriter();
            easystore.Open(strPathAndName);
            easystore.BeginSection("META");
            easystore.WriteKeyValue("NAME", this.Name);
            easystore.WriteKeyValue("SHORTNAME", this.ShortName);
            easystore.WriteKeyValue("ISIN", this.ISIN);
            easystore.WriteKeyValue("WKN", this.WKN);
            easystore.WriteKeyValue("BONUSLEVEL", this.BonusLevel.ToString());
            easystore.WriteKeyValue("CAP", this.Cap.ToString());
            easystore.WriteKeyValue("KNOCKOUT", this.KnockOut.ToString());
            easystore.WriteKeyValue("EXPIRE", this.Expire.ToString());
            easystore.WriteKeyValue("RATIO", this.Ratio.ToString());
            easystore.EndSection();
 
            easystore.BeginSection("CLOSE");
            SaveQuotes(easystore, m_QuotesClose);
            easystore.EndSection();

            easystore.BeginSection("LOW");
            SaveQuotes(easystore, m_QuotesLow);
            easystore.EndSection();

            easystore.Close();
        }

        /// <summary>
        /// Speichert Kursdaten aus der Datei
        /// </summary>
        private void SaveQuotes(EasyStoreWriter easystore, DataContainer quotes)
        {
            foreach (WorkDate workdate in quotes.Dates)
            {
                double dValue = Math.Round(quotes[workdate], 2);
                easystore.WriteKeyValue(workdate.ToString(), dValue.ToString());
            }
        }

    }
}
