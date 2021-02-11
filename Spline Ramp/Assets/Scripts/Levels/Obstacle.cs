using UnityEngine;
using Assets.Scripts.Ramps;

namespace Assets.Scripts.Levels
{
    public class Obstacle : MonoBehaviour
    {
        public Transform[] nodes;
        public int index;
        public float t;
        private Vector3 start;
        private Vector3 destination;
        public AnimationCurve curve;

        private void Awake()
        {
            index = 1;
            t = Time.time;
            start = nodes[0].position;
            destination = nodes[1].position;
        }

        void Update()
        {
            float interpolation = curve.Evaluate((Time.time - t));
            transform.position = start * (1 - interpolation) + destination * interpolation;

            if (interpolation >= 1)
            {
                start = destination;
                t = Time.time;
                index++;
                if (index == nodes.Length)
                    index = 1;
                destination = nodes[index].position;
            }
        }
    }
}
