using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Levels
{
    class Simulation : MonoBehaviour
    {
        [SerializeField]
        private Ball ball;
        [SerializeField]
        private LevelEdit level_edit;
        [SerializeField]
        private UIEvent ui_event;
        [SerializeField]
        private Transform ball_spawn;
        [SerializeField]
        private GameObject ball_gameobject;
        [SerializeField]
        private bool simulation_running;
        [SerializeField]
        private CameraController camera_controller;
        [SerializeField]
        private int coins_picked;
        [SerializeField]
        private GameObject[] coins;
        [SerializeField]
        private bool win = false;

        private void Awake()
        {
            ui_event = GetComponent<UIEvent>();
            level_edit = GetComponent<LevelEdit>();
            coins_picked = 0;
        }

        public void StartStopSimulation()
        {
            if (!simulation_running)
            {
                GameObject tmp = Resources.Load<GameObject>("Ball");
                ui_event.SetStopImage();
                ball_gameobject = GameObject.Instantiate(tmp);
                ball = ball_gameobject.GetComponent<Ball>();
                tmp.transform.position = ball_spawn.position;
                ball.win = new UnityEngine.Events.UnityEvent();
                ball.win.AddListener(Win);
                ball.pick_coin = new UnityEngine.Events.UnityEvent();
                ball.pick_coin.AddListener(RegisterCoin);
                simulation_running = true;
                ui_event.DisplayHideCoinIndicators(true);
            }
            else
            {
                ball.StopAllCoroutines();
                simulation_running = false;
                Destroy(ball_gameobject);
                camera_controller.StopSimulation();

                if (!win)
                { 
                    ui_event.SetPlayImage();
                    ui_event.DisplayHideCoinIndicators(false);
                    EnableAllCoins();
                    ResetPickedCoins();
                }
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
            if(simulation_running)
            {
                if(ball_gameobject.transform.position.y<=Constants.Y_KILL)
                    StartStopSimulation();
            }
        }

        public void EnableAllCoins()
        {
            for (int i = 0; i < coins.Length; i++)
                coins[i].SetActive(true);
        }

        public void RegisterCoin()
        {
            ui_event.FillCoinIndicator(coins_picked);
            coins_picked++;
        }

        public void ResetPickedCoins()
        {
            ui_event.ResetCoinIndicator();
            coins_picked = 0;
        }

        public void Win()
        {
            ui_event.EnableDisableInteractionPlaySettingsButtons(false);
            win = true;
            ui_event.DisplayWinPopUp(coins_picked);
        }

        public void ReturnLevel()
        {
            win = false;
            ui_event.SetPlayImage();
            ui_event.DisplayHideCoinIndicators(false);
            ui_event.EnableDisableInteractionPlaySettingsButtons(true);
            EnableAllCoins();
            ResetPickedCoins();
            ui_event.HideWinPopUp();
        }

        public void LoadNextLevel()
        {
            IEnumerator ChangeScene()
            {
                ui_event.AppearBlackScreen();
                yield return new WaitForSeconds(1f);
                if (SceneManager.GetActiveScene().buildIndex + 1 < Constants.LEVEL_NUMBER)
                {
                    //Load next level
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }
                else
                {
                    //Return to main menu
                    SceneManager.LoadScene(Constants.MENU_SCENE_INDEX);
                }
            }

            StartCoroutine(ChangeScene());
        }
    }
}
