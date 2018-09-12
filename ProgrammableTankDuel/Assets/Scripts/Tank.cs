using System;
using System.Collections.Generic;
using Assets.Scripts.LuaInteraction;
using UnityEngine;

namespace Assets.Scripts
{
    public class Tank : MonoBehaviour, IPlaceable, IExplodable
    {

        public float TowerMaxRotationSpeed;
        public float MaxRotationSpeed;
        public float MaxCooldown;
        public float MaxHp = 100.0f;
        public int MaxMessages = 10;

        public GameObject[] ShellPrefabs;
        [SerializeField]
        private Explosion _explosionModelPrefab;
        public float ShellSpeed;

        [SerializeField]
        private float _hp = 100.0f;
        private float _cooldown;
        [SerializeField] private GameObject _fireEffect;
        private Color _teamColor;
        [SerializeField]
        private float _speed;

        private bool _exploded;

        private Queue<Message> _messageQueue = new Queue<Message>();

        internal Message PopMessage()
        {
            if(_messageQueue.Count != 0)
                return _messageQueue.Dequeue();
            Message msg = new Message();
            msg.Data = null;
            msg.Sender = null;
            return msg;
        }

        internal void PushMessage(Message msg)
        {
            if (MaxMessages > _messageQueue.Count)
                _messageQueue.Enqueue(msg);
        }

        public Color GetTeam()
        {
            return _teamColor;
        }

        public float GetMaxSpeed
        {
            get { return _speed; }
        }

        public void SetTeam(Color color)
        {
            Transform flagTransform = Extensions.FindInChildren(gameObject.transform, "Flag");
            if (flagTransform == null)
                Debug.LogError("Flag not found!");
            else
            {
                _flag = flagTransform.gameObject;
            }
            _teamColor = color;
            _flag.GetComponent<MeshRenderer>().material.color = color;
        }

        private GameObject _flag;
        private GameObject _tower;
        
        private float _tmpSpeed;

        private Rigidbody2D _rig;

        [SerializeField]
        private float _wantsRotation;
        [SerializeField]
        private float _towerWantsRotation;
        private Vector2 _wantsPosition;
        [SerializeField]
        private bool _autoMove;
        private float _moveTimeout;

        // Use this for initialization
        void Awake ()
        {
            _rig = GetComponent<Rigidbody2D>();
            Transform towerTran = Extensions.FindInChildren(gameObject.transform, "Tower");
            if(towerTran == null)
                Debug.LogError("Tower not found!");
            else
            {
                _tower = towerTran.gameObject;
            }
            Transform flagTransform = Extensions.FindInChildren(gameObject.transform, "Flag");
            if (flagTransform == null)
                Debug.LogError("Flag not found!");
            else
            {
                _flag = flagTransform.gameObject;
            }

            TowerRotation = Rotation;
        }

        void Turn(float dir)
        {
            float a = transform.rotation.eulerAngles.z;
            //float rot = Extensions.GetClosestDirection(dir, a, 0.01f);
            float diff = Mathf.DeltaAngle(a, dir);
            if (Mathf.Abs(diff) > 0.1f)
            {
                _rig.angularVelocity = Mathf.Sign(diff) * MaxRotationSpeed; // rot
            }
            else
            {
                _rig.angularVelocity = diff;
            }

        }

        void TurnTower()
        {
            float rotD = TowerMaxRotationSpeed * Time.deltaTime;
            //float dir = Extensions.GetClosestDirection(_towerWantsRotation, TowerRotation, 0.001f);
            float andgeD = Mathf.DeltaAngle(TowerRotation, _towerWantsRotation + 180);
            //Debug.Log("My: " + dir + ". His: " + Mathf.Sign(andgeD));
            if (Math.Abs(andgeD) > rotD)
            {
                _tower.transform.RotateAround(_tower.transform.position, _tower.transform.forward, rotD * Mathf.Sign(andgeD)); // - rot * dir
            }
            else
            {
                //_tower.transform.localRotation = _tower.transform.localRotation.SetupEuler(Extensions.Axis.Z, 360 -_towerWantsRotation);
                _tower.transform.RotateAround(_tower.transform.position, _tower.transform.forward, andgeD);
            }
        }

        void Update ()
        {
            if (_autoMove)
            {
                if (Vector2.Distance(_wantsPosition, Position) > 1)
                {
                    _rig.velocity = transform.right * _tmpSpeed * Time.deltaTime;
                    float dir = Extensions.GetDirection(Position, _wantsPosition);
                    Turn(dir);
                }
            }
            else
            {
                if (_moveTimeout > 0)
                {
                    _moveTimeout -= Time.deltaTime;
                    _rig.velocity = transform.right * _tmpSpeed * Time.deltaTime;
                }
                else
                {
                    _rig.velocity = Vector2.zero;
                    _moveTimeout = 0;
                }
                Turn(_wantsRotation);
            }

            TurnTower();
            
            // Cooldown
            _cooldown -= Time.deltaTime;
        }

        public float Rotation
        {
            get
            {
                return Extensions.AngleNormalize(transform.rotation.eulerAngles.z);
            }
            set
            {
                //Debug.Log("Setting + " + value);
                _wantsRotation = value;
            }
        }

        public Vector2 Position
        {
            get
            {
                var res = new Vector2(transform.position.x, transform.position.y);
                return res;
            }
        }

        public void MoveTo(Vector2 pos)
        {
            _autoMove = true;
            _wantsPosition = pos;
            _tmpSpeed = _speed;
        }

        public float TowerRotation
        {
            get
            {
                float local = 360 - _tower.transform.localRotation.eulerAngles.z;
                float body = Rotation;
                float result = Extensions.AngleNormalize(body + local);
                return result;
            }
            set
            {
                _towerWantsRotation = Extensions.AngleNormalize(value);
            }
        }

        public void MoveForward(float speed, float time)
        {
            if (Math.Abs(speed) <= Mathf.Abs(_speed))
                _tmpSpeed = speed;
            else
            {
                _tmpSpeed = Mathf.Sign(speed) * _speed;
            }

            _autoMove = false;
            _moveTimeout = time;
        }

        public float HP
        {
            get { return _hp; }
        }

        public void ReceiveDamage(float amount)
        {
            _hp -= amount;
            if (_hp <= 0)
            {
                _hp = 0;
                Explode();
            }

            if (_hp / MaxHp <= 0.25f)
            {
                _fireEffect.SetActive(true);
            }

        }

        public float Cooldown
        {
            get { return _cooldown; }
        }

        public void Shoot(GameObject bullet, float speed)
        {
            if (_cooldown <= 0)
            {
                GetComponent<AudioSource>().Play();
                _cooldown = MaxCooldown;
                Vector3 shootPos = transform.position + _tower.transform.forward * 2.0f;
                shootPos.z = -1;
                GameObject shell = Instantiate(bullet, shootPos, Quaternion.Euler(0, 0, TowerRotation), GameObject.Find("Environment").transform);
                shell.GetComponent<ShellComponent>().Owner = gameObject;
                Physics2D.IgnoreCollision(shell.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>());
                shell.GetComponent<Rigidbody2D>().velocity = shell.transform.right * speed;
            }
        }

        public float X
        {
            get { return gameObject.transform.position.x; }
        }

        public float Y
        {
            get { return gameObject.transform.position.y; }
        }

        public float Width
        {
            get { return GetComponent<Collider2D>().bounds.size.x; }
        }

        public float Height
        {
            get { return GetComponent<Collider2D>().bounds.size.y; }
        }

        public GameObject GameObject
        {
            get { return gameObject; }
        }

        public Explosion ExplosionModel
        {
            get { return _explosionModelPrefab; }
            set { _explosionModelPrefab = value; }
        }
        public void Explode()
        {
            if (ExplosionModel != null)
            {
                Vector3 spawn = gameObject.transform.position;
                spawn.z = -2.5f;
                GameObject explosion = Instantiate(ExplosionModel.gameObject, spawn, gameObject.transform.rotation);
                //explosion.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -1);
                explosion.GetComponent<Explosion>().Init(4.0f);
            }
            //GetComponent<TankController>()
            Destroy(gameObject);
        }
    }
}
