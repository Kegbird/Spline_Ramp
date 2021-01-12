using System;
using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Ramps
{
    public class BezierRamp : Ramp
    {
        public BezierRamp(Vector3 starting_point) : base(starting_point)
        {
        }
        
        public override IEnumerator BuildRamp()
        {
            int num_nodes = ramp_gameobject.transform.childCount;
            line_renderer.positionCount = Constants.CURVE_STEPS + 1;
            Vector2[] points = new Vector2[Constants.CURVE_STEPS + 1];
            Vector3[] support_points = new Vector3[num_nodes];

            for (int i = 0; i < num_nodes; i++)
                support_points[i] = ramp_gameobject.transform.GetChild(i).localPosition;

            for (int i = 0; i <= Constants.CURVE_STEPS; i++)
            {
                points[i] = Curve.BezierCurve((float)i / Constants.CURVE_STEPS, support_points);
                line_renderer.SetPosition(i, points[i]);
            }
            edge_collider.points = points;
            yield return null;
        }
    }
}
