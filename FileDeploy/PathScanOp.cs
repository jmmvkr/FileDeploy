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

        internal Action<ScanParam> ActScan;


        internal PathScanOp()
        {
            ActScan = PrintScan;
        }

        void PrintScan(ScanParam sp)
        {
            var p = sp.Part;
            var lv = sp.Level;
            var rt = sp.PathIn;

            if (EScanPart.File == p)
            {
                PrintTab(lv);
                Console.WriteLine("f{1} {0}", rt, lv);
            }
            else if (EScanPart.Dir == p)
            {
                PrintTab(lv);
                Console.WriteLine("d{1} {0}", rt, lv);
            }
        }

        internal void AddPresetDev()
        {
            AddNameSkip(".git");
            AddNameSkip(".vs");
            AddNameSkip("Debug");
            AddNameSkip("bin");
            AddNameSkip("obj");
            AddExtSkip(".user");
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


        internal void ScanDir(string rt)
        {
            ScanThisDir(rt, 0);
        }

        void ScanThisDir(string rt, int lv)
        {
            ScanParam sp;
            sp.PathIn = rt;
            sp.Level = lv;
            if (Directory.Exists(rt))
            {
                sp.Part = EScanPart.Dir;
                ActScan(sp);
            }
            else
            {
                if (File.Exists(rt))
                {
                    sp.Part = EScanPart.File;
                    ActScan(sp);
                }
                return;
            }

            var dirs = Directory.GetDirectories(rt);
            var files = Directory.GetFiles(rt);
            foreach (var d in dirs)
            {
                var di = new DirectoryInfo(d);
                if (MapNmSkip.ContainsKey(di.Name)) { continue; }

                ScanThisDir(d, (1 + lv));
            }
            foreach (var f in files)
            {
                var fi = new DirectoryInfo(f);
                var nm = fi.Name;
                int pos = nm.LastIndexOf('.');

                if (pos >= 0)
                {
                    var ext = nm.Substring(pos);
                    if (MapExtSkip.ContainsKey(ext))
                    {
                        continue;
                    }
                }
                ScanThisDir(f, (1 + lv));
            }
        }

        void PrintTab(int lv)
        {
            for (int i = 0; i < lv; i++)
            {
                Console.Write('\t');
            }
        }

        internal struct ScanParam
        {
            internal EScanPart Part;
            internal string PathIn;
            internal int Level;
        }

    } // end - class PathScanOp
}
