using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Ramps
{
    class BezierCurve
    {
        private List<Vector3> points;

        public BezierCurve()
        {
            points = new List<Vector3>();
        }

        public void AddPoint(Vector3 point)
        {
            points.Add(point);
        }

        public Vector3 GetSupportPoint(int index)
        {
            if (index >= points.Count)
                return Vector3.zero;
            return points[index];
        }

        public void SetSupportPoint(int index, Vector3 point)
        {
            points[index] = point;
        }

        public int GetSupportPointsNumber()
        {
            return points.Count;
        }

        public Vector3 GetInterpolationPoint(float t)
        {
            t = Mathf.Clamp01(t);
            if (points.Count < 2)
                return points[0];
            if (points.Count == 2)
                return Vector3.Lerp(points[0], points[1], t);
            Vector3 result = Vector3.zero;
            int k = points.Count-1;
            int k_factorial = Factorial(k);
            for (int i = 0; i <= k; i++)
                result += points[i] * (k_factorial/(Factorial(i)*Factorial(k-i)))*Mathf.Pow(1-t, k-i)*Mathf.Pow(t, i);
            return result;
        }

        private int Factorial(int i)
        {
            if (i == 0)
                return 1;
            return Factorial(i - 1) * i;
        }
        
        public Vector3 GetFirstDerivativeNormalized(float t)
        {
            if (points.Count == 1)
                return Vector3.zero;
            int k = points.Count - 2;
            int k_factorial = Factorial(k);
            Vector3 result = Vector3.zero;
            for(int i=0; i<=k; i++)
                result += (points[i + 1] - points[i])*(k_factorial/(Factorial(i)*Factorial(k-i))*Mathf.Pow(1-t, k-i)*Mathf.Pow(t, i));
            result = result * (k + 2);
            result=result.normalized;
            return result;
        }

        public Vector3 GetWorldTransform()
        {
            if (points.Count == 0)
                return Vector3.zero;
            return points[0];
        }
    }
}
