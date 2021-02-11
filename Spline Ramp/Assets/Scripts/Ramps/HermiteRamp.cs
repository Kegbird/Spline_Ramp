using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Ramps
{
    public class HermiteRamp : Ramp
    {

        public HermiteRamp(Vector3 starting_point) : base(starting_point)
        {
        }

        public override void AddPoint(Vector3 node_position)
        {
            node_position = ramp_gameobject.transform.InverseTransformPoint(node_position);
            GameObject node = GameObject.Instantiate(Resources.Load<GameObject>("Point_Hermit"));
            node.transform.parent = ramp_gameobject.transform;
            node.transform.localPosition = node_position;
        }

        public override void AllowEdit()
        {
            for (int i = 0; i < ramp_gameobject.transform.childCount; i++)
                ramp_gameobject.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
        }

        public override void DenyEdit()
        {
            for (int i = 0; i < ramp_gameobject.transform.childCount; i++)
                ramp_gameobject.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);

        }

        public override IEnumerator BuildRamp()
        {
            int num_points = Constants.CURVE_STEPS + 1;
            int num_curves = ramp_gameobject.transform.childCount - 1;
            Vector2[] points = new Vector2[num_points];
            line_renderer.positionCount = num_points;
            for (int i = 0; i < num_curves; i++)
            {
                for (int j = 0; j <= Constants.CURVE_STEPS/num_curves; j++)
                {
                    int k = j + (i * Constants.CURVE_STEPS / num_curves);
                    Vector3 P0 = ramp_gameobject.transform.GetChild(i).transform.localPosition;
                    Vector3 P1 = ramp_gameobject.transform.GetChild(i+1).transform.localPosition;
                    Vector3 M0 = ramp_gameobject.transform.InverseTransformVector(ramp_gameobject.transform.GetChild(i).transform.right);
                    Vector3 M1 = ramp_gameobject.transform.InverseTransformVector(ramp_gameobject.transform.GetChild(i + 1).transform.right);
                    points[k] = Curve.HermiteCurve((float)j / ((float)Constants.CURVE_STEPS/num_curves), P0, P1, M0, M1);
                    line_renderer.SetPosition(k, points[k]);
                }
            }
            points[num_points-1] = ramp_gameobject.transform.GetChild(num_curves).transform.localPosition;
            line_renderer.SetPosition(num_points-1, ramp_gameobject.transform.GetChild(num_curves).transform.localPosition);
            DenyEdit();
            edge_collider.points = points;
            yield return null;
        }
    }
}
