using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets
{
    public class ColorPickerItem : MonoBehaviour
    {
        void Start ()
        {
            int i = 1;
            for (; i < transform.parent.childCount; i++)
            {
                if (transform.parent.GetChild(i) == transform)
                    break;
            }
            //Debug.Log(i);
            if(i-1 < ColorPicker.Colors.Length)
                GetComponent<Image>().color = ColorPicker.Colors[i - 1];
            //StartCoroutine(wait());
        }

        IEnumerator wait()
        {
            yield return new WaitForSeconds(0.5f);
            
        }
	
        void Update ()
        {
		
        }
    }
}
