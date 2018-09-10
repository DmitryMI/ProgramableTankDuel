using UnityEngine;

namespace Assets.Scripts
{
    public interface IPlaceable
    {
        float X { get; }
        float Y { get; }
        float Width { get; }
        float Height { get; }
        GameObject GameObject { get; }
        void ReceiveDamage(float amount);
    }
}
