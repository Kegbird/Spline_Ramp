using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;


namespace Assets.Scripts
{
    class SimulationManager : MonoBehaviour
    {
        [SerializeField]
        private BallController ball_controller;
        [SerializeField]
        private LevelManager level_manager;
        [SerializeField]
        private UIManager ui_manager;
        [SerializeField]
        private Transform ball_spawn;
        [SerializeField]
        private GameObject ball_gameobject;
        [SerializeField]
        private bool simulation_running;
        [SerializeField]
        private CameraController camera_controller;

        private void Awake()
        {
            ui_manager = GetComponent<UIManager>();
            level_manager = GetComponent<LevelManager>();
        }

        public void StartStopSimulation()
        {
            if (!simulation_running)
            {
                ui_manager.DisplayHideEditLevelBtn(false);
                GameObject tmp = Resources.Load<GameObject>("Ball");
                ui_manager.ChangeSimulationButtonText("Reset Simulation (B)");
                ball_controller = tmp.GetComponent<BallController>();
                tmp.transform.position = ball_spawn.position;
                ball_gameobject = GameObject.Instantiate(tmp);
                simulation_running = true;
                camera_controller.StartSimulationLock(ball_gameobject.transform);
            }
            else
            {
                ui_manager.DisplayHideEditLevelBtn(true);
                ui_manager.ChangeSimulationButtonText("Start Simulation (B)");
                ball_controller.StopAllCoroutines();
                simulation_running = false;
                Destroy(ball_gameobject);
                camera_controller.StopSimulation();
            }
        }

        public bool IsSimulationRunning()
        {
            return simulation_running;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
                StartStopSimulation();
        }
    }
}
