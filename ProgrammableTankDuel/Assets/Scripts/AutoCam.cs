using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class AutoCam : MonoBehaviour
    {

        public float MouseSpeed;
        public float MouseWheelSpeed;

        public float MaxCamSpeed;
        public float SpeedDelta;
        public float SmoothnessRange;
        public float MinCamSpeed;

        public Vector3 Offset;
        public float AttackAngle;

        public enum CamMode
        {
            Auto, Mouse
        }

        public float MinHeight;
        public CamMode Mode = CamMode.Auto;
        private GameObject[] _players;
        public GameObject TargetPointerPrefab;

        private Vector3 _wantsPosition;
        private float _prevSpeed;
        private bool _gameStarted;
        private GameObject _tarPointer = null;

        public void UpdateTankInfo()
        {
            Tank[] tanks = FindObjectsOfType<Tank>();
            _players = new GameObject[tanks.Length];
            for (int i = 0; i < tanks.Length; i++)
            {
                _players[i] = tanks[i].gameObject;
            }

            //_wantsPosition = _cam.transform.position;
        }

        public void Setup()
        {
            _gameStarted = true;
            StartCoroutine(MoveSmoothly());
        }
	
        // Update is called once per frame
        void Update ()
        {
            if (!_gameStarted)
                return;

            UpdateTankInfo();

            if (Input.GetButtonDown("Jump"))
            {
                switch (Mode)
                {
                    case CamMode.Auto:
                        Mode = CamMode.Mouse;
                        if (_tarPointer == null)
                        {
                            _tarPointer = Instantiate(TargetPointerPrefab, Vector3.zero, Quaternion.identity,
                                GameObject.Find("Canvas").transform);
                        }
                        break;
                    case CamMode.Mouse:
                        Mode = CamMode.Auto;
                        if (_tarPointer != null)
                        {
                           Destroy(_tarPointer);
                        }
                        Cursor.lockState = CursorLockMode.None;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            if (Mode == CamMode.Auto)
            {
                if (_players.Length == 0)
                    return;

                float summx = 0, summy = 0;
                foreach (var p in _players)
                {
                    summx += p.transform.position.x;
                    summy += p.transform.position.y;
                }

                var middle = new Vector3(summx / _players.Length, summy / _players.Length, 0);

                float maxDist = 0;
                foreach (var p in _players)
                {
                    float dist = Vector3.Distance((Vector2) p.transform.position, middle);
                    if (dist > maxDist)
                        maxDist = dist;
                }

                float angle = gameObject.GetComponent<Camera>().fieldOfView * Mathf.Deg2Rad / 2;
                
                float z = Mathf.Sin(Mathf.PI / 2 - angle) * maxDist / Mathf.Sin(angle);

                if (z < MinHeight)
                    z = MinHeight;
                _wantsPosition = new Vector3(middle.x, middle.y, -z) + Offset;
                //gameObject.transform.position = new Vector3(middle.x, middle.y, -z);

            }

            if (Mode == CamMode.Mouse)
            {
                MovePointer();
                Cursor.lockState = CursorLockMode.Locked;
                float x = Input.GetAxis("Mouse X") * MouseSpeed;
                float y = Input.GetAxis("Mouse Y") * MouseSpeed;
                float wheel = Input.GetAxis("Mouse ScrollWheel") * MouseWheelSpeed;
                gameObject.transform.position += new Vector3(x, y, wheel);
            }
        }

        void MovePointer()
        {
            if(_tarPointer == null)
                return;

            _tarPointer.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        IEnumerator MoveSmoothly()
        {
            while (true)
            {
                if (Mode == CamMode.Auto)
                {
                    Vector3 delta = (_wantsPosition) - gameObject.transform.position;
                    float magn = delta.magnitude;
                    if (magn > SmoothnessRange)
                        _prevSpeed += Time.deltaTime * SpeedDelta;
                    else
                    {
                        _prevSpeed -= Time.deltaTime * SpeedDelta;
                    }
                    if (_prevSpeed < MinCamSpeed)
                        _prevSpeed = MinCamSpeed;
                    if (_prevSpeed > MaxCamSpeed)
                        _prevSpeed = MaxCamSpeed;

                    if (magn > _prevSpeed)
                    {
                        if (magn > 0)
                            delta *= 1 / magn;
                        delta *= _prevSpeed;
                    }

                    gameObject.transform.position += delta;
                }
                yield return new WaitForEndOfFrame();
            }
        }

        void OnDestroy()
        {
            Cursor.lockState = CursorLockMode.None;
        }

    }

    
}
