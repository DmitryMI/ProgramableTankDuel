using System.Security.Policy;
using UnityEngine;

namespace Assets.Scripts.LuaInteraction
{
    public class VehicleInfo : Obstacle
    {
        public VehicleInfo(float width, float height, GameObject bind) : base(width, height, bind)
        {
        }

        public float HP
        {
            get { return Bind.GetComponent<Tank>().HP; }
        }

        public Color Color
        {
            get
            {
                Color my = Bind.GetComponent<Tank>().GetTeam();
                return my;
            }
        }

        public bool IsAlly(Color color)
        {
            Color my = Color;
            return my.Equals(color);
        }

        public bool IsAllyInfo(VehicleInfo info)
        {
            return Color.Equals(info.Color);
        }


        internal void SendMessage(Message msg)
        {
            Tank tank = Bind.GetComponent<Tank>();
            //Debug.Log("Pushing message to: " + tank.name);
            tank.PushMessage(msg);
        }

    }
}
