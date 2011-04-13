//
// Chart.cs
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
using System.IO;

namespace FinancialObjects
{
   /// <summary>
   /// A class that visualizes data/value key pairs.
   /// </summary>
   public class Chart
   {
      int m_nWidth = 640;
      int m_nHeight = 480;
      int[] m_aiTicsY = null;
      int m_nTicsYInterval = 0;
      int m_nMinorXTics = 1;
      int m_nLineWidth = 2;
      bool m_bShowLegend = true;
      bool m_bAutoDeleteTempFiles = true;
      bool m_bAutoDateRange = true;
      bool m_bLogScaleY = false;
      string m_strTitle = null;
      string m_strLabelY = null;
      WorkDate m_fromDate = null;
      WorkDate m_toDate = null;
      double m_dMinValue;
      double m_dMaxValue;
      List<PlotSet> m_plotsets = new List<PlotSet>();
      OutputFormat m_format = OutputFormat.PNG;

      public enum OutputFormat
      {
         PNG = 0,
         SVG = 1
      };

      public enum LineType
      {
         Undefined     = -1,
         Black         =  0,
         White         =  1,
         Gray          =  2,
         Red           =  3,
         Green         =  4,
         Blue          =  5,
         Cyan          =  6,
         Magenta       =  7,
         Yellow        =  8,
         PaleRed       =  9,
         PaleGreen     = 10,
         PaleBlue      = 11,
         PaleCyan      = 12,
         PaleMagenta   = 13,
         PaleYellow    = 14,
         PaleGray      = 15,
         LightRed      = 16,
         LightGreen    = 17,
         LightBlue     = 18,
         LightCyan     = 19,
         LightMagenta  = 20,
         LightYellow   = 21,
         LightGray     = 22,
         MediumRed     = 23,
         MediumGreen   = 24,
         MediumBlue    = 25,
         MediumCyan    = 26,
         MediumMagenta = 27,
         MediumYellow  = 28,
         MediumGray    = 29,
         HeavyRed      = 30,
         HeavyGreen    = 31,
         HeavyBlue     = 32,
         HeavyCyan     = 33,
         HeavyMagenta  = 34,
         LightOlive    = 35,
         HeavyGray     = 36,
         DeepRed       = 37,
         DeepGreen     = 38,
         DeepBlue      = 39,
         DeepCyan      = 40,
         DeepMagenta   = 41,
         Olive         = 42,
         DeepGray      = 43,
         DarkRed       = 44,
         DarkGreen     = 45,
         DarkBlue      = 46,
         DarkCyan      = 47,
         DarkMagenta   = 48,
         DarkOlive     = 49,
         DarkGray      = 50,
         Orange        = 51,
         Fuchsia       = 52,
         ChartReuse    = 53,
         SpringGreen   = 54,
         Purple        = 55,
         RoyalBlue     = 56,
         GoLong        = 100,
         GoShort       = 101
      };

      private string[] strColorCodes = { "#000000", "#FFFFFF", "#808080", "#FF0000", "#00FF00", "#0000FF",
                                         "#00FFFF", "#FF00FF", "#FFFF00", "#FFBFBF", "#BFFFBF", "#BFBFFF",
                                         "#BFFFFF", "#FFBFFF", "#FFFFBF", "#F2F2F2", "#FF8080", "#80FF80",
                                         "#8080FF", "#80FFFF", "#FF80FF", "#FFFF80", "#E5E5E5", "#FF4040",
                                         "#40FF40", "#4040FF", "#40FFFF", "#FF40FF", "#FFFF40", "#BFBFBF",
                                         "#BF0000", "#00BF00", "#0000BF", "#00BFBF", "#BF00BF", "#BFBF00",
                                         "#404040", "#800000", "#008000", "#000080", "#008080", "#800080",
                                         "#808000", "#202020", "#400000", "#004000", "#000040", "#004040",
                                         "#400040", "#404000", "#101010", "#FF8000", "#FF0080", "#80FF00",
                                         "#00FF80", "#8000FF", "#0080FF"
                                       };


      /// <summary>
      /// Creates a new chart object.
      /// </summary>
      public Chart()
      {
         Reset();
      }

      /// <summary>
      /// Removes all data drawing from the chart. All other settings like size are untouched.
      /// </summary>
      public void Clear()
      {
         m_plotsets.Clear();
         m_strTitle = null;
         m_strLabelY = null;
         m_fromDate = null;
         m_toDate = null;
         m_dMaxValue = Double.MinValue;
         m_dMinValue = Double.MaxValue;
      }

      /// <summary>
      /// Set the chart object to it's initial state and clears all modifications.
      /// </summary>
      public void Reset()
      {
         Clear();
         m_nWidth = 640;
         m_nHeight = 480;
         m_aiTicsY = null;
         m_bAutoDateRange = true;
         m_bAutoDeleteTempFiles = true;
         m_bLogScaleY = false;
         m_bShowLegend = true;
         m_nTicsYInterval = 0;
         m_nMinorXTics = 1;
         m_nLineWidth = 2;
         m_format = OutputFormat.PNG;
      }

      /// <summary>
      /// Add a new data set to the chart.
      /// </summary>
      /// <param name="data">The datacontainer that contains the data to be visualized.</param>
      /// <param name="strLabel">A label, that describes the data in the chart's legend.</param>
      public void Add(DataContainer data, string strLabel)
      {
         Add(data, LineType.Undefined, strLabel);
      }

      public void Add(DataContainer data, LineType type)
      {
         Add(data, type, "");
      }

      /// <summary>
      /// Add a new data set to the chart.
      /// </summary>
      /// <param name="data">The datacontainer that contains the data to be visualized.</param>
      /// <param name="LineType">The color of the pen used to draw this data on the chart.</param>
      /// <param name="strLabel">A label, that describes the data in the chart's legend.</param>
      public void Add(DataContainer data, LineType type, string strLabel)
      {
         PlotSet plotset = new PlotSet();

         plotset.Label = strLabel;
         plotset.Data = data;
         plotset.LineType = type;

         m_plotsets.Add(plotset);

         if (m_fromDate == null || m_fromDate > data.OldestDate)
            m_fromDate = data.OldestDate.Clone();

         if (m_toDate == null || m_toDate < data.YoungestDate)
            m_toDate = data.YoungestDate.Clone();

         foreach (WorkDate workdate in data.Dates)
         {
            if (m_dMinValue > data[workdate])
               m_dMinValue = data[workdate];

            if (m_dMaxValue < data[workdate])
               m_dMaxValue = data[workdate];
         }
      }

      /// <summary>
      /// Specifies the chart's width in pixel.
      /// </summary>
      public int Width
      {
         get { return m_nWidth; }
         set { m_nWidth = value; }
      }

      /// <summary>
      /// Specifies the chart's height in pixel.
      /// </summary>/
      public int Height
      {
         get { return m_nHeight; }
         set { m_nHeight = value; }
      }

      /// <summary>
      /// If true, the oldest and the youngest date of all data sets will be used
      /// to limit the chart's visual range.
      /// </summary>
      public bool AutoDateRange
      {
         get { return m_bAutoDateRange; }
         set { m_bAutoDateRange = value; }
      }

      /// <summary>
      /// If true, the chart's legend will be drawn.
      /// </summary>
      public bool ShowLegend
      {
         get { return m_bShowLegend; }
         set { m_bShowLegend = value; }
      }

      /// <summary>
      /// If true, the y-scale is set to logarithmic.
      /// </summary>
      public bool LogScaleY
      {
         get { return m_bLogScaleY; }
         set { m_bLogScaleY = value; }
      }

      /// <summary>
      /// Specify the chart output format.
      /// </summary>
      public OutputFormat Format
      {
         get { return m_format; }
         set { m_format = value; }
      }

      /// <summary>
      /// Specify the distance between two y-tics.
      /// </summary>
      public int TicsYInterval
      {
         get { return m_nTicsYInterval; }
         set { m_nTicsYInterval = value; }
      }

      /// <summary>
      /// If true, the temporary files (in temp-directory) for creating the chart
      /// will be deleted after the chart has been drawn.
      /// </summary>
      public bool AutoDeleteTempFiles
      {
         get { return m_bAutoDeleteTempFiles; }
         set { m_bAutoDeleteTempFiles = value; }
      }

      /// <summary>
      /// Specify the sections between two tics on the x-axis.
      /// </summary>
      public int SubSectionsX
      {
         get { return m_nMinorXTics; }
         set { m_nMinorXTics = value > 0 ? value : 1; }
      }

      /// <summary>
      /// Specify the tics shown on the y-axis.
      /// </summary>
      public int[] TicsY
      {
         get { return m_aiTicsY; }
         set { m_aiTicsY = value; }
      }

      /// <summary>
      ///Specify the label of the y-axis.
      /// </summary>
      public string LabelY
      {
         get { return m_strLabelY; }
         set { m_strLabelY = value; }
      }

      /// <summary>
      /// Specify the title of the chart.
      /// </summary>
      public string Title
      {
         get { return m_strTitle; }
         set { m_strTitle = value; }
      }

      /// <summary>
      /// Specify the left date of the chart.
      /// </summary>
      public WorkDate LeftDate
      {
         get { return m_fromDate.Clone(); }
         set { m_fromDate = value.Clone(); }
      }

      /// <summary>
      /// Specify the right date of the chart.
      /// </summary>
      public WorkDate RightDate
      {
         get { return m_toDate.Clone(); }
         set { m_toDate = value.Clone(); }
      }

      /// <summary>
      /// Specify the line width
      /// </summary>
      public int LineWidth
      {
         get { return m_nLineWidth; }
         set { m_nLineWidth = value; }
      }


      /// <summary>
      /// Create the chart with the given name.
      /// </summary>
      /// <param name="strTargetFilename">A string that specifies the full path and name of the chart./></param>
      public void Create(string strTargetFilename)
      {
         int n;
         string strTmpFilename = "unknown";

         StreamWriter sw = new StreamWriter("/tmp/plot.gpl", false);

         sw.WriteLine("# Gnuplot script file");
         sw.WriteLine("#");
         sw.WriteLine("# auto-generated by PerfectTrade");
         sw.WriteLine("# Do not modify!");
         sw.WriteLine("#");
         sw.WriteLine("");

         if (m_strTitle != null)
            sw.WriteLine("set title \"{0}\"", m_strTitle);

         switch (m_format)
         {
            case OutputFormat.PNG:
               sw.WriteLine("set term png size {0},{1}", m_nWidth, m_nHeight);
               strTmpFilename = "plot.png";
               if (strTargetFilename.ToLower().EndsWith(".png") == false)
               {
                  strTargetFilename = strTargetFilename + ".png";
               }
               break;

            case OutputFormat.SVG:
               sw.WriteLine("set term svg size {0},{1} dynamic enhanced fname 'arial' fsize 11 butt solid", m_nWidth, m_nHeight);
               strTmpFilename = "plot.svg";
               if (strTargetFilename.ToLower().EndsWith(".svg") == false)
               {
                  strTargetFilename = strTargetFilename + ".svg";
               }
               break;
         }

         sw.WriteLine("set style data lines");
         sw.WriteLine("set grid xtics ytics mytics");
         sw.WriteLine("set mxtics {0}", m_nMinorXTics);
         sw.WriteLine("set xzeroaxis lt 14 lw 2");

         if (m_bShowLegend)
            sw.WriteLine("set key below box");

         if (m_strLabelY != null)
            sw.WriteLine("set ylabel \"{0}\"", m_strLabelY);

         if (m_bLogScaleY)
            sw.WriteLine("set logscale y");

         if (m_aiTicsY != null)
         {
            sw.Write("set ytics (");

            for (int i = 0; i < m_aiTicsY.Length; i++)
            {
               if (i > 0)
                  sw.Write(",");

               sw.Write("\"{0}\" {0}", m_aiTicsY[i]);
            }

            sw.WriteLine(")");
         }

         if (m_nTicsYInterval > 0)
         {
            int nRangeStart = (int)(m_dMinValue - (m_dMinValue % m_nTicsYInterval));
            int nRangeEnd = (int)(m_dMaxValue - (m_dMaxValue % m_nTicsYInterval)) + m_nTicsYInterval;
            sw.Write("set ytics (");

            for (int i = nRangeStart; i <= nRangeEnd; i += m_nTicsYInterval)
            {
               if (i > nRangeStart)
                  sw.Write(",");

               sw.Write("\"{0}\" {0}", i);
            }

            sw.WriteLine(")");
         }

         sw.WriteLine("");
         sw.WriteLine("set style arrow 1 head back filled lt rgb \"{0}\" size screen 0.01,30.0,0.0", strColorCodes[(int)LineType.LightRed]);
         sw.WriteLine("set style arrow 2 head back filled lt rgb \"{0}\" size screen 0.01,30.0,0.0", strColorCodes[(int)LineType.LightGreen]);
         sw.WriteLine("set decimalsign locale");
         sw.WriteLine("set datafile separator \";\"");
         sw.WriteLine("");
         sw.WriteLine("set output \"{0}\"", strTmpFilename);
         sw.WriteLine("set xdata time");
         sw.WriteLine("set timefmt \"%d.%m.%Y\"");
         sw.WriteLine("set format x \"%m/%Y\"");

         if (m_bAutoDateRange)
            sw.WriteLine("set xrange [\"{0}\" : \"{1}\"]", m_fromDate, m_toDate);

         sw.WriteLine("");

         sw.Write("plot ");
         n = 1;

         foreach (PlotSet plotset in m_plotsets)
         {
            plotset.Data.Save("/tmp/gpdata" + n + ".csv", ";");

            switch (plotset.LineType)
            {
               case LineType.Undefined:
                  sw.Write("\"/tmp/gpdata{0}.csv\" using 1:2 lw {1} lt {2} title \"{3}\"", n, m_nLineWidth, n, plotset.Label);
                  break;

               case LineType.GoShort:
                  sw.Write("\"/tmp/gpdata{0}.csv\" using 1:2:(0):(-1) notitle with vectors arrowstyle 1", n);
                  break;

               case LineType.GoLong:
                  sw.Write("\"/tmp/gpdata{0}.csv\" using 1:2:(0):(1) notitle with vectors arrowstyle 2", n);
                  break;

               default:
                  sw.Write("\"/tmp/gpdata{0}.csv\" using 1:2 lw {1} lt rgb \"{2}\" title \"{3}\"", n, m_nLineWidth, strColorCodes[(int)plotset.LineType], plotset.Label);
                  break;
            }

            if (n < m_plotsets.Count)
               sw.Write(",\\");

            sw.WriteLine();
            n++;
         }

         sw.Close();

         System.Diagnostics.Process p = new System.Diagnostics.Process();
         p.StartInfo.WorkingDirectory = "/tmp/";
         p.StartInfo.FileName = "gnuplot";
         p.StartInfo.Arguments = "plot.gpl";
         p.StartInfo.UseShellExecute = false;
         p.StartInfo.CreateNoWindow = true;
         p.Start();
         p.WaitForExit();
         p.Close();

         File.Copy("/tmp/" + strTmpFilename, strTargetFilename, true);

         if (m_bAutoDeleteTempFiles)
         {
            File.Delete("/tmp/plot.gpl");
            File.Delete("/tmp/" + strTmpFilename);

            for (n = 1; n <= m_plotsets.Count; n++)
            {
               File.Delete("/tmp/gpdata" + n + ".csv");
            }
         }
      }

      /// <summary>
      /// Data structure for holding information about one drawing element.
      /// </summary>
      internal class PlotSet
      {
         private DataContainer m_data;
         private LineType m_LineType;
         private string m_strLabel;

         /// <summary>
         /// Creates a new object.
         /// </summary>
         public PlotSet()
         {
         }

         /// <summary>
         /// Contains the data container to be drawn.
         /// </summary>
         public DataContainer Data
         {
            get { return m_data; }
            set { m_data = value; }
         }

         /// <summary>
         /// Specifies the line-type (color) of the drawing line.
         /// </summary>
         public LineType LineType
         {
            get { return m_LineType; }
            set { m_LineType = value; }
         }

         /// <summary>
         /// The description label of the drawing line.
         /// </summary>
         public string Label
         {
            get { return m_strLabel; }
            set { m_strLabel = value; }
         }
      }
   }
}
