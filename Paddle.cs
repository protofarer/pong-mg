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
    float distO2O = ball.origin.Y + ball.R - (this.origin.Y + HEIGHT / 2);
    Console.WriteLine($"dist020: {distO2O}");
    if (Math.Abs(distO2O) > HEIGHT / 3)
    {
        float deflectionAngle = 15 
          + Math.Min(
            ((Math.Abs(distO2O) - HEIGHT / 6) / (HEIGHT / 3)) * 60,
            60
          );
        deflectionAngle *= distO2O > 0 ? 1 : -1;

        // For reflecting angle properly for paddleTwo
        // ! BUG, fuxored
        // if (ball.Velocity.X > 0)
        // {
        //   Console.WriteLine("entered ball.vel.x > 0");
        //   if (deflectionAngle > 0)
        //   {
        //     deflectionAngle = 180 - deflectionAngle;
        //   } else if (deflectionAngle < 0)
        //   {
        //     deflectionAngle = -180 - deflectionAngle;
        //   }
        // }
        ball.HeadingDegrees = deflectionAngle;
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