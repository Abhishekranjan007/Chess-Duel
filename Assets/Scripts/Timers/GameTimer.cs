using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    static float time = 600f, timems = 600000;
    public static GameTimer gameTimerInstance;
    public static float timeRemaining = time;
    public static float timeRemaining2 = time;
    public static float timeRemainingM = timems;
    public static float timeRemainingM2 = timems;
    public static bool timerIsRunning = false;
    public static bool timerIsRunning2 = false;
    private string timerkey1, timerkey2;
    private string timerkeyM1, timerkeyM2;
    float elapsed = 0f, elapsed2 = 0f;

    public NetworkGameManager networkManager;
    private float lastKnownWhiteTime = -1f;
    private float lastKnownBlackTime = -1f;
    
    private bool gameStarted = false;

    //public Text timeText;
    void Awake()
    {
        if (gameTimerInstance == null)
        {
            gameTimerInstance = this;
        }
        timerkey1 = /*Data.CurrentRoom + */"timer1";
        timerkey2 = /*Data.CurrentRoom + */"timer2";
        timerkeyM1 =/* Data.CurrentRoom + */"timerM1";
        timerkeyM2 =/* Data.CurrentRoom +*/ "timerM2";
    }

    //Start timer one in game mode
    public void StartTimer()
    {
        timerIsRunning = true;
        //print("GameTimer Started "+timerIsRunning);
    }

    //Start timer two in game mode
    public void StartTimer2()
    {
        timerIsRunning2 = true;
        ////print("GameTimer Started2 " + timerIsRunning2);
    }

    //Stop timer one in game mode
    public void StopTimer()
    {
        timerIsRunning = false;
        ////print("GameTimer Stopped " + timerIsRunning);
    }

    //Stop timer two in game mode
    public void StopTimer2()
    {
        timerIsRunning2 = false;
        ////print("GameTimer Stopped2 " + timerIsRunning2);
    }

    //ReStart timer one in game mode
    public void ResetTimer()
    {
        timeRemaining = time;
        timeRemainingM = timems;
        timerIsRunning = false;
        ////print("GameTimer ResetTimer " + timerIsRunning);
    }

    //ReStart timer two in game mode
    public void ResetTimer2()
    {
        timeRemaining2 = time;
        timeRemainingM2 = timems;
        timerIsRunning2 = false;
        ////print("GameTimer ResetTimer2 " + timerIsRunning2);
    }

    public void SetNetWorkPlayer(NetworkGameManager manager)
    {
        networkManager = manager;
    }

    public void OnGameStarted()
    {
        gameStarted = true;
    }

    public void DeleteLocalPrefs()
    {
        PlayerPrefs.DeleteKey(timerkey1);
        PlayerPrefs.DeleteKey(timerkey2);
        PlayerPrefs.DeleteKey(timerkeyM1);
        PlayerPrefs.DeleteKey(timerkeyM2);
        TimeController.instance.DeleteLocalPrefs();        
    }

    //public void InitializeTimers(float startTime)
    //{
    //    timeRemaining = startTime;
    //    timeRemaining2 = startTime;

    //    timeRemainingM = startTime * 1000f;
    //    timeRemainingM2 = startTime * 1000f;

    //    timerIsRunning = false;
    //    timerIsRunning2 = false;
    //}



    //void Update()
    //{
    //    elapsed += Time.deltaTime;
    //    elapsed2 += Time.deltaTime;

    //    //Check if timer one is running in game mode
    //    if (timerIsRunning)
    //    {
    //        if (timeRemaining > 0)
    //        {
    //            timeRemainingM -= Time.deltaTime * 1000;
    //            timeRemaining -= Time.deltaTime;
    //            timeRemainingM = timeRemaining * 1000; //this was done because timer in milliseconds was getting out of sync for player 1
    //            ////Debug.Log("timeremaining " + timeRemaining);
    //            if (elapsed >= 1f)//Save time after each elapsed second
    //            {
    //                elapsed = elapsed % 1f;
    //                PlayerPrefs.SetInt(timerkey1, (int)GameTimer.timeRemaining);
    //                PlayerPrefs.SetInt(timerkeyM1, (int)GameTimer.timeRemainingM);
    //            }

    //            //DisplayTime(timeRemaining);
    //        }
    //        else
    //        {
    //            Debug.Log("Timer 1 has run out!");
    //            timeRemaining = 0;
    //            timeRemainingM = 0;
    //            timerIsRunning = false;
    //            //RoomPhoton.room.callRpcTimerEnd(1, "White:Timeover");
    //            if (Chessboard.Instance.GetGameState().CurrentGameMode == ChessGameMode.VsAI)
    //            {
    //                GameOver.instance.GameEnded(1, "White:Timeover", "7");
    //            }
    //            else
    //            {
    //                NetworkGameManager manager = FusionRoomManager.Instance.GetNetworkGameManager();

    //                if (!manager.HasStateAuthority)
    //                    return;

    //                if (manager.AmIWhite())
    //                {
    //                    GameOver.instance.GameEnded(1, "White:Timeover", "7.1");
    //                }
    //                else
    //                {
    //                    GameOver.instance.GameEnded(0, "Black:Timeover", "7.2");
    //                }
    //            }


    //        }
    //    }

    //    //Check if timer two is running in game mode
    //    if (timerIsRunning2)
    //    {
    //        if (timeRemaining2 > 0)
    //        {
    //            timeRemainingM2 -= Time.deltaTime * 1000;
    //            timeRemaining2 -= Time.deltaTime;
    //            timeRemainingM2 = timeRemaining2 * 1000; //this was done because timer in milliseconds was getting out of sync for player 2
    //            if (elapsed2 >= 1f)//Save time after each elapsed second
    //            {
    //                elapsed2 = elapsed2 % 1f;
    //                PlayerPrefs.SetInt(timerkey2, (int)GameTimer.timeRemaining2);
    //                PlayerPrefs.SetInt(timerkeyM2, (int)GameTimer.timeRemainingM2);
    //            }

    //            //DisplayTime(timeRemaining);
    //        }
    //        else
    //        {
    //            Debug.Log("Timer 2 has run out!");
    //            timeRemaining2 = 0;
    //            timeRemainingM2 = 0;
    //            timerIsRunning2 = false;
    //            //RoomPhoton.room.callRpcTimerEnd(0 , "Black:Timeover");
    //            if (Chessboard.Instance.GetGameState().CurrentGameMode == ChessGameMode.VsAI)
    //            {
    //                GameOver.instance.GameEnded(0, "Black:Timeover", "8");
    //            }
    //            else
    //            {
    //                NetworkGameManager manager = FusionRoomManager.Instance.GetNetworkGameManager();

    //                if (!manager.HasStateAuthority)
    //                    return;

    //                if (manager.AmIWhite())
    //                {
    //                    GameOver.instance.GameEnded(0, "Black:Timeover", "8.1");                        
    //                }
    //                else
    //                {
    //                    GameOver.instance.GameEnded(1, "White:Timeover", "8.2");
    //                }
    //            }


    //        }
    //    }
    //}
    void Update()
    {


        // Non-authority clients (Black) derive timer state from host's replicated values.
        // IsWhiteTurn is already correctly synced by Fusion - use it as single source of truth.
        if (networkManager != null && !networkManager.HasStateAuthority && gameStarted && !GameOver.instance.isGameover())
        {
            //Debug.Log($"[SyncBlock] Running | countdownFlag={Chessboard.Instance.countdownFlag} | IsWhiteTurn={networkManager.IsWhiteTurn}");
            timerIsRunning = networkManager.IsWhiteTurn;
            timerIsRunning2 = !networkManager.IsWhiteTurn;

            if (timerIsRunning && networkManager.WhiteRemainingTime != lastKnownWhiteTime)
            {
                lastKnownWhiteTime = networkManager.WhiteRemainingTime;
                timeRemaining = networkManager.WhiteRemainingTime;
            }

            if (timerIsRunning2 && networkManager.BlackRemainingTime != lastKnownBlackTime)
            {
                lastKnownBlackTime = networkManager.BlackRemainingTime;
                timeRemaining2 = networkManager.BlackRemainingTime;
            }
        }

        // Check if timer one is running in game mode
        if (timerIsRunning)
        {
            elapsed += Time.deltaTime;
            if (timeRemaining > 0)
            {
                timeRemainingM -= Time.deltaTime * 1000;
                timeRemaining -= Time.deltaTime;
                timeRemainingM = timeRemaining * 1000;

                if (elapsed >= 1f)
                {
                    elapsed = elapsed % 1f;
                    PlayerPrefs.SetInt(timerkey1, (int)GameTimer.timeRemaining);
                    PlayerPrefs.SetInt(timerkeyM1, (int)GameTimer.timeRemainingM);
                }
            }
            else
            {
                //Debug.Log("Timer 1 has run out!");
                timeRemaining = 0;
                timeRemainingM = 0;
                timerIsRunning = false;

                if (Chessboard.Instance.GetGameState().CurrentGameMode == ChessGameMode.VsAI)
                {
                    GameOver.instance.GameEnded(1, "White:Timeover", "7");
                }
                else
                {
                    gameStarted = false;

                    //if (networkManager.AmIWhite())
                        GameOver.instance.GameEnded(1, "White:Timeover", "7.1");
                    //else
                    //    GameOver.instance.GameEnded(0, "Black:Timeover", "7.2");
                }
            }
        }

        // Check if timer two is running in game mode
        if (timerIsRunning2)
        {
            elapsed2 += Time.deltaTime;
            if (timeRemaining2 > 0)
            {
                timeRemainingM2 -= Time.deltaTime * 1000;
                timeRemaining2 -= Time.deltaTime;
                timeRemainingM2 = timeRemaining2 * 1000;

                if (elapsed2 >= 1f)
                {
                    elapsed2 = elapsed2 % 1f;
                    PlayerPrefs.SetInt(timerkey2, (int)GameTimer.timeRemaining2);
                    PlayerPrefs.SetInt(timerkeyM2, (int)GameTimer.timeRemainingM2);
                }
            }
            else
            {
                //Debug.Log("Timer 2 has run out!");
                timeRemaining2 = 0;
                timeRemainingM2 = 0;
                timerIsRunning2 = false;

                if (Chessboard.Instance.GetGameState().CurrentGameMode == ChessGameMode.VsAI)
                {
                    GameOver.instance.GameEnded(0, "Black:Timeover", "8");
                }
                else
                {
                    gameStarted = false;

                    //if (networkManager.AmIWhite())
                        GameOver.instance.GameEnded(0, "Black:Timeover", "8.1");
                    //else
                    //    GameOver.instance.GameEnded(1, "White:Timeover", "8.2");
                }
            }
        }
    }

    //Utility function to display timer(Not operational, game scene has own display function)
    public void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        //timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
