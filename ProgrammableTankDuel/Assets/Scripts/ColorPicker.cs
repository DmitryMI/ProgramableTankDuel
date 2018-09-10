using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    public static Color[] Colors = new Color[]
    {
        Color.white,
        Color.red,
        Color.yellow,
        Color.green,
        Color.blue,
        Color.magenta,
        Color.black,
        Color.cyan,
        Color.gray
    };

    void Start()
    {
        //SetIndex(0);
    }

    public void SetIndex(int i)
    {
        GetComponent<Dropdown>().value = i;
    }

    public int GetIndex()
    {
        return GetComponent<Dropdown>().value;
    }

    public void OnValChanged()
    {
        int i = GetComponent<Dropdown>().value;
        GetComponent<Image>().color = ColorPicker.Colors[i];
    }
}
