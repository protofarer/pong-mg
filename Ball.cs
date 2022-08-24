using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Constants;

namespace Entity.Ball;

public class Ball
{
  public int R = 3;

  public const float SPEEDUP = 1.2F;
  public readonly Color color;
  public Vector2 origin;
  private Color[] fillRect;
  private Texture2D rectTexture;
  private SpriteBatch _spriteBatch;
  private Game _game;

  private float _headingRadians;
  
  public float _speed; 
  private float HeadingDegrees { 
    get { return _headingRadians * 180 / (float)Math.PI; }
    set { _headingRadians = value * (float)Math.PI / 180; } 
  }
  private Vector2 _velocity;
  private Random rng = new Random();


  public Ball(Game game, SpriteBatch spriteBatch)
  {
    _game = game;
    _spriteBatch = spriteBatch;
    rectTexture = new Texture2D(game.GraphicsDevice, 2 * R, 2 * R);
    color = Color.White;
    fillRect = new Color[4 * R * R];

    ResetBall();
  }

  public void Update() 
  {
    origin += _velocity;
    if (origin.Y < 0 || origin.Y >= VIRTUAL_HEIGHT - 2 * R)
    {
      InvertVelocityY();
    }
  }

  public void ResetBall()
  {
    ResetPosition();
    ResetSpeed();
    RandomizeVelocity();
  }

  public void ResetPosition()
  {
    origin = new Vector2(VIRTUAL_WIDTH / 2 - R, VIRTUAL_HEIGHT / 2 - R);
  }

  public void ResetSpeed()
  {
    _speed = 5;
  }

  public void SpeedUp()
  {
    _speed = _speed * SPEEDUP;
    _velocity *= SPEEDUP;
  }

  public void RandomizeHeading() {
    HeadingDegrees = 
      ( rng.Next(2) * 180 )  // pos or neg X
      + ( rng.Next(1, 2) * -1 )      // pos or neg Y
      * ( 15 + rng.Next(46) );   // angle [15, 60]
  }

  public void RandomizeVelocity() {
    RandomizeHeading();
    _velocity = new Vector2(
      _speed * (float)Math.Cos(_headingRadians),
      _speed * (float)Math.Sin(_headingRadians)
      );
  }

  public void InvertVelocityX() {
    _velocity.X = -_velocity.X;
  }

  public void InvertVelocityY() {
    _velocity.Y = -_velocity.Y;
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