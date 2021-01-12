using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Ramps
{
    public abstract class Ramp
    {
        protected GameObject ramp_gameobject;
        protected EdgeCollider2D edge_collider;
        protected LineRenderer line_renderer;

        public Ramp(Vector3 starting_point)
        {
            ramp_gameobject = new GameObject();
            ramp_gameobject.name = "Ramp";
            ramp_gameobject.layer = LayerMask.NameToLayer("Ramp");
            ramp_gameobject.transform.position = starting_point;
            line_renderer = ramp_gameobject.AddComponent<LineRenderer>();
            line_renderer.startWidth = Constants.RAMP_EDGE_RADIUS;
            line_renderer.endWidth = Constants.RAMP_EDGE_RADIUS;
            line_renderer.useWorldSpace = false;
            edge_collider = ramp_gameobject.AddComponent<EdgeCollider2D>();
            edge_collider.edgeRadius = Constants.RAMP_EDGE_RADIUS;
        }

        public void DestroyRamp()
        {
            GameObject.Destroy(ramp_gameobject);
            GameObject.Destroy(line_renderer);
            GameObject.Destroy(edge_collider);
        }

        public virtual void AddPoint(Vector3 node_position)
        {
            node_position = ramp_gameobject.transform.InverseTransformPoint(node_position);
            GameObject node = new GameObject();
            node.name = String.Format("Node_{0}", ramp_gameobject.transform.childCount);
            node.transform.parent = ramp_gameobject.transform;
            node.layer = Constants.SUPPORT_NODE_LAYER_MASK;
            node.transform.localPosition = node_position;
            SpriteRenderer sprite_renderer = node.AddComponent<SpriteRenderer>();
            sprite_renderer.sprite = Resources.Load<Sprite>("Sprites/Point");
            sprite_renderer.sortingOrder = 1;
            CircleCollider2D collider = node.AddComponent<CircleCollider2D>();
            collider.radius = 0.1f;
            collider.isTrigger = true;
        }

        public void DisplayRotationGizmo()
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

        public void HideRotationGizmo()
        {
            Transform tmp = ramp_gameobject.transform.parent;
            ramp_gameobject.transform.parent = null;
            GameObject.Destroy(tmp.gameObject);
        }

        public void Rotate(Vector3 mouse_position)
        {
            mouse_position.z = 0;
            Transform axis = ramp_gameobject.transform.parent;
            Vector3 axis_screen_position = Camera.main.WorldToScreenPoint(axis.transform.position);
            float angle = Mathf.Atan2(mouse_position.y - axis_screen_position.y, mouse_position.x - axis_screen_position.x) * Mathf.Rad2Deg;
            axis.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        public void Translate(Vector3 new_position)
        {
            Vector3 offset = new_position - ramp_gameobject.transform.position;
            for (int j = 0; j < ramp_gameobject.transform.childCount; j++)
            {
                Vector3 world_node_position = ramp_gameobject.transform.TransformPoint(ramp_gameobject.transform.GetChild(j).localPosition);
                world_node_position += offset;
                if (Constants.MIN_X_BOUNDARY > world_node_position.x || world_node_position.x > Constants.MAX_X_BOUNDARY)
                    return;
            }
            ramp_gameobject.transform.position = new_position;
        }

        public Vector3 GetWorldPosition()
        {
            return ramp_gameobject.transform.position;
        }
        
        public virtual void AllowEdit()
        {
            for (int i = 0; i < ramp_gameobject.transform.childCount; i++)
                ramp_gameobject.transform.GetChild(i).gameObject.SetActive(true);
        }

        public virtual void DenyEdit()
        {
            for (int i = 0; i < ramp_gameobject.transform.childCount; i++)
                ramp_gameobject.transform.GetChild(i).gameObject.SetActive(false);
        }

        public virtual void Edit(Vector3 position, int node_index)
        {
            position.x = Mathf.Clamp(position.x, Constants.MIN_X_BOUNDARY, Constants.MAX_X_BOUNDARY);
            position = ramp_gameobject.transform.InverseTransformPoint(position);
            ramp_gameobject.transform.GetChild(node_index).transform.localPosition = position;
        }

        public abstract IEnumerator BuildRamp();
    }
}
