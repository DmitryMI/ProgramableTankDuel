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

        public float PlayerMaxHeight;
        public float PlayerMaxDistance;
        public float MaxPlayerCamSpeed;
        public float PlayerSpeedDelta;

        public Vector3 Offset;
        public float AttackAngle;

        public enum CamMode
        {
            Auto, PlayerCentered
        }

        public float MinHeight;
        private CamMode _mode = CamMode.Auto;
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

        public Vector3 PlayerTankPostion { get; set; }

        public CamMode Mode { get { return _mode; } set { _mode = value; } }
	
        // Update is called once per frame
        void Update()
        {
            if (!_gameStarted)
                return;

            UpdateTankInfo();
            
            if (_mode == CamMode.Auto)
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

            if (_mode == CamMode.PlayerCentered)
            {

                if (_players.Length == 0)
                    return;

                var middle = PlayerTankPostion;

                float maxDist = 0;
                foreach (var p in _players)
                {
                    //if(p.GetComponent<ManualTankController>() != null)
                        //continue;
                    float dist = Vector3.Distance((Vector2)p.transform.position, middle);
                    if (dist > maxDist)
                        maxDist = dist;
                }

                float angle = gameObject.GetComponent<Camera>().fieldOfView * Mathf.Deg2Rad / 2;

                float z = Mathf.Sin(Mathf.PI / 2 - angle) * maxDist / Mathf.Sin(angle);

                if (z < MinHeight)
                    z = MinHeight;

                if (z > PlayerMaxHeight)
                    z = PlayerMaxHeight;

                _wantsPosition = new Vector3(middle.x, middle.y, -z) + Offset;
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


                //if (Mode == CamMode.Auto)

                float speedDelta;
                if (_mode == CamMode.Auto)
                    speedDelta = SpeedDelta;
                else
                {
                    speedDelta = PlayerSpeedDelta;
                }

                Vector3 delta = (_wantsPosition) - gameObject.transform.position;

                if (_mode == CamMode.PlayerCentered && delta.magnitude > PlayerMaxDistance)
                {
                    gameObject.transform.position = _wantsPosition;
                }
                else
                {

                    float magn = delta.magnitude;
                    if (magn > SmoothnessRange)
                        _prevSpeed += Time.deltaTime * speedDelta;
                    else
                    {
                        _prevSpeed -= Time.deltaTime * speedDelta;
                    }
                    if (_prevSpeed < MinCamSpeed)
                        _prevSpeed = MinCamSpeed;

                    if (_mode == CamMode.Auto)
                    {
                        if (_prevSpeed > MaxCamSpeed)
                            _prevSpeed = MaxCamSpeed;
                    }

                    if (_mode == CamMode.PlayerCentered)
                    {
                        if (_prevSpeed > MaxPlayerCamSpeed)
                            _prevSpeed = MaxPlayerCamSpeed;
                    }

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
