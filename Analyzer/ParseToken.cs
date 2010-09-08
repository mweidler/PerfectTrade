//
// ParseToken.cs
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

