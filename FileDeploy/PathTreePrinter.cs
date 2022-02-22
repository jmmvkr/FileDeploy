using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDeploy
{
    internal class PathTreePrinter : IScan
    {
        const string cBrh = "├- ";
        const string cEnd = "└- ";
        const string cBar = " │  ";
        const string cPad = "    ";

        StringBuilder sb = new StringBuilder();
        TextWriter wr;


        internal PathTreePrinter() : this(Console.Out)
        {
        }

        internal PathTreePrinter(TextWriter writer)
        {
            wr = writer;
        }


        public void Scan(PathScanOp.ScanParam sp)
        {
            bool bDir = (sp.Part == PathScanOp.EScanPart.Dir);
            bool bTopLevel = (0 == sp.Level);
            bool bIconRooted = (sp.AccuIndex < (sp.AccuLen - 1)) || bTopLevel;
            bool bBar = false;
            string strLead = bIconRooted ? cBrh : cEnd;
            string nm;

            if (bTopLevel)
            {
                sb.Clear();
            }
            else
            {
                sb.Length = 4 * sp.Level;
                bBar = bIconRooted;
            }

            string strBars = sb.ToString();
            if (bDir)
            {
                nm = new DirectoryInfo(sp.PathIn).Name;
                wr.WriteLine(" {0} {1} {2}", strBars, strLead, nm);
            }
            else
            {
                nm = new FileInfo(sp.PathIn).Name;
                wr.WriteLine(" {0} {1} {2}", strBars, strLead, nm);
            }

            sb.Append(bBar ? cBar : cPad);
        }

    } // end - class PathTreePrinter
}
