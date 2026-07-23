using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public static GameOver instance;
    private static bool isGameOver = true;
    private string gameEndReason = "", cRoom, message, devicetype, time;
    private int onoffCount = 0;
    private bool leaveRoomCalled = false;
    //public TimeView timeView;
    string v;
    NetworkGameManager manager;

    void Awake()
    {
        if (instance == null)
            instance = this;       
    }   

    //Called when game is over, stops game, shows popup,
    //clears room and its information
    public void GameEnded(int team, string mes, string caller)
    {
        isGameOver = true;
        //SideHeader.instance.HideReadytoResumePopup();
        gameEndReason = mes;

        if (Chessboard.Instance.GetGameState().CurrentGameMode == ChessGameMode.VsAI)
        {
            message = (team == 0) ? "White Player wins" : ((team == 1) ? "Black Player wins" : (team == 2) ? "It's a draw" : "You Won as Other Player Left");
            PlayerPrefs.SetInt("team", team);


            string isdraw = "false";
            string tempWinner = "";
            string tempWinnerId = "";

            if (message.Contains("draw") || message.Contains("Draw") || message.Contains("DRAW"))//== "It's a draw"
            {
                isdraw = "true";


                if (GameTimer.timeRemainingM > GameTimer.timeRemainingM2)
                {
                    PlayerPrefs.SetString("winner", "white");
                    PlayerPrefs.SetString("winnername", PlayerPrefs.GetString(Constants.OwnName));
                    tempWinner = PlayerPrefs.GetString(Constants.OwnName);
                    UIManager.Instance.ShowGameOverUI(0, 1);
                    //tempWinnerId = Chessboard.Instance.GetPlayer1();
                    //GameOverAnim.instance.SetStalematePopMessage("You win by time.");
                    v = "1";
                }
                else if (GameTimer.timeRemainingM < GameTimer.timeRemainingM2)
                {
                    PlayerPrefs.SetString("winner", "black");
                    PlayerPrefs.SetString("winnername", PlayerPrefs.GetString(Constants.OppName));
                    tempWinner = PlayerPrefs.GetString(Constants.OppName);
                    UIManager.Instance.ShowGameOverUI(1, 1);
                    //tempWinnerId = Chessboard.Instance.GetPlayer2();
                    //GameOverAnim.instance.SetStalematePopMessage("You lose by time.");
                    v = "2";
                }
                //else
                //{
                //    tempWinnerId = Chessboard.Instance.GetPlayer1();
                //}
            }
            else
            {
                if (message == "White Player wins")
                {
                    PlayerPrefs.SetString("winner", "white");
                    PlayerPrefs.SetString("winnername", PlayerPrefs.GetString(Constants.OwnName));
                    UIManager.Instance.ShowGameOverUI(0, 0);
                    //tempWinnerId = Chessboard.Instance.GetPlayer1();
                }
                else if (message == "Black Player wins")
                {
                    PlayerPrefs.SetString("winner", "black");
                    PlayerPrefs.SetString("winnername", PlayerPrefs.GetString(Constants.OppName));
                    UIManager.Instance.ShowGameOverUI(1, 0);
                    //tempWinnerId = Chessboard.Instance.GetPlayer2();
                }

                tempWinner = PlayerPrefs.GetString("winnername");
            }
        }
        else
        {
            if (manager == null)
                manager = FusionRoomManager.Instance.GetNetworkGameManager();

            message = (team == 0) ? "White Player wins" : ((team == 1) ? "Black Player wins" : (team == 2) ? "It's a draw" : "You Won as Other Player Left");
            PlayerPrefs.SetInt("team", team);


            string isdraw = "false";
            string tempWinner = "";
            string tempWinnerId = "";

            if (message.Contains("draw") || message.Contains("Draw") || message.Contains("DRAW"))//== "It's a draw"
            {
                isdraw = "true";

                //if (Chessboard.Instance.GetPlayer1() == RoomPhoton.room.GetID())
                if (manager.AmIWhite())
                {
                    if (GameTimer.timeRemainingM > GameTimer.timeRemainingM2)
                    {
                        PlayerPrefs.SetString("winner", "white");
                        PlayerPrefs.SetString("winnername", PlayerPrefs.GetString(Constants.OwnName));
                        tempWinner = PlayerPrefs.GetString(Constants.OwnName);
                        UIManager.Instance.ShowGameOverUI(0, 1);
                        //tempWinnerId = Chessboard.Instance.GetPlayer1();
                        //GameOverAnim.instance.SetStalematePopMessage("You win by time.");
                        v = "1";
                    }
                    else if (GameTimer.timeRemainingM < GameTimer.timeRemainingM2)
                    {
                        PlayerPrefs.SetString("winner", "black");
                        PlayerPrefs.SetString("winnername", PlayerPrefs.GetString(Constants.OppName));
                        tempWinner = PlayerPrefs.GetString(Constants.OppName);
                        UIManager.Instance.ShowGameOverUI(1, 1);
                        //tempWinnerId = Chessboard.Instance.GetPlayer2();
                        //GameOverAnim.instance.SetStalematePopMessage("You lose by time.");
                        v = "2";
                    }
                    //else
                    //{
                    //    tempWinnerId = Chessboard.Instance.GetPlayer1();
                    //}
                }
                //else if (Chessboard.Instance.GetPlayer2() == RoomPhoton.room.GetID())
                else
                {
                    if (GameTimer.timeRemainingM > GameTimer.timeRemainingM2)
                    {
                        PlayerPrefs.SetString("winner", "white");
                        PlayerPrefs.SetString("winnername", PlayerPrefs.GetString(Constants.OppName));
                        tempWinner = PlayerPrefs.GetString(Constants.OppName);
                        UIManager.Instance.ShowGameOverUI(1, 1);
                        //tempWinnerId = Chessboard.Instance.GetPlayer1();
                        //GameOverAnim.instance.SetStalematePopMessage("You lose by time.");
                        v = "3";
                    }
                    else if (GameTimer.timeRemainingM < GameTimer.timeRemainingM2)
                    {
                        PlayerPrefs.SetString("winner", "black");
                        PlayerPrefs.SetString("winnername", PlayerPrefs.GetString(Constants.OwnName));
                        tempWinner = PlayerPrefs.GetString(Constants.OwnName);
                        UIManager.Instance.ShowGameOverUI(0, 1);
                        //tempWinnerId = Chessboard.Instance.GetPlayer2();
                        //GameOverAnim.instance.SetStalematePopMessage("You win by time.");
                        v = "4";
                    }
                    //else
                    //{
                    //    tempWinnerId = Chessboard.Instance.GetPlayer1();
                    //}
                }

            }
            else
            {
                //if (Chessboard.Instance.GetPlayer1() == RoomPhoton.room.GetID())
                if (manager.AmIWhite())
                {
                    if (message == "White Player wins")
                    {
                        PlayerPrefs.SetString("winner", "white");
                        PlayerPrefs.SetString("winnername", PlayerPrefs.GetString(Constants.OwnName));
                        UIManager.Instance.ShowGameOverUI(0, 0);
                        //tempWinnerId = Chessboard.Instance.GetPlayer1();
                    }
                    else if (message == "Black Player wins")
                    {
                        PlayerPrefs.SetString("winner", "black");
                        PlayerPrefs.SetString("winnername", PlayerPrefs.GetString(Constants.OppName));
                        UIManager.Instance.ShowGameOverUI(1, 0);
                        //tempWinnerId = Chessboard.Instance.GetPlayer2();
                    }
                }
                //else if (Chessboard.Instance.GetPlayer2() == RoomPhoton.room.GetID())
                else
                {
                    if (message == "White Player wins")
                    {
                        PlayerPrefs.SetString("winner", "white");
                        PlayerPrefs.SetString("winnername", PlayerPrefs.GetString(Constants.OppName));
                        UIManager.Instance.ShowGameOverUI(1, 0);
                        //tempWinnerId = Chessboard.Instance.GetPlayer1();
                    }
                    else if (message == "Black Player wins")
                    {
                        PlayerPrefs.SetString("winner", "black");
                        PlayerPrefs.SetString("winnername", PlayerPrefs.GetString(Constants.OwnName));
                        UIManager.Instance.ShowGameOverUI(0, 0);
                        //tempWinnerId = Chessboard.Instance.GetPlayer2();
                    }
                }
                tempWinner = PlayerPrefs.GetString("winnername");

            }


            //GameTimer.gameTimerInstance.StopTimer();
            //GameTimer.gameTimerInstance.StopTimer2();

            //GameOverMessageSent(cRoom, team.ToString());
            //Chessboard.Instance.CheckMate(team);
            //bool myTurn = manager.IsMyTurn();
        }



        
        


        
        //if (caller != "10")
        //{
        //    DeleteRoomReq(Data.CurrentRoom, PlayerPrefs.GetString(Constants.Uid), Data.RoomId, Data.RoomTimeId, tempWinner, tempWinnerId, gameEndReason, isdraw, caller, mes);
        //}
        //else
        //{
        //    HandleGameOverForOverGames(message, caller);
        //}

        //Chessboard.Instance.SetWhitePlOnOff("offline");
        //Chessboard.Instance.SetBlackPlOnOff("offline");
        //StartCoroutine("OnlineOfflineAdminPanel");
    }



    //private IEnumerator OnlineOfflineAdminPanel()
    //{
    //    yield return new WaitForSeconds(2.0f);
    //    if (isGameOver == true)
    //    {
    //        onoffCount++;
    //        Chessboard.Instance.SendOnOfflineAdminPanel();
    //    }
    //    if (onoffCount < 5)
    //    {
    //        StartCoroutine("OnlineOfflineAdminPanel");
    //    }

    //}

    ////Function called when other player has left
    //public void OtherPlayerLeft()
    //{
    //    if (isGameOver == false && RoomPhoton.room.NumberofPlayer() < 2)
    //    {
    //        //print("GameOver OtherPlayerLeft");
    //        string tempmes;
    //        if (Chessboard.Instance.GetOtherPlayerLeftFlag() != "")
    //        {
    //            tempmes = Chessboard.Instance.GetOtherPlayerLeftFlag();
    //        }
    //        else
    //        {
    //            tempmes = "Other Player Left";
    //        }
    //        message = "You Won as Other Player Left";
    //        if (Chessboard.Instance.GetPlayer1() == RoomPhoton.room.GetID())
    //        {
    //            //print("GameOver OtherPlayerLeft in player1 ");
    //            GameEnded(0, tempmes, "g");
    //        }
    //        else if (Chessboard.Instance.GetPlayer2() == RoomPhoton.room.GetID())
    //        {
    //            //print("GameOver OtherPlayerLeft in player2 ");
    //            GameEnded(1, tempmes, "h");
    //        }
    //        else
    //        {
    //            //print("GameOver OtherPlayerLeft in else ");
    //            GameEnded(2, tempmes, "i");
    //        }

    //    }
    //}

    ////Things to be done when players leave room
    //private IEnumerator leaveRoomCall()
    //{
    //    yield return new WaitForSeconds(2.0f);
    //    if (leaveRoomCalled == false)
    //    {
    //        ////print("Endgame called");
    //        Destroy(RoomPhoton.room.gameObject);
    //        RoomPhoton.room.Leave();
    //        leaveRoomCalled = true;
    //    }
    //    yield return null;
    //}

    ////Game over (clean up and UI work)
    //public void HandleGameOver(string data)
    //{
    //    GameOverState gameOverState = JsonUtility.FromJson<GameOverState>(data);
    //    int c = 0;
    //    List<string> moves = Chessboard.Instance.GetMoves();
    //    foreach (var move in moves)
    //    {
    //        //Console.WriteLine("move {0} is {1} ", c, move);
    //        c = c + 1;
    //    }

    //    //byte[] bt4 = Serialize2(moves.ToArray());
    //    //string base64Tex4 = System.Convert.ToBase64String(bt4);

    //    string[] movesArray = moves.ToArray();
    //    string base64Tex4 = string.Join(", ", movesArray);
    //    PlayerPrefs.SetString("Moves", base64Tex4);
    //    string uid = PlayerPrefs.GetString(Constants.Uid);
    //    string rid = Data.RoomId;
    //    time = Data.RoomTime;
    //    string roomTimeId = Data.RoomTimeId;
    //    //string s4 = SaveState(uid, rid, roomTimeId, time, state4, base64Tex4);

    //    if (devicetype == "mobile")
    //    {
    //        GameOverAnim.instance.CloseGameLeavePopup();
    //        //timeView.ResetTimeTriggers();
    //        if (SideHeader.instance.header.activeSelf && SideHeader.instance.isHeaderanimPlaying == false)
    //        {
    //            //print("on 6335");
    //            SideHeader.instance.HeaderOffRoutine();
    //        }
    //    }
    //    else if (devicetype == "tab")
    //    {
    //        GameOverAnim.instance.CloseGameLeavePopup();
    //        //timeView.ResetTimeTriggers();
    //        if (SideHeader.instance.headertb.activeSelf && SideHeader.instance.isHeaderanimPlaying == false)
    //        {
    //            //print("on 6377");
    //            SideHeader.instance.HeaderOffRoutine();
    //        }
    //    }
    //    else
    //    {
    //        GameOverAnim.instance.CloseGameLeavePopup();
    //        //timeView.ResetTimeTriggers();
    //        if (SideHeader.instance.headerdt.activeSelf && SideHeader.instance.isHeaderanimPlaying == false)
    //        {
    //            //print("on 6420");
    //            SideHeader.instance.HeaderOffRoutine();
    //        }
    //    }
    //    SideHeader.instance.HideReadytoResumePopup();
    //    //print("HandleGameOver called1");
    //    ShowAndHide(gameOverState.Message, gameOverState.Caller);
    //    StartCoroutine(leaveRoomCall());
    //}

    //Utility Coroutine to clear the data at ending of the room and show messages
    private void ShowAndHide(string mes, string caller)
    {
        //string player1 = Chessboard.Instance.GetPlayer1();
        //string player2 = Chessboard.Instance.GetPlayer2();
        
        //AnimHandler.instance.GameOverAnimation(player1, player2, message);

        //if gameover is because of timer then the losing player's timer should become zero
        //if (mes.Contains("Timeover"))
        //{
            
        //    if (player1 == RoomPhoton.room.GetID())
        //    {
        //        if (mes == "White:Timeover")
        //        {
        //            //white player timer is zero, lower timer that is
        //            timeView.DisplayTimeWhite(0, Chessboard.Instance.GetBlackColor());
        //        }
        //        else if (mes == "Black:Timeover")
        //        {
        //            //black player timer is zero, upper timer that is
        //            timeView.DisplayTimeBlack(0, Chessboard.Instance.GetBlackColor());
        //        }
        //    }
        //    else
        //    {
        //        if (mes == "White:Timeover")
        //        {
        //            //white player timer is zero, upper timer that is
        //            timeView.DisplayTimeBlackBl(0, Chessboard.Instance.GetBlackColor());
        //        }
        //        else if (mes == "Black:Timeover")
        //        {
        //            //black player timer is zero, lower timer that is
        //            timeView.DisplayTimeWhiteBl(0, Chessboard.Instance.GetBlackColor());
        //        }
        //    }
            
        //}

        //char Rnum = Data.CurrentRoom[Data.CurrentRoom.Length - 1];
        //string Rnum = Data.CurrentRoom.ToString().Split(' ')[1];
        //Chessboard.Instance.SetCastlingFlags();

        //PlayerPrefs.DeleteKey(Constants.LastBoardState + Data.CurrentRoom);
        PlayerPrefs.DeleteKey(Constants.WhiteChessManKilled);
        PlayerPrefs.DeleteKey(Constants.BlackChessManKilled);
        PlayerPrefs.DeleteKey(Constants.WhitekingmovedFlag);
        PlayerPrefs.DeleteKey(Constants.WhiterookleftmovedFlag);
        PlayerPrefs.DeleteKey(Constants.WhiterookrightmovedFlag);
        PlayerPrefs.DeleteKey(Constants.BlackkingmovedFlag);
        PlayerPrefs.DeleteKey(Constants.BlackrookleftmovedFlag);
        PlayerPrefs.DeleteKey(Constants.BlackrookrightmovedFlag);

        ////print("Deleting Spec stored data if any");
        //PlayerPrefs.DeleteKey(Constants.SpecRoom + Rnum);
        //PlayerPrefs.DeleteKey(Constants.SpecBoardState + Rnum);
        //PlayerPrefs.DeleteKey(Constants.SpecKilledWh + Rnum);
        //PlayerPrefs.DeleteKey(Constants.SpecKilledBl + Rnum);
        //PlayerPrefs.DeleteKey(Constants.SpecWhiteTime + Rnum);
        //PlayerPrefs.DeleteKey(Constants.SpecBlackTime + Rnum);
        //PlayerPrefs.DeleteKey(Constants.SpecTurn + Rnum);
        //PlayerPrefs.DeleteKey(Constants.SpecMove + Rnum);
        PlayerPrefs.DeleteKey(Constants.TimerOne);
        PlayerPrefs.DeleteKey(Constants.TimerTwo);
        //PlayerPrefs.DeleteKey(Data.CurrentRoom + Constants.TimerOne);
        //PlayerPrefs.DeleteKey(Data.CurrentRoom + Constants.TimerTwo);

        //Over(cRoom);//Socket
        string key = cRoom + Constants.Player1;
        string key2 = cRoom + Constants.Player2;
        //print("Inside HomeAction 2");
        //Data.CurrentRoom = cRoom;
        string isdraw = "false";
        string tempWinner = "";
        //if (message.Contains("draw") || message.Contains("Draw") || message.Contains("DRAW"))//== "It's a draw"
        //{
        //    isdraw = "true";

        //    if (player1 == RoomPhoton.room.GetID())
        //    {
        //        if (GameTimer.timeRemainingM > GameTimer.timeRemainingM2)
        //        {
        //            PlayerPrefs.SetString("winner", "white");
        //            PlayerPrefs.SetString("winnername", PlayerPrefs.GetString(Constants.OwnName));
        //            tempWinner = PlayerPrefs.GetString(Constants.OwnName);
        //        }
        //        else if (GameTimer.timeRemainingM < GameTimer.timeRemainingM2)
        //        {
        //            PlayerPrefs.SetString("winner", "black");
        //            PlayerPrefs.SetString("winnername", PlayerPrefs.GetString(Constants.OppName));
        //            tempWinner = PlayerPrefs.GetString(Constants.OppName);
        //        }
        //    }
        //    else if (player2 == RoomPhoton.room.GetID())
        //    {
        //        if (GameTimer.timeRemainingM > GameTimer.timeRemainingM2)
        //        {
        //            PlayerPrefs.SetString("winner", "white");
        //            PlayerPrefs.SetString("winnername", PlayerPrefs.GetString(Constants.OppName));
        //            tempWinner = PlayerPrefs.GetString(Constants.OppName);
        //        }
        //        else if (GameTimer.timeRemainingM < GameTimer.timeRemainingM2)
        //        {
        //            PlayerPrefs.SetString("winner", "black");
        //            PlayerPrefs.SetString("winnername", PlayerPrefs.GetString(Constants.OwnName));
        //            tempWinner = PlayerPrefs.GetString(Constants.OwnName);
        //        }
        //    }

            
        //}
        //else
        //{
        //    tempWinner = PlayerPrefs.GetString("winnername");
            
        //}

        string tempWinnerId;
        
        //if (player1 == RoomPhoton.room.GetID())
        //{
            
        //    if (PlayerPrefs.GetString("winnername") == PlayerPrefs.GetString(Constants.OwnName))
        //    {
                
        //        tempWinnerId = player1;
        //    }
        //    else /*if (PlayerPrefs.GetString("winnername") == PlayerPrefs.GetString(Constants.OppName))*/
        //    {
                
        //        tempWinnerId = player2;
        //    }

        //}
        //else
        //{
            
        //    if (PlayerPrefs.GetString("winnername") == PlayerPrefs.GetString(Constants.OppName))
        //    {
                
        //        tempWinnerId = player1;
        //    }
        //    else /*if (PlayerPrefs.GetString("winnername") == PlayerPrefs.GetString("OwnName"))*/
        //    {
                
        //        tempWinnerId = player2;
        //    }

        //}


        
        PlayerPrefs.SetString("message", message);
        
        //string screenlogs = "winner name: " + tempWinner + " " + " winner id: " + tempWinnerId + " - " + " userid :" + PlayerPrefs.GetString(Constants.Uid) + "::" + PlayerPrefs.GetString(Constants.OwnName) + "::" + PlayerPrefs.GetString(Constants.OppName) + "::" + PlayerPrefs.GetString("winnername") + "::" + player1 + "::" + player2 + "::" + RoomPhoton.room.GetID() + "::" + message + "::" + caller;
        
        //DeleteRoomReq(Data.CurrentRoom, PlayerPrefs.GetString("Uid"), Data.RoomId, Data.RoomTimeId, tempWinner, tempWinnerId, gameEndReason, isdraw);
        PlayerPrefs.DeleteKey(Constants.OppUid);
        PlayerPrefs.DeleteKey(Constants.OppName);
        PlayerPrefs.DeleteKey(Constants.ORoom);
        PlayerPrefs.DeleteKey(Constants.WhitePlayer);
        PlayerPrefs.DeleteKey(Constants.BlackPlayer);
        //ClearRoomsGameOver(Data.CurrentRoom);

    }

    public bool isGameover()
    {
        return isGameOver;
    }

    public void UpdateGameOverFlag(bool val)
    {
        isGameOver = val;
    }

    public bool GetleaveRoomflag()
    {
        return leaveRoomCalled;
    }
    public void SetleaveRoomflag(bool val)
    {
        leaveRoomCalled = val;
    }

    public string GetMessage()
    {
        return message;
    }

    //If in case some over room still lingers on, it's end is handled
    //public void HandleGameOverForOverGames(string message, string caller)
    //{
    //    int c = 0;
    //    List<string> moves = Chessboard.Instance.GetMoves();
    //    foreach (var move in moves)
    //    {
    //        //Console.WriteLine("move {0} is {1} ", c, move);
    //        c = c + 1;
    //    }

    //    //byte[] bt4 = Serialize2(moves.ToArray());
    //    //string base64Tex4 = System.Convert.ToBase64String(bt4);

    //    string[] movesArray = moves.ToArray();
    //    string base64Tex4 = string.Join(", ", movesArray);
    //    PlayerPrefs.SetString("Moves", base64Tex4);
    //    string uid = PlayerPrefs.GetString(Constants.Uid);
    //    string rid = Data.RoomId;
    //    time = Data.RoomTime;
    //    string roomTimeId = Data.RoomTimeId;
    //    //string s4 = SaveState(uid, rid, roomTimeId, time, state4, base64Tex4);

    //    if (devicetype == "mobile")
    //    {
    //        GameOverAnim.instance.CloseGameLeavePopup();
    //        //timeView.ResetTimeTriggers();
    //        if (SideHeader.instance.header.activeSelf && SideHeader.instance.isHeaderanimPlaying == false)
    //        {
    //            //print("on 6335");
    //            SideHeader.instance.HeaderOffRoutine();
    //        }
    //    }
    //    else if (devicetype == "tab")
    //    {
    //        GameOverAnim.instance.CloseGameLeavePopup();
    //        //timeView.ResetTimeTriggers();
    //        if (SideHeader.instance.headertb.activeSelf && SideHeader.instance.isHeaderanimPlaying == false)
    //        {
    //            //print("on 6377");
    //            SideHeader.instance.HeaderOffRoutine();
    //        }
    //    }
    //    else
    //    {
    //        GameOverAnim.instance.CloseGameLeavePopup();
    //        //timeView.ResetTimeTriggers();
    //        if (SideHeader.instance.headerdt.activeSelf && SideHeader.instance.isHeaderanimPlaying == false)
    //        {
    //            //print("on 6420");
    //            SideHeader.instance.HeaderOffRoutine();
    //        }
    //    }

    //    ////print("Endgame called1");
    //    ShowAndHide(message, caller);
    //    StartCoroutine(leaveRoomCall());
    //}
}
