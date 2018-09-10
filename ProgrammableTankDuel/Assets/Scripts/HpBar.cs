using UnityEngine;

namespace Assets.Scripts
{
    public class HpBar : MonoBehaviour
    {
        private RectTransform _main;
        private RectTransform _shalter;

        public void Setup()
        {
            _main = GetComponent<RectTransform>();
            _shalter = transform.GetChild(0).gameObject
                .GetComponent<RectTransform>(); 
        }

        void Awake()
        {
            Setup();
            //Extensions.FindInChildren(gameObject.transform, "Shalter").gameObject.GetComponent<RectTransform>();
        }

        public void SetHp(float piece)
        {
            /*if (_main == null || _shalter == null)
            {
                Setup();
            }*/

            if (piece >= 1)
                piece = 1;
            if (piece < 0)
                piece = 0;

            //float width = _main.sizeDelta.x;
            float width = _main.rect.width;
            float maskWidth = (1 - piece) * width;
            _shalter.sizeDelta = new Vector2(maskWidth, _shalter.sizeDelta.y); ;
        }
    }
}
