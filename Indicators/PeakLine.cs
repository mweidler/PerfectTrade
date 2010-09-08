/*
 * PeakLine.cs
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
