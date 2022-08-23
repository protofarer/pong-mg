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
    private SpriteFont sfontSilkscreen;
    private int score;

    private RenderTarget2D _renderTarget;

    private const int VIRTUAL_WIDTH = 200;
    private const int VIRTUAL_HEIGHT = 200;
    private const int WINDOW_WIDTH = 800;
    private const int WINDOW_HEIGHT = 800;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
        _graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
        _graphics.ApplyChanges();

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
        paddleOne = new Paddle(_spriteBatch, GraphicsDevice, 5, 100);
        sfontSilkscreen = Content.Load<SpriteFont>("silkscreen");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed 
            || Keyboard.GetState().IsKeyDown(Keys.Escape))
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
        
        int frameRate = (int)(1 / (float)gameTime.ElapsedGameTime.TotalSeconds);

        _spriteBatch.Begin();
        _spriteBatch.DrawString(sfontSilkscreen, $"Score: {score}", new Vector2(5, 5), Color.Red);
        _spriteBatch.DrawString(sfontSilkscreen, $"fps: {frameRate}", new Vector2(5, 20), Color.Green);
        paddleOne.Draw();
        _spriteBatch.End();

        // * END: Render all


        // * Render to screen
        GraphicsDevice.SetRenderTarget(null);

        //  Set hard pixel edges
        _spriteBatch.Begin(
            SpriteSortMode.BackToFront, 
            BlendState.AlphaBlend, 
            SamplerState.PointClamp
        );
        _spriteBatch.Draw(_renderTarget, new Rectangle(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT), Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
