namespace Conways_Game_of_Life
{
    public class GameField
    {
        public static int[,] Field { get; set; }
        public static int[,] FieldCopy { get; set; }

        public bool IsPaused { get; set; }

        public GameField(int fieldWidth, int fieldHeight)
        {
            Field = new int[fieldHeight,fieldWidth];
            FieldCopy = new int[fieldHeight,fieldWidth];
        }
    }
}