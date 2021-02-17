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
        AudioManager audio_manager;
        [SerializeField]
        private Ball ball;
        [SerializeField]
        private LevelEdit level_edit;
        [SerializeField]
        private UIManager ui_manager;
        [SerializeField]
        private Transform ball_spawn;
        [SerializeField]
        private GameObject ball_gameobject;
        [SerializeField]
        private bool simulation_running;
        [SerializeField]
        private int coins_picked;
        [SerializeField]
        private GameObject[] coins;
        [SerializeField]
        private bool win = false;

        private void Awake()
        {
            audio_manager = GetComponent<AudioManager>();
            ui_manager = GetComponent<UIManager>();
            level_edit = GetComponent<LevelEdit>();
            coins_picked = 0;
        }

        public void StartStopSimulation()
        {
            if (!simulation_running)
            {
                audio_manager.PlayButtonPressedSound();
                ui_manager.SetStopImage();
                ball_gameobject = GameObject.Instantiate(Resources.Load<GameObject>("Ball"));
                ball = ball_gameobject.GetComponent<Ball>();
                ball_gameobject.transform.position = ball_spawn.position;
                ball.win = new UnityEngine.Events.UnityEvent();
                ball.win.AddListener(Win);
                ball.pick_coin = new UnityEngine.Events.UnityEvent();
                ball.pick_coin.AddListener(RegisterCoin);
                ball.ui_manager = ui_manager;
                simulation_running = true;
                ui_manager.DisplayHideCoinIndicators(true);
                ui_manager.DisplayHideEditLevelBtn(false);
            }
            else
            {
                audio_manager.PlayButtonPressedSound();
                ball.StopAllCoroutines();
                simulation_running = false;
                Destroy(ball_gameobject);
                ui_manager.SetPlayImage();
                ui_manager.DisplayHideCoinIndicators(false);
                ui_manager.DisplayHideEditLevelBtn(true);
                EnableAllCoins();
                ResetPickedCoins();
            }
        }

        public bool IsSimulationRunning()
        {
            return simulation_running;
        }

        private void Update()
        {
            if (simulation_running)
            {
                if (ball_gameobject.transform.position.y <= Constants.Y_KILL)
                {
                    ball.StopAllCoroutines();
                    simulation_running = false;
                    Destroy(ball_gameobject);
                    ui_manager.SetPlayImage();
                    ui_manager.DisplayHideCoinIndicators(false);
                    ui_manager.DisplayHideEditLevelBtn(true);
                    EnableAllCoins();
                    ResetPickedCoins();
                }
            }
        }

        public void EnableAllCoins()
        {
            if (win)
                return;
            for (int i = 0; i < coins.Length; i++)
                coins[i].SetActive(true);
        }

        public void RegisterCoin()
        {
            audio_manager.PlayCoinPickedSound();
            ui_manager.FillCoinIndicator(coins_picked);
            coins_picked++;
        }

        public void ResetPickedCoins()
        {
            if (win)
                return;
            ui_manager.ResetCoinIndicator();
            coins_picked = 0;
        }

        public void Win()
        {
            audio_manager.PlayWinSound();
            ui_manager.EnableDisableInteractionPlaySettingsButtons(false);
            win = true;
            ui_manager.DisplayWinPopUp(coins_picked);
        }

        public void ReturnLevel()
        {
            audio_manager.PlayButtonPressedSound();
            win = false;
            ui_manager.SetPlayImage();
            ui_manager.DisplayHideCoinIndicators(false);
            ui_manager.EnableDisableInteractionPlaySettingsButtons(true);
            EnableAllCoins();
            ResetPickedCoins();
            ui_manager.HideWinPopUp();
        }

        public void LoadNextLevel()
        {
            audio_manager.PlayButtonPressedSound();
            IEnumerator ChangeScene()
            {
                ui_manager.AppearBlackScreen();
                yield return new WaitForSeconds(1f);
                int level_index = SceneManager.GetActiveScene().buildIndex;
                if (Constants.coin_per_level.ContainsKey(level_index))
                {
                    if (Constants.coin_per_level[level_index] < coins_picked)
                        Constants.coin_per_level[level_index] = coins_picked;
                }
                else
                {
                    Constants.coin_per_level[level_index] = coins_picked;
                }
                if (level_index+1 <= Constants.LEVEL_NUMBER)
                {
                    //Load next level
                    SceneManager.LoadScene(level_index+1);
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
