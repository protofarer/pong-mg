using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Entity.Paddle;
namespace pong_mg;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Paddle paddleOne;
    private SpriteFont font;
    private int score;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        IsFixedTimeStep = false;
        Window.Title = "Pong!";
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        paddleOne = new Paddle(_spriteBatch, GraphicsDevice, 25, 75);
        font = Content.Load<SpriteFont>("Score");

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        score = ++score == 100 ? 0 : score;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        paddleOne.Draw();
        
        int frameRate = (int)(1 / (float)gameTime.ElapsedGameTime.TotalSeconds);

        _spriteBatch.Begin();
        _spriteBatch.DrawString(font, $"Score: {score}", new Vector2(25, 25), Color.Red);
        _spriteBatch.DrawString(font, $"fps: {frameRate}", new Vector2(25, 50), Color.Green);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
