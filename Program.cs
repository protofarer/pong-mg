using System;
public static class Program
{
  private static pong_mg.Game1 game;
  static void Main(string[] args)
  {
    NewGame();
  }

  public static void NewGame()
  {
    // Program.game?.Exit();     // doesnt work
    Program.game = new pong_mg.Game1();
    Program.game.Run();
  }
}