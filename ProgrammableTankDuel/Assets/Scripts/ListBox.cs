using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ListBox : MonoBehaviour
    {
        public void Add(GameObject prefab)
        {
            Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        }

        public void RemoveLast()
        {
            Destroy(transform.GetChild(transform.childCount - 1).gameObject);
        }

        public GameObject GetAt(int i)
        {
            return transform.GetChild(i).gameObject;
        }

        public int GetColorIndex(int scr)
        {
            GameObject scrBox = transform.GetChild(scr).gameObject;
            GameObject colorPicker = scrBox.transform.Find("ForceColor").gameObject;
            return colorPicker.GetComponent<ColorPicker>().GetIndex();
        }

        public void SetColorIndex(int scr, int i)
        {
            GameObject scrBox = transform.GetChild(scr).gameObject;
            GameObject colorPicker = scrBox.transform.Find("ForceColor").gameObject;
            colorPicker.GetComponent<ColorPicker>().SetIndex(i);
        }
    }
}
