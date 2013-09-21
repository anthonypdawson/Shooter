using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Shooter
{
    public class Ship : Actor, IDestructable
    {
        private double _lastShot;

        private double _weaponCooldown = .3;


        public List<Projectile> Projectiles = new List<Projectile>();
        public Vector2 Direction;
        public Ship(float height, float width)
            : base(width, height)
        {
        }

        public void Shoot()
        {
            if (State.GameTime.TotalGameTime.TotalSeconds - _lastShot < _weaponCooldown)
                return;

            var p = new Weapon()
            {
                //Position = new Vector2(Position.X, (Position.Y) + (new Random().Next(2) == 1 ? 16 : 0)),
                Position = new Vector2(Position.X + (Direction.X == 1 ? Width : -10), Position.Y + (Height * .5f)),
                Direction = Direction,
                Enabled = true,
                Velocity = new Vector2(400, Velocity.Y),
                Parent = this
            };
            Projectiles.Add(p);
            State.AddProjectile(p);
            _lastShot = State.GameTime.TotalGameTime.TotalSeconds;
        }

        public void OnCollision(Projectile p)
        {
            Health -= p.Damage;
            p.OnPostCollide();
        }
    }


}
