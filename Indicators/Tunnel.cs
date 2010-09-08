/*
 * Tunnel.cs
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
    public class Tunnel
    {
        public static DataContainer CreateFrom(DataContainer source, int nRange)
        {
            DataContainer result = new DataContainer();

            WorkDate workdate = new WorkDate(source.OldestDate+nRange);
            for (; workdate <= source.YoungestDate; workdate++ )
            {
                WorkDate startdate = new WorkDate(workdate - nRange);
                WorkDate enddate = new WorkDate(workdate);
                double dMax = Double.MinValue;
                double dMin = Double.MaxValue;

                for (; startdate <= enddate && startdate <= source.YoungestDate; startdate++)
                {
                    double dValue = source[startdate];
                    if (dValue > dMax)
                        dMax = dValue;
                    if (dValue < dMin)
                        dMin = dValue;
                }

                result[workdate] = dMax - dMin;
            }

            return result;
        }
    }
}
