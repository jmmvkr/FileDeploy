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
        PathScanOp sc = new PathScanOp();

        public MainWindow()
        {
            InitializeComponent();
            SysInit();
        }

        private void SysInit()
        {
            sc.AddPresetDev();
        }

        private void btnDeploy_Click(object sender, RoutedEventArgs e)
        {
            string rt = @"C:\Users\jmmvk\source\repos\ArduinoFakeUI";
            string dst = @"x:\ArduinoFakeUI";

            PathDeploy.RunDeploy(sc, rt, dst);
        }

    } // end - class MainWindow
}
