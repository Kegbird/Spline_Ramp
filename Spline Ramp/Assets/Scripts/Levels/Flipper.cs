using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Levels
{
    public class Flipper : MonoBehaviour
    {
        private void Update()
        {
            transform.rotation *= Quaternion.Euler(new Vector3(0, 0, Constants.ROTATION_SPEED* Time.deltaTime));
        }
    }
}