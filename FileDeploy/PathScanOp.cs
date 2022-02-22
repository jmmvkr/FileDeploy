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
        Dictionary<string, bool> MapNmSkip = new Dictionary<string, bool>();
        Dictionary<string, bool> MapExtSkip = new Dictionary<string, bool>();

        internal enum EScanPart { None = 0, File = 1, Dir = 2 }


        internal PathScanOp() { }


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
                sp.ActScan.Scan(sp);
            }
            else
            {
                if (File.Exists(rt))
                {
                    sp.Part = EScanPart.File;
                    sp.ActScan.Scan(sp);
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
