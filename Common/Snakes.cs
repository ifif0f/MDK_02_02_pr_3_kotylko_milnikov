using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Snakes
    {
        /// <summary> Класс Point
        public class Point
        {
            /// <summary> Координата X
            public int X { get; set; }
            /// <summary> Координата Y
            public int Y { get; set; }

            public Point(int X, int Y)
            {
                this.X = X;
                this.Y = Y;
            }
            public Point() { }
        }

        /// <summary> Направление движения змеи
        public enum Direction
        {
            Left,
            Right,
            Up,
            Down,
            Start
        }

        /// <summary> Точки из которых состоит змея
        public List<Point> Points = new List<Point>();
        /// <summary> Направление движения в котором двигается змея
        public Direction direction = Direction.Start;
        /// <summary> Переменная говорящая о том, что игрок закончил игру
        public bool GameOver = false;
    }
}
