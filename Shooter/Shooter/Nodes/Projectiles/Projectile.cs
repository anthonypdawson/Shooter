using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter
{
    public enum ProjectileType
    {
        Asteroid,
        Weapon
    }
    public class Projectile : Actor, IGameNode, IDestructable
    {
        public ProjectileType ProjectileType;
        public Ship Parent;

        public Int32 Damage;

        public float MaxVelocity = 400f;

        public Projectile(Int32 width = 0, Int32 height = 0)
            : base(width, height)
        {
            Texture = State.GetTexture("Projectile");
            NodeType = GameNodeType.Projectile;

        }

        public new void Draw()
        {
            State.SpriteBatch.Draw(Texture, Position, Color.White);
        }

        public void OnPostCollide()
        {
            Disable();
        }

        public void OnCollision(Projectile projectile)
        {
            // Nothing
        }
    }

    public class Weapon : Projectile
    {
        public Weapon(Int32 width = 0, Int32 height = 0)
            : base(width, height)
        {
            ProjectileType = ProjectileType.Weapon;
            Damage = 10;
        }

        public new void Update()
        {
            if (Velocity.X > MaxVelocity)
                Velocity.X = MaxVelocity;
            Position += (Velocity * Direction) * new Vector2((float)State.GameTime.ElapsedGameTime.TotalSeconds);

            if (!State.WithinBounds(new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height)))
                Enabled = false;
        }
    }
}
