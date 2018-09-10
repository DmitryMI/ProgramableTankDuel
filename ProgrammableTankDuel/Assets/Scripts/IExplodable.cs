namespace Assets.Scripts
{
    interface IExplodable
    {
        Explosion ExplosionModel { get; set; }
        void Explode();
    }
}
