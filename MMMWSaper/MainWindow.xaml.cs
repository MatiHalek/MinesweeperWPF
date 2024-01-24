using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MMMWSaper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<Cell> Cells = [];
        private readonly List<Button> Buttons = [];
        private UniformGrid? Grid;
        private readonly SolidColorBrush[] Colors = [Brushes.DodgerBlue, Brushes.Green, Brushes.Red, Brushes.DarkBlue, Brushes.DarkRed, Brushes.DarkCyan, Brushes.Black, Brushes.Gray];
        private readonly DispatcherTimer Timer = new();
        private bool IsPlaying = false;
        private uint seconds = 0;
        private uint BoardWidth = 8;
        private uint BoardHeight = 8;
        private uint BombsCount = 10;
        public MainWindow()
        {
            InitializeComponent();
            Timer.Tick += Timer_Tick;
            Timer.Interval = new TimeSpan(0, 0, 1);
            StartGame();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            seconds++;
            timerLabel.Content = seconds;
        }
        private void StartGame()
        {
            BoardWidth = Properties.Settings.Default.BoardWidth;
            BoardHeight = Properties.Settings.Default.BoardHeight;
            BombsCount = Properties.Settings.Default.BombsCount;
            itemsControl.ItemsSource = null;
            if (Grid is not null)
                Grid.Columns = (int)BoardWidth;
            Buttons.Clear();
            Cells.Clear();
            List<uint> numbers = [];
            for(uint i = 0; i < (BoardWidth * BoardHeight); i++)
            {
                numbers.Add(i);
                Cells.Add(new Cell(i % BoardWidth, i / BoardWidth, false, false, false));
            }
            itemsControl.ItemsSource = Cells;
            for(uint i = 0; i < BombsCount; i++)
            {
                Random r = new();
                uint bomb = numbers[r.Next(0, numbers.Count)];
                Cells[(int)bomb].IsBomb = true;
                numbers.Remove(bomb);
            }
            Timer.Start();
            timerLabel.Content = 0;
            bombsLabel.Content = BombsCount;
            seconds = 0;
            IsPlaying = true;
        }
        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settings = new();
            settings.ShowDialog();
        }

        private void Button_Loaded(object sender, RoutedEventArgs e)
        {
            Buttons.Add((Button)sender);
        }

        private void Button_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsPlaying)
                return;
            Button b = (Button)sender;
            Cell c = (Cell)b.CommandParameter;
            if (c.IsRevealed)
                return;
            c.IsFlagged = !c.IsFlagged;
            if (c.IsFlagged)
                b.Foreground = Brushes.Red;
            b.Content = c.IsFlagged ? "🚩" : string.Empty;
            bombsLabel.Content = BombsCount - Cells.Where(cell => cell.IsFlagged).Count();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!IsPlaying)
                return;
            Button b = (Button)sender;
            Cell c = (Cell)b.CommandParameter;
            if (c.IsRevealed || c.IsFlagged)
                return;
            c.IsRevealed = true;
            b.Background = Brushes.LightBlue;
            if(c.IsBomb)
            {
                Timer.Stop();
                List<Cell> bombs = Cells.Where(cell => cell.IsBomb).ToList();    
                foreach (Cell cell in bombs)
                {
                    Button button = Buttons[(int)(cell.PositionY * BoardWidth + cell.PositionX)];
                    button.Foreground = Brushes.Black;
                    button.Background = Brushes.Red;
                    button.Content = "💣";
                }
                Properties.Settings.Default.GamesPlayed++;
                Properties.Settings.Default.Save();
                MessageBox.Show($"Niestety, ale ta gra kończy się Twoją przegraną.\n\nRozegrane gry: {Properties.Settings.Default.GamesPlayed}\nWygrane gry: {Properties.Settings.Default.GamesWon}\nProcent wygranych: {Math.Round((float)Properties.Settings.Default.GamesWon / Properties.Settings.Default.GamesPlayed * 100)}%", "Porażka", MessageBoxButton.OK, MessageBoxImage.Information);
                IsPlaying = false;
                return;
            }
            List<Cell> nearestBombs = Cells.Where(cell => cell.IsBomb && Math.Abs((int)c.PositionX - (int)cell.PositionX) <= 1 && Math.Abs((int)c.PositionY - (int)cell.PositionY) <= 1).ToList();
            if(nearestBombs.Count > 0 )
            {
                b.Foreground = Colors[nearestBombs.Count - 1];
                b.Content = nearestBombs.Count;
            }
            if(Cells.Where(cell => !cell.IsRevealed).Count() == BombsCount)
            {
                Timer.Stop();
                Properties.Settings.Default.GamesPlayed++;
                Properties.Settings.Default.GamesWon++;
                Properties.Settings.Default.Save();
                MessageBox.Show($"Gratulacje, wygrana!.\n\nRozegrane gry: {Properties.Settings.Default.GamesPlayed}\nWygrane gry: {Properties.Settings.Default.GamesWon}\nProcent wygranych: {Math.Round((float)Properties.Settings.Default.GamesWon / Properties.Settings.Default.GamesPlayed * 100)}%", "Wygrana", MessageBoxButton.OK, MessageBoxImage.Information);
                IsPlaying = false;
            }

        }

        private void UniformGrid_Loaded(object sender, RoutedEventArgs e)
        {
            Grid = (UniformGrid)sender;
            Grid.Columns = (int)BoardWidth;
        }
    }
}