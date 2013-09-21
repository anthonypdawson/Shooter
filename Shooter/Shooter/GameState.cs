using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Shooter
{
    public static class State
    {
        private static Int32 _maxAsteroids = 10;
        private static Int32 _asteroidCooldown = 10;
        private static List<Actor> _nodes = new List<Actor>();
        private static Dictionary<String, Texture2D> _textureCache;
        public static SpriteBatch SpriteBatch;
        public static SpriteFont SpriteFont;
        public static GraphicsDeviceManager GraphicsManager;
        public static GameTime GameTime;
        public static Rectangle ClientBounds;
        public static ContentManager Content;
        public static Player Player;
        public static List<Enemy> Enemies = new List<Enemy>();
        public static List<Projectile> Projectiles = new List<Projectile>();
        public static List<Actor> Entities = new List<Actor>();

        public static FpsCounter FpsCounter;
        public static Int32 Kills = 0;

        public static Random Generator = new Random();

        public static GamePadState CurrentGamePadState;
        public static GamePadState PreviousGamePadState;
        public static KeyboardState CurrentState;
        public static KeyboardState PreviousState;

        public static Boolean GameOver = false;

        public static void Initialize(ContentManager contentManager)
        {
            Content = contentManager;
            FpsCounter = new FpsCounter();
            FpsCounter.Initialize();
        }
        public static void Update(GameTime gameTime)
        {
            if (GameOver)
                return;

            GameTime = gameTime;

            FpsCounter.Update(gameTime);

            InputManager.Update();
            CullNodes();

            UpdateAsteroidField();

            Randomizer.Setup();

            foreach (var e in Enemies)
            {
                if (e.Health > 0)
                    e.Enabled = true;
                else
                {
                    e.Enabled = false;
                }
            }

            foreach (var node in _nodes.ToList())
            {
                var collidedNodes = _nodes.Where(n => n != node && n.NodeType == GameNodeType.Projectile && node.Intersects(n)).ToList();
                if (collidedNodes.Any())
                {
                    collidedNodes.ToList().ForEach(cn => node.Collided(((Projectile)cn)));
                }
            }
            foreach (var p in Projectiles)
            {
                var colProjectiles = Projectiles.ToList().Where(pr => (pr.ProjectileType != ProjectileType.Weapon || p.ProjectileType != ProjectileType.Weapon) && pr != p && pr.Intersects(p));

                if (colProjectiles.Any())
                    colProjectiles.ToList().ForEach(pr => pr.Collided(p));

                if (p.Parent != Player && p.Intersects(Player))
                    Player.Collided(p);

                // This will allow enemies to harm each other when not checked for Player projectile

                if (p.Parent == Player)
                {
                    var enemies = Enemies.Where(e => !e.Projectiles.Contains(p) && p.Intersects(e));
                    if (enemies.Any())
                    {
                        enemies.ToList().ForEach(e => e.Collided(p));
                        Kills += enemies.Count(e => !e.Enabled);
                        p.Enabled = false;
                    }
                }

            }
            UpdateNodes();
            if (!Player.Enabled && Player.Health <= 0)
            {
                GameOver = true;
                Reset();
            }
        }

        private static void CheckProjectiles()
        {

        }
        public static void Draw()
        {
            if (GameOver)
                SpriteBatch.DrawString(SpriteFont, "Game Over\n Press Start", new Vector2(ClientBounds.Width / 2, ClientBounds.Height / 2), Color.Red);

            DrawNodes();

            SpriteBatch.DrawString(SpriteFont, String.Format("HP: {0}", Player.Health), new Vector2(ClientBounds.Width - 100, 5), Color.Black);
            SpriteBatch.DrawString(SpriteFont, String.Format("Kills: {0}", Kills), new Vector2(ClientBounds.Width - 100, 25), Color.Red);
        }

        public static void Remove(IGameNode node)
        {
            RemoveNode(n => n == node);
        }

        private static void RemoveNode(Predicate<IGameNode> func)
        {
            _nodes.RemoveAll(func);
            Projectiles.RemoveAll(p => !_nodes.Contains(p));
        }

        public static void RemoveEnemy(Predicate<Enemy> func)
        {
            Enemies.RemoveAll(func);
            RemoveNode(n => !Enemies.Contains(n));
        }

        public static void RemoveProjectile(Func<Projectile, Boolean> func)
        {
            Projectiles.ForEach(p => p.Parent.RemoveChild(p));
            Projectiles.RemoveAll(n => func(n));
            RemoveNode(n => !Projectiles.Contains(n));
        }

        public static void AddEnemy(Enemy e)
        {
            if (!Enemies.Contains(e)) Enemies.Add(e);
            AddNode(e);
        }
        public static void AddProjectile(Projectile p)
        {
            Projectiles.Add(p);
            AddNode(p);
        }
        public static void AddEntity(Actor n)
        {
            AddNode(n);
        }

        private static void AddNode(Actor node)
        {
            if (!_nodes.Contains(node))
                _nodes.Add(node);
        }

        private static void CullNodes()
        {
            RemoveNode(n => !n.IsEnabled());
        }


        private static void UpdateNodes()
        {

            _nodes.ForEach(n =>
                {
                    switch (n.NodeType)
                    {
                        case GameNodeType.Projectile:
                            switch (((Projectile)n).ProjectileType)
                            {
                                case ProjectileType.Asteroid:
                                    ((Asteroid)n).Update();
                                    break;
                                case ProjectileType.Weapon:
                                    ((Weapon)n).Update();
                                    break;
                            }
                            break;
                        case GameNodeType.Enemy:
                            ((Enemy)n).Update();
                            break;
                        case GameNodeType.Player:
                            break;
                        case GameNodeType.Ship:
                        case GameNodeType.Actor:
                            n.Update();
                            break;
                    }
                });
        }

        private static void DrawNodes()
        {
            _nodes.ForEach(n => n.Draw());
        }

        private static void OnPostRemove()
        {
            Projectiles.RemoveAll(p => !_nodes.Contains(p));
            Enemies.RemoveAll(e => !_nodes.Contains(e));
        }
        private static void OnPostAdd()
        {
            Projectiles.Where(p => !_nodes.Contains(p)).ToList().ForEach(AddNode);
            Enemies.Where(e => !_nodes.Contains(e)).ToList().ForEach(AddNode);
        }

        public static Texture2D GetTexture(string name)
        {
            if (_textureCache == null)
                _textureCache = new Dictionary<string, Texture2D>();

            if (!_textureCache.ContainsKey(name))
                _textureCache.Add(name, Texture2D.FromStream(GraphicsManager.GraphicsDevice, TitleContainer.OpenStream(String.Format("Content/{0}.png", name))));

            return _textureCache[name];
        }

        public static IEnumerable<Asteroid> Asteroids
        {
            get { return Projectiles.Where(p => p.ProjectileType == ProjectileType.Asteroid).Select(p => (Asteroid)p); }
        }

        private static void UpdateAsteroidField()
        {
            if (Asteroids.Count() < _maxAsteroids && _asteroidCooldown <= 0)
            {
                var g = new Random((int)GameTime.ElapsedGameTime.Ticks * DateTime.Now.Millisecond);
                var roll = g.Next(1000);
                if (roll > 900)
                {
                    var asteroid = new Asteroid(35, 30)
                    {
                        Texture = State.GetTexture("Asteroid"),
                        Direction = new Vector2(-1, 0),
                        Velocity = new Vector2(200, 0)
                    };
                    asteroid.Position = new Vector2(ClientBounds.Width,
                                                    Randomizer.Next(asteroid.Height, State.ClientBounds.Height - asteroid.Height));
                    State.AddProjectile(asteroid);
                    _asteroidCooldown = 50 + g.Next(50);
                }
            }

            _asteroidCooldown--;
        }

        public static Boolean WithinBounds(float x, float y, float width, float height)
        {
            return WithinBounds(new Rectangle((int)x, (int)y, (int)width, (int)height));
        }
        public static Boolean WithinBounds(Rectangle obj, Double withinPercentage)
        {
            var bRect = GraphicsManager.GraphicsDevice.Viewport.Bounds;
            if (withinPercentage != 0)
            {

                bRect = new Rectangle((int)(bRect.X - (bRect.Width * withinPercentage)),
                                          (int)(bRect.Y - (bRect.Height + withinPercentage)),
                                          (int)(bRect.Width + (bRect.Width * withinPercentage)),
                                          (int)(bRect.Height + (bRect.Height * withinPercentage)));
            }

            return bRect.Contains(obj);
        }

        public static Boolean WithinBounds(Rectangle obj)
        {
            return WithinBounds(obj, 0);
        }

        public static Type GetType(Actor a)
        {
            switch (a.ActorType)
            {
                case ActorType.Actor:
                    return typeof(Actor);
                case ActorType.Asteroid:
                    return typeof(Asteroid);
                case ActorType.Enemy:
                    return typeof(Enemy);
                case ActorType.Player:
                    return typeof(Player);
                case ActorType.Projectile:
                    return typeof(Projectile);
                case ActorType.Ship:
                    return typeof(Ship);
            }
            return typeof(Actor);
        }



        private static void Reset()
        {
            RemoveNode(n => n != null);
            Player.ResetHealth();
            Player.Reset();
            Player.Position = new Vector2(64);
            _nodes.ForEach(n => n.Reset());
            Entities.Clear();
            ClearAsteroids();
            Enemies.Clear();
            Kills = 0;
        }

        private static void ClearAsteroids()
        {
            Projectiles.RemoveAll(p => Asteroids.Contains(p));
        }
    }


}
