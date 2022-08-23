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

    private RenderTarget2D _renderTarget;

    private const int VIRTUAL_WIDTH = 200;
    private const int VIRTUAL_HEIGHT = 100;
    private const int WINDOW_WIDTH = 800;
    private const int WINDOW_HEIGHT = 400;

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
        _renderTarget = new RenderTarget2D(GraphicsDevice, VIRTUAL_WIDTH, VIRTUAL_HEIGHT);  // ! unsure if correct graphics/graphicsdevice
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
        // * GPU render to _renderTarget
        GraphicsDevice.SetRenderTarget(_renderTarget);


        // * START: Render all

        GraphicsDevice.Clear(Color.CornflowerBlue);
        paddleOne.Draw();
        
        int frameRate = (int)(1 / (float)gameTime.ElapsedGameTime.TotalSeconds);

        _spriteBatch.Begin();
        _spriteBatch.DrawString(font, $"Score: {score}", new Vector2(25, 25), Color.Red);
        _spriteBatch.DrawString(font, $"fps: {frameRate}", new Vector2(25, 50), Color.Green);
        _spriteBatch.End();

        // * END: Render all


        // * Render to screen
        GraphicsDevice.SetRenderTarget(null);
        _spriteBatch.Begin();
        _spriteBatch.Draw(_renderTarget, new Vector2(WINDOW_WIDTH, WINDOW_HEIGHT), Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
