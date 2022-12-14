using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using static Constants;
using pong_mg;

namespace Entity;

public class Paddle
{
  public const int WIDTH = 5;
  public const int HEIGHT = 50;
  public const int SPEED = 300;
  public Vector2 origin;
  public Vector2 Center => new Vector2(
    origin.X + WIDTH / 2, 
    origin.Y + HEIGHT / 2
  );
  private Color[] fillRect = new Color[WIDTH * HEIGHT];
  private Texture2D rectTexture;
  private SpriteBatch _spriteBatch;
  public bool IsAI { get; private set; }

  public Paddle(GraphicsDevice gfxDev, SpriteBatch spriteBatch, int x, bool isAI = false)
  {
    origin = new Vector2(x, VIRTUAL_HEIGHT / 2 - HEIGHT / 2);
    _spriteBatch = spriteBatch;
    rectTexture = new Texture2D(gfxDev, WIDTH, HEIGHT);
    IsAI = isAI;
  }

  public void MoveUp(double dt) {
    origin.Y = Math.Max(0, (float)(origin.Y - SPEED * dt));
  }

  public void MoveDown(double dt) {
    origin.Y = Math.Min(VIRTUAL_HEIGHT - HEIGHT, (float)(origin.Y + SPEED * dt));
  }

  public void TurnAIOff() {
    IsAI = false;
  }

  public void TurnAIOn() {
    IsAI = true;
  }

  public void AddEnglish(Ball ball, float distO2O)
  {
      float deflectionAngle = 15 
        + Math.Min(
          // Proportionate angle based on collision point wrt outer third height of paddle
          ((Math.Abs(distO2O) - (float)HEIGHT / 6) / ((float)HEIGHT / 3)) * 60,
          60
        );

      deflectionAngle *= distO2O > 0 ? 1 : -1;

      // Making use of ball's invert method, otherwise could set deflectionAngle correctly before this block
      if (ball.Velocity.X > 0)
      {
        ball.HeadingDegrees = deflectionAngle;
        ball.InvertVelocityX();
      }
      else
        ball.HeadingDegrees = deflectionAngle;
  }

  public void HitBall(Ball ball)
  {
    float distO2O = ball.Center.Y - Center.Y;

    if (Math.Abs(distO2O) > (float)HEIGHT / 6)
      AddEnglish(ball, distO2O);
    else
      ball.InvertVelocityX();

    ball.SpeedUp();
  }

  public void ResetPosition()
  {
    origin.Y = VIRTUAL_HEIGHT / 2 - HEIGHT / 2;
  }

  public void Draw(Phase phase) 
  {
    for (int i = 0; i < fillRect.Length; i++)
      fillRect[i] = Color.White;

    rectTexture.SetData(fillRect);

    _spriteBatch.Draw(rectTexture, origin, phase == Phase.PrePlay ? Color.Blue : Color.White);
  }
}