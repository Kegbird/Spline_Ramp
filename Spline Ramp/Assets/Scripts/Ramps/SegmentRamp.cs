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

        public override void Translate(Vector3 new_position)
        {
            int i = 0;

            Vector3 offset = new_position - ramp_gameobject.transform.position;
            for (int j = 0; j < line_renderer.positionCount; j++)
            {
                Vector3 world_node_position = ramp_gameobject.transform.TransformPoint(line_renderer.GetPosition(j));
                world_node_position += offset;
                if (Constants.MIN_X_BOUNDARY > world_node_position.x || world_node_position.x > Constants.MAX_X_BOUNDARY)
                    return;
                i++;
            }

            if (i == line_renderer.positionCount)
                ramp_gameobject.transform.position = new_position;
        }

        public override void AddPoint(Vector3 node_position)
        {
            node_position = ramp_gameobject.transform.InverseTransformPoint(node_position);
            base.AddPoint(node_position);
        }

        public override void Rotate(Vector3 mouse_position)
        {
            mouse_position.z = 0;
            Transform axis = ramp_gameobject.transform.parent;
            Vector3 axis_screen_position = Camera.main.WorldToScreenPoint(axis.transform.position);
            float angle = Mathf.Atan2(mouse_position.y - axis_screen_position.y, mouse_position.x - axis_screen_position.x) * Mathf.Rad2Deg;
            axis.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        public override void Edit(Vector3 point, int node_index)
        {
            if (node_index >= ramp_gameobject.transform.childCount)
                return;
            point.x = Mathf.Clamp(point.x, Constants.MIN_X_BOUNDARY, Constants.MAX_X_BOUNDARY);
            point = ramp_gameobject.transform.InverseTransformPoint(point);
            ramp_gameobject.transform.GetChild(node_index).transform.localPosition = point;
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
            rotation_ball.layer = Constants.ROTATION_GIZMO_LAYER_MASK;
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
            line_renderer.positionCount = ramp_gameobject.transform.childCount;
            if (ramp_gameobject.GetComponent<EdgeCollider2D>())
                GameObject.Destroy(edge_collider);
            edge_collider = ramp_gameobject.AddComponent<EdgeCollider2D>();
            edge_collider.edgeRadius = Constants.RAMP_EDGE_RADIUS;
            Vector2[] points = new Vector2[ramp_gameobject.transform.childCount];

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
