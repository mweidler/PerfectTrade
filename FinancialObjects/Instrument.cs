/*
 * Instrument.cs
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
    /// Zusammenfassung von Meta-Daten eines Wertpapieres.
    /// </summary>
    public class Instrument
    {
        private string m_strName;
        private string m_strID;

        /// <summary>
        /// Legt ein neues Objekt von Instrument an.
        /// </summary>
        /// <param name="strName">Name des Wertpapieres</param>
        /// <param name="strID">WKN oder ISIN des Wertpapieres</param>
        public Instrument(string strName, string strID)
        {
            m_strName = strName;
            m_strID = strID;
        }

        /// <summary>
        /// Legt ein neues Objekt von Instrument an.
        /// </summary>
        public Instrument()
        {
            m_strName = null;
            m_strID = null;
        }

        /// <summary>
        /// Setzt oder liefert den Namen des wertpapieres
        /// </summary>
        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        /// <summary>
        /// Setzt oder liefert die WKN/ISIN des wertpapieres
        /// </summary>
        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }
    }
}
