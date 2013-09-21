
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shooter
{
    public enum ActorType
    {
        Actor,
        Ship,
        Player,
        Enemy,
        Projectile,
        Asteroid
    }

    public class Entity : GameNode, IGameNode
    {
        private float _height;
        private float _width;

        public Color Color;

        public Texture2D Texture;

        public Vector2 Position;

        private Boolean _enabled;
        public Boolean Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public List<IGameNode> Children = new List<IGameNode>();

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)Width, (int)Height);
            }
        }
        public Vector2 Scale
        {
            get { return new Vector2(_width == 0 ? 1 : _width / Texture.Width, _height == 0 ? 1 : _height / Texture.Height); }
        }
        public Entity(float width, float height)
        {
            _width = width;
            _height = height;
            Color = Color.White;
        }
        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public float Height
        {
            get { return Texture.Height * Scale.Y; }
        }

        public float Width
        {
            get { return Texture.Width * Scale.X; }
        }

        public Boolean Intersects(Actor actor)
        {
            return BoundingBox.Intersects(actor.BoundingBox);
        }

        public void Disable()
        {
            Enabled = false;
        }

        public bool IsEnabled()
        {
            return Enabled;
        }

        public void AddChild(IGameNode node)
        {
            Children.Add(node);
        }

        public void DetachChild(IGameNode node)
        {
            Children.Remove(node);
        }

        public void RemoveChild(IGameNode node)
        {
            Children.Remove(node);
            node.Disable();
        }


        public void Update()
        {

        }

        public void Draw()
        {
            State.SpriteBatch.Draw(Texture, Position, null, Color, 0f, new Vector2(0), Scale, SpriteEffects.None, 0);
        }

        public void Reset()
        {
            Disable();
        }
    }

    public class Actor : Entity, IGameNode, IDestructable
    {
        protected Boolean _confinedToWindow = false;
        protected Int32 _baseHealth;

        protected Int32 _health;

        public Int32 Health
        {
            get { return _health; }
            protected set { _health = value; }
        }



        public ActorType ActorType;

        public Vector2 Direction;



        public Vector2 Acceleration;

        public Vector2 Velocity;

        public Vector2 MaxVelocity;

        public Vector2 MaxAcceleration;

        public Boolean Invincible;

        protected Int32 InvincibleTimeLeft;


        public Actor(float width, float height)
            : base(width, height)
        {
        }

        public new void Update()
        {
            base.Update();

            var newPosition = Position + Velocity;

            if (State.WithinBounds(new Rectangle((int)newPosition.X, (int)newPosition.Y, (int)Width, (int)Height)))
                Position += Velocity;
            else
            {
                if (_confinedToWindow)
                    Velocity = Acceleration = Vector2.Zero;
            }

            if (InvincibleTimeLeft > 0)
                InvincibleTimeLeft--;

            if (InvincibleTimeLeft == 0 && Invincible)
                Invincible = false;
        }

        public new void Draw()
        {
            var color = Color;
            if (Invincible)
                color = Color.DarkGray;
            State.SpriteBatch.Draw(Texture, Position, null, color, 0f, new Vector2(0), Scale, SpriteEffects.None, 0);
        }

        public new void Reset()
        {
            Acceleration = Vector2.Zero;
            Velocity = Vector2.Zero;
            base.Reset();
        }


        // Main game rules for collision
        public void Collided(GameNode p)
        {
            switch (p.NodeType)
            {
                case GameNodeType.Projectile:
                    Collided((Projectile)p);
                    break;
                case GameNodeType.Player:
                    Collided((Player)p);
                    break;

            }
        }

        public void Collided(Player p)
        {
            p.OnCollision(this);
        }
        public void Collided(Projectile p)
        {
            switch (NodeType)
            {
                case GameNodeType.Enemy:
                    ((Enemy)this).OnCollision(p);
                    break;
                case GameNodeType.Player:
                    ((Player)this).OnCollision(p);
                    break;
                case GameNodeType.Projectile:
                    ((Projectile)this).OnCollision(p);
                    break;
                case GameNodeType.Ship:
                    ((Ship)this).OnCollision(p);
                    break;
            }
        }


        public void Collided(Asteroid a)
        {

        }

        public void OnPostCollide()
        {

        }

        public virtual void OnCollision(Projectile p)
        {

        }

        public virtual void OnCollision(Actor a)
        {

        }

        public virtual void OnCollision(IDestructable d)
        {

        }

        public virtual void OnCollision(Ship s)
        {

        }
        public void CheckHealth()
        {
            if (_health <= 0)
                Enabled = false;
        }

        public new void Dispose()
        {
            base.Update();
            State.Remove(this);
        }
    }



}
