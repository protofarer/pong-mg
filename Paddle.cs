using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using static Constants;

namespace Entity.Paddle;

public class Paddle
{
  public const int WIDTH = 5;
  public const int HEIGHT = 40;
  public const int SPEED = 3;
  public const string color = "White"; // must call an Xna.Color
  private Vector2 origin;
  private Color[] fillRect = new Color[WIDTH * HEIGHT];
  private Texture2D rectTexture;
  private GraphicsDevice _graphicsDevice;
  private SpriteBatch _spriteBatch;
  private Game _game;

  public Paddle(Game game, SpriteBatch spriteBatch, int x)
  {
    origin = new Vector2(x, VIRTUAL_HEIGHT / 2 - HEIGHT / 2);
    _game = game;
    _spriteBatch = spriteBatch;
    _graphicsDevice = _game.GraphicsDevice;
    rectTexture = new Texture2D(_graphicsDevice, WIDTH, HEIGHT);
  }

  public void moveUp() {
    origin.Y = Math.Max(0, origin.Y - SPEED);
  }

  public void moveDown() {
    origin.Y = Math.Min(VIRTUAL_HEIGHT - HEIGHT, origin.Y + SPEED);
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

    _spriteBatch.Draw(rectTexture, origin, Color.White);

  }
}