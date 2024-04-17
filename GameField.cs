using System;
using System.Diagnostics;
using System.Threading;

namespace Conways_Game_of_Life
{
    public class GameField
    {
        public EventHandler<string> RedrawEvent;
        private bool[,] FutureField { get; set; }
        public bool[,] Field { get; set; }
        private bool[,] Rules { get; set; }

        public bool IsPaused { get; set; }

        public GameField(int fieldWidth, int fieldHeight)
        {
            Field = new bool[fieldHeight, fieldWidth];
            FutureField = new bool[fieldHeight, fieldWidth];
            Rules = new bool[2, 9];
            Rules[0, 3] = true; // 1 - 3
            Rules[1, 2] = true; // 2 - 2
            Rules[1, 3] = true; // 2 - 3
            IsPaused = true;
        }

        public void SetRules(string ruleString)
        {
            int value = 0;
            Rules = new bool[2, 9];
            foreach (var character in ruleString)
            {
                if (character == '/')
                {
                    value = 1;
                    continue;
                }
                var index = int.Parse(character + "");
                if (index is < 9 and >= 0)
                    Rules[value, index] = true;
            }
        }

        public void RiseRedrawEvent()
        {
            RedrawEvent?.Invoke(this, "Redraw");
        }

        public void LoopCalculations()
        {
            while (true)
                if (!IsPaused)
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    CalcNextState();
                    stopwatch.Stop();
                    if (1000 - stopwatch.ElapsedMilliseconds > 0)
                        Thread.Sleep((int)(1000 - stopwatch.ElapsedMilliseconds));
                    RiseRedrawEvent();
                }
        }

        public void CalcNextState()
        {
            FutureField = new bool[Field.GetLength(0), Field.GetLength(1)];
            for (var y = 0; y < Field.GetLength(0); y++)
            for (var x = 0; x < Field.GetLength(1); x++)
            {
                var aliveNeighbours = 0;
                for (var i = -1; i <= 1; i++)
                {
                    var tmpI = i;
                    //Y Edge is repeating
                    if (y + i < 0)
                        i = Field.GetLength(0) - 1;
                    else if (y + i > Field.GetLength(0) - 1)
                        i = -Field.GetLength(0) + 1;
                    for (var j = -1; j <= 1; j++)
                    {
                        int tmpJ = j;
                        //X Edge is repeating
                        if (x + j < 0)
                            j = Field.GetLength(1) - 1;
                        else if (x + j > Field.GetLength(1) - 1)
                            j = -Field.GetLength(1) + 1;
                        aliveNeighbours += Field[y + i, x + j] ? 1 : 0;
                        j = tmpJ;
                    }
                    i = tmpI;
                }

                aliveNeighbours -= Field[y, x] ? 1 : 0;
                if (Rules[0, aliveNeighbours] && Rules[1, aliveNeighbours])
                    FutureField[y, x] = true;
                else if (!Rules[0, aliveNeighbours] && !Rules[1, aliveNeighbours])
                    FutureField[y, x] = false;
                else
                    FutureField[y, x] = Field[y, x];
            }
            Field = FutureField;
        }
    }
}