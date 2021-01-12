using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Ramps
{
    class BSplineRamp : Ramp
    {
        public BSplineRamp(Vector3 starting_point) : base(starting_point)
        {
        }

        public override IEnumerator BuildRamp()
        {
            int num_points = ramp_gameobject.transform.childCount;
            Vector2[] points;
            Vector3[] support_points = new Vector3[num_points];
            int num_nodes = ramp_gameobject.transform.childCount;
            for (int i = 0; i < num_nodes; i++)
                support_points[i] = ramp_gameobject.transform.GetChild(i).localPosition;

            if (num_points <= Constants.BSPLINE_DEGREE)
            {
                //Normal bezier curve
                line_renderer.positionCount = Constants.CURVE_STEPS + 1;
                points = new Vector2[Constants.CURVE_STEPS + 1];

                for (int j = 0; j <= Constants.CURVE_STEPS; j++)
                {
                    points[j] = Curve.BezierCurve((float)j / Constants.CURVE_STEPS, support_points);
                    line_renderer.SetPosition(j, points[j]);
                }
                edge_collider.points = points;
                yield return null;
            }
            else
            {
                List<float> knots;
                int num_curves;

                switch (num_nodes)
                {
                    case 4:
                        knots = new List<float>(Constants.KNOTS_4);
                        num_curves = 2;
                        break;
                    case 5:
                        knots = new List<float>(Constants.KNOTS_5);
                        num_curves = 2;
                        break;
                    default:
                        knots = new List<float>(Constants.KNOTS_6);
                        num_curves = 3;
                        break;
                }
                float t = knots[0];
                float increment = (float)num_curves / Constants.CURVE_STEPS;
                points = new Vector2[Constants.CURVE_STEPS + 1];
                line_renderer.positionCount = Constants.CURVE_STEPS + 1;

                for (int k = 0; k < Constants.CURVE_STEPS; k++)
                {
                    points[k] = Curve.BSplineCurve(t, support_points, knots, Constants.BSPLINE_DEGREE);
                    t += increment;
                    line_renderer.SetPosition(k, points[k]);
                }
                points[Constants.CURVE_STEPS] = support_points[num_nodes - 1];
                line_renderer.SetPosition(Constants.CURVE_STEPS, points[Constants.CURVE_STEPS]);
                edge_collider.points = points;
                yield return null;
            }
        }
    }
}
