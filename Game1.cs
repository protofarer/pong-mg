using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Entity.Paddle;
using Entity.Ball;
using static Constants;
using static Program;
using static Physics;
namespace pong_mg;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Paddle paddleOne, paddleTwo;
    private Ball ball;
    private SpriteFont sfontSilkscreen;
    private SpriteFont sfontDebug;
    private int scoreTwo, scoreOne;
    private RenderTarget2D _renderTarget;
    private bool isDebugOverlay = false;
    private KeyboardState oldKBState;

    private Color[] _netFill = new Color[VIRTUAL_HEIGHT];
    private Texture2D _netTexture;

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
        _netTexture = new Texture2D(GraphicsDevice, 1, VIRTUAL_HEIGHT);
        var toggleNetDraw = false;
        for (int i = 0; i < _netFill.Length; i++)
        {
            if (i % 6 == 0)
                toggleNetDraw = !toggleNetDraw;
            
            if (toggleNetDraw)
                _netFill[i] = Color.White;
        }
        _netTexture.SetData(_netFill);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        paddleOne = new Paddle(this, _spriteBatch, 20, true);
        paddleTwo = new Paddle(this, _spriteBatch, VIRTUAL_WIDTH - 20 - Paddle.WIDTH, true);
        ball = new Ball(this, _spriteBatch);
        sfontSilkscreen = Content.Load<SpriteFont>("silkscreen");
        sfontDebug = Content.Load<SpriteFont>("DebugFont");
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState newKBState = Keyboard.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed 
            || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (paddleOne.IsAI) {
            if (ball.origin.Y < paddleOne.origin.Y + Paddle.WIDTH - ball.R)
                paddleOne.MoveUp();
            else if (ball.origin.Y > paddleOne.origin.Y + Paddle.WIDTH - ball.R)
                paddleOne.MoveDown();
        }
        else
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W)) 
                paddleOne.MoveUp();
            else if (Keyboard.GetState().IsKeyDown(Keys.S)) 
                paddleOne.MoveDown();
        }

        if (paddleTwo.IsAI) {
            if (ball.origin.Y < paddleTwo.origin.Y + Paddle.WIDTH - ball.R)
                paddleTwo.MoveUp();
            else if (ball.origin.Y > paddleTwo.origin.Y + Paddle.WIDTH - ball.R)
                paddleTwo.MoveDown();
        }
        else {
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) 
                paddleTwo.MoveUp();
            else if (Keyboard.GetState().IsKeyDown(Keys.Down)) 
                paddleTwo.MoveDown();
        }

        if (oldKBState.IsKeyDown(Keys.Q) && newKBState.IsKeyUp(Keys.Q))
            isDebugOverlay = !isDebugOverlay;

        if (oldKBState.IsKeyDown(Keys.R) && newKBState.IsKeyUp(Keys.R))
            Program.NewGame();

        if (oldKBState.IsKeyDown(Keys.T) && newKBState.IsKeyUp(Keys.T))
        {
            paddleOne.TurnAIOff();
            paddleTwo.TurnAIOff();
        }

        oldKBState = newKBState;

        ball.Update();

        // ? emit score event for respective player
        if (ball.origin.X < 0)
        {
            scoreTwo++;
            ball.ResetBall();
        }
        else if (ball.origin.X >= VIRTUAL_WIDTH - 2 * ball.R)
        {
            scoreOne++;
            ball.ResetBall();
        }

        if (HaveCollided(paddleOne, ball))
        {
            ball.SpeedUp();
            ball.InvertVelocityX();
            ball.origin.X = paddleOne.origin.X + Paddle.WIDTH;
        }
        
        if (HaveCollided(paddleTwo, ball))
        {
            ball.SpeedUp();
            ball.InvertVelocityX();
            ball.origin.X = paddleTwo.origin.X - 2 * ball.R;
        }

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

        _spriteBatch.DrawString(
            sfontSilkscreen, 
            $"{scoreOne}", 
            new Vector2((float)(VIRTUAL_WIDTH * 0.2), (float)(VIRTUAL_HEIGHT * 0.05)), 
            Color.White
        );

        _spriteBatch.DrawString(
            sfontSilkscreen, 
            $"{scoreTwo}", 
            new Vector2((float)(VIRTUAL_WIDTH * 0.7), (float)(VIRTUAL_HEIGHT * 0.05)), 
            Color.White
        );

        if (isDebugOverlay)
        {
            _spriteBatch.DrawString(sfontDebug, $"fps: {frameRate}", new Vector2(VIRTUAL_WIDTH - 75, 3), Color.Green);
            _spriteBatch.DrawString(sfontDebug, $"sp_b: {ball._speed}", new Vector2(VIRTUAL_WIDTH - 75, 10), Color.Green);
        }

        _spriteBatch.Draw(
            _netTexture, 
            new Vector2(VIRTUAL_WIDTH / 2 - 1, 0), 
            new Color(172,172,172)
        );

        paddleOne.Draw();
        paddleTwo.Draw();
        ball.Draw();

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
