using UnityEngine;

namespace Assets.Scripts.LuaInteraction
{
    public class Obstacle
    {
        protected float _width;
        protected float _height;
        protected GameObject Bind;

        public Obstacle(float width, float height, GameObject bind)
        {
            _width = width;
            _height = height;
            Bind = bind;
        }

        public bool IsAlive
        {
            get { return Bind != null; }
        }

        public float X
        {
            get
            {
                if (IsAlive)
                    return Bind.transform.position.x;
                return 0;
            }
        }

        public float Y
        {
            get
            {
                if (IsAlive)
                    return Bind.transform.position.y;
                return 0;
            }
        }

        public float Width
        {
            get { return _width; }
        }

        public float Height
        {
            get { return _height; }
        }

        public float Direction
        {
            get
            {
                if (IsAlive)
                    return Bind.transform.rotation.eulerAngles.z;
                return 0;
            }
        }
    }
}
