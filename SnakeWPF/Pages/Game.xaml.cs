using Common;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SnakeWPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для Game.xaml
    /// </summary>
    public partial class Game : Page
    {
        public int StepCadr = 0;
        public DateTime _lastUpdateTime = DateTime.Now;
        public Game()
        {
            InitializeComponent();
        }

        public void CreateUI(List<ViewModelGames> AllSnakes, List<ViewModelGames> PreviousSnakes)
        {
            Dispatcher.Invoke(() =>
            {
                if (StepCadr == 0) StepCadr = 1;
                else StepCadr = 0;

                double progress = (DateTime.Now - _lastUpdateTime).TotalMilliseconds / 100.0;
                if(progress > 1.0)
                    progress = 1.0;

                canvas.Children.Clear();

                foreach(var snake in AllSnakes)
                {
                    var prevSnake = PreviousSnakes?.FirstOrDefault(x => x.IdSnake == snake.IdSnake);

                    for (int iPoint = snake.SnakesPlayers.Points.Count - 1; iPoint >= 0; iPoint--)
                    {
                        Snakes.Point SnakePoint = snake.SnakesPlayers.Points[iPoint];

                        double drawX = SnakePoint.X;
                        double drawY = SnakePoint.Y;

                        if(prevSnake != null && iPoint < prevSnake.SnakesPlayers.Points.Count)
                        {
                            var prevPoint = prevSnake.SnakesPlayers.Points[iPoint];
                            drawX = prevPoint.X + (SnakePoint.X - prevPoint.X) * progress;
                            drawY = prevPoint.Y + (SnakePoint.Y - prevPoint.Y) * progress;
                        }

                        if (iPoint != 0)
                        {
                            Snakes.Point NextSnakePoint = snake.SnakesPlayers.Points[iPoint - 1];

                            if (SnakePoint.X > NextSnakePoint.X || SnakePoint.X < NextSnakePoint.X)
                            {
                                if (iPoint % 2 == 0)
                                {
                                    if (StepCadr % 2 == 0)
                                        SnakePoint.Y -= 1;
                                    else
                                        SnakePoint.Y += 1;
                                }
                                else
                                {
                                    if (StepCadr % 2 == 0)
                                        SnakePoint.Y += 1;
                                    else
                                        SnakePoint.Y -= 1;
                                }
                            }
                            else if (SnakePoint.Y > NextSnakePoint.Y || SnakePoint.Y < NextSnakePoint.Y)
                            {
                                if (iPoint % 2 == 0)
                                {
                                    if (StepCadr % 2 == 0)
                                        SnakePoint.X -= 1;
                                    else
                                        SnakePoint.X += 1;
                                }
                                else
                                {
                                    if (StepCadr % 2 == 0)
                                        SnakePoint.X += 1;
                                    else
                                        SnakePoint.X -= 1;
                                }
                            }
                        }
                        Brush Color;
                        if(snake.IdSnake == MainWindow.mainWindow.ViewModelUserSettings.IdSnake)
                        {
                            if (iPoint == 0)
                                Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 127, 14));
                            else
                                Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 198, 19));
                        }
                        else
                        {
                            if (iPoint == 0)
                                Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 0, 0, 150));
                            else
                                Color = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 50, 50, 255));
                        }

                        Ellipse ellipse = new Ellipse()
                        {
                            Width = 20,
                            Height = 20,
                            Margin = new Thickness(SnakePoint.X - 10, SnakePoint.Y - 10, 0, 0),
                            Fill = Color,
                            Stroke = Brushes.Black
                        };
                        canvas.Children.Add(ellipse);
                    }                    
                }
                
                var mySnake = AllSnakes.FirstOrDefault(x => x.IdSnake == MainWindow.mainWindow.ViewModelUserSettings.IdSnake);
                if (mySnake != null)
                {
                    ImageBrush myBrush = new ImageBrush();
                    myBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/apple.png"));
                    Ellipse points = new Ellipse()
                    {
                        Width = 40,
                        Height = 40,
                        Margin = new Thickness(
                            mySnake.Points.X - 20,
                            mySnake.Points.Y - 20, 0, 0),
                        Fill = myBrush
                    };
                    canvas.Children.Add(points);
                }
            });
        }
    }
}
