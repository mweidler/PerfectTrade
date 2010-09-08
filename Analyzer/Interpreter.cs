//
// Interpreter.cs
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
using FinancialObjects;
using Indicators;

namespace Analyzer
{
    delegate void InterpretHandler(ParseToken token);

    public enum ParameterType
    {
        INTEGER = 0,
        DECIMAL = 1,
        WORKDAY = 2,
        DATASET = 3,
        VALFIX = 4,
        VALFLOAT = 5,
        STRING = 6,
        UNKNOWN = 7
    }

    public class Interpreter
    {
        private static SortedList<string, DataContainer> m_varDataContainers = new SortedList<string, DataContainer>();
        private static SortedList<string, decimal> m_varDecimals = new SortedList<string, decimal>();
        private static SortedList<string, long> m_varIntegers = new SortedList<string, long>();
        private static SortedList<string, WorkDate> m_varWorkDates = new SortedList<string, WorkDate>();
        private SortedList<string, InterpretHandler> keywords = new SortedList<string, InterpretHandler>();


        public Interpreter()
        {
            m_varDataContainers.Clear();
            keywords.Add("DATASET", DeclareDataSet);
            keywords.Add("DECIMAL", DeclareDecimal);
            keywords.Add("INTEGER", DeclareInteger);
            keywords.Add("WORKDAY", DeclareWorkday);
            keywords.Add("VALUE", DoValue);
            keywords.Add("GETVALUE", DoGetValue);
            keywords.Add("SETVALUE", DoSetValue);
            keywords.Add("ADD", DoAdd);
            keywords.Add("SUB", DoSub);
            keywords.Add("DIV", DoDiv);
            keywords.Add("PRINT", DoPrint);
        }

        public static void Error(ParseToken token, string strMessage)
        {
            System.Console.WriteLine("Line {0}: {1}", token.LineNumber, token.OriginalLine);
            System.Console.WriteLine("Script error: " + strMessage);
        }

        public static void Error(string strMessage)
        {
            System.Console.WriteLine("Script error: " + strMessage);
        }

        public static void DeclareDataSet(ParseToken token)
        {
            foreach (string strVar in token.Parameters) {
                m_varDataContainers.Add(strVar, new DataContainer());
                System.Console.WriteLine("DataSet {0} added.", strVar);
            }
        }

        public static void DeclareDecimal(ParseToken token)
        {
            foreach (string strVar in token.Parameters) {
                m_varDecimals.Add(strVar, new decimal());
                System.Console.WriteLine("Decimal {0} added.", strVar);
            }
        }

        public static void DeclareInteger(ParseToken token)
        {
            foreach (string strVar in token.Parameters) {
                m_varIntegers.Add(strVar, new long());
                System.Console.WriteLine("Integer {0} added.", strVar);
            }
        }

        public static void DeclareWorkday(ParseToken token)
        {
            foreach (string strVar in token.Parameters) {
                m_varWorkDates.Add(strVar, new WorkDate());
                System.Console.WriteLine("Workday {0} added.", strVar);
            }
        }

        public static ParameterType DetermineParameterType(string str)
        {
            if (Char.IsNumber(str[0])) {
                if (str.Contains(".")) {
                    return ParameterType.VALFLOAT;
                } else {
                    return ParameterType.VALFIX;
                }
            } else {
                if (str.StartsWith("\"") && str.EndsWith("\""))
                    return ParameterType.STRING;

                if (m_varDataContainers.ContainsKey(str))
                    return ParameterType.DATASET;
                
                if (m_varDecimals.ContainsKey(str))
                    return ParameterType.DECIMAL;
                
                if (m_varIntegers.ContainsKey(str))
                    return ParameterType.INTEGER;
                
                if (m_varWorkDates.ContainsKey(str))
                    return ParameterType.WORKDAY;
            }

            Error("Unknown parameter: " + str);
            return ParameterType.UNKNOWN;
        }

        public static void DoValue(ParseToken token)
        {
            ParameterType targetType = DetermineParameterType(token.Target);
            ParameterType paramType = DetermineParameterType(token.Parameters[0]);

            if (token.Parameters.Count > 1) {
                Error(token, "Only 1 parameter allowed.");
                return;
            }

            if (targetType == ParameterType.UNKNOWN) {
                Error(token, "Unknown Target variable " + token.Target);
                return;
            }
            
            if (paramType == ParameterType.UNKNOWN) {
                Error(token, "Unknown Value variable/type " + token.Parameters[0]);
                return;
            }
            
            if (targetType == ParameterType.INTEGER && paramType == ParameterType.INTEGER) {
                m_varIntegers[token.Target] = m_varIntegers[token.Parameters[0]];
            }

            if (targetType == ParameterType.INTEGER && paramType == ParameterType.DECIMAL) {
                m_varIntegers[token.Target] = (long)m_varIntegers[token.Parameters[0]];
            }
            
            if (targetType == ParameterType.INTEGER && paramType == ParameterType.VALFIX) {
                m_varIntegers[token.Target] = long.Parse(token.Parameters[0]);
            }

            if (targetType == ParameterType.INTEGER && paramType == ParameterType.VALFLOAT) {
                m_varIntegers[token.Target] = long.Parse(token.Parameters[0]);
            }
            
            if (targetType == ParameterType.DECIMAL && paramType == ParameterType.INTEGER) {
                m_varDecimals[token.Target] = (decimal)m_varIntegers[token.Parameters[0]];
            }
            
            if (targetType == ParameterType.DECIMAL && paramType == ParameterType.DECIMAL) {
                m_varDecimals[token.Target] = (decimal)m_varDecimals[token.Parameters[0]];
            }

            if (targetType == ParameterType.DECIMAL && paramType == ParameterType.VALFIX) {
                m_varDecimals[token.Target] = decimal.Parse(token.Parameters[0]);
            }

            if (targetType == ParameterType.DECIMAL && paramType == ParameterType.VALFLOAT) {
                m_varDecimals[token.Target] = decimal.Parse(token.Parameters[0]);
            }

            if (targetType == ParameterType.WORKDAY && paramType == ParameterType.WORKDAY) {
                m_varWorkDates[token.Target] = m_varWorkDates[token.Parameters[0]].Clone();
            }

            if (targetType == ParameterType.DATASET && paramType == ParameterType.DATASET) {
                m_varDataContainers[token.Target] = m_varDataContainers[token.Parameters[0]].Clone();
            }
        }

        public static void DoGetValue(ParseToken token)
        {
            ParameterType targetType = DetermineParameterType(token.Target);
            ParameterType param1Type = DetermineParameterType(token.Parameters[0]);
            ParameterType param2Type = DetermineParameterType(token.Parameters[1]);

            if (targetType == ParameterType.DECIMAL &&
                param1Type == ParameterType.DATASET &&
                param2Type == ParameterType.WORKDAY)
            {
                DataContainer container = m_varDataContainers[token.Parameters[0]];
                WorkDate workdateSource = m_varWorkDates[token.Parameters[1]];
                if (container.Contains(workdateSource)) {
                    m_varDecimals[token.Target] = (decimal)container[workdateSource];
                } else {
                    Error(token, "Date " + token.Parameters[1] + " not in DataSet "  + token.Parameters[0]);
                }

            } else {
                Error(token, "Parameter combination not supported.");
            }
        }

        public static void DoSetValue(ParseToken token)
        {
            ParameterType param1Type = DetermineParameterType(token.Parameters[0]);
            ParameterType param2Type = DetermineParameterType(token.Parameters[1]);
            ParameterType param3Type = DetermineParameterType(token.Parameters[2]);

            if (param1Type == ParameterType.DATASET &&
                param2Type == ParameterType.WORKDAY &&
                param3Type == ParameterType.DECIMAL)
            {
                DataContainer container = m_varDataContainers[token.Parameters[0]];
                WorkDate workdateSource = m_varWorkDates[token.Parameters[1]];
                container[workdateSource] = (double)m_varDecimals[token.Parameters[2]];
            } else {
                Error(token, "Parameter combination not supported.");
            }
        }

        public static decimal GetNumber(string strParam)
        {
            ParameterType targetType = DetermineParameterType(strParam);

            if (targetType == ParameterType.INTEGER)
                return (decimal)m_varIntegers[strParam];

            if (targetType == ParameterType.DECIMAL)
                return m_varDecimals[strParam];

            if (targetType == ParameterType.VALFIX)
                return decimal.Parse(strParam);

            if (targetType == ParameterType.VALFLOAT)
                return decimal.Parse(strParam);

            Error("Can not convert " + strParam + " to a number.");
            return 0;
        }

        public static void DoAdd(ParseToken token)
        {
            ParameterType targetType = DetermineParameterType(token.Target);
            ParameterType param1Type = DetermineParameterType(token.Parameters[0]);
            ParameterType param2Type = DetermineParameterType(token.Parameters[1]);

            if (targetType == ParameterType.INTEGER && param1Type == ParameterType.INTEGER && param2Type == ParameterType.INTEGER)
            {
                long value1 = (long)GetNumber(token.Parameters[0]);
                long value2 = (long)GetNumber(token.Parameters[1]);
                m_varIntegers[token.Target] = value1 + value2;
            }
            else if (targetType == ParameterType.DECIMAL && param1Type == ParameterType.DECIMAL && param2Type == ParameterType.DECIMAL)
            {
                decimal value1 = GetNumber(token.Parameters[0]);
                decimal value2 = GetNumber(token.Parameters[1]);
                m_varDecimals[token.Target] = value1 + value2;
            }
            else if (targetType == ParameterType.WORKDAY && param1Type == ParameterType.WORKDAY && param2Type == ParameterType.INTEGER)
            {
                int value1 = (int)GetNumber(token.Parameters[1]);
                m_varWorkDates[token.Target] = m_varWorkDates[token.Parameters[0]] + value1;
            }
            else if (targetType == ParameterType.WORKDAY && param1Type == ParameterType.WORKDAY && param2Type == ParameterType.VALFIX)
            {
                int value1 = (int)GetNumber(token.Parameters[1]);
                m_varWorkDates[token.Target] = m_varWorkDates[token.Parameters[0]] + value1;
            } else {
                Error(token, "Parameter combination not supported.");
            }
        }

        public static void DoSub(ParseToken token)
        {
            ParameterType targetType = DetermineParameterType(token.Target);
            ParameterType param1Type = DetermineParameterType(token.Parameters[0]);
            ParameterType param2Type = DetermineParameterType(token.Parameters[1]);

            if (targetType == ParameterType.INTEGER && param1Type == ParameterType.INTEGER && param2Type == ParameterType.INTEGER) {
                long value1 = (long)GetNumber(token.Parameters[0]);
                long value2 = (long)GetNumber(token.Parameters[1]);
                m_varIntegers[token.Target] = value1 - value2;
            } else if (targetType == ParameterType.DECIMAL && param1Type == ParameterType.DECIMAL && param2Type == ParameterType.DECIMAL) {
                decimal value1 = GetNumber(token.Parameters[0]);
                decimal value2 = GetNumber(token.Parameters[1]);
                m_varDecimals[token.Target] = value1 - value2;
            } else if (targetType == ParameterType.WORKDAY && param1Type == ParameterType.WORKDAY && param2Type == ParameterType.INTEGER) {
                int value1 = (int)GetNumber(token.Parameters[1]);
                m_varWorkDates[token.Target] = m_varWorkDates[token.Parameters[0]] - value1;
            } else if (targetType == ParameterType.WORKDAY && param1Type == ParameterType.WORKDAY && param2Type == ParameterType.VALFIX) {
                int value1 = (int)GetNumber(token.Parameters[1]);
                m_varWorkDates[token.Target] = m_varWorkDates[token.Parameters[0]] - value1;
            } else {
                Error(token, "Parameter combination not supported.");
            }
        }

        public static void DoDiv(ParseToken token)
        {
            ParameterType targetType = DetermineParameterType(token.Target);
            ParameterType param1Type = DetermineParameterType(token.Parameters[0]);
            ParameterType param2Type = DetermineParameterType(token.Parameters[1]);

            if (targetType == ParameterType.INTEGER && param1Type == ParameterType.INTEGER && param2Type == ParameterType.INTEGER) {
                long value1 = (long)GetNumber(token.Parameters[0]);
                long value2 = (long)GetNumber(token.Parameters[1]);
                m_varIntegers[token.Target] = value1 / value2;
            } else if (targetType == ParameterType.DECIMAL && param1Type == ParameterType.DECIMAL && param2Type == ParameterType.DECIMAL) {
                decimal value1 = GetNumber(token.Parameters[0]);
                decimal value2 = GetNumber(token.Parameters[1]);
                m_varDecimals[token.Target] = value1 / value2;
            } else {
                Error(token, "Parameter combination not supported.");
            }
        }

        public static void DoPrint(ParseToken token)
        {
            foreach (string strVar in token.Parameters) {
                if (strVar.StartsWith("\"") && strVar.EndsWith("\"")) {
                    System.Console.Write(strVar.Substring(1, strVar.Length - 2));
                } else if (m_varDecimals.ContainsKey(strVar)) {
                    System.Console.Write(m_varDecimals[strVar]);
                } else if (m_varIntegers.ContainsKey(strVar)) {
                    System.Console.Write(m_varIntegers[strVar]);
                } else if (m_varWorkDates.ContainsKey(strVar)) {
                    System.Console.Write(m_varWorkDates[strVar]);
                } else {
                    Error(token, "Unknown parameter: " + strVar);
                }
            }

            System.Console.WriteLine();
        }

        public static void DoGetQuotes(ParseToken token)
        {
            ParameterType targetType = DetermineParameterType(token.Target);
            //ParameterType param1Type = DetermineParameterType(token.Parameters[0]);
            //ParameterType param2Type = DetermineParameterType(token.Parameters[1]);

            if (targetType == ParameterType.DATASET) {

              DBEngine dbengine = DBEngine.GetInstance();
                if (dbengine.Exists(token.Parameters[0]) == false) {
                    Error(token, "Quotes of " + token.Parameters[0] + "not available.");
                    return;
                }

              Stock stock = dbengine.GetStock(token.Parameters[0]);
              m_varDataContainers[token.Target] = stock.Quotes;
            }
         }

        public void Interprete(ParseToken token)
        {
            if (token.IsUnary) {
                if (keywords.ContainsKey(token.Command)) {
                    keywords[token.Command](token);
                } else {
                    Error(token, "Unknown command: " + token.Command);
                }
            } else {
                if (token.Target != null) {
                    if (keywords.ContainsKey(token.Command)) {
                        keywords[token.Command](token);
                    } else {
                        Error(token, "Unknown command: " + token.Command);
                    }
                }
            }
        }
    }
}

