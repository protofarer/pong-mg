using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Entity.Paddle;

public class Paddle
{
  public const int WIDTH = 1;
  public const int HEIGHT = 20;
  public const int SPEED = 100;
  public const string color = "White"; // must call an Xna.Color
  private Vector2 origin;
  private Color[] fillRect = new Color[WIDTH * HEIGHT];
  private Texture2D rectTexture;
  private GraphicsDevice _graphicsDevice;
  private SpriteBatch _spriteBatch;

  public Paddle(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, int x, int y)
  {
    origin = new Vector2(x, y);

    _spriteBatch = spriteBatch;
    _graphicsDevice = graphicsDevice;
    rectTexture = new Texture2D(_graphicsDevice, WIDTH, HEIGHT);
  }

  public void Update() 
  {
    // update origin based on input

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