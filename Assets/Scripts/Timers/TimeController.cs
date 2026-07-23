using UnityEngine;

public class TimeController : MonoBehaviour
{
    public static TimeController instance;
    private int activeTimer = 0;
    private string timeractivekey;

    //Set up Instaces
    void Awake()
    {

        if (instance == null)
            instance = this;

        timeractivekey = /*Data.CurrentRoom +*/ Constants.ActiveTimer;
        //Debug.Log("TimeController Awake");
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    //Set the time of player one on second player's device
    public void SetTimeone(string time)
    {
        //if (Chessboard.Instance.GetPlayer2() == RoomPhoton.room.GetID())
        //{
            GameTimer.timeRemaining = float.Parse(time);
            GameTimer.timeRemainingM = GameTimer.timeRemaining * 1000;//this was done because timer in milliseconds was getting out of sync for player 1, also done in GameTimer class ,update function first half 
                                                                      //if(((int)float.Parse(time)) == 0)
            if ((int)GameTimer.timeRemaining == 0)
            {
                GameOver.instance.UpdateGameOverFlag(true);
                GameOver.instance.GameEnded(1, "White:Timeover", "1");
            }
        //}
    }

    //Set the time of player two on first player's device
    public void SetTimetwo(string time)
    {
        //if (Chessboard.Instance.GetPlayer1() == RoomPhoton.room.GetID())
        //{
            GameTimer.timeRemaining2 = float.Parse(time);
            GameTimer.timeRemainingM2 = GameTimer.timeRemaining2 * 1000;//this was done because timer in milliseconds was getting out of sync for player 2, also done in GameTimer class ,update function second half 
            //if (((int)float.Parse(time)) == 0)
            if ((int)GameTimer.timeRemaining2 == 0)
            {
                GameOver.instance.UpdateGameOverFlag(true);
                GameOver.instance.GameEnded(0, "Black:Timeover", "2");
            }
        //}
    }

    //Change timer after each move
    public void SetTimer()
    {
        //Debug.Log($"[SetTimer] called | activeTimer BEFORE={activeTimer} | " +
        //      $"timerIsRunning={GameTimer.timerIsRunning} | " +
        //      $"timerIsRunning2={GameTimer.timerIsRunning2} | " +
        //      $"isExecutingRemoteMove={Chessboard.Instance.GetIsExecutingRemoteMove()}");


        if (activeTimer == 0)
        {
            activeTimer = 1;
            GameTimer.gameTimerInstance.StopTimer();
            GameTimer.gameTimerInstance.StartTimer2();
        }
        else if (activeTimer == 1)
        {
            activeTimer = 0;
            GameTimer.gameTimerInstance.StopTimer2();
            GameTimer.gameTimerInstance.StartTimer();
        }
        PlayerPrefs.SetInt(timeractivekey, activeTimer);
    }

    public int GetActiveTimer()
    {
        return activeTimer;
    }

    public void SetActiveTimer(int val)
    {
        activeTimer = val;
    }

    public void DeleteLocalPrefs()
    {
        PlayerPrefs.DeleteKey(timeractivekey);
    }
}
