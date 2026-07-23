// Lots of this code is similar to Unity's original Mathf source to match functionality.
// The original Mathf.cs source https://github.com/Unity-Technologies/UnityCsReference/blob/master/Runtime/Export/Math/Mathf.cs
// ...and the trace amounts of it left in here is copyright (c) Unity Technologies with license: https://unity3d.com/legal/licenses/Unity_Reference_Only_License
// 
// Collected and expanded upon to by Freya Holmér (https://github.com/FreyaHolmer/Mathfs)

using System;
using System.Collections.Generic;
using UnityEngine;
using Uei = UnityEngine.Internal;
using System.Linq;
//using DG.Tweening;
//using UnityEditor.Callbacks; // used for arbitrary count min/max functions, so it's safe and won't allocate garbage don't worry~

namespace EX {

	public static class MathEX {

		// Constants
		public const float TAU = 6.28318530717959f;
		public const float PI = 3.14159265359f;
		public const float E = 2.71828182846f;
		public const float GOLDEN_RATIO = 1.61803398875f;
		public const float SQRT2 = 1.41421356237f;
		public const float Infinity = Single.PositiveInfinity;
		public const float NegativeInfinity = Single.NegativeInfinity;
		public const float Deg2Rad = TAU / 360f;
		public const float Rad2Deg = 360f / TAU;
		public static readonly float Epsilon = UnityEngineInternal.MathfInternal.IsFlushToZeroEnabled ? UnityEngineInternal.MathfInternal.FloatMinNormal : UnityEngineInternal.MathfInternal.FloatMinDenormal;

		// Enums
		public enum AngleUnits { Degrees, Radians};

		// Math operations
		public static float Sqrt( float value ) => (float)Math.Sqrt( value );
		public static float Pow( float @base, float exponent ) => (float)Math.Pow( @base, exponent );
		public static float Exp( float power ) => (float)Math.Exp( power );
		public static float Log( float value, float @base ) => (float)Math.Log( value, @base );
		public static float Log( float value ) => (float)Math.Log( value );
		public static float Log10( float value ) => (float)Math.Log10( value );
		public static bool Approximately( float a, float b ) => Abs( b - a ) < Max( 0.000001f * Max( Abs( a ), Abs( b ) ), Epsilon * 8 );

		// Trig
		public static float Sin( float angRad ) => (float)Math.Sin( angRad );
		public static float Cos( float angRad ) => (float)Math.Cos( angRad );
		public static float Tan( float angRad ) => (float)Math.Tan( angRad );
		public static float Asin( float value ) => (float)Math.Asin( value );
		public static float Acos( float value ) => (float)Math.Acos( value );
		public static float Atan( float value ) => (float)Math.Atan( value );
		public static float Atan2( float y, float x ) => (float)Math.Atan2( y, x );
		public static float Csc( float x ) => 1f / (float)Math.Sin( x );
		public static float Sec( float x ) => 1f / (float)Math.Cos( x );
		public static float Cot( float x ) => 1f / (float)Math.Tan( x );
		public static float Ver( float x ) => 1 - (float)Math.Cos( x );
		public static float Cvs( float x ) => 1 - (float)Math.Sin( x );
		public static float Crd( float x ) => 2 * (float)Math.Sin( x / 2 );

		// Absolute values
		public static float Abs( float value ) => Math.Abs( value );
		public static int Abs( int value ) => Math.Abs( value );
		public static Vector2 Abs( Vector2 v ) => new Vector2( Abs( v.x ), Abs( v.y ) );
		public static Vector3 Abs( Vector3 v ) => new Vector3( Abs( v.x ), Abs( v.y ), Abs( v.z ) );
		public static Vector4 Abs( Vector4 v ) => new Vector4( Abs( v.x ), Abs( v.y ), Abs( v.z ), Abs( v.w ) );

		// Clamping
		public static float Clamp( float value, float min, float max ) {
			if( value < min ) value = min;
			if( value > max ) value = max;
			return value;
		}

		public static int Clamp( int value, int min, int max ) {
			if( value < min ) value = min;
			if( value > max ) value = max;
			return value;
		}

		public static float Clamp01( float value ) {
			if( value < 0f ) value = 0f;
			if( value > 1f ) value = 1f;
			return value;
		}

		public static float ClampNeg1to1( float value ) {
			if( value < -1f ) value = -1f;
			if( value > 1f ) value = 1f;
			return value;
		}

		// Min & Max
		public static float Min( float a, float b ) => a < b ? a : b;
		public static float Min( float a, float b, float c ) => Min( Min( a, b ), c );
		public static float Min( float a, float b, float c, float d ) => Min( Min( a, b ), Min( c, d ) );
		public static float Max( float a, float b ) => a > b ? a : b;
		public static float Max( float a, float b, float c ) => Max( Max( a, b ), c );
		public static float Max( float a, float b, float c, float d ) => Max( Max( a, b ), Max( c, d ) );
		public static int Min( int a, int b ) => a < b ? a : b;
		public static int Min( int a, int b, int c ) => Min( Min( a, b ), c );
		public static int Max( int a, int b ) => a > b ? a : b;
		public static int Max( int a, int b, int c ) => Max( Max( a, b ), c );

		public static float Min( params float[] values ) => values.Min();
		public static float Max( params float[] values ) => values.Max();
		public static int Min( params int[] values ) => values.Min();
		public static int Max( params int[] values ) => values.Max();

		// Rounding
		public static int Sign( float value ) => value >= 0f ? 1 : -1;
		public static int Sign( int value ) => value >= 0 ? 1 : -1;
		public static int SignWithZero( int value ) => value == 0 ? 0 : Sign( value );
		public static int SignWithZero( float value, float epsilon = 0.000001f ) => Abs( value ) < epsilon ? 0 : Sign( value );
		public static float Floor( float value ) => (float)Math.Floor( value );
        public static Vector2 Floor( Vector2 value ) => new Vector2( (float)Math.Floor( value.x ), (float)Math.Floor( value.y ) );
		public static Vector3 Floor( Vector3 value ) => new Vector3( (float)Math.Floor( value.x ), (float)Math.Floor( value.y ), (float)Math.Floor( value.z ) );
		public static Vector4 Floor( Vector4 value ) => new Vector4( (float)Math.Floor( value.x ), (float)Math.Floor( value.y ), (float)Math.Floor( value.z ), (float)Math.Floor( value.w ) );
		

		// Interpolation & Remapping
		public static float InverseLerp( float a, float b, float value ) => ( value - a ) / ( b - a );
		public static float InverseLerpClamped( float a, float b, float value ) => Clamp01( ( value - a ) / ( b - a ) );
		public static float Lerp( float a, float b, float t ) => ( 1f - t ) * a + t * b;

		public static float LerpClamped( float a, float b, float t ) {
			t = Clamp01( t );
			return ( 1f - t ) * a + t * b;
		}

		public static float Eerp( float a, float b, float t ) => Mathf.Pow( a, 1 - t ) * Mathf.Pow( b, t );
		public static float InverseEerp( float a, float b, float v ) => Mathf.Log( a / v ) / Mathf.Log( a / b );

        public static float ExpDecay(float a, float b, float decay, float dt) => b + (a-b) * Exp(-decay * dt);
        public static Vector2 ExpDecay(Vector2 a, Vector2 b, float decay, float dt) => new Vector2(ExpDecay(a.x, b.x, decay, dt), ExpDecay(a.y, b.y, decay, dt));

        public static float Remap( float iMin, float iMax, float oMin, float oMax, float value ) {
			float t = InverseLerp( iMin, iMax, value );
			return Lerp( oMin, oMax, t );
		}


		// Vector math
		public static Vector2 Rotate90CW( Vector2 v ) => new Vector2( v.y, -v.x );
		public static Vector2 Rotate90CCW( Vector2 v ) => new Vector2( -v.y, v.x );
		public static float DistanceSquared( Vector2 a, Vector2 b ) => ( a.x - b.x ).Square() + ( a.y - b.y ).Square();
		public static float DistanceSquared( Vector3 a, Vector3 b ) => ( a.x - b.x ).Square() + ( a.y - b.y ).Square() + ( a.z - b.z ).Square();
		public static float DistanceSquared( Vector4 a, Vector4 b ) => ( a.x - b.x ).Square() + ( a.y - b.y ).Square() + ( a.z - b.z ).Square() + ( a.w - b.w ).Square();
        public static Vector3 MultiplyEach(this Vector3 vector, float x, float y, float z = 1) => new Vector3(vector.x * x, vector.y * y, vector.z * z);
        public static Vector2 MultiplyEach(this Vector2 vector, float x, float y) => new Vector2(vector.x * x, vector.y * y);

        public static float AngleUnitConversion(float value, AngleUnits unitsFrom, AngleUnits unitsTo)
        {
            string unitString = "" + (int)unitsFrom + "" + (int)unitsTo;
            switch (unitString)
            {
                case "01": /*Degrees -> Radians*/ return Mathf.Deg2Rad * value;
                case "10": /*Radians -> Degrees*/ return Mathf.Rad2Deg * value;
                default: return value;
            }
        }
    }

	public static class Ex
	{
		public static bool ifAny(params bool[] conditions)
		{
			foreach (bool condition in conditions)
				if (condition)
					return true;

			return false;
        }

        public static bool ifNone(params bool[] conditions)
        {
            foreach (bool condition in conditions)
                if (condition)
                    return false;

            return true;
        }

        //public static Ease EasingFunctionToDoTweenEase(EasingFunction.Ease ease)
        //{
        //    return ease switch
        //    {
        //        EasingFunction.Ease.Linear => Ease.Linear,
        //        EasingFunction.Ease.Spring => Ease.Flash,
        //        EasingFunction.Ease.QuadIn => Ease.InQuad,
        //        EasingFunction.Ease.QuadOut => Ease.OutQuad,
        //        EasingFunction.Ease.QuadInOut => Ease.InOutQuad,
        //        EasingFunction.Ease.CubicIn => Ease.InCubic,
        //        EasingFunction.Ease.CubicOut => Ease.OutCubic,
        //        EasingFunction.Ease.CubicInOut => Ease.InOutCubic,
        //        EasingFunction.Ease.QuartIn => Ease.InQuart,
        //        EasingFunction.Ease.QuartOut => Ease.OutQuart,
        //        EasingFunction.Ease.QuartInOut => Ease.InOutQuart,
        //        EasingFunction.Ease.QuintIn => Ease.InQuint,
        //        EasingFunction.Ease.QuintOut => Ease.OutQuint,
        //        EasingFunction.Ease.QuintInOut => Ease.InOutQuint,
        //        EasingFunction.Ease.SineIn => Ease.InSine,
        //        EasingFunction.Ease.SineOut => Ease.OutSine,
        //        EasingFunction.Ease.SineInOut => Ease.InOutSine,
        //        EasingFunction.Ease.ExponentialIn => Ease.InExpo,
        //        EasingFunction.Ease.ExponentialOut => Ease.OutExpo,
        //        EasingFunction.Ease.ExponentialInOut => Ease.InOutExpo,
        //        EasingFunction.Ease.CircleIn => Ease.InCirc,
        //        EasingFunction.Ease.CircleOut => Ease.OutCirc,
        //        EasingFunction.Ease.CircleInOut => Ease.InOutCirc,
        //        EasingFunction.Ease.BounceIn => Ease.InBounce,
        //        EasingFunction.Ease.BounceOut => Ease.OutBounce,
        //        EasingFunction.Ease.BounceInOut => Ease.InOutBounce,
        //        EasingFunction.Ease.BackIn => Ease.InBack,
        //        EasingFunction.Ease.BackOut => Ease.OutBack,
        //        EasingFunction.Ease.BackInOut => Ease.InOutBack,
        //        EasingFunction.Ease.ElasticIn => Ease.InElastic,
        //        EasingFunction.Ease.ElasticOut => Ease.OutElastic,
        //        EasingFunction.Ease.ElasticInOut => Ease.InOutElastic,
        //        _ => Ease.Linear
        //    };
        //}
    }
}