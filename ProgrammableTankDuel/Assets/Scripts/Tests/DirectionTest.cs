using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tests
{
    public class DirectionTest : MonoBehaviour
    {
        public InputField
            X1Text,
            Y1Text,
            X2Text,
            Y2Text;
        public Text ResultText;
        public void Click()
        {
            try
            {
                float x1 = (float) Double.Parse(X1Text.text);
                float y1 = (float) Double.Parse(Y1Text.text);
                float x2 = (float) Double.Parse(X2Text.text);
                float y2 = (float) Double.Parse(Y2Text.text);
                float angle = Extensions.GetDirectionCoords(x1, y1, x2, y2);
                ResultText.text = angle.ToString();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }
    }
}
