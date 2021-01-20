using System;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Ramps
{
    public class Curve
    {
        public static Vector3 BezierCurve(float t, Vector3[] support_points)
        {
            t = Mathf.Clamp01(t);
            int k = support_points.Length - 1;
            Vector3 result = Vector3.zero;
            for (int i = 0; i <= k; i++)
                result += support_points[i] * BernsteinPolynomial(k, i, t);
            return result;
        }

        public static Vector3 HermiteCurve(float t, Vector3 P0, Vector3 P1, Vector3 M0, Vector3 M1)
        {
            M0 *= Constants.DERIVATIVE_MAGNITUDE;
            M1 *= Constants.DERIVATIVE_MAGNITUDE;
            Vector3 result = Vector3.zero;
            float B_3_0 = BernsteinPolynomial(3, 0, t);
            float B_3_1 = BernsteinPolynomial(3, 1, t);
            float B_3_2 = BernsteinPolynomial(3, 2, t);
            float B_3_3 = BernsteinPolynomial(3, 3, t);
            float H_0_3 = B_3_0 + B_3_1;
            float H_1_3 = B_3_1 / 3f;
            float H_2_3 = B_3_2 / -3f;
            float H_3_3 = B_3_2 + B_3_3;
            result = H_0_3 * P0 + H_1_3 * M0 + H_2_3 * M1 + H_3_3 * P1;
            return result;
        }

        public static Vector3 BSplineCurve(float t, Vector3[] support_points, List<float> knots, int degree)
        {
            Vector3 result = Vector3.zero;

            for (int i = 0; i < support_points.Length; i++)
                result += DeBoorCox(t, i, knots, degree) * support_points[i];
            return result;
        }

        private static float DeBoorCox(float t, int i, List<float> knots, int k)
        {
            if (k == 0)
            {
                if (knots[i] <= t && t < knots[i + 1])
                    return 1;
                return 0;
            }

            float result = 0f;
            if ((knots[i + k] - knots[i]) != 0)
                result += ((t - knots[i]) / (knots[i + k] - knots[i])) * DeBoorCox(t, i, knots, k - 1);
            if ((knots[i + k + 1] - knots[i + 1]) != 0)
                result += ((knots[i + k + 1] - t) / (knots[i + k + 1] - knots[i + 1])) * DeBoorCox(t, i + 1, knots, k - 1);
            return result;
        }

        private static float Weight(float t, float t_i, float t_i_k)
        {
            if (t < t_i_k && (t_i - t_i_k) != 0f)
                return (t + t_i) / (t_i - t_i_k);
            return 0f;
        }

        private static float BernsteinPolynomial(int k, int i, float t)
        {
            return (Factorial(k) / (Factorial(i) * Factorial(k - i))) * Mathf.Pow(1 - t, k - i) * Mathf.Pow(t, i);
        }

        private static int Factorial(int i)
        {
            if (i == 0)
                return 1;
            return Factorial(i - 1) * i;
        }

        public static float CrescendoInterpolation(float t)
        {
            Vector3[] support_points ={ new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, 1, 0)};
            return BezierCurve(t, support_points).y;
        }

        public static float EaseOutInterpolation(float t)
        {
            Vector3[] support_points = { new Vector3(0, 0, 0), new Vector3(0.35f, 0.75f, 0), new Vector3(1, 1, 0) };
            return BezierCurve(t, support_points).y;
        }
    }
}
