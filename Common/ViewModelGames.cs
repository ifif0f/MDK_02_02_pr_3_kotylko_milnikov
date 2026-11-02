using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ViewModelGames
    {
        /// <summary> Змея персонажа
        public Snakes SnakesPlayers = new Snakes();
        /// <summary> Точка на карте
        public Snakes.Point Points = new Snakes.Point();
        /// <summary> Место игрока
        public int Top = 0;
        /// <summary> Код змеи
        public int IdSnake { get; set; }
    }
}
