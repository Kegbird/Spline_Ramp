using UnityEngine; 

namespace Assets.Scripts
{
    public static class Constants
    {
        public const int MAX_ORT_SIZE = 10;
        public const int MIN_ORT_SIZE = 3;
        public const float ZOOM_SPEED = 5f;
        public const float PAN_SPEED = 10f;
        public const float MOVE_SPEED = 10f;
        public const int ROTATION_PIVOT_INDEX = 0;
        public const float ACCELERATION_FACTOR = 0.1f;
        public const float ACCELERATION_TICK = 0.1f;
        public const float MIN_X_BOUNDARY = -10f;
        public const float RAMP_EDGE_RADIUS = 0.1f;
        public static int GOAL_LAYER_MASK= LayerMask.NameToLayer("Goal");
    }
}
