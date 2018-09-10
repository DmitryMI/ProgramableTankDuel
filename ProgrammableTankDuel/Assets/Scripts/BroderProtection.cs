using UnityEngine;

namespace Assets.Scripts
{
    public class BroderProtection : MonoBehaviour
    {

        // Use this for initialization
        void Start () {
		
        }
	
        // Update is called once per frame
        void Update () 
        {
		
        }

        void OnTriggerExit2D(Collider2D other)
        {
            // If bullet
            ShellComponent shell = other.gameObject.GetComponent<ShellComponent>();
            if (shell != null)
            {
                shell.Explode();
                return;
            }

            // If placable
            IPlaceable placeable = other.gameObject.GetComponent<IPlaceable>();
            if (placeable != null)
            {

                placeable.GameObject.transform.position =
                    Extensions.ClampInBounds(placeable.GameObject.transform.position, GetComponent<Collider2D>().bounds,
                        0.00f);
            }
        }
    }
}
