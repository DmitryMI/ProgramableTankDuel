﻿using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Logger : MonoBehaviour
    {
        private Text _text;
        private string _textBuffer;

        public static string GetColorModificator(Color color)
        {
            string res = "#";
            int r = (int)(255 * color.r);
            int g = (int)(255 * color.g);
            int b = (int)(255 * color.b);

            string rs = r.ToString("X");
            if (r <= 0xF)
                rs = "0" + rs;
            string gs = g.ToString("X");
            if (g <= 0xF)
                gs = "0" + gs;
            string bs = b.ToString("X");
            if (b <= 0xF)
                bs = "0" + bs;
            res += rs + gs + bs;
            return res;
        }

        void Awake()
        {
            _text = GetComponent<Text>();
            //Print("Initiated!", Color.red, true, false);
        }

        public void Print(string message, Color color, bool bold, bool italic)
        {
            string openingTags = "<Color=" + GetColorModificator(color) + ">";

            string closingTags = "</Color>";
            if (bold)
            {
                openingTags = "<b>" + openingTags;
                closingTags += "</b>";
            }
            if (italic)
            {
                openingTags += "<i>" + openingTags;
                closingTags += "</i>";
            }

            //_text.text += openingTags + message + closingTags;
            _textBuffer += openingTags + message + closingTags;
            _text.text += openingTags + message + closingTags;
        }

        public void Print(string message)
        {
            _text.text += message;
        }

        public void Endl()
        {
            _text.text += '\n';
            _textBuffer += '\n';
        }

        public void SetText(string text)
        {
            _text.text = text;
            _textBuffer = text;
        }

        public string GetText()
        {
            return _textBuffer;
        }
    }
}
