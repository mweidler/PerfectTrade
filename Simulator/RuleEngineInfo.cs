//
// RuleEngineInfo.cs
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

namespace Simulator
{
   public class RuleEngineInfo
   {
      private readonly WorkDate m_FromDate = new WorkDate(WorkDate.MinDate);
      private readonly WorkDate m_ToDate = new WorkDate(WorkDate.MinDate);
      private readonly Variants m_variants = new Variants();
      private readonly Depot    m_depot = new Depot();
      private readonly WorkDate m_today = new WorkDate();

      private int m_targetpositions = 10;
      private double m_minimuminvestment = 5000.0;
      private double m_maxloss = 0.1;

      public RuleEngineInfo()
      {
      }

      public WorkDate FromDate
      {
         get { return m_FromDate; }
         set { m_FromDate.Set(value.Year, value.Month, value.Day); }
      }

      public WorkDate ToDate
      {
         get { return m_ToDate; }
         set { m_ToDate.Set(value.Year, value.Month, value.Day); }
      }

      public Variants Variants
      {
         get { return m_variants; }
      }

      public Depot Depot
      {
         get { return m_depot; }
      }

      public WorkDate Today
      {
         get { return m_today; }
         set { m_today.Set(value.Year, value.Month, value.Day); }
      }

      public int TargetPositions
      {
         get { return m_targetpositions; }
         set { m_targetpositions = value; }
      }

      public double MinimumInvestment
      {
         get { return m_minimuminvestment; }
         set { m_minimuminvestment = value; }
      }

      public double MaxLoss
      {
         get { return m_maxloss; }
         set { m_maxloss = value; }
      }
   }
}
