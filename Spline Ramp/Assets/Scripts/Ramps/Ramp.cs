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
        }

        public void DestroyRamp()
        {
            GameObject.Destroy(ramp_gameobject);
            GameObject.Destroy(line_renderer);
            GameObject.Destroy(edge_collider);
        }

        public virtual void AddPoint(Vector3 node_position)
        {
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

        public abstract void Translate(Vector3 new_position);
        public abstract void Rotate(Vector3 mouse_position);
        public abstract void Edit(Vector3 point, int node_index);
        public abstract void DisplayRotationGizmo();
        public abstract void HideRotationGizmo();
        public abstract IEnumerator BuildRamp();

        public Vector3 GetWorldPosition()
        {
            return ramp_gameobject.transform.position;
        }
    }
}
