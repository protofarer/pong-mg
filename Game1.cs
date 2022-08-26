using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Entity;
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
    private Phase phase;
    private int roundWinner;     // 0 for none, 1 for playerOne, 2 for playerTwo
    private SoundEffect sfxPaddlehit;
    private SoundEffect sfxScore;
    private SoundEffect sfxWallhit;
    private const int volleyMin = 8;
    private int volleyCount = 0;

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
        phase = Phase.Play;
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

        sfontSilkscreen = Content.Load<SpriteFont>("silkscreen");
        sfontDebug = Content.Load<SpriteFont>("DebugFont");

        sfxPaddlehit = Content.Load<SoundEffect>("sounds/paddlehit");
        sfxScore = Content.Load<SoundEffect>("sounds/score");
        sfxWallhit = Content.Load<SoundEffect>("sounds/wallhit");

        paddleOne = new Paddle(this, _spriteBatch, 20);
        paddleTwo = new Paddle(this, _spriteBatch, VIRTUAL_WIDTH - 20 - Paddle.WIDTH);
        ball = new Ball(this, _spriteBatch) { HeadingDegrees = 205 };
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState newKBState = Keyboard.GetState();
        double dt = gameTime.ElapsedGameTime.TotalSeconds;

        if (oldKBState.IsKeyDown(Keys.Q) && newKBState.IsKeyUp(Keys.Q))
            isDebugOverlay = !isDebugOverlay;

        if (oldKBState.IsKeyDown(Keys.R) && newKBState.IsKeyUp(Keys.R))
            Program.NewGame();

        if (oldKBState.IsKeyDown(Keys.B) && newKBState.IsKeyUp(Keys.B))
            phase = phase == Phase.Play ? Phase.Pause : Phase.Play;

        if (oldKBState.IsKeyDown(Keys.T) && newKBState.IsKeyUp(Keys.T))
        {
            paddleOne.TurnAIOff();
            paddleTwo.TurnAIOff();
        }

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed 
            || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // ? Phase.Preplay
        if (phase == Phase.Play) {
            if (paddleOne.IsAI) {
                if (ball.Center.Y < paddleOne.Center.Y)
                    paddleOne.MoveUp(dt);
                else if (ball.Center.Y > paddleOne.Center.Y)
                    paddleOne.MoveDown(dt);
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.W)) 
                    paddleOne.MoveUp(dt);
                else if (Keyboard.GetState().IsKeyDown(Keys.S)) 
                    paddleOne.MoveDown(dt);
            }

            if (paddleTwo.IsAI) {
                if (ball.Center.Y < paddleTwo.Center.Y)
                    paddleTwo.MoveUp(dt);
                else if (ball.Center.Y > paddleTwo.Center.Y)
                    paddleTwo.MoveDown(dt);
            }
            else {
                if (Keyboard.GetState().IsKeyDown(Keys.Up)) 
                    paddleTwo.MoveUp(dt);
                else if (Keyboard.GetState().IsKeyDown(Keys.Down)) 
                    paddleTwo.MoveDown(dt);
            }

            ball.Update(sfxWallhit);
            // ? emit score event for respective player
            if (ball.origin.X < 0)
            {
                scoreTwo++;
                sfxScore.Play();
                roundWinner = 2;
                if (scoreTwo == 10)
                    phase = Phase.EndGame;
                else
                    phase = Phase.EndRound;
            }
            else if (ball.origin.X >= VIRTUAL_WIDTH - 2 * ball.R)
            {
                scoreOne++;
                sfxScore.Play();
                roundWinner = 1;
                if (scoreOne == 10)
                    phase = Phase.EndGame;
                else
                    phase = Phase.EndRound;
            }

            if (HaveCollided(paddleOne, ball))
            {
                paddleOne.HitBall(ball);

                // Paddles don't know if they're player one or two
                ball.origin.X = paddleOne.origin.X + Paddle.WIDTH + 1;
                sfxPaddlehit.Play();
                volleyCount++;
            }
            
            if (HaveCollided(paddleTwo, ball))
            {
                Console.WriteLine("P2 collision");
                paddleTwo.HitBall(ball);

                // Paddles don't know if they're player one or two
                ball.origin.X = paddleTwo.origin.X - 2 * ball.R - 1;
                sfxPaddlehit.Play();
                volleyCount++;
            }
        } 
        else if (phase == Phase.EndRound)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                ball.ResetBall();
                paddleOne.ResetPosition();
                paddleTwo.ResetPosition();
                phase = Phase.Play;
            }
        }
        else if (phase == Phase.EndGame)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                scoreOne = scoreTwo = 0;
                phase = Phase.EndRound;
            }
        }

        oldKBState = newKBState;

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
            phase == Phase.EndGame 
                ? scoreOne > scoreTwo
                    ? Color.Green
                    : Color.Red
                : Color.White
        );

        _spriteBatch.DrawString(
            sfontSilkscreen, 
            $"{scoreTwo}", 
            new Vector2((float)(VIRTUAL_WIDTH * 0.7), (float)(VIRTUAL_HEIGHT * 0.05)), 
            phase == Phase.EndGame 
                ? scoreOne > scoreTwo
                    ? Color.Red
                    : Color.Green
                : Color.White
        );

        _spriteBatch.Draw(
            _netTexture, 
            new Vector2(VIRTUAL_WIDTH / 2 - 1, 0), 
            new Color(172,172,172)
        );

        if (isDebugOverlay)
        {
            _spriteBatch.DrawString(sfontDebug, $"fps: {frameRate}", new Vector2(VIRTUAL_WIDTH - 75, 3), Color.Green);
            _spriteBatch.DrawString(sfontDebug, $"sp_b: {ball._speed}", new Vector2(VIRTUAL_WIDTH - 75, 10), Color.Green);
        }

        paddleOne.Draw();
        paddleTwo.Draw();
        ball.Draw();

        if (phase == Phase.Pause)
        {
            _spriteBatch.DrawString(
                sfontSilkscreen, 
                "PAUSED", 
                new Vector2(
                    VIRTUAL_WIDTH * 0.2F, 
                    VIRTUAL_HEIGHT / 2 - 25
                ), 
                Color.Red
            );
        }
        else if (phase == Phase.EndRound)
        {
            if (roundWinner == 1)
                _spriteBatch.DrawString(
                    sfontDebug, 
                    "Player 1 Scores!", 
                    new Vector2(
                        VIRTUAL_WIDTH * 0.05F, 
                        VIRTUAL_HEIGHT * 0.22F), 
                    Color.Green
                );
            else if (roundWinner == 2)
                _spriteBatch.DrawString(
                    sfontDebug, 
                    "Player 2 Scores!", 
                    new Vector2(
                        VIRTUAL_WIDTH * 0.52F, 
                        VIRTUAL_HEIGHT * 0.22F), 
                    Color.Green
                );

            if (volleyCount >= volleyMin)
            {
                // Praise a good volley
                _spriteBatch.DrawString(
                    sfontSilkscreen, 
                    "Nice", 
                    new Vector2(
                        VIRTUAL_WIDTH * 0.25F, 
                        VIRTUAL_HEIGHT / 2 - 45
                    ), 
                    Color.White
                );

                _spriteBatch.DrawString(
                    sfontSilkscreen, 
                    "Volley!", 
                    new Vector2(
                        VIRTUAL_WIDTH * 0.45F, 
                        VIRTUAL_HEIGHT / 2 + 5
                    ), 
                    Color.White
                );
            }

            _spriteBatch.DrawString(
                sfontDebug, 
                "[Enter] for next round", 
                new Vector2(
                    VIRTUAL_WIDTH * 0.5F - (22 * 4),        // 22=charCount, 4=approxCharWidth
                    VIRTUAL_HEIGHT * 0.75F), 
                Color.Red
            );
        }
        else if (phase == Phase.EndGame)
        {
            _spriteBatch.DrawString(
                sfontSilkscreen, 
                $"Player {(scoreOne > scoreTwo ? "One" : "Two")}", 
                new Vector2(
                    VIRTUAL_WIDTH * 0.25F, 
                    VIRTUAL_HEIGHT / 2 - 45
                ), 
                Color.Green
            );

            _spriteBatch.DrawString(
                sfontSilkscreen, 
                "Wins!", 
                new Vector2(
                    VIRTUAL_WIDTH * 0.4F, 
                    VIRTUAL_HEIGHT / 2 + 25
                ), 
                Color.Green
            );
        }

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

public enum Phase {
    Preplay,
    Play,
    Pause,
    EndRound,
    EndGame
}