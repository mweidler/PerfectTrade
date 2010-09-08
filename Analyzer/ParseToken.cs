//
// <filename>.cs
// 
// (C)OPYRIGHT 2007 BY MARC WEIDLER, ULRICHSTR. 12/1, 71672 MARBACH, GERMANY.
// 
// All rights reserved. This product and related documentation are protected by
// copyright restricting its use, copying, distribution, and decompilation. No part
// of this product or related documentation may be reproduced in any form by any
// means without prior written authorization of Marc Weidler or his partners, if any.
// Unless otherwise arranged, third parties may not have access to this product or 
// related documentation.
// 
// THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW.
// THE COPYRIGHT HOLDERS AND/OR OTHER PARTIES PROVIDE THE PROGRAM "AS IS" WITHOUT
// WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO,
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.
// THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU.
// SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY
// SERVICING, REPAIR OR CORRECTION.
//

using System;
using System.Collections.Generic;

namespace Analyzer
{
    public class ParseToken
    {
        private bool m_bUnary;
        private string m_strTarget;
        private string m_strCommand;
        private string m_strOriginalLine;
        private int    m_nLineNumber;
        private List<string> m_lParameters;

        public ParseToken()
        {
            m_lParameters = new List<string>();
        }

        /// <summary>
        /// Liefert die Anzahl der Kaeufe.
        /// </summary>
        public bool IsUnary {
            get { return m_bUnary; }
            set { m_bUnary = value; }
        }

        /// <summary>
        /// Liefert die Anzahl der Kaeufe.
        /// </summary>
        public string Target {
            get { return m_strTarget; }
            set { m_strTarget = value; }
        }

        /// <summary>
        /// Liefert die Anzahl der Kaeufe.
        /// </summary>
        public string Command {
            get { return m_strCommand; }
            set { m_strCommand = value; }
        }

        public string OriginalLine {
            get { return  m_strOriginalLine; }
            set { m_strOriginalLine = value; }
        }

        public int LineNumber {
            get { return m_nLineNumber; }
            set { m_nLineNumber = value; }
        }

        public List<string> Parameters {
            get { return m_lParameters; }
        }
    }
}

