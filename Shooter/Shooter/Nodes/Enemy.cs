using System;
using Microsoft.Xna.Framework;

namespace Shooter
{
    public class Enemy : Ship, IGameNode
    {
        public Enemy(Vector2 position)
            : base(64, 64)
        {
            Direction = new Vector2(-1, 0);
            Position = position;
            //State.Enemies.Add(this);
            Texture = State.GetTexture("Enemy");

            MaxAcceleration = new Vector2(50, 50);
            MaxVelocity = new Vector2(200, 200);
            _baseHealth = Health = 50;

            NodeType = GameNodeType.Enemy;
            Enabled = true;
        }

        public void Reset()
        {
            base.Reset();
            this.Health = 0;
            this.Enabled = false;
        }
        public new void Update()
        {
            var generator = new Random((int)State.GameTime.TotalGameTime.TotalMilliseconds * (int)this.Position.Y);
            if (generator.Next(100) > 98)
                Shoot();
            Acceleration = new Vector2(generator.Next(2) * (generator.Next(10) > 5 ? 1 : -1), generator.Next(5) * (generator.Next(10) > 5 ? 1 : -1));

            if (Position.X < State.Player.Position.X)
            {
                if (Acceleration.X < 0)
                    Acceleration.X = Acceleration.X * -1;
            }

            if (Acceleration.X < MaxAcceleration.X && Acceleration.Y < MaxAcceleration.Y)
            {
                var newVelocity = Velocity + Acceleration * new Vector2((float)State.GameTime.ElapsedGameTime.TotalSeconds);
                if (newVelocity.X < MaxVelocity.X && newVelocity.Y < MaxVelocity.Y)
                    Velocity = newVelocity;
            }

            var newPosition = Position + Velocity;

            if (State.WithinBounds(newPosition.X, newPosition.Y, Width, Height) || (newPosition.X - State.Player.Position.X > 200 && newPosition.Y - State.Player.Position.Y > 200))
            {

                base.Update();
            }
            else
            {
                if (Position.X > State.ClientBounds.Width)
                    Position.X = State.ClientBounds.Width - (Width * 2);
                if (Position.Y > State.ClientBounds.Height - Height)
                    Position.Y = State.ClientBounds.Height - Height;
                Acceleration = (Acceleration / 2) * -1;
                Velocity = Velocity * -1;
            }

        }

        public void OnCollision(Asteroid a)
        {
            // Nothing happens
        }
        public void OnCollision(Projectile p)
        {
            // Only player shots will hurt
            if (p.Parent.NodeType == GameNodeType.Player)
                if (!Invincible) _health -= p.Damage;
        }
    }
}
