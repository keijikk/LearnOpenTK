namespace LearnOpenTK
{
    internal class Program
    {
        #region -- Private Methods --

        private static void Main(string[] args)
        {
            using (var game = new Game(800, 600, "LearnOpenTK"))
            {
                game.Run(60);
            }
        }

        #endregion
    }
}