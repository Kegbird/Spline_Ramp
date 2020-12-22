using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;


namespace Assets.Scripts
{
    class SimulationManager : MonoBehaviour
    {
        [SerializeField]
        private bool stop;
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

        private void Awake()
        {
            ui_manager = GetComponent<UIManager>();
            level_manager = GetComponent<LevelManager>();
        }

        public void StartSimulation()
        {
            GameObject tmp = Resources.Load<GameObject>("Ball");
            ui_manager.DisplayHideEditLevelBtn(false);
            ball_controller = tmp.GetComponent<BallController>();
            tmp.transform.position = ball_spawn.position;
            ball_gameobject=GameObject.Instantiate(tmp);
        }

        public void ResetSimulation()
        {
            ui_manager.DisplayHideEditLevelBtn(true);
            Destroy(ball_gameobject);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
                StartSimulation();
            if (Input.GetKeyDown(KeyCode.R))
                ResetSimulation();
        }
    }
}
