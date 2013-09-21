using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Shooter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ShooterGame : Microsoft.Xna.Framework.Game
    {
        private const Boolean DEBUG = true;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private SpriteFont spriteFont;
        private Player Player;
        private const Int32 _maxEnemies = 4;


        public ShooterGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = @"Content";
            State.GraphicsManager = graphics;
            State.Initialize(Content);
            State.ClientBounds = Window.ClientBounds;
            this.IsFixedTimeStep = false;

            InputManager.Setup();

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            State.SpriteBatch = spriteBatch;
            spriteFont = Content.Load<SpriteFont>("Font");
            State.SpriteFont = spriteFont;
            Player = new Player(new Vector2(64));
            State.Player = Player;
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            var kbState = Keyboard.GetState();
            var gpState = GamePad.GetState(PlayerIndex.One);
            State.CurrentState = kbState;
            State.CurrentGamePadState = gpState;
            // Allows the game to exit
            if (kbState.IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            State.Update(gameTime);

            if (State.GameOver)
            {
                if (InputManager.SinglePlayer.IsPressed(Keys.Enter))
                    State.GameOver = false;
            }
            else
            {
                if (InputManager.SinglePlayer.EitherDownWasUp(Keys.Right))
                    Player.Acceleration += new Vector2(6, 0);
                if (InputManager.SinglePlayer.EitherDownWasUp(Keys.Left))
                    Player.Acceleration += new Vector2(-6, 0);
                if (InputManager.SinglePlayer.EitherUpWasDown(Keys.Right))
                    Player.Acceleration.X = 0;
                if (InputManager.SinglePlayer.EitherUpWasDown(Keys.Left))
                    Player.Acceleration.X = 0;
                if (InputManager.SinglePlayer.EitherDownWasUp(Keys.Up))
                    Player.Acceleration += new Vector2(0, -10);
                if (InputManager.SinglePlayer.EitherDownWasUp(Keys.Down))
                    Player.Acceleration += new Vector2(0, 10);
                if (InputManager.SinglePlayer.EitherUpWasDown(Keys.Up))
                    Player.Acceleration.Y = 0;
                if (InputManager.SinglePlayer.EitherUpWasDown(Keys.Down))
                    Player.Acceleration.Y = 0;

                if (InputManager.SinglePlayer.EitherDownWasUp(Keys.P))
                {
                    if (State.Enemies.Count < _maxEnemies)
                    {
                        var unit = State.ClientBounds.Width / 2;
                        var e = new Enemy(Vector2.Zero);
                        e.Position = new Vector2(unit + Randomizer.Next(unit / 2, unit - e.Width),
                                                 Randomizer.Next(e.Height, State.ClientBounds.Height - e.Height));
                        State.AddEnemy(e);
                    }
                }
                //if (kbState.IsKeyDown(Keys.Space) && !State.KeyWasPressed(Keys.Space))
                if (InputManager.SinglePlayer.EitherIsPressed(Keys.Space))
                    Player.Shoot();


                Player.Update();
            }

            base.Update(gameTime);
            State.PreviousState = kbState;
            State.PreviousGamePadState = gpState;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            if (DEBUG)
                spriteBatch.DrawString(spriteFont,
        String.Format("FPS: {8}\nPlayer X: {0} Y: {1}\n Width: {2} Height: {3} HP: {6} \nProjectiles \n {4}\n EnemyCount: {7} Enemies \n {5}",
        Player.Position.X, Player.Position.Y, Player.Width, Player.Height, String.Join("\n",
        State.Projectiles.Select(p => String.Format("X: {0}, Y: {1}", p.Position.X, p.Position.Y))), String.Join("\n",
        State.Enemies.Select(p => String.Format("X: {0}, Y: {1}, HP: {2} Invincible: {3}", p.Position.X, p.Position.Y, p.Health, p.Invincible))), Player.Health, State.Enemies.Count(), State.FpsCounter.Fps), new Vector2(25), Color.Black
        );
            State.Draw();
            Player.Draw();
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }




}
