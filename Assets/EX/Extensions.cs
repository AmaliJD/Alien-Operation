// collected and expended upon by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using UnityEngine;
using static EX.MathEX;

namespace EX
{

    public static class MathExtensions {

        public static bool Within( this float v, float min, float max ) => v >= min && v <= max;
        public static bool NotWithin(this float v, float min, float max) => v <= min || v >= max;
        public static bool Between( this float v, float min, float max ) => v > min && v < max;
        public static bool NotBetween(this float v, float min, float max) => v < min || v > max;

		public static float Square( this float v ) => v * v;
		public static float Abs( this float v ) => MathEX.Abs( v );
        public static float Sign(this float v) => MathEX.Sign(v);
        public static float SignWithZero(this float v) => MathEX.SignWithZero(v);
    }

    public static class VectorExtensions
	{
        public static Vector2 Rotate90CW(this Vector2 v) => new Vector2(v.y, -v.x);
        public static Vector2 Rotate90CCW(this Vector2 v) => new Vector2(-v.y, v.x);
        public static Vector2 RotateAround(this Vector2 v, Vector2 pivot, float angle, AngleUnits units = AngleUnits.Degrees) => Rotate(v - pivot, AngleUnitConversion(angle, units, AngleUnits.Radians)) + pivot;

        public static Vector2 Rotate(this Vector2 v, float angle, AngleUnits units = AngleUnits.Degrees)
        {
            angle = AngleUnitConversion(angle, units, AngleUnits.Radians);

            float ca = Mathf.Cos(angle);
            float sa = Mathf.Sin(angle);
            return new Vector2(ca * v.x - sa * v.y, sa * v.x + ca * v.y);
        }
        public static Vector2 Abs(this Vector2 vector) => new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
        public static Vector2 SetSign(this Vector2 vector, bool positiveX, bool positiveY) => new Vector2(Mathf.Abs(vector.x) * (positiveX ? 1 : -1), Mathf.Abs(vector.y) * (positiveY ? 1 : -1));
        public static Vector2 ZeroNegatives(this Vector2 v) => new Vector2(v.x > 0 ? v.x : 0, v.y > 0 ? v.y : 0);
        public static Vector3 Add(this Vector3 vector, float x, float y) { vector.x += x; vector.y += y; return vector; }

       
        public static Vector3 SetXY(this Vector3 vector, float x, float y) { vector.x = x; vector.y = y; return vector; }
        public static Vector3 SetXZ(this Vector3 vector, float x, float z) { vector.x = x; vector.z = z; return vector; }
        public static Vector3 SetYZ(this Vector3 vector, float y, float z) { vector.y = y; vector.z = z; return vector; }
        public static Vector3 SetX(this Vector3 vector, float x) { vector.x = x; return vector; }
        public static Vector3 SetY(this Vector3 vector, float y) { vector.y = y; return vector; }
        public static Vector3 SetZ(this Vector3 vector, float z) { vector.z = z; return vector; }

        public static bool FacingUp(this Vector2 direction)
        {
            float signedAngle = Vector2.SignedAngle(direction, Vector2.up);
            return signedAngle <= 67.5f && signedAngle > -67.5f;
        }
        public static bool FacingDown(this Vector2 direction)
        {
            float signedAngle = Vector2.SignedAngle(direction, Vector2.down);
            return signedAngle <= 67.5f && signedAngle > -67.5f;
        }
        public static bool FacingRight(this Vector2 direction)
        {
            float signedAngle = Vector2.SignedAngle(direction, Vector2.right);
            return signedAngle <= 22.5f && signedAngle > -22.5f;
        }
        public static bool FacingLeft(this Vector2 direction)
        {
            float signedAngle = Vector2.SignedAngle(direction, Vector2.left);
            return signedAngle <= 22.5f && signedAngle > -22.5f;
        }

        public static Vector2 GetFacingDirection(this Vector2 direction)
        {
            if (direction.FacingDown())
                return Vector2.down;
            else if (direction.FacingUp())
                return Vector2.up;
            else if (direction.FacingLeft())
                return Vector2.left;
            else if (direction.FacingRight())
                return Vector2.right;

            return Vector2.zero;
        }
    }

    public static class ColorExtensions
    {
        public static Color SetAlpha(this Color color, float Alpha)
        {
            color.a = Alpha;
            return color;
        }
        public static Color SetRed(this Color color, float R)
        {
            color.r = R;
            return color;
        }
        public static Color SetBlue(this Color color, float G)
        {
            color.b = G;
            return color;
        }
        public static Color SetGreen(this Color color, float B)
        {
            color.g = B;
            return color;
        }
    }

    public static class LayerMaskExtensions
    {
        public static bool Contains(this LayerMask mask, int layer) => (mask & (1 << layer)) != 0;
    }

    public static class CameraExtensions
    {
        public static bool WithinCamUp(this Camera camera, Vector2 direction)
        {
            float signedAngle = Vector2.SignedAngle(direction, (Vector2)camera.transform.up);
            return signedAngle <= 67.5f && signedAngle > -67.5f;
        }
        public static bool WithinCamDown(this Camera camera, Vector2 direction)
        {
            float signedAngle = Vector2.SignedAngle(direction, -(Vector2)camera.transform.up);
            return signedAngle <= 67.5f && signedAngle > -67.5f;
        }
        public static bool WithinCamRight(this Camera camera, Vector2 direction)
        {
            float signedAngle = Vector2.SignedAngle(direction, (Vector2)camera.transform.right);
            return signedAngle <= 22.5f && signedAngle > -22.5f;
        }
        public static bool WithinCamLeft(this Camera camera, Vector2 direction)
        {
            float signedAngle = Vector2.SignedAngle(direction, -(Vector2)camera.transform.right);
            return signedAngle <= 22.5f && signedAngle > -22.5f;
        }

        public static bool SameCamDirection(this Camera camera, Vector2 direction1, Vector2 direction2) =>
            (camera.WithinCamUp(direction1) && camera.WithinCamUp(direction2)) ||
            (camera.WithinCamDown(direction1) && camera.WithinCamDown(direction2)) ||
            (camera.WithinCamRight(direction1) && camera.WithinCamRight(direction2)) ||
            (camera.WithinCamLeft(direction1) && camera.WithinCamLeft(direction2));

        public static bool OppositeCamDirection(this Camera camera, Vector2 direction1, Vector2 direction2) =>
            (camera.WithinCamUp(direction1) && camera.WithinCamDown(direction2)) ||
            (camera.WithinCamDown(direction1) && camera.WithinCamUp(direction2)) ||
            (camera.WithinCamRight(direction1) && camera.WithinCamLeft(direction2)) ||
            (camera.WithinCamLeft(direction1) && camera.WithinCamRight(direction2));

        public static string GetCameraDirection(this Camera camera, Vector2 direction)
        {
            if (camera.WithinCamDown(direction))
                return "Down";// Vector2.down;
            else if (camera.WithinCamUp(direction))
                return "Up";// Vector2.up;
            else if (camera.WithinCamRight(direction))
                return "Right";// Vector2.right;
            else if (camera.WithinCamLeft(direction))
                return "Left";// Vector2.left;

            return "None";// Vector2.zero;
        }
    }
}