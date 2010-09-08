//
// DBEngine.cs
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
