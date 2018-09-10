using System;
using UnityEngine;

namespace Assets.Scripts.LuaInteraction
{
    public class ReleasedShell : Obstacle
    {
        public ReleasedShell(GameObject bind) : base(0, 0, bind)
        {
            Bind = bind;
        }

        public float Speed
        {
            get
            {
                if(IsAlive)
                    return Bind.GetComponent<Rigidbody2D>().velocity.magnitude;
                return 0;
            }
        }
    }
}
