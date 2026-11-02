using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    internal class Program
    {
        /// <summary> Коллекция рекордов
        public static List<Leaders> Leaders = new List<Leaders>();
        /// <summary> Коллекция ViewModelUserSetting, содержащая IP адрес игрока, порт, ...
        public static List<ViewModelUserSettings> remoteIPAddress = new List<ViewModelUserSettings>();
        /// <summary> Коллекция ViewModelGames, содержащая точки змеи, точку на карте
        public static List<ViewModelGames> viewModelGames = new List<ViewModelGames>();
        /// <summary> Локальный порт, который прослушивается для ответов
        private static int localPort = 5001;
        /// <summary> Максимальная скорость движения змейки
        public static int MaxSpeed = 15;
        static void Main(string[] args)
        {

        }
    }
}
