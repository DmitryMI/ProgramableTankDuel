using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.LuaInteraction
{
    public class TankWrapper
    {
        private Tank _tank;
        private float _aiDelay;

        public VehicleInfo GetMyVehicleInfo()
        {
            return new VehicleInfo(_tank.Width, _tank.Height, _tank.gameObject);
        }

        public void SendMessage(object msg, VehicleInfo target)
        {
            //Debug.Log("Sending message: " + msg + " to target: " + target);
            Message message = new Message
            {
                Data = msg,
                Sender = _tank.gameObject
            };
            target.SendMessage(message);
        }

        public MessageWrapper ReadMessage()
        {
            Message msg = _tank.PopMessage();
            if (msg.Data == null)
            {
                MessageWrapper res = new MessageWrapper();
                res.Data = null;
                res.Sender = new VehicleInfo(0, 0, null);
                return res;
            }
            GameObject sender = msg.Sender;
            object data = msg.Data;
            MessageWrapper wrapped = new MessageWrapper();
            wrapped.Data = data;
            Tank tank = sender.GetComponent<Tank>();
            wrapped.Sender = new VehicleInfo(tank.Width, tank.Height, sender);
            return wrapped;
        }

        public TankWrapper(Tank tank, float aiDelay)
        {
            _tank = tank;
            _aiDelay = aiDelay;
        }

        public void MoveForward()
        {
            _tank.MoveForward(_tank.GetMaxSpeed, _aiDelay);
        }

        public void FullStop()
        {
            _tank.MoveForward(0, 0);
        }

        public void MoveBackward()
        {
            //Debug.Log("BACKWARD");
            _tank.MoveForward(-_tank.GetMaxSpeed, _aiDelay);
        }

        public void MoveTo(float x, float y)
        {
            _tank.MoveTo(new Vector2(x, y));
        }

        public void SetTowerRotation(float angle)
        {
            _tank.TowerRotation = angle;
        }

        public float GetTowerRotation()
        {
            return _tank.TowerRotation;
        }

        public void SetRotation(float angle)
        {
            _tank.Rotation = angle;
        }

        public float GetRotation()
        {
            return _tank.Rotation;
        }

        public float GetCooldown()
        {
            return _tank.Cooldown;
        }

        public void Shoot()
        {
            _tank.Shoot(_tank.ShellPrefabs[0], _tank.ShellSpeed);
        }

        public float GetX()
        {
            return _tank.transform.position.x;
        }

        public float GetY()
        {
            return _tank.transform.position.y;
        }

        public Color GetTeam()
        {
            return _tank.GetTeam();
        }

        public float GetHp()
        {
            return _tank.HP;
        }

    }

}
