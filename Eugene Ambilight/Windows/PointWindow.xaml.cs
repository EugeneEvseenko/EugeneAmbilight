using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Eugene_Ambilight.Windows
{
    /// <summary>
    /// Interaction logic for Point.xaml
    /// </summary>
    public partial class PointWindow : Window
    {
        public PointWindow(int number)
        {
            InitializeComponent();
            LedNumber.Content = number.ToString();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }
    }
}
