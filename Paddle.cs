using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using static Constants;

namespace Entity;

public class Paddle
{
  public const int WIDTH = 5;
  public const int HEIGHT = 40;
  public const int SPEED = 5;
  public readonly Color color;
  
  public Vector2 origin;

  public Vector2 Center => new Vector2(
    origin.X + WIDTH / 2, 
    origin.Y + HEIGHT / 2
  );
  private Color[] fillRect = new Color[WIDTH * HEIGHT];
  private Texture2D rectTexture;
  private SpriteBatch _spriteBatch;
  private Game _game;

  public bool IsAI { get; private set; }

  public Paddle(Game game, SpriteBatch spriteBatch, int x, bool isAI = false)
  {
    origin = new Vector2(x, VIRTUAL_HEIGHT / 2 - HEIGHT / 2);
    _game = game;
    _spriteBatch = spriteBatch;
    rectTexture = new Texture2D(game.GraphicsDevice, WIDTH, HEIGHT);
    color = Color.White;
    IsAI = isAI;
  }

  public void MoveUp() {
    origin.Y = Math.Max(0, origin.Y - SPEED);
  }

  public void MoveDown() {
    origin.Y = Math.Min(VIRTUAL_HEIGHT - HEIGHT, origin.Y + SPEED);
  }

  public void TurnAIOff() {
    IsAI = false;
  }

  public void AddEnglish(Ball ball)
  {
    float distO2O = ball.Center.Y - Center.Y;
    Console.WriteLine($"dist020: {distO2O}");
    if (Math.Abs(distO2O) > (float)HEIGHT / 6)
    {
      float deflectionAngle = 15 
        + Math.Min(
          ((Math.Abs(distO2O) - (float)HEIGHT / 6) / ((float)HEIGHT / 3)) * 60,
          60
        );

      deflectionAngle *= distO2O > 0 ? 1 : -1;

      ball.HeadingDegrees = deflectionAngle;

      if (ball.Velocity.X > 0)
        ball.InvertVelocityX();

      Console.WriteLine($"defAng: {deflectionAngle}");
      Console.WriteLine($"ballHeadingDeg: {ball.HeadingDegrees}");
    }
  }


  public void Update() 
  {
  }

  public void Draw() 
  {
    for (int i = 0; i < fillRect.Length; i++) {
        fillRect[i] = Color.White;
    }
    rectTexture.SetData(fillRect);

    _spriteBatch.Draw(rectTexture, origin, color);

  }
}