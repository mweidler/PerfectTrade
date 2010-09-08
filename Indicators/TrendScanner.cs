/*
 * TrendScanner.cs
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
    public class TrendScanner : PeakLine
    {
        public const int OUTPERFORM = 10;
        public const int CANCEL_OUTPERFORM = 5;
        public const int TREND_UNKNOWN = 0;
        public const int CANCEL_UNDERPERFORM = -5;
        public const int UNDERPERFORM = -10;

        public static DataContainer CreateFrom(DataContainer source, int nRange)
        {
            DataContainer result = new DataContainer();

            WorkDate reference = source.OldestDate.Clone();
            reference += nRange;
            WorkDate today = reference.Clone();
            today += nRange;
            WorkDate enddate = source.YoungestDate.Clone();
            enddate -= nRange;

            double dUpperPeak = Double.MaxValue;
            double dLowerPeak = Double.MinValue;
            double dTrend = TREND_UNKNOWN;

            while (today <= source.YoungestDate)
            {
                result[today] = dTrend;

                if (reference <= enddate)
                {
                    if (IsPeak(reference, nRange, PeakType.UPPER, source))
                    {
                        dUpperPeak = source[reference];
                    }

                    if (IsPeak(reference, nRange, PeakType.LOWER, source))
                    {
                        dLowerPeak = source[reference];
                    }
                }

                if (source[today] > dUpperPeak)
                {
                    result[today] = OUTPERFORM;
                    dTrend = CANCEL_OUTPERFORM;
                }

                if (source[today] < dLowerPeak)
                {
                    result[today] = UNDERPERFORM;
                    dTrend = CANCEL_UNDERPERFORM;
                }

                today++;
                reference++;
            }

            return result;
        }
    }
}
