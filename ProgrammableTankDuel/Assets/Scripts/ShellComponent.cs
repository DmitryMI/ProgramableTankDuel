using UnityEngine;

namespace Assets.Scripts
{
    
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public class ShellComponent : MonoBehaviour, IExplodable
    {
        public float ExplosionRadius; // 0 Means on-impact damage only
        public float EpicenterDamage; // Damage dealt to a unit, which is in the center of explosion
        public float ContactDamage;
        private bool _exploded;

        [SerializeField]
        private Explosion _explosionModelPrefab;
        public GameObject Owner;
        public string BorderFieldName;

        private Collider2D _contact;

        protected virtual float CalculateDamage(float distance)
        {
            // y = -max / dist + max;
            float dmg = -EpicenterDamage / ExplosionRadius * distance + EpicenterDamage;
            if (dmg > EpicenterDamage)
                return EpicenterDamage;
            if (dmg < 0)
                return 0;
            return dmg;
        }

        public Explosion ExplosionModel { get { return _explosionModelPrefab; }set { _explosionModelPrefab = value; }}

        public void Explode()
        {
            if(_exploded)
                return;
            _exploded = true;
            if (ExplosionModel != null)
            {
                GameObject explosion = Instantiate(ExplosionModel.gameObject, transform.position, transform.rotation);
                explosion.GetComponent<Explosion>().Init(4.0f);
            }
            GameObject[] targets = FindObjectsOfType<GameObject>();

            // Damage contacter
            if (_contact != null)
            {
                GameObject contacter = _contact.gameObject;
                IPlaceable placeable = contacter.GetComponent<IPlaceable>();
                if (placeable != null)
                {
                    placeable.ReceiveDamage(ContactDamage);
                }
            }

            foreach (var tar in targets)
            {
                IPlaceable placeable = tar.GetComponent<IPlaceable>();
                if(placeable == null)
                    continue;
                //float dist = Vector2.Distance(transform.position, tar.transform.position);
                float dist = Physics2D.Distance(transform.GetComponent<Collider2D>(),
                    tar.transform.GetComponent<Collider2D>()).distance;
                if (dist >= ExplosionRadius)
                    continue;
                float dmg = CalculateDamage(dist);
                if(dmg > 0)
                    tar.GetComponent<IPlaceable>().ReceiveDamage(dmg);
            }
            Destroy(gameObject);
        }

        public void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.gameObject != Owner && BorderFieldName != coll.gameObject.name)
            {
                _contact = coll;
                Explode();
            }
        }
    }
}
