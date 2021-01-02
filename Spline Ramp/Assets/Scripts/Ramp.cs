using System;
using UnityEngine;

namespace Assets.Scripts
{
    class Ramp
    {
        enum RampType
        {
            Segment,
            Spline
        }

        private static GameObject ramp_gameobject;
        private static EdgeCollider2D edge_collider;
        private static LineRenderer line_renderer;
        private static int index = 0;
        private static bool ramp_created = false;

        private static void CreateRamp(Vector3 starting_point)
        {
            ramp_gameobject = new GameObject();
            ramp_gameobject.name = "Ramp";
            ramp_gameobject.tag = "Interactable";
            ramp_gameobject.layer = LayerMask.NameToLayer("Ramp");
            ramp_gameobject.transform.position = starting_point;
            line_renderer = ramp_gameobject.AddComponent<LineRenderer>();
            line_renderer.startWidth = Constants.RAMP_EDGE_RADIUS;
            line_renderer.endWidth = Constants.RAMP_EDGE_RADIUS;
            line_renderer.useWorldSpace = false;
            index = 0;
            ramp_created = false;
        }

        public static bool Created()
        {
            return ramp_created;
        }

        public static Vector3 GetWorldPosition()
        {
            return ramp_gameobject.transform.position;
        }

        public static void Translate(Vector3 new_position, float downer_limit, float upper_limit)
        {
            if (!ramp_created)
                return;

            int i = 0;

            Vector3 offset = new_position - ramp_gameobject.transform.position;
            for (int j = 0; j < line_renderer.positionCount; j++)
            {
                Vector3 world_node_position = ramp_gameobject.transform.TransformPoint(line_renderer.GetPosition(j));
                world_node_position += offset;
                if (downer_limit > world_node_position.x || world_node_position.x > upper_limit)
                    return;
                i++;
            }

            if (i == line_renderer.positionCount)
                ramp_gameobject.transform.position = new_position;
        }

        public static void Rotate(Vector3 mouse_position, float downer_limit, float upper_limit)
        {
            mouse_position.z = 0;
            Transform axis = ramp_gameobject.transform.parent;
            Vector3 axis_screen_position = Camera.main.WorldToScreenPoint(axis.transform.position);
            float angle = Mathf.Atan2(mouse_position.y - axis_screen_position.y, mouse_position.x - axis_screen_position.x) * Mathf.Rad2Deg;
            Quaternion old_rotation = axis.transform.rotation;
            axis.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            for (int j = 0; j < line_renderer.positionCount; j++)
            {
                Vector3 world_node_position = ramp_gameobject.transform.TransformPoint(line_renderer.GetPosition(j));
                if (downer_limit > world_node_position.x || world_node_position.x > upper_limit)
                {
                    axis.transform.rotation = old_rotation;
                    return;
                }
            }
        }

        public static void Edit(Vector3 point, int node_index)
        {
            if (!ramp_created || node_index >= ramp_gameobject.transform.childCount)
                return;
            point = Ramp.ramp_gameobject.transform.InverseTransformPoint(point);
            ramp_gameobject.transform.GetChild(node_index).transform.localPosition = point;
            line_renderer.SetPosition(node_index, point);
            Vector2[] points = edge_collider.points;
            points[node_index] = point;
            edge_collider.points = points;
        }

        public static void AddPoint(Vector3 point)
        {
            if (!ramp_gameobject)
            {
                CreateRamp(point);
                point = point - ramp_gameobject.transform.position;
                line_renderer.SetPosition(index, point);
                index++;
                return;
            }
            else if (ramp_created)
                return;
            SetLastPoint(point);
            index++;
            line_renderer.positionCount++;
        }

        public static void DisplayNodeTranslationGizmo()
        {
            for (int i = 0; i < line_renderer.positionCount; i++)
            {
                GameObject translation_node = new GameObject();
                translation_node.name = String.Format("Node_{0}", i);
                translation_node.transform.parent = ramp_gameobject.transform;
                translation_node.transform.localPosition = edge_collider.points[i];
                translation_node.layer = Constants.TRANSLATION_GIZMO_LAYER_MASK;
                SpriteRenderer sprite_renderer = translation_node.AddComponent<SpriteRenderer>();
                sprite_renderer.sprite = Resources.Load<Sprite>("Sprites/Point");
                sprite_renderer.sortingOrder = 1;
                CircleCollider2D collider = translation_node.AddComponent<CircleCollider2D>();
                collider.radius = 0.1f;
                collider.isTrigger = true;
            }
        }

        public static void HideNodeTranslationGizmo()
        {
            for (int i = ramp_gameobject.transform.childCount - 1; i >= 0; i--)
            {
                GameObject child = ramp_gameobject.transform.GetChild(i).gameObject;
                GameObject.Destroy(child);
            }
        }

        public static void DisplayRotationGizmo()
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

        public static void HideRotationGizmo()
        {
            Transform tmp = ramp_gameobject.transform.parent;
            ramp_gameobject.transform.parent = null;
            GameObject.Destroy(tmp.gameObject);
        }

        public static void SetLastPoint(Vector3 point)
        {
            if (!ramp_gameobject || edge_collider || ramp_created)
                return;
            point = point - ramp_gameobject.transform.position;
            line_renderer.SetPosition(index, point);
        }

        public static void BuildEdgeCollider()
        {
            if (index == 0 || !line_renderer || ramp_created)
                return;
            //Creation edge collider
            edge_collider = ramp_gameobject.AddComponent<EdgeCollider2D>();
            edge_collider.edgeRadius = Constants.RAMP_EDGE_RADIUS;
            Vector2[] points = new Vector2[line_renderer.positionCount];
            points[0] = Vector2.zero;

            for (int i = 1; i < line_renderer.positionCount; i++)
            {
                Vector2 point = line_renderer.GetPosition(i);
                points[i] = point;
            }
            edge_collider.points = points;
            ramp_created = true;
        }
    }
}
