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

namespace MMMWSaper
{
    /// <summary>
    /// Logika interakcji dla klasy SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            boardHeightSlider.Value = Properties.Settings.Default.BoardHeight;
            boardWidthSlider.Value = Properties.Settings.Default.BoardWidth;
            bombCountSlider.Maximum = Math.Floor(boardWidthSlider.Value * boardHeightSlider.Value / 2);
            bombCountSlider.Value = Properties.Settings.Default.BombsCount;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            bombCountSlider.Maximum = Math.Floor(boardWidthSlider.Value * boardHeightSlider.Value / 2);
        }
    }
}
