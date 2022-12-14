using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using static Constants;

namespace Entity;

public class Ball
{
  public int R = 3;
  private const int _initSpeed = 200;
  public const float SPEEDUP = 1.05F;
  public Vector2 origin;
  private float _headingRadians;
  public float _speed; 
  public Vector2 Velocity 
  {
    get 
    { 
      return new Vector2(
        _speed * (float)Math.Cos(_headingRadians),
        _speed * (float)Math.Sin(_headingRadians)
        );
    }
  }
  public Vector2 Center => new Vector2(origin.X + R, origin.Y + R);
  public float HeadingDegrees { 
    get { return (_headingRadians * 180 / (float)Math.PI) % 360; }
    set 
    { 
      _headingRadians = (value * (float)Math.PI / 180) % 360;
    } 
  }
  public readonly Color color;
  private Color[] fillRect;
  private Texture2D rectTexture;
  private SpriteBatch _spriteBatch;
  private Random rng = new Random();

  public Ball(GraphicsDevice gfxDev, SpriteBatch spriteBatch)
  {
    _spriteBatch = spriteBatch;
    rectTexture = new Texture2D(gfxDev, 2 * R, 2 * R);
    color = Color.White;
    fillRect = new Color[4 * R * R];

    ResetBall();
  }

  public void ResetBall()
  {
    ResetPosition();
    ResetSpeed();
    RandomizeHeading();
  }

  public void ResetPosition()
  {
    origin = new Vector2(VIRTUAL_WIDTH / 2 - R, VIRTUAL_HEIGHT / 2 - R);
  }

  public void ResetSpeed()
  {
    _speed = _initSpeed;
  }

  public void SpeedUp()
  {
    _speed *= SPEEDUP;
  }

  public void RandomizeHeading() {
    HeadingDegrees = 
      ( rng.Next(2) * 180 )       // pos or neg X
      + ( rng.Next(1, 2) * -1 )   // pos or neg Y
      * ( 15 + rng.Next(46) );    // angle [15, 60]
  }

  public void InvertVelocityX() {
    float deltaAngle = 0.0F;

    deltaAngle = HeadingDegrees >= 0
      ? 2 * (90 - HeadingDegrees)
      : 2 * (-90 - HeadingDegrees);

    HeadingDegrees += deltaAngle;
  }

  public void InvertVelocityY() {
    HeadingDegrees -= HeadingDegrees * 2;
  }

  public void Update(double dt, SoundEffect sfxWallhit) 
  {
    origin += Velocity * (float)dt;
    if (origin.Y < 0 || origin.Y >= VIRTUAL_HEIGHT - 2 * R)
    {
      if (origin.Y < 0)
        origin.Y = 0;
      else
        origin.Y = VIRTUAL_HEIGHT - 2 * R;

      sfxWallhit.Play();
      InvertVelocityY();
    }
  }

  public void Draw(Color phaseColor) 
  {
    for (int i = 0; i < fillRect.Length; i++) {
        fillRect[i] = Color.White;
    }

    rectTexture.SetData(fillRect);

    _spriteBatch.Draw(rectTexture, origin, phaseColor);
  }
}