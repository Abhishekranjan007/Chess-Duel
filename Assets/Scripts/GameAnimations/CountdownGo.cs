using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownGo : MonoBehaviour
{ 

    public TMP_Text countdownDisplaydt;
    public GameObject gettingReady;
    public Animator countdown_anim;

    //string devicetype;


    // Start is called before the first frame update
    void Awake()
    {        
    }

    //Coroutine call to start Countdown timer
    public void StartCountDown()
    {        
        StartCoroutine(CountdownToStart());
    }

    //Actual Coroutine to start Countdown timer
    IEnumerator CountdownToStart()
    {        
        while (Chessboard.Instance.GetCountdownTime() > 0)
        {
            yield return new WaitForSeconds(1f);

            Chessboard.Instance.SetCountdownTime(Chessboard.Instance.GetCountdownTime() - 1);

        }

        Chessboard.Instance.countdownFlag = false;

        yield return new WaitForSeconds(1f);

        if (Chessboard.Instance.GetGameState().CurrentGameMode == ChessGameMode.VsAI)
        {
            GameTimer.gameTimerInstance.StartTimer();
        }
        else
        {
            NetworkGameManager manager = FusionRoomManager.Instance.GetNetworkGameManager();
          //  Debug.Log($"[Countdown] About to start timer | AmIWhite={manager.AmIWhite()} " +
          //$"timeRemaining={GameTimer.timeRemaining} " +
          //$"timeRemaining2={GameTimer.timeRemaining2}");
            if (manager.AmIWhite())
            {
                GameTimer.gameTimerInstance.StartTimer();
            }
            else
            {
                TimeController.instance.SetActiveTimer(1);
                GameTimer.gameTimerInstance.StartTimer2();
                //Debug.Log($"[Countdown] About to start | countdownFlag={Chessboard.Instance.countdownFlag}");
            }
        }
        GameTimer.gameTimerInstance.OnGameStarted();
        AnimHandler.instance.StartAnim(0);        
        //Debug.Log("After 3-2-1 go ");
        
        //if (Chessboard.Instance.GetPlayer1() == RoomPhoton.room.GetID())
        //{
        //    string uid = PlayerPrefs.GetString("Uid");
        //    string rid = Data.RoomId;
        //    string roomTimeId = Data.RoomTimeId;
        //    InitiateTime(uid, rid, roomTimeId);
        //}
    }

    public void HideGettingReadyImage()
    {
        gettingReady.SetActive(false);
    }

    public void StartCoundownAnim()
    {
        countdown_anim.SetTrigger("StartCoundown");
    }

    //Handles UI of countdown
    public void ShowCountdown()
    {       
       countdownDisplaydt.gameObject.SetActive(true);       
    }
}
