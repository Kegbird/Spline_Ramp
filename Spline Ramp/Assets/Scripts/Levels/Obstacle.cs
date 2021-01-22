using UnityEngine;
using Assets.Scripts.Ramps;

namespace Assets.Scripts.Levels
{
    public class Obstacle : MonoBehaviour
    {
        public Transform[] nodes;
        public int index;
        public float speed;
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

        // Update is called once per frame
        void Update()
        {
            if (index < nodes.Length)
            {
                transform.position = Vector3.Lerp(start, destination, (curve.Evaluate((Time.time - t) / 2f))*speed);

                if (transform.position == nodes[index].position)
                {
                    start = destination;
                    t = Time.time;
                    index++;
                    index = index % nodes.Length;
                    destination = nodes[index].position;
                }
            }
        }
    }
}
