public static class Program
{
  static void Main(string[] args)
  {
    NewGame();
  }

  public static void NewGame()
  {
    using var game = new pong_mg.Game1();
    game.Run();
  }
}