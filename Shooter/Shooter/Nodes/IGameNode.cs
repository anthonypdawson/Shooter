using System;

namespace Shooter
{
    public enum GameNodeType
    {
        Actor,
        Ship,
        Enemy,
        Projectile,
        Player
    }
    public interface IDestructable
    {
        void OnCollision(Projectile p);
        void OnCollision(IDestructable p);
        void PostCollision();
        void CheckHealth();
    }
    public interface IGameNode
    {
        void AddChild(IGameNode node);

        void Disable();

        Boolean IsEnabled();

        void DetachChild(IGameNode node);

        void RemoveChild(IGameNode node);

        void Update();

        void Draw();

        void Reset();
    }

    public abstract class GameNode : IDisposable
    {
        public GameNodeType NodeType;
        public abstract void Dispose();
    }
}
