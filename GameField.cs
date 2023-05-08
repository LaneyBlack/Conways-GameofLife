using System;
using System.Diagnostics;
using System.Threading;

namespace Conways_Game_of_Life
{
    public class GameField
    {
        private int[,] FutureField { get; set; }
        public int[,] Field { get; set; }
        public int[,] Rules { get; set; }

        public EventHandler<string> RedrawEvent;

        public bool IsPaused { get; set; }

        public GameField(int fieldWidth, int fieldHeight)
        {
            Field = new int[fieldHeight, fieldWidth]; //ToDo check if indexes are same in Grid and Here
            FutureField = new int[fieldHeight, fieldWidth];
            Rules = new int[2, 9];
            Rules[0, 3] = 1;
            Rules[1, 2] = 1;
            Rules[1, 3] = 1;
            IsPaused = true;
        }

        public void RiseRedrawEvent()
        {
            if (RedrawEvent != null)
                RedrawEvent.Invoke(this,"");
        }

        public void LoopCalculations()
        {
            while (true)
                if (!IsPaused)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    CalcNextState();
                    stopwatch.Stop();
                    if (1500 - stopwatch.ElapsedMilliseconds > 0)
                        Thread.Sleep((int)(1500 - stopwatch.ElapsedMilliseconds));
                    RiseRedrawEvent();
                }
        }

        public void CalcNextState()
        {
            try
            {
                FutureField = new int[Field.GetLength(0), Field.GetLength(1)];
                for (int y = 1; y < Field.GetLength(0) - 1; y++) //ToDo make it infinite
                for (int x = 1; x < Field.GetLength(1) - 1; x++)
                {
                    int aliveNeighbours = 0;
                    for (int i = -1; i <= 1; i++)
                    for (int j = -1; j <= 1; j++)
                    {
                        aliveNeighbours += Field[y + i, x + j];
                    }

                    aliveNeighbours -= Field[y, x];
                    if (Rules[0, aliveNeighbours] == 1 && Rules[1, aliveNeighbours] == 1)
                        FutureField[y, x] = 1;
                    else if (Rules[0, aliveNeighbours] == 0 && Rules[1, aliveNeighbours] == 0)
                        FutureField[y, x] = 0;
                    else
                        FutureField[y, x] = Field[y, x];
                }

                Field = FutureField;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }
    }
}