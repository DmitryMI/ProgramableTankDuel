using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{

    static class Extensions
    {
        public enum Axis
        {
            X, Y, Z
        }

        public enum ClockRotation
        {
            Clockwise,  Counterclockwise
        }

        public static Quaternion SetupEuler(this Quaternion quat, Axis axis, float value)
        {
            Vector3 euler = quat.eulerAngles;
            switch (axis)
            {
                case Axis.X:
                    euler.x = value;
                    break;
                case Axis.Y:
                    euler.y = value;
                    break;
                case Axis.Z:
                    euler.z = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("axis", axis, null);
            }

            quat = Quaternion.Euler(euler);
            return quat;
        }

        public static float GetClosestDirection(float targetAngle, float currAngle, float eps)
        {
            float delta = targetAngle - currAngle;
            if (Mathf.Abs(delta) < eps)
                return 0; 

            if (delta > 180f)
                delta -= 360f;
            else if (delta <= -180f)
                delta += 360f;

            if (delta > 0)
                return 1;

            return -1;
        }

        [Obsolete("Working not correctly. Use Mathf.DeltaAngle() instead")]
        public static float AngleDifference(float a, float b)
        {
            float r1, r2;
            if (a > b)
            {
                r1 = a - b;
                r2 = b - a + 180;
            }
            else
            {
                r1 = b - a;
                r2 = a - b + 180;
            }
            return Mathf.Abs((r1 > r2) ? r2 : r1);
        }

        public static float AngleNormalize(float a)
        {
            float abs = Mathf.Abs(a);
            int circles = (int) a / 360;
            float r = abs - circles * 360;
            float b = r * Math.Sign(a); //return
            if (b < 0) //
                b += 360; //
            return b;
        }

        public static float AngleTranslate(float a)
        {
            //a = AngleNormalize(a);
            if (a > 180)
                return -(360 - a);
            return a;
        }

        private static Vector3 VecAbs(Vector3 vec)
        {
            return new Vector3(Mathf.Abs(vec.x), Mathf.Abs(vec.y), Mathf.Abs(vec.z));
        }

        public static float GetDirection(Vector2 from, Vector2 to)
        {
            to -= from;
            return Mathf.Rad2Deg * Mathf.Atan2(to.y, to.x);
        }

        public static float GetDirectionCoords(float srcX, float srcY, float tarX, float tarY)
        {
            Vector2 to = new Vector2(tarX, tarY);
            Vector2 from = new Vector2(srcX, srcY);
            to -= from;
            return AngleNormalize(Mathf.Rad2Deg * Mathf.Atan2(to.y, to.x));
        }

        public static Vector3 ClampInBounds(Vector3 position, Bounds bounds, float clampOffset)
        {
            Debug.DrawRay(Vector3.zero, bounds.max);
            if (
                position.x <= bounds.max.x &&
                position.y <= bounds.max.y && 
                position.z <= bounds.max.z &&
                position.x >= bounds.min.x &&
                position.y >= bounds.min.y &&
                position.z >= bounds.min.z)
                return position;
            Vector3 nPos = bounds.ClosestPoint(position);
            if(clampOffset > 0.001)
                nPos = Vector3.ClampMagnitude(nPos, nPos.magnitude - clampOffset);
            return nPos;
        }

        public static Vector2 RotateVec2(Vector2 point, float angle)
        {
            angle *= Mathf.Deg2Rad;
            Vector2 rotatedPoint;
            rotatedPoint.x = point.x * Mathf.Cos(angle) - point.y * Mathf.Sin(angle);
            rotatedPoint.y = point.x * Mathf.Sin(angle) + point.y * Mathf.Cos(angle);
            return rotatedPoint;
        }


        public static Color GetTransperentColor()
        {
            return new Color(0, 0, 0, 0);
        }

        public static Color SetAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        public static Transform FindInChildren(Transform src, string name)
        {
            for (int i = 0; i < src.childCount; i++)
            {
                if (src.GetChild(i).gameObject.name == name)
                    return src.GetChild(i);
                Transform t = FindInChildren(src.GetChild(i), name);
                if (t != null)
                    return t;
            }
            return null;
        }

        public static Vector2 RandomPointInRect(Bounds bounds)
        {
            float x = Random.value * (bounds.max.x - bounds.min.x) + bounds.min.x;
            float y = Random.value * (bounds.max.y - bounds.min.y) + bounds.min.y;
            return new Vector2(x, y);
        }

        public static int ToInt(object obj)
        {
            if (obj is int)
                return (int)obj;
            return 0;
        }

        public static double ToDouble(object obj)
        {
            if (obj is double)
                return (double)obj;
            return 0;
        }

        public static Vector2 ToVector2(object obj)
        {
            if (obj is Vector2)
                return (Vector2)obj;
            return Vector2.zero;
        }

    }
}
