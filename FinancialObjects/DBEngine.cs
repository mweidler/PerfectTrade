/*
 * DBEngine.cs
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
    public class DBEngine
    {
        private static DBEngine m_engine = null;
        private SortedList<string, Stock> m_stocks;
        private string m_strPath;
        
        private DBEngine()
        {
            m_stocks = new SortedList<string, Stock>();
            m_strPath = World.GetInstance().QuotesPath;
            if (m_strPath == null)
                throw new NullReferenceException("Quotespath not set in World object.");
        }

        public void AddVirtualInvestment( string strWKN, string strName, DataContainer datacontainer )
        {
            Stock stock = new Stock();
            DataContainer.Copy(datacontainer, stock.Quotes);
            stock.ISIN = "n.a.";
            stock.WKN = strWKN;
            stock.Name = strName;
            stock.ShortName = strName;

            m_stocks.Add(strWKN, stock);
        }

        public string GetName(string strWKN)
        {
            if (m_stocks.ContainsKey(strWKN) == false)
            {
                GetQuotes(strWKN);
            }

            return m_stocks[strWKN].ShortName;
        }

        public bool Exists(string strWKN)
        {
            if (m_stocks.ContainsKey(strWKN) || File.Exists(m_strPath + strWKN + ".sto"))
            {
                return true;
            }

            return false;
        }

        public Stock GetStock(string strWKN)
        {
            string strFullPathName = m_strPath + strWKN + ".sto";

            if (m_stocks.ContainsKey(strWKN) == false)
            {
                if (File.Exists(strFullPathName) == false)
                {
                    throw new FileNotFoundException("Unbekanntes Wertpapier: " + strWKN, strFullPathName);
                }

                Stock stock = new Stock();
                stock.Load(strFullPathName);
                m_stocks.Add(strWKN, stock);
            }

            return m_stocks[strWKN];
        }

        public DataContainer GetQuotes(string strWKN)
        {
            return GetStock(strWKN).Quotes;
        }

        public bool HasPrice(string strWKN, WorkDate workdate)
        {
            DataContainer data = GetQuotes(strWKN);
            return data.Contains(workdate);
        }

        public double GetPrice(string strWKN, WorkDate workdate)
        {
            DataContainer data = GetQuotes(strWKN);
            return data[workdate];
        }

        public static DBEngine GetInstance()
        {
            if (m_engine == null)
            {
                m_engine = new DBEngine();
            }

            return m_engine;
        }
    }
}
