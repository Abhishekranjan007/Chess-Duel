using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeView : MonoBehaviour
{
    public GameObject whiteplTimeronedt, blackplTimeronedt, whiteplTimertwodt, blackplTimertwodt;
    
    private bool whPlfiveMinTimeflag = false, blPlfiveMinTimeflag = false,
                 whPloneMinTimeflag = false, blPloneMinTimeflag = false, whPloneSecTimeflag = false, blPloneSecTimeflag = false;
    string devicetype, offlineTimerColor = "#282828";
    private float timeanimfive = 300f, timeanimone = 60f, timeanimonesec = 1f;
    public GameObject topwaitingTimerdt, bottomwaitingTimerdt;
    Color newCol;

   
    //Method to display white timer for white player
    public void DisplayTimeWhite(float timeToDisplay, UnityEngine.Color color)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        if (Color.red == color)
        {
            topwaitingTimerdt.GetComponent<TMP_Text>().text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (ColorUtility.TryParseHtmlString(offlineTimerColor, out newCol))
            {
                whiteplTimeronedt.GetComponent<TMP_Text>().color = newCol;
                topwaitingTimerdt.GetComponent<TMP_Text>().color = newCol;
            }
            else
            {
                whiteplTimeronedt.GetComponent<TMP_Text>().color = color;
                topwaitingTimerdt.GetComponent<TMP_Text>().color = color;
            }
        }
        else
        {
            whiteplTimeronedt.GetComponent<TMP_Text>().text = string.Format("{0:00}:{1:00}", minutes, seconds);
            whiteplTimeronedt.GetComponent<TMP_Text>().color = color;
        }
    }

    //Method to display black timer for white player
    public void DisplayTimeBlack(float timeToDisplay, UnityEngine.Color color)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        if (Color.red == color)
        {
            topwaitingTimerdt.GetComponent<TMP_Text>().text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (ColorUtility.TryParseHtmlString(offlineTimerColor, out newCol))
            {
                blackplTimeronedt.GetComponent<TMP_Text>().color = newCol;
                topwaitingTimerdt.GetComponent<TMP_Text>().color = newCol;
            }
            else
            {
                blackplTimeronedt.GetComponent<TMP_Text>().color = color;
                topwaitingTimerdt.GetComponent<TMP_Text>().color = color;
            }
        }
        else
        {
            blackplTimeronedt.GetComponent<TMP_Text>().text = string.Format("{0:00}:{1:00}", minutes, seconds);
            blackplTimeronedt.GetComponent<TMP_Text>().color = color;
        }
    }

    //Method to display white timer for black player
    public void DisplayTimeWhiteBl(float timeToDisplay, UnityEngine.Color color)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        if (Color.red == color)
        {
            topwaitingTimerdt.GetComponent<TMP_Text>().text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (ColorUtility.TryParseHtmlString(offlineTimerColor, out newCol))
            {
                blackplTimertwodt.GetComponent<TMP_Text>().color = newCol;
                topwaitingTimerdt.GetComponent<TMP_Text>().color = newCol;
            }
            else
            {
                blackplTimertwodt.GetComponent<TMP_Text>().color = color;
                topwaitingTimerdt.GetComponent<TMP_Text>().color = color;
            }
        }
        else
        {
            blackplTimertwodt.GetComponent<TMP_Text>().text = string.Format("{0:00}:{1:00}", minutes, seconds);
            blackplTimertwodt.GetComponent<TMP_Text>().color = color;
        }
    }

    //Method to display white timer for white player
    public void DisplayTimeBlackBl(float timeToDisplay, UnityEngine.Color color)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        if (Color.red == color)
        {
            topwaitingTimerdt.GetComponent<TMP_Text>().text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (ColorUtility.TryParseHtmlString(offlineTimerColor, out newCol))
            {
                whiteplTimertwodt.GetComponent<TMP_Text>().color = newCol;
                topwaitingTimerdt.GetComponent<TMP_Text>().color = newCol;
            }
            else
            {
                whiteplTimertwodt.GetComponent<TMP_Text>().color = color;
                topwaitingTimerdt.GetComponent<TMP_Text>().color = color;
            }
        }
        else
        {
            whiteplTimertwodt.GetComponent<TMP_Text>().text = string.Format("{0:00}:{1:00}", minutes, seconds);
            whiteplTimertwodt.GetComponent<TMP_Text>().color = color;
        }
    }

    public void DisplayTime()
    {
        //if (Chessboard.Instance.GetPlayer1() == RoomPhoton.room.GetID() && !SideHeader.instance.isconnectionMesOn)
        //{
            if (GameTimer.timeRemaining >= 0f)
            {
                DisplayTimeWhite(GameTimer.timeRemaining, Chessboard.Instance.GetBlackColor());
            }

            if (GameTimer.timeRemaining2 >= 0f)
            {
                DisplayTimeBlack(GameTimer.timeRemaining2, Chessboard.Instance.GetBlackColor());
            }
        //}
        //else if (Chessboard.Instance.GetPlayer2() == RoomPhoton.room.GetID() && !SideHeader.instance.isconnectionMesOn)
        //{
        //    if (GameTimer.timeRemaining >= 0f)
        //    {
        //        DisplayTimeBlackBl(GameTimer.timeRemaining, Chessboard.Instance.GetBlackColor());
        //    }

        //    if (GameTimer.timeRemaining2 >= 0f)
        //    {
        //        DisplayTimeWhiteBl(GameTimer.timeRemaining2, Chessboard.Instance.GetBlackColor());
        //    }
        //}
    }    

    //Player one time animation
    public void PlayerOneTimeAnim()
    {
        if (GameTimer.timeRemaining <= timeanimfive && whPlfiveMinTimeflag == false)
        {
            whPlfiveMinTimeflag = true;
            //whiteOneanimdt.SetTrigger("TimeScaleIn");
        }

        if (GameTimer.timeRemaining2 <= timeanimfive && blPlfiveMinTimeflag == false)
        {
            blPlfiveMinTimeflag = true;
            //blackOneanimdt.SetTrigger("TimeScaleIn");
        }

        if (GameTimer.timeRemaining <= timeanimone && whPloneMinTimeflag == false)
        {
            whPloneMinTimeflag = true;
            //whiteOneanimdt.SetTrigger("TimeScaleLoopIn");
        }

        if (GameTimer.timeRemaining2 <= timeanimone && blPloneMinTimeflag == false)
        {
            blPloneMinTimeflag = true;
            //blackOneanimdt.SetTrigger("TimeScaleLoopIn");
        }

        if (GameTimer.timeRemaining <= timeanimonesec && whPloneSecTimeflag == false)
        {
            whPloneSecTimeflag = true;
            blPloneSecTimeflag = true;
            //whiteOneanimdt.SetTrigger("TimeScaleIn");
            //blackOneanimdt.SetTrigger("TimeScaleIn");
        }

        if (GameTimer.timeRemaining2 <= timeanimonesec && blPloneSecTimeflag == false)
        {
            whPloneSecTimeflag = true;
            blPloneSecTimeflag = true;
            //whiteOneanimdt.SetTrigger("TimeScaleIn");
            //blackOneanimdt.SetTrigger("TimeScaleIn");
        }
    }

    //Player two time animation
    public void PlayerTwoTimeAnim()
    {
        if (GameTimer.timeRemaining <= timeanimfive && whPlfiveMinTimeflag == false)
        {
            whPlfiveMinTimeflag = true;
            //whiteTwoanimdt.SetTrigger("TimeScaleBOIN");
        }

        if (GameTimer.timeRemaining2 <= timeanimfive && blPlfiveMinTimeflag == false)
        {
            blPlfiveMinTimeflag = true;
            //blackTwoanimdt.SetTrigger("TimeScaleBOIN");

        }

        if (GameTimer.timeRemaining <= timeanimone && whPloneMinTimeflag == false)
        {
            whPloneMinTimeflag = true;
            //whiteTwoanimdt.SetTrigger("TimeScaleBOLoopIn");
        }

        if (GameTimer.timeRemaining2 <= timeanimone && blPloneMinTimeflag == false)
        {
            blPloneMinTimeflag = true;
            //blackTwoanimdt.SetTrigger("TimeScaleBOLoopIn");
        }

        if (GameTimer.timeRemaining <= timeanimonesec && whPloneSecTimeflag == false)
        {
            whPloneSecTimeflag = true;
            blPloneSecTimeflag = true;
            //whiteTwoanimdt.SetTrigger("TimeScaleBOIN");
            //blackTwoanimdt.SetTrigger("TimeScaleBOIN");
        }

        if (GameTimer.timeRemaining2 <= timeanimonesec && blPloneSecTimeflag == false)
        {
            whPloneSecTimeflag = true;
            blPloneSecTimeflag = true;
            //whiteTwoanimdt.SetTrigger("TimeScaleBOIN");
            //blackTwoanimdt.SetTrigger("TimeScaleBOIN");
        }
    }

    //Player one timer display
    public void DisplayTimerOne()
    {
        whiteplTimeronedt.SetActive(true);
        blackplTimeronedt.SetActive(true);
    }

    //Player Two timer display
    public void DisplayTimerTwo()
    {
        whiteplTimertwodt.SetActive(true);
        blackplTimertwodt.SetActive(true);
    }

    public bool GetWhPloneMinTimeflag()
    {
        return whPloneMinTimeflag;
    }

    public bool GetBlPloneMinTimeflag()
    {
        return blPloneMinTimeflag;
    }
}
