using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDeploy
{
    internal class PathScanOp
    {
        internal enum EScanPart { None = 0, File = 1, Dir = 2 }
        internal delegate bool FilterOp(ref ScanParam sp);

        Dictionary<string, bool> MapNmSkip = new Dictionary<string, bool>();
        Dictionary<string, bool> MapExtSkip = new Dictionary<string, bool>();
        internal FilterOp Filter;


        internal PathScanOp() { Filter = AcceptAll; }

        internal void AddPresetDev()
        {
            // C#
            AddNameSkip(".git");
            AddNameSkip(".vs");
            AddNameSkip("Debug");
            AddNameSkip("bin");
            AddNameSkip("obj");
            AddExtSkip(".user");

            // VC++
            AddNameSkip("ipch");
            AddExtSkip(".sdf");
        }

        internal void AddNameSkip(params string[] nmSkip)
        {
            foreach (var nm in nmSkip)
            {
                MapNmSkip[nm] = true;
            }
        }

        internal void AddExtSkip(params string[] extSkip)
        {
            foreach (var ext in extSkip)
            {
                if (!ext.StartsWith("."))
                {
                    throw new ArgumentException("invalid ext: " + ext);
                }
                MapExtSkip[ext] = true;
            }
        }

        public static bool AcceptAll(ref ScanParam sp)
        {
            return true;
        }

        internal void ScanDir(string rt, IScan scan)
        {
            ScanParam spRoot = spEmpty;
            spRoot.PathIn = rt;
            spRoot.ActScan = scan;
            ScanDir(rt, spRoot);
        }


        void ScanDir(string rt, ScanParam spRoot)
        {
            ScanThisDir(rt, 0, ref spRoot);
        }

        void ScanThisDir(string rt, int lv, ref ScanParam spOuter)
        {
            ScanParam sp = spOuter;
            sp.PathIn = rt;
            sp.Level = lv;
            if (Directory.Exists(rt))
            {
                sp.Part = EScanPart.Dir;
                if (Filter(ref sp))
                {
                    sp.ActScan.Scan(sp);
                }
            }
            else
            {
                if (File.Exists(rt))
                {
                    sp.Part = EScanPart.File;
                    if (Filter(ref sp))
                    {
                        sp.ActScan.Scan(sp);
                    }
                }
                return;
            }

            var dirs = Directory.GetDirectories(rt).Where(FilterDir).ToArray();
            var files = Directory.GetFiles(rt).Where(FilterExt).ToArray();

            int lenAccu = dirs.Length + files.Length;
            int idxAccu = 0;
            for (int i = 0, len = dirs.Length; i < len;  i++)
            {
                var d = dirs[i];

                ScanParam spDir = sp;
                spDir.AccuLen = lenAccu;
                spDir.AccuIndex = idxAccu++;
                ScanThisDir(d, (1 + lv), ref spDir);
            }
            for (int i = 0, len = files.Length; i < len; i++)
            {
                var f = files[i];

                ScanParam spFile = sp;
                spFile.AccuLen = lenAccu;
                spFile.AccuIndex = idxAccu++;
                ScanThisDir(f, (1 + lv), ref spFile);
            }
        }


        bool FilterDir(string d)
        {
            var di = new DirectoryInfo(d);
            if (MapNmSkip.ContainsKey(di.Name)) { return false; }
            return true;
        }

        bool FilterExt(string f)
        {
            var fi = new FileInfo(f);
            var nm = fi.Name;
            int pos = nm.LastIndexOf('.');
            if (pos >= 0)
            {
                var ext = nm.Substring(pos);
                if (MapExtSkip.ContainsKey(ext)) { return false; }
            }
            return true;
        }

        internal static bool InToday(ref ScanParam sp)
        {
            var today = DateTime.Now.Date;
            return InDayRange(ref sp, today, 1.0);
        }

        internal static bool InLastDays(ref ScanParam sp, int nDays)
        {
            var today = DateTime.Now.Date;
            var tmEnd = today.AddDays(1.0);
            var tmStart = tmEnd.AddDays(-nDays);
            return InDayRange(ref sp, tmStart, tmEnd);
        }

        internal static bool InDayRange(ref ScanParam sp, DateTime tmStart, double offEnd)
        {
            return InDayRange(ref sp, tmStart, tmStart.AddDays(offEnd));
        }

        internal static bool InDayRange(ref ScanParam sp, DateTime tmStart, DateTime tmEnd)
        {
            if (PathScanOp.EScanPart.Dir == sp.Part)
            {
                return true;
            }
            if (PathScanOp.EScanPart.File == sp.Part)
            {
                var tm = File.GetCreationTime(sp.PathIn);
                if (tmStart <= tm && tm < tmEnd)
                {
                    return true;
                }
            }
            return false;
        }


        static readonly ScanParam spEmpty = new ScanParam();

        internal struct ScanParam
        {
            internal EScanPart Part;
            internal string PathIn;
            internal int Level;
            internal IScan ActScan;
            internal int AccuLen;
            internal int AccuIndex;
        }

    } // end - class PathScanOp
}
