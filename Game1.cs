using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Entity.Paddle;
using static Constants;
namespace pong_mg;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Paddle paddleOne, paddleTwo;
    private SpriteFont sfontSilkscreen;
    private int score;
    private RenderTarget2D _renderTarget;
    private bool isDebugOverlay = false;
    private KeyboardState oldKBState;

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
        paddleOne = new Paddle(this, _spriteBatch, 5);
        paddleTwo = new Paddle(this, _spriteBatch, VIRTUAL_WIDTH - 5 - Paddle.WIDTH);
        sfontSilkscreen = Content.Load<SpriteFont>("silkscreen");
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState newKBState = Keyboard.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed 
            || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (Keyboard.GetState().IsKeyDown(Keys.W)) 
        {
            paddleOne.moveUp();
        } 
        else if (Keyboard.GetState().IsKeyDown(Keys.S)) 
        {
            paddleOne.moveDown();
        }

        if (Keyboard.GetState().IsKeyDown(Keys.Up)) 
        {
            paddleTwo.moveUp();
        } 
        else if (Keyboard.GetState().IsKeyDown(Keys.Down)) 
        {
            paddleTwo.moveDown();
        }

        if (oldKBState.IsKeyDown(Keys.Q) && newKBState.IsKeyUp(Keys.Q))
            isDebugOverlay = !isDebugOverlay;

        oldKBState = newKBState;

        score = ++score == 100 ? 0 : score;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // * GPU render to _renderTarget
        GraphicsDevice.SetRenderTarget(_renderTarget);


        // * START: Render all

        GraphicsDevice.Clear(BG_COLOR);
        
        int frameRate = (int)(1 / (float)gameTime.ElapsedGameTime.TotalSeconds);

        _spriteBatch.Begin();
        _spriteBatch.DrawString(sfontSilkscreen, $"Score: {score}", new Vector2(5, 5), Color.Red);
        if (isDebugOverlay)
            _spriteBatch.DrawString(sfontSilkscreen, $"fps: {frameRate}", new Vector2(VIRTUAL_WIDTH - 50, 5), Color.Green);
        paddleOne.Draw();
        paddleTwo.Draw();
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
