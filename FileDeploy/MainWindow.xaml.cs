using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileDeploy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [Flags]
        internal enum Ops { None = 0, Print = 0x1, PrintTree = 0x2, Deploy = 0x4, CsCompile = 0x08 }
        internal const Ops AllOp = Ops.Print | Ops.PrintTree | Ops.Deploy;

        PathScanOp sc = new PathScanOp();


        public MainWindow()
        {
            InitializeComponent();
            SysInit();
            this.Loaded += MainWindow_Loaded;
        }

        private void SysInit()
        {
            sc.AddPresetDev();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            txtPathSrc.Text = @"C:\Users\jmmvk\source\repos\FileDeploy";
            txtPathDst.Text = @"X:\";
        }

        private void btnDeploy_Click(object sender, RoutedEventArgs e)
        {
            string rt = txtPathSrc.Text;
            string dstRt = txtPathDst.Text.TrimEnd('\\');
            string nm = new FileInfo(rt).Name;
            string dst = $@"{dstRt}\{nm}";

            try
            {
                RunSamples(rt, dst, AllOp);
            }
            catch (Exception ex)
            {
                uiOnError(ex);
            }
        }

        void uiOnError(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ex.Message);
            sb.AppendLine();
            sb.AppendLine(ex.StackTrace);
            MessageBox.Show(sb.ToString(), ex.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        internal void RunSamples(string rt, string dst, Ops ops)
        {
            if ((Ops.Print & ops) != Ops.None)
            {
                IScan op1 = new PathPrint();
                sc.ScanDir(rt, op1);
            }
            if ((Ops.PrintTree & ops) != Ops.None)
            {
                IScan op2 = new PathTreePrinter();
                sc.ScanDir(rt, op2);
            }
            if ((Ops.Deploy & ops) != Ops.None)
            {
                PathDeploy.RunDeploy(sc, rt, dst);
            }
            if ((Ops.CsCompile & ops) != Ops.None)
            {
                sc.ScanDir(rt, new PathCsCompile());
            }
        }

    } // end - class MainWindow
}
