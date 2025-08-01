using System.Net;
using UnityEngine;

namespace ProceduralSwordGenerator {
    public class MathsUtil : MonoBehaviour {
        static public float CalculateXOnLinearLine(float y, Vector2 a, Vector2 b) {

            float m = CalculateGradient(a, b, out bool undefined);

            // If undefined a.x = b.x
            if (undefined) {
                return a.x;

                // Else calculate x for given y
            } else {
                return (y - a.y) * (1.0f / m) + a.x;

            }

        }

        static public float CalculateYOnLinearLine(float x, Vector2 a, Vector2 b) {
            float m = CalculateGradient(a, b, out bool undefined);

            // If undefined a.y = b.y - b.y can be any real number but this shouldnt ever happen with intended use case
            if (undefined) {
                return a.y;

                // Else calculate y for given x
            } else {
                return m * (x - a.x) + a.y;

            }

        }

        static public float CalculateGradient(Vector2 a, Vector2 b, out bool undefined) {
            float deltaX = b.x - a.x;

            // Checks if dividing by 0 - undefined
            if (deltaX == 0) {
                undefined = true;
                return 0;

            }

            undefined = false;
            return (b.y - a.y) / deltaX;

        }

        static public float CalculateFactor(float point, float length) {
            return length == 0 ? 0 : point / length;

        }

        static public Vector3 CalculateNormal(Vector3 a, Vector3 b, Vector3 c) {
            Vector3 ab = b - a;
            Vector3 ac = c - a;

            return Vector3.Cross(ab, ac).normalized;

        }

        static public float CalcXOnCircle(float y, float radius, Vector2 origin) {
            float x = Mathf.Sqrt(Mathf.Abs(Mathf.Pow(radius, 2) - Mathf.Pow(y - origin.y, 2)));

            // Check if x is positive or negative, then return
            return Mathf.Approximately(radius, CalcRadiusOfCircle(new Vector2(x, y), origin)) ? x + origin.x : -x + origin.x;

        }

        static public float CalcYOnCircle(float x, float radius, Vector2 origin) {
            float y = Mathf.Sqrt(Mathf.Abs(Mathf.Pow(radius, 2) - Mathf.Pow(x - origin.x, 2)));

            // Check if x is positive or negative, then return
            return Mathf.Approximately(radius, CalcRadiusOfCircle(new Vector2(x, y), origin)) ? y + origin.y : -y + origin.y;

        }

        static public void GetEquationOfCircle(Vector2 a, Vector2 b, out float radius, out Vector2 origin) {
            Vector2 midPoint = CalculateMidPoint(a, b);

            float gradient = CalculateGradient(a, b, out _);

            gradient = gradient != 0 ? -1 / gradient : 0;

            //float y = gradient * (a.x - midPoint.x) + midPoint.y;

            //origin = new Vector2(a.x, y);

            float x = gradient != 0 ? (a.y - midPoint.y) / gradient + midPoint.x : 0;

            origin = new Vector2(x, a.y);

            radius = CalcRadiusOfCircle(a, origin);

        }

        static public float CalcRadiusOfCircle(Vector2 a, Vector2 origin) {
            return Mathf.Sqrt(Mathf.Abs(Mathf.Pow(a.x - origin.x, 2) + Mathf.Pow(a.y - origin.y, 2)));

        }

        static public Vector2 CalculateMidPoint(Vector2 a, Vector2 b) {
            return a + (b - a) * 0.5f;

        }

        static public float CalcVolumneOfSquarePyramid(float length, float height) {
            return CalcVolumneOfRectPyramid(length, length, height);

        }

        static public float CalcVolumneOfRectPyramid(float length, float width, float height) {
            // V = l * w * h * 1/3
            return (length * width * height) / 3.0f;

        }

        static public float CalcVolumeOfTriPrism(Vector2 A, Vector2 B, Vector2 C, float height) {
            float a = (B - A).magnitude;
            float b = (C - B).magnitude;
            float c = (C - A).magnitude;

            // V = 1/4 * h * (-a^4 + 2(ab)^2 + 2(ac)^2 - b^4 + 2(bc)^2 - c^4)^1/2
            return 0.25f * height * Mathf.Sqrt(Mathf.Abs(-Mathf.Pow(a, 4) + 2 * Mathf.Pow(a * b, 2) + 2 * Mathf.Pow(a * c, 2) - Mathf.Pow(b, 4) + 2 * Mathf.Pow(b * c, 2) - Mathf.Pow(c, 4)));

        }


    }

}
