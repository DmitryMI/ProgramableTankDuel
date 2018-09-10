using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tests
{
    public class DifferenceTest : MonoBehaviour
    {
        public InputField
            AText,
            BText;
        public Text ResultText;
        public void Click()
        {
            try
            {
                float a = (float) Double.Parse(AText.text);
                float b = (float) Double.Parse(BText.text);

                //float angleDiff = Extensions.AngleDifference(a, b);
                float angleDiff = Mathf.DeltaAngle(a, b);
                ResultText.text = angleDiff.ToString();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }
    }
}
