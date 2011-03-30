//
// World.cs
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
using System.IO;
using System.Text;

namespace FinancialObjects
{
   public class World
   {
      private static World m_WorldObject = null;
      private string m_strBasePath = null;
      private string m_strQuotesPath = null;
      private string m_strDataPath = null;
      private string m_strResultPath = null;

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

      public void SetWorldPaths(string strApplicationName)
      {
         string strBasePath = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
         string strResultPath = strBasePath + "/PerfectTrade/Results/" + strApplicationName + "/";
         string strDataPath = strBasePath + "/PerfectTrade/Data/" + strApplicationName + "/";
         string strQuotesPath = strBasePath + "/PerfectTrade/Quotes/";

         if (Directory.Exists(strResultPath) == false)
         {
            Directory.CreateDirectory(strResultPath);
         }

         if (Directory.Exists(strDataPath) == false)
         {
            Directory.CreateDirectory(strDataPath);
         }

         if (Directory.Exists(strQuotesPath) == false)
         {
            Directory.CreateDirectory(strQuotesPath);
         }

         m_strBasePath   = strBasePath;
         m_strResultPath = strResultPath;
         m_strDataPath   = strDataPath;
         m_strQuotesPath = strQuotesPath;
      }

      public string BasePath {
         get { return m_strBasePath; }
         //set { m_strQuotesPath = value; }
      }

      public string QuotesPath
      {
         get { return m_strQuotesPath; }
         //set { m_strQuotesPath = value; }
      }

      public string DataPath
      {
         get { return m_strDataPath; }
         //set { m_strDataPath = value; }
      }

      public string ResultPath
      {
         get { return m_strResultPath; }
         //set { m_strResultPath = value; }
      }
   }
}
