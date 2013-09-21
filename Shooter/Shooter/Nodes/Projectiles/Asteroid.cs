using Microsoft.Xna.Framework;

namespace Shooter
{

    public class Asteroid : Projectile, IGameNode
    {
        public Asteroid(float width, float height)
        {
            Texture = State.GetTexture("Asteroid");
            Enabled = true;
            ProjectileType = ProjectileType.Asteroid;
        }

        public new void Update()
        {
            if (Position.X < (0 - Width))
                Enabled = false;

            if (Velocity.X > MaxVelocity)
                Velocity.X = MaxVelocity;
            Position += (Velocity * Direction) * new Vector2((float)State.GameTime.ElapsedGameTime.TotalSeconds);
        }

        public new void Collided(Projectile p)
        { base.Collided(p); }
    }
}
