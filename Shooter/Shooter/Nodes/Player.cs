using Microsoft.Xna.Framework;

namespace Shooter
{
    public class Player : Ship, IGameNode
    {

        private double _shotLimit = .5;
        public Player(Vector2 position)
            : base(64, 64)
        {
            Direction = new Vector2(1, 0);
            Texture = State.GetTexture("Player");
            Position = position;
            Acceleration = Vector2.Zero;
            Velocity = Vector2.Zero;
            _baseHealth = Health = 40;
            _confinedToWindow = true;
        }

        public new void Update()
        {
            if (Acceleration.X == 0)
                Velocity.X -= Velocity.X * .10f;
            if (Acceleration.Y == 0)
                Velocity.Y -= Velocity.Y * .10f;

            Velocity += Acceleration * new Vector2((float)State.GameTime.ElapsedGameTime.TotalSeconds);

            if (Velocity.X < .1f && Velocity.X > -0.1f)
                Velocity.X = 0;

            if (Velocity.Y < .1f && Velocity.Y > -0.1f)
                Velocity.Y = 0;

            base.Update();

        }

        public new void OnCollision(Projectile p)
        {
            if (!Invincible)
            {
                Health -= p.Damage;
                Invincible = true;
                InvincibleTimeLeft = 50;
            }

            OnPostCollide();
        }

        public void ResetHealth()
        {
            _health = _baseHealth;
        }


        private float SlowDown(float velocity)
        {
            if (velocity > 0)
                velocity -= (velocity * 0.25f);
            if (velocity < 0)
                velocity -= (velocity * 0.25f);
            if (velocity != 0 && velocity < 0 && velocity > 0)
                velocity = 0;
            return velocity;
        }
    }
}
