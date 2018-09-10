using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class Explosion : MonoBehaviour
    {
        public void Init(float lifetime)
        {
            StartCoroutine(Wait(lifetime));
        }

        IEnumerator Wait(float time)
        {
            yield return new WaitForSeconds(time);
            Destroy(gameObject);
        }

    }
}
