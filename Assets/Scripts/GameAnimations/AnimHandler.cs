using UnityEngine;
using UnityEngine.UI;

public class AnimHandler : MonoBehaviour
{
    public static AnimHandler instance;
    public Animator whitePlTrnAnimdt, blackPlTrnAnimdt, whitePlChkAnimdt,
        blackPlChkAnimdt;

    public Animator animWinOnedt, animWinTwodt, animDefeatOnedt, animDefeatTwodt, animDrawOnedt, animDrawTwodt;

    public Animator w_Holderdt, b_Holderdt;
    public Text logdt;

    bool isWhiteTurn, playerOneCheck = false, playerTwoCheck = false, playerOneTurnFlag = true, playerTwoTurnFlag = false;
    public bool topofflineactive = false, bottomofflineactive = false, turnoffForOffline = false;

    string devicetype = null, player1 = null, player2 = null;
    public GameObject upperCheckObjectdt, lowerCheckObjectdt;
    public Text yourTurnDowndt, yourTurnTopdt;
    public Text yourTurnDownreddt, yourTurnTopreddt;
    public GameObject upperCheckText, lowerCheckText;

    
    public GameObject upplayerprofiledt, downplayerprofiledt;
    public Animator upholderanimdt, downholderanimdt;
    private string whiteplTurnin = "BottomTurnIn", blackplTurnin = "TopTurnIn",
                    whiteplTurnout = "BottomTurnOut", blackplTurnout = "TopTurnOut", 
        //whiteplTurnin = "White Player Turn In", blackplTurnin = "Black Player Turn In",
        //            whiteplTurnout = "White Player Turn Out", blackplTurnout = "Black Player Turn Out",
                    whiteplconnlost = "White Player Connection Lost In", whiteplconnlostout = "White Player Back Online",
        blackplleave = "Black Player Connection Lost In", blackplleaveout = "Black Player Back Online",
        waittowin = "White Player Wait to win", waittowinout = "White Player Wait to win Out",
        whiteplconnoutsolo = "White Player Connection Out Solo";

    private string whitePlayerWinin = "White Player Win In", whitePlayerWinout = "White Player Win Out",
        whitePlayerDefeatin = "White Player Defeat In", whitePlayerDefeatout = "White Player Defeat Out",
        blackPlayerWinin = "Black Player Win In", blackPlayerWinout = "Black Player Win Out",
        blackPlayerDefeatin = "Black Player Defeat In", blackPlayerDefeatout = "Black Player Defeat Out",
        blackPlayerDrawin = "Black Player Draw In", whitePlayerDrawin = "White Player Draw In";

    private string lowerCheckin = "BottomCheckIn", lowerCheckOut = "BottomCheckOut";
    private string upperCheckin = "TopCheckIn", upperCheckOut = "TopCheckOut";

    public GameObject infodt, connectionLostdt, connectionLost1dt;
    NetworkGameManager manager;

    

    //InterOp calls to javascript for http calls to server
    //[DllImport("__Internal")]
    //private static extern string ShouldWeSendReadyMessage(string tournamentId, string roomid, string roomTimeId);

    //InterOp calls to javascript for http calls to server
    //[DllImport("__Internal")]
    //private static extern string NeedReadyMessage(string tournamentId, string roomid, string roomTimeId);

    void Awake()
    {
        if (instance == null)
            instance = this;
    }


    //Play the turn animation for both players
    //public void TurnAnim()
    //{
    //    Debug.Log("TurnAnim sees turn = " +Chessboard.Instance.IsWhiteTurn());

    //    if (Chessboard.Instance.GetGameState().CurrentGameMode == ChessGameMode.VsAI)
    //    {
    //        isWhiteTurn = Chessboard.Instance.IsWhiteTurn();

    //        Debug.Log("Inside TurnAnim AI | WhiteTurn = " + isWhiteTurn);

    //        if (isWhiteTurn)
    //        {
    //            // WHITE'S TURN

    //            if (playerOneCheck)
    //            {
    //                playerOneCheck = false;
    //                WhitePlayerCheckOutDt();
    //            }

    //            if (playerTwoCheck)
    //            {
    //                playerTwoCheck = false;
    //                BlackPlayerCheckOutDt();
    //            }

    //            WhitePlayerTurnInDt();
    //            playerOneTurnFlag = true;

    //            BlackPlayerTurnOutDt();
    //            playerTwoTurnFlag = false;
    //        }
    //        else
    //        {
    //            // BLACK'S TURN

    //            if (playerOneCheck)
    //            {
    //                playerOneCheck = false;
    //                WhitePlayerCheckOutDt();
    //            }

    //            if (playerTwoCheck)
    //            {
    //                playerTwoCheck = false;
    //                BlackPlayerCheckOutDt();
    //            }

    //            WhitePlayerTurnOutDt();
    //            playerOneTurnFlag = false;

    //            BlackPlayerTurnInDt();
    //            playerTwoTurnFlag = true;
    //        }

    //        return;
    //    }
    //    else
    //    {
    //        if (manager == null)
    //            manager = FusionRoomManager.Instance.GetNetworkGameManager();


    //        isWhiteTurn = Chessboard.Instance.IsWhiteTurn();

    //        bool myTurn = manager.IsMyTurn();
    //            //(manager.AmIWhite() && Chessboard.Instance.IsWhiteTurn()) ||
    //            //(manager.AmIBlack() && !Chessboard.Instance.IsWhiteTurn());

    //        Debug.Log($@"TurnAnim Multiplayer
    //                    AmIWhite : {manager.AmIWhite()}
    //                    AmIBlack : {manager.AmIBlack()}
    //                    IsWhiteTurn : {isWhiteTurn}
    //                    MyTurn : {myTurn}");

    //        // Remove any active check animations first
    //        //if (playerOneCheck)
    //        //{
    //        //    playerOneCheck = false;
    //        //    WhitePlayerCheckOutDt();
    //        //}

    //        //if (playerTwoCheck)
    //        //{
    //        //    playerTwoCheck = false;
    //        //    BlackPlayerCheckOutDt();
    //        //}

    //        if (myTurn)
    //        {
    //            // Bottom player (me)
    //            WhitePlayerTurnInDt();
    //            playerOneTurnFlag = true;

    //            // Top player (opponent)
    //            BlackPlayerTurnOutDt();
    //            playerTwoTurnFlag = false;
    //        }
    //        else
    //        {
    //            // Bottom player (me)
    //            WhitePlayerTurnOutDt();
    //            playerOneTurnFlag = false;

    //            // Top player (opponent)
    //            BlackPlayerTurnInDt();
    //            playerTwoTurnFlag = true;
    //        }

    //        return;
    //    }

    //    //if (Chessboard.Instance.GetGameState().CurrentGameMode == ChessGameMode.VsAI)
    //    //{
    //    //    Debug.Log($"TurnAnim called | WhiteTurn = {Chessboard.Instance.IsWhiteTurn()} | Frame = {Time.frameCount}");
    //    //    isWhiteTurn = Chessboard.Instance.IsWhiteTurn();
    //    //    if (isWhiteTurn)
    //    //    {
    //    //        if (playerOneCheck == false)//if (!checkOne.activeSelf)
    //    //        {
    //    //            //print("**01");                   
    //    //            WhitePlayerTurnInDt();
    //    //            playerOneTurnFlag = true;
    //    //        }
    //    //        else if (playerOneCheck == true)
    //    //        {
    //    //            //print("**02");
    //    //            playerOneCheck = false;
    //    //            WhitePlayerCheckOutDt();
    //    //        }

    //    //        if (playerTwoCheck == false)
    //    //        {
    //    //            //print("**03");                   
    //    //            BlackPlayerTurnOutDt();
    //    //            playerTwoTurnFlag = false;
    //    //        }
    //    //        else if (playerTwoCheck == true)
    //    //        {
    //    //            //print("**04");                    
    //    //            BlackPlayerCheckOutDt();
    //    //        }
    //    //    }
    //    //    else
    //    //    {
    //    //        //if (playerTwoCheck == false && playerOneCheck == false)
    //    //        if (playerTwoCheck == false)
    //    //        {
    //    //            BlackPlayerTurnOutDt();
    //    //            playerTwoTurnFlag = false;
    //    //            //print("**13");
    //    //        }
    //    //        else if (playerTwoCheck == true)
    //    //        {
    //    //            //print("**14");
    //    //            playerTwoCheck = false;
    //    //            //if (Chessboard.Instance.insideCanvasDesktop)
    //    //            //{
    //    //            WhitePlayerCheckOutDt();
    //    //            //BlackPlayerCheckOutDt();
    //    //            //}
    //    //            //else
    //    //            //{
    //    //            //    BlackPlayerCheckRotOutDt();
    //    //            //}
    //    //        }

    //    //        if (playerOneCheck == false)//if (!checkOne.activeSelf)
    //    //        {
    //    //            WhitePlayerTurnInDt();
    //    //            playerOneTurnFlag = true;
    //    //            //print("**15");
    //    //        }
    //    //        else if (playerOneCheck == true)
    //    //        {
    //    //            playerOneCheck = false;
    //    //            //if (Chessboard.Instance.insideCanvasDesktop)
    //    //            //{
    //    //            BlackPlayerCheckOutDt();
    //    //            //WhitePlayerCheckOutDt();
    //    //            //}
    //    //            //else
    //    //            //{
    //    //            //    WhitePlayerCheckRotOutDt();
    //    //            //}

    //    //            WhitePlayerTurnInDt();
    //    //            playerOneTurnFlag = true;
    //    //            //print("**16");
    //    //        }
    //    //    }
    //    //}
    //    //else
    //    //{
    //    //    if (manager == null)
    //    //        manager = FusionRoomManager.Instance.GetNetworkGameManager();

    //    //    isWhiteTurn = Chessboard.Instance.IsWhiteTurn();

    //    //    //print("TurnAnim called");
    //    //    if (isWhiteTurn)
    //    //    {
    //    //        //if (player1 == RoomPhoton.room.GetID())
    //    //        //{
    //    //        if (manager.AmIWhite())
    //    //        {
    //    //            if (playerOneCheck == false)//if (!checkOne.activeSelf)
    //    //            {
    //    //                //print("**01");                   
    //    //                WhitePlayerTurnInDt();
    //    //                playerOneTurnFlag = true;
    //    //            }
    //    //            else if (playerOneCheck == true)
    //    //            {
    //    //                //print("**02");
    //    //                playerOneCheck = false;
    //    //                WhitePlayerCheckOutDt();
    //    //            }

    //    //            if (playerTwoCheck == false)
    //    //            {
    //    //                //print("**03");                   
    //    //                BlackPlayerTurnOutDt();
    //    //                playerTwoTurnFlag = false;
    //    //            }
    //    //            else if (playerTwoCheck == true)
    //    //            {
    //    //                //print("**04");                    
    //    //                BlackPlayerCheckOutDt();
    //    //            }

    //    //        }
    //    //        //else if (player2 == RoomPhoton.room.GetID())
    //    //        else if (manager.AmIBlack())
    //    //        {
    //    //            if (playerOneCheck == false && playerTwoCheck == false)
    //    //            {
    //    //                WhitePlayerTurnOutDt();
    //    //                playerOneTurnFlag = false;
    //    //                //print("**05");
    //    //            }
    //    //            else if (playerOneCheck == true)
    //    //            {
    //    //                playerOneCheck = false;
    //    //                //if (Chessboard.Instance.insideCanvasDesktop)
    //    //                //{
    //    //                BlackPlayerCheckOutDt();
    //    //                //WhitePlayerCheckOutDt();
    //    //                //}
    //    //                //else
    //    //                //{
    //    //                //    WhitePlayerCheckRotOutDt();
    //    //                //}
    //    //            }

    //    //            if (playerTwoCheck == false)//if (!checkTwo.activeSelf)
    //    //            {
    //    //                BlackPlayerTurnInDt();
    //    //                playerTwoTurnFlag = true;
    //    //                //print("**07");
    //    //            }
    //    //            else if (playerTwoCheck == true)
    //    //            {
    //    //                playerTwoCheck = false;
    //    //                //if (Chessboard.Instance.insideCanvasDesktop)
    //    //                //{
    //    //                WhitePlayerCheckOutDt();
    //    //                //BlackPlayerCheckOutDt();
    //    //                //}
    //    //                //else
    //    //                //{
    //    //                //    BlackPlayerCheckRotOutDt();
    //    //                //}

    //    //                BlackPlayerTurnInDt();
    //    //                playerTwoTurnFlag = true;
    //    //                //print("**08");

    //    //            }

    //    //        }
    //    //    }
    //    //    else
    //    //    {
    //    //        //if (player1 == RoomPhoton.room.GetID())
    //    //        //{
    //    //        if (manager.AmIWhite())
    //    //        {
    //    //            if (playerTwoCheck == false)//if (!checkTwo.activeSelf)
    //    //            {
    //    //                //print("**09");                    
    //    //                BlackPlayerTurnInDt();
    //    //                playerTwoTurnFlag = true;
    //    //            }
    //    //            else if (playerTwoCheck == true)
    //    //            {
    //    //                //print("**10");
    //    //                playerTwoCheck = false;
    //    //                BlackPlayerCheckOutDt();
    //    //            }

    //    //            if (playerOneCheck == false)
    //    //            {
    //    //                //print("**11");  
    //    //                WhitePlayerTurnOutDt();
    //    //                playerOneTurnFlag = false;
    //    //            }
    //    //            else if (playerOneCheck == true)
    //    //            {
    //    //                //print("**12");
    //    //                playerOneCheck = false;
    //    //                WhitePlayerCheckOutDt();
    //    //            }


    //    //        }
    //    //        //else if (player2 == RoomPhoton.room.GetID())
    //    //        else if (manager.AmIBlack())
    //    //        {
    //    //            if (playerTwoCheck == false && playerOneCheck == false)
    //    //            {
    //    //                BlackPlayerTurnOutDt();
    //    //                playerTwoTurnFlag = false;
    //    //                //print("**13");
    //    //            }
    //    //            else if (playerTwoCheck == true)
    //    //            {
    //    //                //print("**14");
    //    //                playerTwoCheck = false;
    //    //                //if (Chessboard.Instance.insideCanvasDesktop)
    //    //                //{
    //    //                WhitePlayerCheckOutDt();
    //    //                //BlackPlayerCheckOutDt();
    //    //                //}
    //    //                //else
    //    //                //{
    //    //                //    BlackPlayerCheckRotOutDt();
    //    //                //}
    //    //            }

    //    //            if (playerOneCheck == false)//if (!checkOne.activeSelf)
    //    //            {
    //    //                WhitePlayerTurnInDt();
    //    //                playerOneTurnFlag = true;
    //    //                //print("**15");
    //    //            }
    //    //            else if (playerOneCheck == true)
    //    //            {
    //    //                playerOneCheck = false;
    //    //                //if (Chessboard.Instance.insideCanvasDesktop)
    //    //                //{
    //    //                BlackPlayerCheckOutDt();
    //    //                //WhitePlayerCheckOutDt();
    //    //                //}
    //    //                //else
    //    //                //{
    //    //                //    WhitePlayerCheckRotOutDt();
    //    //                //}

    //    //                WhitePlayerTurnInDt();
    //    //                playerOneTurnFlag = true;
    //    //                //print("**16");
    //    //            }

    //    //        }
    //    //    }
    //    //}
    //}
    public void TurnAnim()
    {
        //Debug.Log("TurnAnim sees turn = " + Chessboard.Instance.IsWhiteTurn());

        if (Chessboard.Instance.GetGameState().CurrentGameMode == ChessGameMode.VsAI)
        {
            isWhiteTurn = Chessboard.Instance.IsWhiteTurn();

            if (isWhiteTurn)
            {
                if (playerOneCheck)
                {
                    playerOneCheck = false;
                    WhitePlayerCheckOutDt();
                }

                if (playerTwoCheck)
                {
                    playerTwoCheck = false;
                    BlackPlayerCheckOutDt();
                }

                if (!playerOneTurnFlag)
                {
                    WhitePlayerTurnInDt();
                    playerOneTurnFlag = true;
                }

                if (playerTwoTurnFlag)
                {
                    BlackPlayerTurnOutDt();
                    playerTwoTurnFlag = false;
                }
            }
            else
            {
                if (playerOneCheck)
                {
                    playerOneCheck = false;
                    WhitePlayerCheckOutDt();
                }

                if (playerTwoCheck)
                {
                    playerTwoCheck = false;
                    BlackPlayerCheckOutDt();
                }

                if (playerOneTurnFlag)
                {
                    WhitePlayerTurnOutDt();
                    playerOneTurnFlag = false;
                }

                if (!playerTwoTurnFlag)
                {
                    BlackPlayerTurnInDt();
                    playerTwoTurnFlag = true;
                }
            }

            return;
        }

        // -------- MULTIPLAYER --------

        if (manager == null)
            manager = FusionRoomManager.Instance.GetNetworkGameManager();

        bool myTurn = manager.IsMyTurn();

        //Debug.Log($@"TurnAnim Multiplayer
        //        AmIWhite : {manager.AmIWhite()}
        //        AmIBlack : {manager.AmIBlack()}
        //        IsWhiteTurn : {Chessboard.Instance.IsWhiteTurn()}
        //        MyTurn : {myTurn}");

        if (myTurn)
        {
            // Bottom player

            if (!playerOneTurnFlag)
            {
                WhitePlayerTurnInDt();
                playerOneTurnFlag = true;
            }

            // Top player

            if (playerTwoTurnFlag)
            {
                BlackPlayerTurnOutDt();
                playerTwoTurnFlag = false;
            }
        }
        else
        {
            // Bottom player

            if (playerOneTurnFlag)
            {
                WhitePlayerTurnOutDt();
                playerOneTurnFlag = false;
            }

            // Top player

            if (!playerTwoTurnFlag)
            {
                BlackPlayerTurnInDt();
                playerTwoTurnFlag = true;
            }
        }
    }

    public void ClearCheckAnimation()
    {
        if (playerOneCheck)
        {
            playerOneCheck = false;
            WhitePlayerCheckOutDt();
        }

        if (playerTwoCheck)
        {
            playerTwoCheck = false;
            BlackPlayerCheckOutDt();
        }
    }


    //Play turn animation at the start of game or if a player rejoins
    public void StartAnim(int num)
    {
        //if (player1 == null)
        //{
        //    Init();
        //}

        if (Chessboard.Instance.GetGameState().CurrentGameMode == ChessGameMode.VsAI)
        {
            if (num == 0)//white player's turn
            {
                //print("Num 0 white player");         
                WhitePlayerTurnInDt();
                playerOneTurnFlag = true;
            }
            else if (num == 1)//black player's turn
            {
                //print("Num 1 white player");               
                BlackPlayerTurnInDt();
                playerTwoTurnFlag = true;
            }
        }
        else
        {
            if (manager == null)
                manager = FusionRoomManager.Instance.GetNetworkGameManager();

            //if (player1 == RoomPhoton.room.GetID())
            //{
            if (manager.AmIWhite())
            {
                if (num == 0)//white player's turn
                {
                    //print("Num 0 white player");         
                    WhitePlayerTurnInDt();
                    playerOneTurnFlag = true;
                }
                else if (num == 1)//black player's turn
                {
                    //print("Num 1 white player");               
                    BlackPlayerTurnInDt();
                    playerTwoTurnFlag = true;
                }
            }
            //else if (player2 == RoomPhoton.room.GetID())
            else if (manager.AmIBlack())
            {
                if (num == 0)//white player's turn
                {
                    //print("Num 0 black player "+ devicetype);
                    BlackPlayerTurnInDt();
                    playerTwoTurnFlag = true;
                }
                else if (num == 1)//black player's turn
                {
                    //print("Num 1 black player "+ devicetype);             
                    WhitePlayerTurnInDt();
                    playerTwoTurnFlag = true;
                }
            }
        }
            
    }

    //Play check animation if a player rejoins
    public void StartCheckAnim(int num)
    {
        //if (player1 == null)
        //{
        //    Init();
        //}

        if (Chessboard.Instance.GetGameState().CurrentGameMode == ChessGameMode.VsAI)
        {
            if (num == 0)//white player's turn
            {
                //print(" StartCheckAnim Num 0 white player");
                playerOneCheck = true;
                WhitePlayerCheckInDt();
            }
            else if (num == 1)//black player's turn
            {
                //print(" StartCheckAnim Num 1 white player");
                playerTwoCheck = true;
                BlackPlayerCheckInDt();
            }
        }
        else
        {
            if (manager == null)
                manager = FusionRoomManager.Instance.GetNetworkGameManager();

            //if (player1 == RoomPhoton.room.GetID())
            //{
            if (manager.AmIWhite())
            {
                if (num == 0)//white player's turn
                {
                    //print(" StartCheckAnim Num 0 white player");
                    playerOneCheck = true;
                    WhitePlayerCheckInDt();
                }
                else if (num == 1)//black player's turn
                {
                    //print(" StartCheckAnim Num 1 white player");
                    playerTwoCheck = true;
                    BlackPlayerCheckInDt();
                }
            }
            //else if (player2 == RoomPhoton.room.GetID())
            else if (manager.AmIBlack())
            {
                if (num == 0)//white player's turn
                {
                    //print(" StartCheckAnim2 Num 0 black player");
                    playerOneCheck = true;
                    //if (Chessboard.Instance.insideCanvasDesktop)
                    //{
                    BlackPlayerCheckInDt();
                    //}
                    //else
                    //{
                    //    WhitePlayerCheckInDt();
                    //}
                }
                else if (num == 1)//black player's turn
                {
                    //print("StartCheckAnim2 Num 1 black player");
                    playerTwoCheck = true;
                    //if (Chessboard.Instance.insideCanvasDesktop)
                    //{
                    WhitePlayerCheckInDt();
                    //}
                    //else
                    //{
                    //    BlackPlayerCheckInDt();
                    //}
                }
            }
        }
            
    }

    //Initialization setup
    private void Init()
    {
        //devicetype = PlayerPrefs.GetString(Constants.Device);
        isWhiteTurn = Chessboard.Instance.IsWhiteTurn();
        //player1 = Chessboard.Instance.GetPlayer1();
        //player2 = Chessboard.Instance.GetPlayer2();        
    }

    //Check Animation
   // public void HandleCheckAnimation(ChessPiece targetKing)
   // {
   //     if (Chessboard.Instance.GetGameState().CurrentGameMode == ChessGameMode.VsAI)
   //     {
   //         // -------- AI MODE --------
   //         if (targetKing.team == 0)
   //         {
   //             playerOneCheck = true;
   //             playerTwoCheck = false;

   //             WhitePlayerCheckInDt();
   //             BlackPlayerCheckOutDt();
   //         }
   //         else
   //         {
   //             playerOneCheck = false;
   //             playerTwoCheck = true;

   //             WhitePlayerCheckOutDt();
   //             BlackPlayerCheckInDt();
   //         }

   //         WhitePlayerTurnOutDt();
   //         BlackPlayerTurnOutDt();
   //         return;
   //     }

   //     // -------- MULTIPLAYER --------

   //     if (manager == null)
   //         manager = FusionRoomManager.Instance.GetNetworkGameManager();

   //     if (manager == null)
   //         return;

   //     Debug.Log(
   //$"CHECK ANIM | Local={manager.Runner.LocalPlayer}" +
   //$" | AmIWhite={manager.AmIWhite()}" +
   //$" | TargetKing={targetKing.team}" +
   //$" | MyKingChecked={((manager.AmIWhite() && targetKing.team == 0) || (manager.AmIBlack() && targetKing.team == 1))}" +
   //$" | Frame={Time.frameCount}");

   //     bool myKingChecked =
   //         (manager.AmIWhite() && targetKing.team == 0) ||
   //         (manager.AmIBlack() && targetKing.team == 1);
       

   //     if (myKingChecked)
   //     {
   //         // Bottom player (me) is in check
   //         playerOneCheck = true;
   //         playerTwoCheck = false;

   //         WhitePlayerCheckInDt();
   //         BlackPlayerCheckOutDt();
   //     }
   //     else
   //     {
   //         // Top player (opponent) is in check
   //         playerOneCheck = false;
   //         playerTwoCheck = true;

   //         WhitePlayerCheckOutDt();
   //         BlackPlayerCheckInDt();
   //     }

   //     // Hide turn animations while check animation is active
   //     WhitePlayerTurnOutDt();
   //     BlackPlayerTurnOutDt();
   // }

    public void HandleCheckAnimation(ChessPiece targetKing)
    {
        if (Chessboard.Instance.GetGameState().CurrentGameMode == ChessGameMode.VsAI)
        {
            // -------- AI MODE --------
            if (targetKing.team == 0)
            {
                // Bottom player enters check
                if (!playerOneCheck)
                {
                    WhitePlayerCheckInDt();
                    playerOneCheck = true;
                }

                // Top player leaves check (only if needed)
                if (playerTwoCheck)
                {
                    BlackPlayerCheckOutDt();
                    playerTwoCheck = false;
                }
            }
            else
            {
                // Top player enters check
                if (!playerTwoCheck)
                {
                    BlackPlayerCheckInDt();
                    playerTwoCheck = true;
                }

                // Bottom player leaves check (only if needed)
                if (playerOneCheck)
                {
                    WhitePlayerCheckOutDt();
                    playerOneCheck = false;
                }
            }

            // Hide turn indicators only if currently visible
            if (playerOneTurnFlag)
            {
                WhitePlayerTurnOutDt();
                playerOneTurnFlag = false;
            }

            if (playerTwoTurnFlag)
            {
                BlackPlayerTurnOutDt();
                playerTwoTurnFlag = false;
            }

            return;
        }

        // -------- MULTIPLAYER --------

        if (manager == null)
            manager = FusionRoomManager.Instance.GetNetworkGameManager();

        if (manager == null)
            return;

        bool myKingChecked =
            (manager.AmIWhite() && targetKing.team == 0) ||
            (manager.AmIBlack() && targetKing.team == 1);

        if (myKingChecked)
        {
            // Bottom player enters check
            if (!playerOneCheck)
            {
                WhitePlayerCheckInDt();
                playerOneCheck = true;
            }

            // Top player leaves check
            if (playerTwoCheck)
            {
                BlackPlayerCheckOutDt();
                playerTwoCheck = false;
            }
        }
        else
        {
            // Top player enters check
            if (!playerTwoCheck)
            {
                BlackPlayerCheckInDt();
                playerTwoCheck = true;
            }

            // Bottom player leaves check
            if (playerOneCheck)
            {
                WhitePlayerCheckOutDt();
                playerOneCheck = false;
            }
        }

        // Hide turn indicators only if currently visible
        if (playerOneTurnFlag)
        {
            WhitePlayerTurnOutDt();
            playerOneTurnFlag = false;
        }

        if (playerTwoTurnFlag)
        {
            BlackPlayerTurnOutDt();
            playerTwoTurnFlag = false;
        }
    }
    //public void HandleCheckAnimation(ChessPiece targetKing)
    //{
    //    if (manager == null)
    //        manager = FusionRoomManager.Instance.GetNetworkGameManager();

    //    if (manager.AmIWhite())
    //    {
    //        if (targetKing.team == 0)
    //        {
    //            playerOneCheck = true;
    //            WhitePlayerCheckInDt();

    //            if (playerTwoCheck == true)
    //            {                    
    //                playerTwoCheck = false;
    //                BlackPlayerCheckOutDt();
    //            }
    //            else
    //            {
    //                BlackPlayerTurnOutDt();
    //                playerTwoTurnFlag = false;
    //            }

    //            if (playerOneTurnFlag == true)
    //            {
    //                WhitePlayerTurnOutDt();
    //                playerOneTurnFlag = false;
    //            }

    //            if (playerTwoTurnFlag == true)
    //            {
    //                playerTwoTurnFlag = false;
    //                BlackPlayerTurnOutDt();
    //            }

    //        }
    //        else if (targetKing.team == 1)
    //        {
    //            playerTwoCheck = true;
    //            //print("Inside white***" + 1);                
    //            if (playerOneCheck == true)
    //            {                    
    //                playerOneCheck = false;
    //                WhitePlayerCheckOutDt();                    
    //            }
    //            else
    //            {                   
    //                WhitePlayerTurnOutDt();
    //                playerOneTurnFlag = false;
    //            }


    //            if (playerOneTurnFlag == true)
    //            {
    //                WhitePlayerTurnOutDt();                    
    //                playerOneTurnFlag = false;
    //            }

    //            if (playerTwoTurnFlag == true)
    //            {
    //                playerTwoTurnFlag = false;
    //                BlackPlayerTurnOutDt();
    //            }


    //            BlackPlayerCheckInDt();


    //        }
    //    }
    //    else if (manager.AmIBlack())
    //    {
    //        print("Inside black***");
    //        if (targetKing.team == 0)
    //        {

    //            playerOneCheck = true;
    //            if (playerTwoCheck == true)
    //            {
    //                playerTwoCheck = false;
    //                //if (Chessboard.Instance.insideCanvasDesktop)
    //                //{
    //                WhitePlayerCheckOutDt();
    //                //}
    //                //else
    //                //{
    //                //    BlackPlayerCheckRotOutDt();
    //                //}
    //            }
    //            else
    //            {
    //                WhitePlayerTurnOutDt();
    //                playerOneTurnFlag = false;
    //            }

    //            if (playerOneTurnFlag == true)
    //            {
    //                WhitePlayerTurnOutDt();
    //                playerOneTurnFlag = false;
    //            }

    //            if (playerTwoTurnFlag == true)
    //            {
    //                playerTwoTurnFlag = false;
    //                BlackPlayerTurnOutDt();
    //            }
    //            //if (Chessboard.Instance.insideCanvasDesktop)
    //            //{
    //            BlackPlayerCheckInDt();
    //            //}
    //            //else
    //            //{
    //            //    WhitePlayerCheckRotInDt();
    //            //}
    //        }
    //        else if (targetKing.team == 1)
    //        {
    //            playerTwoCheck = true;
    //            //if (Chessboard.Instance.insideCanvasDesktop)
    //            //{
    //            WhitePlayerCheckInDt();
    //            //}
    //            //else
    //            //{
    //            //    BlackPlayerCheckRotInDt();
    //            //}
    //            if (playerOneCheck == true)
    //            {
    //                //Console.WriteLine("Check {0} is {1} ", targetKing.team, 7);
    //                playerOneCheck = false;
    //                //if (Chessboard.Instance.insideCanvasDesktop)
    //                //{
    //                BlackPlayerCheckOutDt();
    //                //}
    //                //else
    //                //{
    //                //    WhitePlayerCheckRotOutDt();
    //                //}
    //            }
    //            else
    //            {
    //                BlackPlayerTurnOutDt();
    //                playerTwoTurnFlag = false;
    //            }

    //            if (playerOneTurnFlag == true)
    //            {
    //                WhitePlayerTurnOutDt();
    //                playerOneTurnFlag = false;
    //            }

    //            if (playerTwoTurnFlag == true)
    //            {
    //                playerTwoTurnFlag = false;
    //                BlackPlayerTurnOutDt();
    //            }
    //        }
    //    }
    //}

    //GameOver Animation
    public void GameOverAnimation(string player1, string player2, string message)
    {        
        //if (player1 == RoomPhoton.room.GetID())
        //{
            if (message == "White Player wins")
            {
                //GameOverAnim.instance.ParticleConfettiStop();
                PlayerPrefs.SetString("winner", "white");
                PlayerPrefs.SetString("winnername", PlayerPrefs.GetString("OwnName"));                               
                animWinOnedt.SetTrigger("WinOneIn");
                animDefeatTwodt.SetTrigger("DefeatTwoIn");
                if (playerTwoCheck == true)
                {
                    playerTwoCheck = false;
                    BlackPlayerCheckOutDt();
                }
                if (playerOneCheck == true)
                {
                    playerOneCheck = false;
                    WhitePlayerCheckOutDt();
                }
                if (playerOneTurnFlag == true)
                {
                    WhitePlayerTurnOutDt();
                }

                if (!isWhiteTurn && (playerTwoTurnFlag == true))
                {
                    BlackPlayerTurnOutDt();
                }
            }
            else if (message == "Black Player wins")
            {
                //GameOverAnim.instance.StopConfettiForLoserOrDraw();
                PlayerPrefs.SetString("winner", "black");
                PlayerPrefs.SetString("winnername", PlayerPrefs.GetString("OppName"));
                animWinTwodt.SetTrigger("WinTwoIn");
                animDefeatOnedt.SetTrigger("DefeatOneIn");
                if (playerOneCheck == true)
                {
                    playerOneCheck = false;
                    WhitePlayerCheckOutDt();
                }
                if (playerTwoCheck == true)
                {
                    playerTwoCheck = false;
                    BlackPlayerCheckOutDt();
                }
                if (playerTwoTurnFlag == true)
                {
                    BlackPlayerTurnOutDt();
                }

                if (isWhiteTurn && (playerOneTurnFlag == true))
                {
                    WhitePlayerTurnOutDt();
                }
            }
            else if (message.Contains("draw") || message.Contains("Draw") || message.Contains("DRAW"))
            {
                //GameOverAnim.instance.StopConfettiForLoserOrDraw();                
                animDrawOnedt.SetTrigger("DrawOneIn");
                animDrawTwodt.SetTrigger("DrawTwoIn");
                if (playerOneTurnFlag == true)
                {
                    WhitePlayerTurnOutDt();
                }

                if (playerTwoTurnFlag == true)
                {
                    BlackPlayerTurnOutDt();
                }

                if (!isWhiteTurn && (playerTwoTurnFlag == true))
                {
                    BlackPlayerTurnOutDt();
                }
                if (isWhiteTurn && (playerOneTurnFlag == true))
                {
                    WhitePlayerTurnOutDt();
                }
            }
            else if (message == "You Won as Other Player Left")
            {
                //GameOverAnim.instance.ParticleConfettiStop();
                PlayerPrefs.SetString("winner", "white");
                PlayerPrefs.SetString("winnername", PlayerPrefs.GetString("OwnName"));
                animWinOnedt.SetTrigger("WinOneIn");
                animDefeatTwodt.SetTrigger("DefeatTwoIn");
                if (playerOneTurnFlag == true)
                {
                    WhitePlayerTurnOutDt();
                }

                if (!isWhiteTurn && (playerTwoTurnFlag == true))
                {
                    BlackPlayerTurnOutDt();
                }
                if (isWhiteTurn && (playerOneTurnFlag == true))
                {
                    WhitePlayerTurnOutDt();
                }

            }
        //}
        //else if (player2 == RoomPhoton.room.GetID())
        //{
            if (message == "White Player wins")
            {
                //GameOverAnim.instance.StopConfettiForLoserOrDraw();
                PlayerPrefs.SetString("winner", "white");
                PlayerPrefs.SetString("winnername", PlayerPrefs.GetString("OppName"));
                animWinTwodt.SetTrigger("WinTwoIn");
                animDefeatOnedt.SetTrigger("DefeatOneIn");
                if (playerOneCheck == true)
                {
                    playerOneCheck = false;
                    WhitePlayerCheckOutDt();
                }
                if (playerTwoCheck == true)
                {
                    playerTwoCheck = false;
                    BlackPlayerCheckOutDt();
                }
                if (playerTwoTurnFlag == true)
                {
                    BlackPlayerTurnOutDt();
                }

                if (!isWhiteTurn && (playerOneTurnFlag == true))
                {
                    WhitePlayerTurnOutDt();
                }
            }
            else if (message == "Black Player wins")
            {
                //GameOverAnim.instance.ParticleConfettiStop();
                PlayerPrefs.SetString("winner", "black");
                PlayerPrefs.SetString("winnername", PlayerPrefs.GetString("OwnName"));
                animWinOnedt.SetTrigger("WinOneIn");
                animDefeatTwodt.SetTrigger("DefeatTwoIn");
                if (playerTwoCheck == true)
                {
                    playerTwoCheck = false;
                    BlackPlayerCheckOutDt();
                }
                if (playerOneCheck == true)
                {
                    playerOneCheck = false;
                    WhitePlayerCheckOutDt();
                }
                if (playerOneTurnFlag == true)
                {
                    WhitePlayerTurnOutDt();
                }

                if (isWhiteTurn && (playerTwoTurnFlag == true))
                {
                    BlackPlayerTurnOutDt();
                }

            }
            else if (message.Contains("draw") || message.Contains("Draw") || message.Contains("DRAW"))
            {
                //GameOverAnim.instance.StopConfettiForLoserOrDraw();  
                animDrawOnedt.SetTrigger("DrawOneIn");
                animDrawTwodt.SetTrigger("DrawTwoIn");
                if (playerOneTurnFlag == true)
                {
                    BlackPlayerTurnOutDt();
                }
                if (playerTwoTurnFlag == true)
                {
                    WhitePlayerTurnOutDt();
                }

                if (isWhiteTurn && (playerTwoTurnFlag == true))
                {
                    BlackPlayerTurnOutDt();
                }
                if (!isWhiteTurn && (playerOneTurnFlag == true))
                {
                    WhitePlayerTurnOutDt();
                }
            }
            else if (message == "You Won as Other Player Left")
            {
                //GameOverAnim.instance.ParticleConfettiStop();
                PlayerPrefs.SetString("winner", "black");
                PlayerPrefs.SetString("winnername", PlayerPrefs.GetString("OwnName"));
                animWinOnedt.SetTrigger("WinOneIn");
                animDefeatTwodt.SetTrigger("DefeatTwoIn");
                if (playerOneTurnFlag == true)
                {
                    WhitePlayerTurnOutDt();
                }

                if (isWhiteTurn && (playerTwoTurnFlag == true))
                {
                    BlackPlayerTurnOutDt();
                }
                if (!isWhiteTurn && (playerOneTurnFlag == true))
                {
                    WhitePlayerTurnOutDt();
                }
            }

        //}
    }    

    //The profile whose turn it is, is not faded, other one is which
    //is handled from here
    public void HandleCanvasGroupParent()
    {        
        //if (player1 == null)
        //{
        //    Init();
        //}
        isWhiteTurn = Chessboard.Instance.IsWhiteTurn();
        HandleCanvasgroup(upplayerprofiledt, downplayerprofiledt, isWhiteTurn);                
    }

    //Actual function to fade the profile whose turn it is not
    //is handled from here
    public void HandleCanvasgroup(GameObject upgobj, GameObject downgobj, bool isWhiteTurn)
    {
        if (isWhiteTurn)
        {
            //if (player1 == RoomPhoton.room.GetID())
            //{
                upgobj.GetComponent<CanvasGroup>().alpha = 0.5f;
                downgobj.GetComponent<CanvasGroup>().alpha = 1f;
            //}
            //else
            //{
                upgobj.GetComponent<CanvasGroup>().alpha = 1f;
                downgobj.GetComponent<CanvasGroup>().alpha = 0.5f;
            //}
        }
        else
        {
            //if (player1 == RoomPhoton.room.GetID())
            //{
                upgobj.GetComponent<CanvasGroup>().alpha = 1f;
                downgobj.GetComponent<CanvasGroup>().alpha = 0.5f;
            //}
            //else
            //{
                upgobj.GetComponent<CanvasGroup>().alpha = 0.5f;
                downgobj.GetComponent<CanvasGroup>().alpha = 1f;
            //}
        }
    }

    //Time Animation
    public void Timeholderanimations(string device)
    {
        downholderanimdt.SetTrigger("White Player Timer In");
        upholderanimdt.SetTrigger("Black Player Timer In");
    }

    //Win Animation
    public void TesWinAnim()
    {
        WhitePlayerTurnInDt();
        Invoke("TestChangWinAnim", 3f);
    }

    //Change win Animation in case the borad rotation
    public void TestChangWinAnim()
    {
        b_Holderdt.SetTrigger(blackplleave);
        Invoke("TestChangWinAnim2", 3f);
        b_Holderdt.SetTrigger(blackplleaveout);
    }

    //Change win Animation in case the borad rotation
    public void TestChangWinAnim2()
    {
        b_Holderdt.SetTrigger(blackplleaveout);
    }

    //toggle Check flag
    public bool NotinCheck()
    {
        //if (player1 == null)
        //{
        //    Init();
        //}

        //if (player1 == RoomPhoton.room.GetID())
        //{
            if (playerOneCheck == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        //}
        //else if (player2 == RoomPhoton.room.GetID())
        //{

            if (playerTwoCheck == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        //}

        return true;
    }


    /* Animations start*/

    /*Turn In start */
    //show turn of lower player in desktop
    private void WhitePlayerTurnInDt()
    {
        //Debug.Log("WhitePlayerTurnInDt");
        w_Holderdt.SetTrigger(whiteplTurnin);
    }

    //show turn of upper player in desktop
    private void BlackPlayerTurnInDt()
    {
        //Debug.Log("BlackPlayerTurnInDt");
        b_Holderdt.SetTrigger(blackplTurnin);
    }

    /*Turn In stop */

    /*Turn Out start */

    //hide turn of lower player in deskTop
    private void WhitePlayerTurnOutDt()
    {
        //Debug.Log("WhitePlayerTurnOutDt");
        w_Holderdt.SetTrigger(whiteplTurnout);  
    }

    //hide turn of upper player in desktop
    private void BlackPlayerTurnOutDt()
    {
        //Debug.Log("BlackPlayerTurnOutDt");
        b_Holderdt.SetTrigger(blackplTurnout);      
    }

    /*Turn Out stop */

    /*Check Out start*/    
    //hide check of lower player in desktop
    private void WhitePlayerCheckOutDt()
    {
        whitePlChkAnimdt.SetTrigger(lowerCheckOut);
        w_Holderdt.SetTrigger(whiteplTurnout);
    }

    //hide check of upper, and show check on lower player in mobile
    private void WhitePlayerCheckRotOutDt()
    {
        whitePlChkAnimdt.SetTrigger(lowerCheckOut);
        b_Holderdt.SetTrigger(blackplTurnout);
    }

    
    //hide check of upper player in desktop
    private void BlackPlayerCheckOutDt()
    {
        blackPlChkAnimdt.SetTrigger(upperCheckOut);
        b_Holderdt.SetTrigger(blackplTurnout);
    }

    //hide check of upper, show check on lower player in desktop
    private void BlackPlayerCheckRotOutDt()
    {
        blackPlChkAnimdt.SetTrigger(upperCheckOut);
        w_Holderdt.SetTrigger(whiteplTurnout);
    }

    /*Check Out end*/

    /*Check In Starts*/    
    //show check of lower player in desktop
    private void WhitePlayerCheckInDt()
    {
        lowerCheckObjectdt.gameObject.SetActive(true);
        whitePlChkAnimdt.SetTrigger(lowerCheckin);
        w_Holderdt.SetTrigger(whiteplTurnin);
    }

    //show check of upper player,hide check on lower player in desktop
    private void WhitePlayerCheckRotInDt()
    {
        lowerCheckObjectdt.gameObject.SetActive(true);
        whitePlChkAnimdt.SetTrigger(lowerCheckin);
        yourTurnTopdt.gameObject.SetActive(true);
        b_Holderdt.SetTrigger(blackplTurnin);
    }
    
    //show check of upper player in desktop
    private void BlackPlayerCheckInDt()
    {
        upperCheckObjectdt.gameObject.SetActive(true);
        blackPlChkAnimdt.SetTrigger(upperCheckin);
        b_Holderdt.SetTrigger(blackplTurnin);
    }

    //show check of upper player,hide on lower player in desktop
    private void BlackPlayerCheckRotInDt()
    {
        upperCheckObjectdt.gameObject.SetActive(true);
        blackPlChkAnimdt.SetTrigger(upperCheckin);
        yourTurnDowndt.gameObject.SetActive(true);
        w_Holderdt.SetTrigger(whiteplTurnin);
    }
    /*Check In end*/

    /* Animations end*/
    //Own connection lost UI to be shown
    public void OwnConnectionLost()
    {
        bottomofflineactive = true;
        infodt.SetActive(false);
        connectionLostdt.SetActive(true);
        connectionLost1dt.SetActive(true);
        w_Holderdt.SetTrigger(whiteplconnlost);
        downplayerprofiledt.GetComponent<CanvasGroup>().alpha = 1f;
        ShowDownredturn();
    }

    //Own Online UI reset
    public void OwnBackOnline()
    {
        bottomofflineactive = false;
        //SideHeader.instance.SetisconnectionMesOn(false);
        w_Holderdt.SetTrigger(whiteplconnlostout);
        HandleCanvasGroupParent();
        HideDownredTurn();
    }

    //Opponent offline UI
    public void Opponentoffline()
    {
        topofflineactive = true;
        b_Holderdt.SetTrigger(blackplleave);
        upplayerprofiledt.GetComponent<CanvasGroup>().alpha = 1f;
        ShowTopredturn();
        infodt.SetActive(true);
        connectionLostdt.SetActive(false);
        connectionLost1dt.SetActive(false);
        w_Holderdt.SetTrigger(whiteplconnlost);
        downplayerprofiledt.GetComponent<CanvasGroup>().alpha = 1f;
        ShowDownredturn();
        Invoke("Checkyouself", 1f);        
    }

    private bool checkyourselfflag = false;
    private int checkyourselfCount = 0;

    //Kind of like checking own pulse when opponent is rejoining,
    //making sure that we are not left offline at that very same moment
    //public void Checkyouself()
    //{
    //    if (player1 == null)
    //    {
    //        Init();
    //    }

        
    //    if (AnimHandler.instance.topofflineactive && !Timer.instance.IsTimerOneRunning() && !Timer.instance.IsTimerTwoRunning() && !checkyourselfflag)
    //    {
    //        checkyourselfflag = true;
            
    //        if (player1 == RoomPhoton.room.GetID() && !SideHeader.instance.GetReadyToPlayOneflag())
    //        {                
    //            SideHeader.instance.SetReadyToPlayOneflag(true);
    //            if (checkyourselfCount == 0)
    //            {
    //                SideHeader.instance?.ShowReadytorejoinForRejoin();
    //            }

    //            Timer.instance.StartTimer2();
    //        }
    //        else if (player2 == RoomPhoton.room.GetID() && !SideHeader.instance.GetReadyToPlayTwoflag())
    //        {
    //            SideHeader.instance.SetReadyToPlayTwoflag(true);
    //            if (checkyourselfCount == 0)
    //            {
    //                SideHeader.instance?.ShowReadytorejoinForRejoin();
    //            }
    //            Timer.instance.StartTimer();
    //        }
    //        checkyourselfCount++;

    //    }
        
    //}

    public void ResetCheckyouself()
    {
        checkyourselfflag = false;
    }

    public void ResetCheckYourselfCount()
    {
        checkyourselfCount = 0;
    }

    public void IncreaseCheckyourselfCount()
    {
        checkyourselfCount++;
    }

    public int GetCheckyourselfCount()
    {
        return checkyourselfCount;
    }

    public void OpponentBackOnline()
    {
        topofflineactive = false;
        b_Holderdt.SetTrigger(blackplleaveout);
        HandleCanvasGroupParent();
        HideTopredTurn();
        yourTurnDownreddt.gameObject.SetActive(false);
        w_Holderdt.SetTrigger(whiteplconnoutsolo);
        Invoke("HideDownredTurn", 1.1f);
        Invoke("ReadyEnquiry", 2f);
    }

    //Make sure if we are ready to send ready message
    //public void ReadyEnquiry()
    //{
    //    ShouldWeSendReadyMessage(tournamentId, Data.RoomId, Data.RoomTimeId);
    //}

    //Is still waiting message
    //public void IsStillWaiting(string data)
    //{
    //    IsStillWaiting isStillWaiting = JsonUtility.FromJson<IsStillWaiting>(data);
    //    if (isStillWaiting != null && isStillWaiting.TournamentId == tournamentId && isStillWaiting.RoomId == Data.RoomId && isStillWaiting.RoomTimeId == Data.RoomTimeId)
    //    {
    //        if (AnimHandler.instance.topofflineactive)
    //        {
    //            NeedReadyMessage(isStillWaiting.TournamentId, isStillWaiting.RoomId, isStillWaiting.RoomTimeId);
    //        }
    //    }
    //}

    //Show readymessage
    //public void ShowReadyMessage(string data)
    //{
    //    NeedReadyMessage needReadyMessage = JsonUtility.FromJson<NeedReadyMessage>(data);
    //    if (needReadyMessage != null && needReadyMessage.TournamentId == tournamentId && needReadyMessage.RoomId == Data.RoomId && needReadyMessage.RoomTimeId == Data.RoomTimeId)
    //    {
    //        SideHeader.instance?.ShowReadytorejoinForRejoin();
    //    }
    //}

    //Own offline, show red color turn text
    public void ShowDownredturn()
    {

        if (player1 == null)
        {
            Init();
        }

        isWhiteTurn = Chessboard.Instance.IsWhiteTurn();

        //if (player1 == RoomPhoton.room.GetID())
        //{
            if (isWhiteTurn)
            {
                yourTurnDownreddt.gameObject.SetActive(true);                
            }

        //}
        //else if (player2 == RoomPhoton.room.GetID())
        //{
            if (!isWhiteTurn)
            {
                yourTurnDownreddt.gameObject.SetActive(true);
            }
        //}
    }

    //Opponent offline, show red color turn text
    public void ShowTopredturn()
    {

        if (player1 == null)
        {
            Init();
        }

        isWhiteTurn = Chessboard.Instance.IsWhiteTurn();

        //if (player1 == RoomPhoton.room.GetID())
        //{
            if (!isWhiteTurn)
            {
                yourTurnTopreddt.gameObject.SetActive(true);
            }

        //}
        //else if (player2 == RoomPhoton.room.GetID())
        //{
            if (isWhiteTurn)
            {
                yourTurnTopreddt.gameObject.SetActive(true);
            }
        //}
    }


    //Show turn animation
    public void ResetTopTurnOnOppRejoin()
    {

        if (player1 == null)
        {
            Init();
        }

        isWhiteTurn = Chessboard.Instance.IsWhiteTurn();
        //if (player1 == RoomPhoton.room.GetID())
        //{
            if (!isWhiteTurn)
            {
                b_Holderdt.SetTrigger(blackplTurnin);
            }

        //}
        //else if (player2 == RoomPhoton.room.GetID())
        //{
            if (isWhiteTurn)
            {
                b_Holderdt.SetTrigger(blackplTurnin);
            }
        //}
        turnoffForOffline = false;
    }

    //Take out turn animation
    public void ResetTopTurnOnOppRejoinOff()
    {
        turnoffForOffline = true;
        if (player1 == null)
        {
            Init();
        }

        isWhiteTurn = Chessboard.Instance.IsWhiteTurn();
        //if (player1 == RoomPhoton.room.GetID())
        //{
            if (!isWhiteTurn)
            {
                b_Holderdt.SetTrigger(blackplTurnout);
            }

        //}
        //else if (player2 == RoomPhoton.room.GetID())
        //{
            if (isWhiteTurn)
            {
                b_Holderdt.SetTrigger(blackplTurnout);
            }
        //}
    }

    //Opponent rejoins, removes red color turn text
    public void HideDownredTurn()
    {
        if (player1 == null)
        {
            Init();
        }

        isWhiteTurn = Chessboard.Instance.IsWhiteTurn();

        //if (player1 == RoomPhoton.room.GetID())
        //{
            if (isWhiteTurn)
            {
                yourTurnDownreddt.gameObject.SetActive(false);
                w_Holderdt.SetTrigger(whiteplTurnin);
            }

        //}
        //else if (player2 == RoomPhoton.room.GetID())
        //{
            if (!isWhiteTurn)
            {
                yourTurnDownreddt.gameObject.SetActive(false);
                w_Holderdt.SetTrigger(whiteplTurnin);
            }
        //}
    }

    //Old Function,shows lower turn 
    public void WaittoWin()
    {
        w_Holderdt.SetTrigger(whiteplTurnin);
    }

    //Opponent rejoins, removes red color turn text
    public void HideTopredTurn()
    {
        if (player1 == null)
        {
            Init();
        }

        isWhiteTurn = Chessboard.Instance.IsWhiteTurn();

        //if (player1 == RoomPhoton.room.GetID())
        //{
            if (!isWhiteTurn)
            {
                yourTurnTopreddt.gameObject.SetActive(false);
            }

        //}
        //else if (player2 == RoomPhoton.room.GetID())
        //{
            if (isWhiteTurn)
            {
                yourTurnTopreddt.gameObject.SetActive(false);
            }
        //}
    }

    //Reset opponents animation triggers
    public void ResetOpponentTriggers()
    {
        b_Holderdt.ResetTrigger(blackplleave);
        b_Holderdt.ResetTrigger(blackplleaveout);
    }

    //Reset own animation triggers
    public void ResetOwnTriggers()
    {
        w_Holderdt.ResetTrigger(whiteplconnlost);
        w_Holderdt.ResetTrigger(whiteplconnlostout);
    }

    //Store value of animation triggers
    public void ValueOfTriggers()
    {
        bool triggerValue = b_Holderdt.GetBool(blackplleave);
        bool triggerValueOut = b_Holderdt.GetBool(blackplleaveout);
    }

    //Show turn animation
    public void ResetDownTurnOnOwnRejoin()
    {

        if (player1 == null)
        {
            Init();
        }

        isWhiteTurn = Chessboard.Instance.IsWhiteTurn();
        //if (player1 == RoomPhoton.room.GetID())
        //{
            if (isWhiteTurn)
            {
                w_Holderdt.SetTrigger(whiteplTurnin);
            }

        //}
        //else if (player2 == RoomPhoton.room.GetID())
        //{
            if (!isWhiteTurn)
            {
                w_Holderdt.SetTrigger(whiteplTurnin);
            }
        //}

    }

    //Take out turn animation
    public void ResetDownTurnOnOwnRejoinOff()
    {
        if (player1 == null)
        {
            Init();
        }

        isWhiteTurn = Chessboard.Instance.IsWhiteTurn();
        //if (player1 == RoomPhoton.room.GetID())
        //{
            if (isWhiteTurn)
            {
                w_Holderdt.SetTrigger(whiteplTurnout);
            }

        //}
        //else if (player2 == RoomPhoton.room.GetID())
        //{
            if (!isWhiteTurn)
            {
                w_Holderdt.SetTrigger(whiteplTurnout);
            }
        //}
    }

    public bool GetPlayerOneTurnFlag()
    {
        return playerOneTurnFlag;
    }
    public bool GetPlayerTwoTurnFlag()
    {
        return playerTwoTurnFlag;
    }
    public void SetPlayerOneTurnFlag(bool val)
    {
        playerOneTurnFlag = val;
    }

    public void SetPlayerTwoTurnFlag(bool val)
    {
        playerTwoTurnFlag = val;
    }

    public bool GetPlayerOneCheck()
    {
        return playerOneCheck;
    }
    public bool GetPlayerTwoCheck()
    {
        return playerTwoCheck;
    }

    public void SetPlayerOneCheck(bool val)
    {
        playerOneCheck = val;
    }

    public void SetPlayerTwoCheck(bool val)
    {
        playerTwoCheck = val;
    }

    //Probably old, but turn text's color was set from here
    public void SetTurnColor(UnityEngine.Color color)
    {
        yourTurnDowndt.color = color;
    }

    //rotate check ui direction in case board is rotated
    public void RotateCheckHolders()
    {
        upperCheckText.transform.Rotate(Vector3.up, 180f, Space.World);
        lowerCheckText.transform.Rotate(Vector3.up, 180f, Space.World);
    }

}
