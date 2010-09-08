//
// PeakLine.cs
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
using FinancialObjects;

namespace Indicators
{
    public class PeakLine
    {
        public enum PeakType
        {
            UPPER = 1,
            LOWER = 2
        };

        public static DataContainer CreateFrom(DataContainer source, int nRange, PeakType peaktype)
        {
            if (nRange < 1)
                throw new ArgumentOutOfRangeException("nRange", nRange, "Must be greater than zero.");

            DataContainer result = new DataContainer();

            WorkDate reference = source.OldestDate.Clone();
            reference += nRange;
            WorkDate today = reference.Clone();
            today += nRange;
            WorkDate enddate = source.YoungestDate.Clone();
            enddate -= nRange;
            double dPeakValue = 0;

            while (today <= source.YoungestDate)
            {
                if (reference <= enddate)
                {
                    if (IsPeak(reference, nRange, peaktype, source))
                    {
                        dPeakValue = source[reference];
                    }
                }

                result[today] = dPeakValue;
                today++;
                reference++;
            }

            return result;
        }

        protected static bool IsPeak(WorkDate testdate, int nRange, PeakType peaktype, DataContainer source)
        {
            WorkDate workdate = testdate.Clone();
            workdate -= nRange;
            WorkDate rangeend = testdate.Clone();
            rangeend += nRange;

            while (workdate <= rangeend)
            {
                if ((peaktype == PeakType.LOWER && source[workdate] < source[testdate]) ||
                    (peaktype == PeakType.UPPER && source[workdate] > source[testdate]))
                {
                   return false;
                }

               workdate++;
            }

            return true;
        }
    }
}
