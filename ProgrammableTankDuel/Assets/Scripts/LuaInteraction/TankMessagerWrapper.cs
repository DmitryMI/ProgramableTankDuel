using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.LuaInteraction
{
    class TankMessagerWrapper
    {
        private TankTextMessager _messager;

        public TankMessagerWrapper(TankTextMessager messager)
        {
            _messager = messager;
        }

        public void PrintTimed(string message, Color color, bool bold, bool italic, float seconds)
        {
            //_messager.DisplayMessageTimed(message, color, bold, italic, seconds);
        }

        public void PrintRawTimed(string message, float seconds)
        {
            //_messager.DisplayMessageTimed(message, Color.black, false, false, seconds);
            _messager.PrintRawTimed(message, seconds);
        }

        public void PrintRawAuto(string message)
        {
            //_messager.DisplayMessageTimed(message, Color.black, false, false, 1.0f + message.Length * 0.5f);
        }
    }
}
