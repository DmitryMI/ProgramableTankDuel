using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class TankTextMessenger : MonoBehaviour
    {
        [SerializeField] private GameObject _messageBoxPrefab;
        [SerializeField]
        private Vector2 _offset;

        private Transform _canvasTransform;
        private GameObject _messageBox;
        private Text _text;

        
        private RectTransform _rectTransform;

        private TextPrinter _textPrinter;

        private float _timeLeft;
        private bool _started = false;

        private void Start()
        {
            if (!_started)
            {
                Init();
                _started = true;
            }
        }

        private void Init()
        {
            StartCoroutine(Countdown());

            _canvasTransform = FindObjectOfType<Canvas>().gameObject.transform;

            _messageBox = Instantiate(_messageBoxPrefab, _canvasTransform);

            _rectTransform = _messageBox.GetComponent<RectTransform>();

            _text = _messageBox.GetComponentInChildren<Text>();
            _textPrinter = new TextPrinter(_text);

            _messageBox.SetActive(false);
        }

        public void PrintRawTimed(string message, float time)
        {
            if (!_started)
            {
                Init();
                _started = true;
            }

            //_textPrinter.SetText(message);

            if(String.IsNullOrEmpty(message))
                Debug.Log("Message is null or empty");

            _text.text = message;
            _messageBox.SetActive(true);
            _timeLeft = time;
        }

        private void Update()
        {
            Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
            pos += _offset;
            _rectTransform.position = pos;

            if (_timeLeft <= 0 && _messageBox.activeSelf)
            {
                _messageBox.SetActive(false);
            }
        }

        void OnDestroy()
        {
            Destroy(_messageBox);
        }

        IEnumerator Countdown()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                _timeLeft -= Time.deltaTime;

                if (_timeLeft < 0)
                    _timeLeft = 0;
            }
        }
    }
}
