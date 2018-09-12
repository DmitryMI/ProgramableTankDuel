using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    class ManualTankController : MonoBehaviour
    {
        private Tank _tank;
        private float _angle = 0;
        private AutoCam _autoCam;

        void Start()
        {
            _tank = GetComponent<Tank>();
        }

        void Update()
        {
            Move();
            RotateTower();

            UpdateCamera();
        }

        void UpdateCamera()
        {
            if (_autoCam != null)
            {
                _autoCam.PlayerTankPostion = _tank.Position;
            }
            else
            {
                _autoCam = FindObjectOfType<AutoCam>();
                _autoCam.Mode = AutoCam.CamMode.PlayerCentered;
            }
        }

        void Move()
        {
            float movement = Input.GetAxis("Vertical");
            float rotation = Input.GetAxis("Horizontal");
            //Debug.Log(rotation);
            float shoot = Input.GetAxis("Fire1");

            _tank.MoveForward(movement * _tank.GetMaxSpeed, 1.0f);
            _angle -= rotation * _tank.MaxRotationSpeed * Time.deltaTime;
            _angle = Extensions.AngleNormalize(_angle);
            //Debug.Log("Before set: " + _angle);
            _tank.Rotation = _angle;
            if (shoot > 0)
                _tank.Shoot(_tank.ShellPrefabs[0], _tank.ShellSpeed);
        }

        void RotateTower()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (!Physics.Raycast(ray, out hit))
                return;

            Vector2 mousePos = hit.point;

            float angle = Extensions.GetDirection(_tank.Position, mousePos);
            _tank.TowerRotation = angle;
        }
    }
}
