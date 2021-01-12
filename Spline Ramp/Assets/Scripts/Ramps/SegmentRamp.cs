using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Ramps
{
    public class SegmentRamp : Ramp
    {
        public SegmentRamp(Vector3 starting_point) : base(starting_point)
        {

        }
        
        public override IEnumerator BuildRamp()
        {
            int num_points = ramp_gameobject.transform.childCount;
            line_renderer.positionCount = num_points;
            Vector2[] points = new Vector2[num_points];

            for (int i = 0; i < ramp_gameobject.transform.childCount; i++)
            {
                Vector2 point = ramp_gameobject.transform.GetChild(i).transform.localPosition;
                line_renderer.SetPosition(i, point);
                points[i] = point;
            }
            edge_collider.points = points;
            yield return null;
        }
    }
}
