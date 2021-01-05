using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Ramps
{
    class BSplineRamp : Ramp
    {
        private List<Vector3> points;

        public BSplineRamp(Vector3 starting_point):base(starting_point)
        {
            points = new List<Vector3>();
        }

        public override void AddPoint(Vector3 point)
        {
            points.Add(point);
        }

        public Vector3 GetInterpolationPoint(float f)
        {
            return Vector3.zero;
        }

        public override IEnumerator BuildRamp()
        {
            throw new NotImplementedException();
        }
        
        public override void DisplayRotationGizmo()
        {
            throw new NotImplementedException();
        }

        public override void Edit(Vector3 point, int node_index)
        {
            throw new NotImplementedException();
        }

        public override void HideRotationGizmo()
        {
            throw new NotImplementedException();
        }

        public override void Rotate(Vector3 mouse_position)
        {
            throw new NotImplementedException();
        }

        public override void Translate(Vector3 new_position)
        {
            throw new NotImplementedException();
        }
    }
}
