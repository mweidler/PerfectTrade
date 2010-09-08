/*
 * World.cs
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
    public class World
    {
        private static World m_WorldObject = null;
        private string m_strQuotesPath = null;
        private string m_strDataPath = null;
        private string m_strResultPath = null;
        private SortedList<string, Instrument> m_instruments = new SortedList<string, Instrument>();

        private World()
        {
        }

        public static World GetInstance()
        {
            if (m_WorldObject == null)
            {
                m_WorldObject = new World();
            }

            return m_WorldObject;
        }

        public string QuotesPath
        {
            get { return m_strQuotesPath; }
            set { m_strQuotesPath = value; }
        }

        public string DataPath
        {
            get { return m_strDataPath; }
            set { m_strDataPath = value; }
        }

        public string ResultPath
        {
            get { return m_strResultPath; }
            set { m_strResultPath = value; }
        }

        public SortedList<string, Instrument> Instruments
        {
            get { return m_instruments; }
        }
    }
}
