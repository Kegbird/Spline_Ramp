using System;
using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Ramps
{
    public class BezierRamp : Ramp
    {
        private BezierCurve curve;

        public BezierRamp(Vector3 starting_point) : base(starting_point)
        {
            curve = new BezierCurve();
        }

        public override void Translate(Vector3 new_position)
        {
            int i = 0;

            Vector3 offset = new_position - ramp_gameobject.transform.position;
            for (int j = 0; j < ramp_gameobject.transform.childCount; j++)
            {
                Vector3 world_node_position = ramp_gameobject.transform.GetChild(j).position;
                world_node_position += offset;
                if (Constants.MIN_X_BOUNDARY > world_node_position.x || world_node_position.x > Constants.MAX_X_BOUNDARY)
                    return;
                i++;
            }

            if (i == ramp_gameobject.transform.childCount)
                ramp_gameobject.transform.position = new_position;
        }

        public override void Rotate(Vector3 mouse_position)
        {
            mouse_position.z = 0;
            Transform axis = ramp_gameobject.transform.parent;
            Vector3 axis_screen_position = Camera.main.WorldToScreenPoint(axis.transform.position);
            float angle = Mathf.Atan2(mouse_position.y - axis_screen_position.y, mouse_position.x - axis_screen_position.x) * Mathf.Rad2Deg;
            axis.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        public override void Edit(Vector3 position, int node_index)
        {
            if (curve.GetSupportPointsNumber() <= node_index)
                return;
            position.x = Mathf.Clamp(position.x, Constants.MIN_X_BOUNDARY, Constants.MAX_X_BOUNDARY);
            position = ramp_gameobject.transform.InverseTransformPoint(position);
            ramp_gameobject.transform.GetChild(node_index).transform.localPosition = position;
            curve.SetSupportPoint(node_index, position);
        }

        public override void AddPoint(Vector3 node_position)
        {
            node_position = ramp_gameobject.transform.InverseTransformPoint(node_position);
            base.AddPoint(node_position);
            curve.AddPoint(node_position);
        }

        public override void DisplayRotationGizmo()
        {
            Vector2 centre = line_renderer.bounds.center;
            GameObject axis = new GameObject();
            GameObject rotation_ball = new GameObject();
            axis.transform.position = centre;
            axis.name = "Rotation Axis";
            rotation_ball.name = "Rotation Gizmo";
            rotation_ball.transform.position = new Vector3(centre.x + 2 * Mathf.Cos(ramp_gameobject.transform.rotation.z * Mathf.Deg2Rad), centre.y + 2 * Mathf.Sin(ramp_gameobject.transform.rotation.z * Mathf.Deg2Rad));
            SpriteRenderer sprite_renderer = rotation_ball.AddComponent<SpriteRenderer>();
            sprite_renderer.sprite = Resources.Load<Sprite>("Sprites/Point");
            CircleCollider2D collider = rotation_ball.AddComponent<CircleCollider2D>();
            collider.radius = 0.1f;
            collider.isTrigger = true;
            rotation_ball.layer = LayerMask.NameToLayer("RotationGizmo");
            rotation_ball.transform.parent = axis.transform;
            ramp_gameobject.transform.parent = axis.transform;
        }

        public override void HideRotationGizmo()
        {
            Transform tmp = ramp_gameobject.transform.parent;
            ramp_gameobject.transform.parent = null;
            GameObject.Destroy(tmp.gameObject);
        }

        public override IEnumerator BuildRamp()
        {
            line_renderer.positionCount = Constants.CURVE_STEPS+1;
            Vector2[] points = new Vector2[line_renderer.positionCount];
            points[0] = ramp_gameobject.transform.GetChild(0).localPosition;
            points[Constants.CURVE_STEPS] = ramp_gameobject.transform.GetChild(ramp_gameobject.transform.childCount - 1).localPosition;
            line_renderer.SetPosition(0, points[0]);
            line_renderer.SetPosition(Constants.CURVE_STEPS, points[Constants.CURVE_STEPS]);
            for (int i = 1; i < Constants.CURVE_STEPS; i++)
            {
                points[i] = curve.GetInterpolationPoint((float)i / Constants.CURVE_STEPS);
                line_renderer.SetPosition(i, points[i]);
            }
            if (ramp_gameobject.GetComponent<EdgeCollider2D>())
                GameObject.Destroy(edge_collider);
            edge_collider = ramp_gameobject.AddComponent<EdgeCollider2D>();
            edge_collider.edgeRadius = Constants.RAMP_EDGE_RADIUS;
            edge_collider.points = points;
            yield return null;
        }
    }
}
