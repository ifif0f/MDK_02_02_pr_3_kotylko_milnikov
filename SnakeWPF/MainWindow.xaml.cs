using Common;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SnakeWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow mainWindow;
        public ViewModelUserSettings ViewModelUserSettings = new ViewModelUserSettings();
        public ViewModelGames ViewModelGames = null;
        public static IPAddress remoteIPAddress = IPAddress.Parse("10.49.1.215");
        public static int remotePort = 5001;
        public Thread tRec;
        public UdpClient receivingUdpClient;
        public CancellationTokenSource cts = new CancellationTokenSource();
        public Pages.Home Home = new Pages.Home();
        public Pages.Game Game = new Pages.Game();

        public List<ViewModelGames> AllSnakes = null;
        public List<ViewModelGames> PreviousSnakes = null;
        public bool isGameOpened = false;
        public DispatcherTimer renderTimer;

        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;
            OpenPage(Home);

            renderTimer = new DispatcherTimer();
            renderTimer.Interval = TimeSpan.FromMilliseconds(16);
            renderTimer.Tick += (s, e) =>
            {
                if(AllSnakes != null && isGameOpened)
                {
                    Game.CreateUI(AllSnakes, PreviousSnakes);
                }
            };
            renderTimer.Start();
        }

        public void StartReceiver()
        {
            tRec = new Thread(new ThreadStart(Receiver));
            tRec.Start();
        }

        public void OpenPage(Page PageOpen)
        {
            DoubleAnimation startAnimation = new DoubleAnimation();
            startAnimation.From = 1;
            startAnimation.To = 0;
            startAnimation.Duration = TimeSpan.FromSeconds(0.6);
            startAnimation.Completed += delegate
            {
                frame.Navigate(PageOpen);
                DoubleAnimation endAnimation = new DoubleAnimation();
                endAnimation.From = 0;
                endAnimation.To = 1;
                endAnimation.Duration = TimeSpan.FromSeconds(0.6);
                frame.BeginAnimation(OpacityProperty, endAnimation);
            };
            frame.BeginAnimation(OpacityProperty, startAnimation);
        }

        public void Receiver()
        {
            receivingUdpClient = new UdpClient(int.Parse(ViewModelUserSettings.Port));
            IPEndPoint RemoteIpEndPoint = null;

            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    byte[] receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);
                    string returnData = Encoding.UTF8.GetString(receiveBytes);

                    if(returnData.StartsWith("MyIdSnake|"))
                    {
                        var data = returnData.Split("|");
                        ViewModelUserSettings.IdSnake = int.Parse(data[1]);
                        continue;
                    }

                    //if (ViewModelGames == null) // !(frame.Content is Pages.Game)
                    //{
                    //    Dispatcher.Invoke(() =>
                    //    {
                    //        OpenPage(Game);
                    //    });
                    //}
                    if (!isGameOpened)
                    {
                        isGameOpened = true;
                        Dispatcher.Invoke(() =>
                        {
                            OpenPage(Game);
                        });
                    }

                    if (AllSnakes != null)
                    {
                        PreviousSnakes = AllSnakes;
                    }

                    //ViewModelGames = JsonConvert.DeserializeObject<ViewModelGames>(returnData.ToString());
                    AllSnakes = JsonConvert.DeserializeObject<List<ViewModelGames>>(returnData);

                    Game._lastUpdateTime = DateTime.Now;

                    //if (ViewModelGames.SnakesPlayers.GameOver)
                    //{
                    //    Dispatcher.Invoke(() =>
                    //    {
                    //        OpenPage(new Pages.EndGame());
                    //    });
                    //}

                    var mySnake = AllSnakes.FirstOrDefault(x => x.IdSnake == ViewModelUserSettings.IdSnake);
                    if (mySnake != null && mySnake.SnakesPlayers.GameOver)
                    {
                        ViewModelGames = mySnake;
                        Dispatcher.Invoke(() =>
                        {
                            OpenPage(new Pages.EndGame());
                        });
                        isGameOpened = false;
                    }
                    //else
                    //{
                    //    //Game.CreateUI();
                    //    Game.CreateUI(AllSnakes);
                    //}
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Возникло исключнение: " + ex.ToString() + "\n " + ex.Message);
            }
        }

        public static void Send(string datagram)
        {
            UdpClient sender = new UdpClient();
            IPEndPoint endPoint = new IPEndPoint(remoteIPAddress, remotePort);
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(datagram);
                sender.Send(bytes, bytes.Length, endPoint);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Возникло исключнение: " + ex.ToString() + "\n " + ex.Message);
            }
            finally
            {
                sender.Close();
            }
        }

        private void EventKeyUp(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(ViewModelUserSettings.IPAddress) &&
                !string.IsNullOrEmpty(ViewModelUserSettings.Port))  //&& (ViewModelGames != null && !ViewModelGames.SnakesPlayers.GameOver)
            { 
                if (e.Key == Key.Up)
                    Send($"Up|{JsonConvert.SerializeObject(ViewModelUserSettings)}");
                else if (e.Key == Key.Down)
                    Send($"Down|{JsonConvert.SerializeObject(ViewModelUserSettings)}");
                else if (e.Key == Key.Left)
                    Send($"Left|{JsonConvert.SerializeObject(ViewModelUserSettings)}");
                else if (e.Key == Key.Right)
                    Send($"Right|{JsonConvert.SerializeObject(ViewModelUserSettings)}");
            }
        }

        public void QuitApplication(object sender, System.ComponentModel.CancelEventArgs e)
        {
            receivingUdpClient.Close();

            if (tRec != null)
            {
                cts.Cancel();
            }
        }
    }
}