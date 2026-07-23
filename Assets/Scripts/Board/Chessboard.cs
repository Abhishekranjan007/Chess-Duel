using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using UnityEngine.InputSystem;


public enum SpecialMove
{
    None = 0,
    EnPassant,
    Castling,
    Promotion
}

public class Chessboard : MonoBehaviour
{
    public static Chessboard Instance { get; set; }
        
    ChessPiece[,] pieces;
    List<Vector2Int[]> mvList;
    private ChessPiece currentlyDragging;
    //List<string> moves = new List<string>();
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private List<Vector2Int> prevavailableMoves = new List<Vector2Int>();
    private List<GameObject> highlightedSpots = new List<GameObject>();//##
    private List<GameObject> prevhighlightedSpots = new List<GameObject>();//##
    //private bool isWhiteTurn;

    //Removing dead pieces
    private List<ChessPiece> deadWhites = new List<ChessPiece>();
    private List<ChessPiece> deadBlacks = new List<ChessPiece>();
    List<string> blackPlayersKilled = new List<string>();
    List<string> whitePlayersKilled = new List<string>();
    private float deathSize = 0.5f;
    private float deathSpacing /*= 54.35f*/;
    private float whiteX, blackX, commonY;

    //Coordinates of selected chessman
    private int selectionX = -1;
    private int selectionY = -1, hisPromX = -1, hisPromY = -1;

    public TouchSelect touchSelect;
    public GameObject backGround;
    private Camera currentCamera;
    string[] ynom = { "1", "2", "3", "4", "5", "6", "7", "8" };
    string[] xnom = { "a", "b", "c", "d", "e", "f", "g", "h" };
    string prev_to1;

    //Special moves
    private List<Vector2Int[]> moveList = new List<Vector2Int[]>();
    private string prevselectionX = "";
    private SpecialMove specialMove;
    public HandleHighlight handleHighlight;
    private BoardFormation boardFormation;
    bool checkmateFlag = false;
    public bool countdownFlag = false;
    Color blackCol;
    float elapsed = 0f;
    public TimeView timeView;
    public CountdownGo countdownGo;
    private int countdownTime = 3;
    private bool isExecutingRemoteMove = false;
    private PromotionType networkPromotionType = PromotionType.Queen;

    

    public bool IsExecutingLocalNetworkMove { get; private set; }


    private ChessGameState gameState;
    private NetworkGameManager manager;


    private void Awake()
    {
       
    }

    private void Start()
    {
        //Debug.Log("List Hash Awake = " + deadBlacks.GetHashCode());
        //Debug.Log("Selected Mode = " + GameManager.Instance.SelectedMode);
        boardFormation = BoardFormation.Instance;

        if (Instance == null)
            Instance = this;

        //commonY = 4.5f;
        //whiteX = -4f;
        //blackX = 3.8f;

        currentCamera = Camera.main;         
        gameState = new ChessGameState();
        gameState.CurrentGameMode = GameManager.Instance.SelectedMode;
        gameState.IsWhiteTurn = true;        

        whiteX = boardFormation.GetBoardWidth() / 2f - 0.5f;
        blackX = boardFormation.GetBoardWidth() / 2f - 0.5f;
        commonY = boardFormation.GetBoardHeight() / 2f + 0.35f;

        deathSpacing = boardFormation.GetDistanceX() * 0.5f;        

        blackCol = Color.black;
        GameOver.instance.UpdateGameOverFlag(false);        
        StartCoroutine(InitializeGame());
    }


    private void Update()
    {

        if (GameOver.instance.isGameover() == false /*&& countdownFlag == false && onePlayerMissingFlag == false*/)
        {
            elapsed += Time.deltaTime;

            //if (GameTimer.timeRemaining >= 0f)
            //{
            //    timeView.DisplayTimeWhite(GameTimer.timeRemaining, blackCol);
            //}

            //if (GameTimer.timeRemaining2 >= 0f)
            //{
            //    timeView.DisplayTimeBlack(GameTimer.timeRemaining2, blackCol);
            //}

            if (GameTimer.timeRemaining >= 0f && GameTimer.timeRemaining2 >= 0f)
            {
                bool isBlack = manager != null && manager.AmIBlack();

                if (!isBlack)
                {
                    timeView.DisplayTimeWhite(GameTimer.timeRemaining, blackCol);
                    timeView.DisplayTimeBlack(GameTimer.timeRemaining2, blackCol);
                }
                else
                {
                    timeView.DisplayTimeWhiteBl(GameTimer.timeRemaining2, blackCol);
                    timeView.DisplayTimeBlackBl(GameTimer.timeRemaining, blackCol);
                }
            }

            touchSelect.UpdateSelectionUI();
            
            if (!currentCamera)
            {
                currentCamera = Camera.main;
                return;
            }


            if (true/*GameOver.instance.isGameover() == false && GameTimer.timeRemaining2 > 1f && GameTimer.timeRemaining > 1f && blackPlayerJoinedMessage == true && !GameOverAnim.instance.GetGameleavepopOn()*/)
            {
                if (!CanCurrentPlayerMove())
                    return;

                if (gameState.CurrentGameMode == ChessGameMode.VsAI)
                {
                    if (gameState.IsWhiteTurn)
                        HandlePlayerInput(0);
                }
                else
                {
                    //NetworkGameManager manager = FusionRoomManager.Instance.GetNetworkGameManager();

                    if (manager == null)
                        return;

                    if (manager.AmIWhite())
                        HandlePlayerInput(0);
                    else
                        HandlePlayerInput(1);
                }

                //if (gameState.IsWhiteTurn == true/*gameState.IsWhiteTurn == true && player1 == RoomPhoton.room.GetID() && numOfPlayers == 2*/)
                //{
                //    HandlePlayerInput(0);
                //}
                //else if (gameState.IsWhiteTurn == false/*gameState.IsWhiteTurn == false && player2 == RoomPhoton.room.GetID() && numOfPlayers == 2*/)
                //{
                //    HandlePlayerInput(1);
                //}
            }
        }
    }

    private IEnumerator InitializeGame()
    {
        // AI mode doesn't need to wait for networking
        if (gameState.CurrentGameMode == ChessGameMode.VsAI)
        {
            boardFormation.GenerateGriddt();
            boardFormation.GenerateBoard();
            boardFormation.SpawnAllPieces();
            boardFormation.PlaceAllPieces();

            ProfileImage.instance.ShowProfile();            
            countdownGo.HideGettingReadyImage();
            yield return null;
            countdownFlag = true;
            countdownGo.StartCountDown();
            countdownGo.StartCoundownAnim();
            yield break;
        }

        NetworkGameManager managerN = null;

        // Wait until FusionRoomManager exists
        while (FusionRoomManager.Instance == null)
            yield return null;

        // Wait until NetworkGameManager exists
        while ((managerN = FusionRoomManager.Instance.GetNetworkGameManager()) == null)
            yield return null;

        // Wait until both players have been assigned
        while (managerN.WhitePlayer == default || managerN.BlackPlayer == default)
            yield return null;

        manager = managerN;
        GameTimer.gameTimerInstance.SetNetWorkPlayer(manager);
        ProfileImage.instance.ShowProfile();
        yield return null;

        boardFormation.GenerateGriddt();
        boardFormation.GenerateBoard();
        boardFormation.SpawnAllPieces();
        boardFormation.PlaceAllPieces();

        // Rotate if I am black
        if (managerN.AmIBlack())
        {
            RotateBoardForBlackPlayer();
        }
                
        yield return null;

        // Hide the Getting Ready overlay
        countdownGo.HideGettingReadyImage();

        // Let Unity render one frame with the board visible
        yield return null;

        // NOW start the countdown
        countdownFlag = true;
        countdownGo.StartCountDown();
        countdownGo.StartCoundownAnim();
    }

    private void RotateBoardForBlackPlayer()
    {
        currentCamera.transform.rotation = Quaternion.Euler(0, 0, 180);
        backGround.transform.rotation = Quaternion.Euler(0, 0, 180);
        boardFormation.TurnChessPieces();
        boardFormation.SetupBoardSprite();
    }

    private bool CanCurrentPlayerMove()
    {
        switch (gameState.CurrentGameMode)
        {
            case ChessGameMode.VsAI:
                return gameState.IsWhiteTurn;

            case ChessGameMode.Multiplayer:
                {
                    NetworkGameManager manager = FusionRoomManager.Instance.GetNetworkGameManager();

                    if (manager == null)
                        return false;

                    return manager.IsMyTurn();
                }
        }

        return false;
    }

    private IEnumerator AIMoveRoutine()
    {
        yield return new WaitForSeconds(3f);

        AIPlayer.Instance.TryMakeMove();
    }

    private ChessMove CreateMove(ChessPiece piece, int toX, int toY)
    {
        return new ChessMove(piece.currentX, piece.currentY, toX, toY, specialMove, ChessPieceType.None);
    }

    private void HandlePlayerInput(int teamId)
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {

            if (selectionX >= 0 && selectionY >= 0)
            {
                if (currentlyDragging == null && /*chessPieces[selectionX, selectionY]*/boardFormation.ChessPieces[selectionX, selectionY] != null)
                {
                    if (/*chessPieces[selectionX, selectionY]*/boardFormation.ChessPieces[selectionX, selectionY].team == teamId)
                    {
                        SetupPiece();
                    }

                }
                else if (currentlyDragging != null && (currentlyDragging == boardFormation.ChessPieces[selectionX, selectionY]/*chessPieces[selectionX, selectionY]*/))
                {
                    availableMoves.Clear();
                    specialMove = SpecialMove.None;
                    handleHighlight.RemoveHighlightTiles();
                    SetSelectX(-1);
                    SetSelectY(-1);
                    currentlyDragging = null;
                }
                else if (currentlyDragging != null)
                {
                    MoveIfPromotionFalse();
                }

            }
            else
            {
                if (currentlyDragging != null)
                {
                    MoveIfPromotionFalse();
                }
            }


        }
    }

    //Handles chesspiece moves if promotion isn't available
    public void MoveIfPromotionFalse()
    {
        if (prevavailableMoves.Count != 0)
        {
            if (prevavailableMoves[prevavailableMoves.Count - 1].x == currentlyDragging.currentX && prevavailableMoves[prevavailableMoves.Count - 1].y == currentlyDragging.currentY)
            {
                return;
            }
        }

        //Debug.Log("MoveIFPromotionFalse");
        Vector2Int tempvec = new Vector2Int(selectionX, selectionY);

        if ((currentlyDragging.type == ChessPieceType.Pawn) && (availableMoves.Count > 1) && availableMoves.Contains(tempvec))
        {
            if (currentlyDragging.currentY == 6 && currentlyDragging.team == 0)
            {
                if (PromotionController.instance.GetPromotionPosibbleFlag() == false)
                {
                    PromotionController.instance.PromotionStripOnW();

                    //Create a function to select the promotion
                    //Store selected promotion type in a global variable
                    //reset the flag in operation 1 and allow normal game operation
                    hisPromX = selectionX;
                    hisPromY = selectionY;
                }

            }
            else if (currentlyDragging.currentY == 1 && currentlyDragging.team == 1)
            {
                if (PromotionController.instance.GetPromotionPosibbleFlag() == false)
                {
                    PromotionController.instance.PromotionStripOnB();


                    //Create a function to select the promotion
                    //Store selected promotion type in a global variable
                    //reset the flag in operation 1 and allow normal game operation
                    hisPromX = selectionX;
                    hisPromY = selectionY;
                }

            }
        }


        if (PromotionController.instance.GetPromotionPosibbleFlag() == false)
        {
            Vector2Int previousPosition = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);

            
            bool validMove = MoveTo(currentlyDragging, selectionX, selectionY, 0);
            handleHighlight.RemoveHighlightTiles();
            
            if (currentlyDragging != null)
            {
                if (validMove)//&& (currentlyDragging != chessPieces[selectionX, selectionY])
                {
                    handleHighlight.HighlightPrevTiles();
                }
            }

            if (!validMove)
            {
                if (currentlyDragging != null)
                {
                    currentlyDragging.SetPosition(boardFormation.GetTileCenter(previousPosition.x, previousPosition.y));
                    currentlyDragging = null;
                }
            }
            else
            {
                currentlyDragging = null;
            }
        }
    }


    //Move the chesspiece to (x,y) on the board
    private bool MoveTo(ChessPiece cp, int x, int y, int z)
    {        
        if (GameOver.instance.isGameover() == false)
        {
            
            if (!Utility.ContainsValidMove(ref availableMoves, new Vector2Int(x, y)))
                return false;

            
            if (cp != boardFormation.ChessPieces[x, y]/*chessPieces[x, y]*/)
            {
                if (prevavailableMoves.Count != 0)
                {
                    handleHighlight.RemovePrevHighlightTiles();
                }

                if (specialMove == SpecialMove.Castling)
                {
                    prevavailableMoves.Add(new Vector2Int(x, y));
                    prevavailableMoves.Add(new Vector2Int(cp.currentX, cp.currentY));
                }
                else
                {
                    prevavailableMoves.Add(new Vector2Int(x, y));
                    prevavailableMoves.Add(availableMoves[availableMoves.Count - 1]);
                }
                
            }

            

            Vector2Int previousPosition = new Vector2Int(cp.currentX, cp.currentY);
            string fr1, fr2, to1, to2;
            fr1 = xnom[cp.currentX];
            fr2 = ynom[cp.currentY];
            to1 = xnom[x];
            to2 = ynom[y];

            //attempt to prevent multiple moves by the same chessman in one go
            if (prev_to1 == fr1 + fr2)
            {                               
                return false;
            }

            prev_to1 = to1 + to2;

            //string turnTemp = gameState.IsWhiteTurn == true ? "white" : "black";
            int a = cp.currentX;
            int b = cp.currentY;        
            //string temp = a + ":" + b + ":" + x + ":" + y + ":" + GameTimer.timeRemaining + ":" + GameTimer.timeRemaining2 + ":" + turnTemp + ":" + TimeController.instance.GetActiveTimer() + ":" + Data.CurrentRoom + ":" + PromotionController.instance.GetSelectedPromotionValue() + ":" + fr1 + fr2 + "_" + to1 + to2 + ":" + GameTimer.timeRemainingM + ":" + GameTimer.timeRemainingM2;//mlintegration        


            string mvPl = "", klPl = "";
            if (cp.type == ChessPieceType.King)
            {
                mvPl = "king";
            }
            else if (cp.type == ChessPieceType.Queen)
            {
                mvPl = "queen";
            }
            else if (cp.type == ChessPieceType.Knight)
            {
                mvPl = "knight";
            }
            else if (cp.type == ChessPieceType.Pawn)
            {
                mvPl = "pawn";
            }
            else if (cp.type == ChessPieceType.Rook)
            {
                mvPl = "rook";
            }
            else if (cp.type == ChessPieceType.Bishop)
            {
                mvPl = "bishop";
            }
            //If there is another piece on the target position
            if (/*chessPieces[x, y]*/boardFormation.ChessPieces[x, y] != null)
            {
                ChessPiece otherChessPiece = boardFormation.ChessPieces[x, y]/*chessPieces[x, y]*/;


                if (cp.team == otherChessPiece.team)
                    return false;

                //If its the enemy team //Removing dead pieces
                if (otherChessPiece.team == 0)
                {
                    if (otherChessPiece.type == ChessPieceType.King)                        
                        GameOver.instance.GameEnded(1, "Normal Game", "a");

                    deadWhites.Add(otherChessPiece);

                    otherChessPiece.SetScale(Vector3.one * deathSize);
                    //if (player1 == RoomPhoton.room.GetID())
                    //{
                    Vector3 deadPos;

                    if (gameState.CurrentGameMode == ChessGameMode.VsAI)
                    {
                        deadPos = new Vector3(whiteX * boardFormation.GetDistanceX(), -commonY * boardFormation.GetDistanceY(), 0) + Vector3.left * deathSpacing * (deadWhites.Count - 1);
                    }
                    else 
                    {
                        if (manager.AmIWhite())
                        {
                            deadPos = new Vector3(whiteX * boardFormation.GetDistanceX(), -commonY * boardFormation.GetDistanceY(), 0) + Vector3.left * deathSpacing * (deadWhites.Count - 1);
                        }
                        else
                        {
                            deadPos = new Vector3(-blackX * boardFormation.GetDistanceX(), -commonY * boardFormation.GetDistanceY(), 0) + Vector3.right * deathSpacing * (deadWhites.Count - 1);
                        }
                    }
                        
                    otherChessPiece.SetPosition(deadPos);

                    //otherChessPiece.SetPosition(new Vector3(whiteX * distanceX, (-commonY * distanceY))  - bounds
                    //    + new Vector3(distanceX / 2, 0, distanceY / 2)
                    //    + (Vector3.right * deathSpacing) * deadWhites.Count);
                    //}
                    //else
                    //{

                    //    otherChessPiece.SetPosition(new Vector3(blackX * distanceX, -commonY * distanceY)
                    //        - bounds
                    //        + new Vector3(distanceX / 2, 0, distanceY / 2)
                    //        + (Vector3.right * -deathSpacing) * deadWhites.Count);
                    //}


                    if (otherChessPiece.type == ChessPieceType.King)
                    {
                        klPl = "king";
                    }
                    else if (otherChessPiece.type == ChessPieceType.Queen)
                    {
                        klPl = "queen";
                    }
                    else if (otherChessPiece.type == ChessPieceType.Knight)
                    {
                        klPl = "knight";
                    }
                    else if (otherChessPiece.type == ChessPieceType.Pawn)
                    {
                        klPl = "pawn";
                    }
                    else if (otherChessPiece.type == ChessPieceType.Rook)
                    {
                        klPl = "rook";
                    }
                    else if (otherChessPiece.type == ChessPieceType.Bishop)
                    {
                        klPl = "bishop";
                    }

                    whitePlayersKilled.Add(otherChessPiece.GetType().ToString());

                }
                else
                {
                    

                    if (otherChessPiece.type == ChessPieceType.King)
                        GameOver.instance.GameEnded(0, "Normal Game", "b");

                        
                    
                    deadBlacks.Add(otherChessPiece);                    


                    otherChessPiece.SetScale(Vector3.one * deathSize);
                    //if (player1 == RoomPhoton.room.GetID())
                    //{
                    Vector3 deadPos;

                    if (gameState.CurrentGameMode == ChessGameMode.VsAI)
                    {
                        deadPos = new Vector3(blackX * boardFormation.GetDistanceX(), commonY * boardFormation.GetDistanceY(), 0) + Vector3.left * deathSpacing * (deadBlacks.Count - 1);
                    }
                    else
                    {
                        if (manager.AmIWhite())
                        {
                            deadPos = new Vector3(whiteX * boardFormation.GetDistanceX(), commonY * boardFormation.GetDistanceY(), 0) + Vector3.left * deathSpacing * (deadBlacks.Count - 1);
                            
                        }
                        else
                        {
                            deadPos = new Vector3(-blackX * boardFormation.GetDistanceX(), commonY * boardFormation.GetDistanceY(), 0) + Vector3.right * deathSpacing * (deadBlacks.Count - 1);
                        }
                    }
                        
                    otherChessPiece.SetPosition(deadPos);
                    //otherChessPiece.SetPosition(new Vector3(whiteX * distanceX, commonY * distanceY)
                    //    - bounds
                    //    + new Vector3(distanceX / 2, 0, distanceY / 2)
                    //    + (Vector3.right * deathSpacing) * deadBlacks.Count);
                    //}
                    //else
                    //{

                    //    otherChessPiece.SetPosition(new Vector3(blackX * distanceX, commonY * distanceY)
                    //        - bounds
                    //        + new Vector3(distanceX / 2, 0, distanceY / 2)
                    //        + (Vector3.right * -deathSpacing) * deadBlacks.Count);
                    //}


                    //1.5f * tileSize
                    if (otherChessPiece.type == ChessPieceType.King)
                    {
                        klPl = "king";
                    }
                    else if (otherChessPiece.type == ChessPieceType.Queen)
                    {
                        klPl = "queen";
                    }
                    else if (otherChessPiece.type == ChessPieceType.Knight)
                    {
                        klPl = "knight";
                    }
                    else if (otherChessPiece.type == ChessPieceType.Pawn)
                    {
                        klPl = "pawn";
                    }
                    else if (otherChessPiece.type == ChessPieceType.Rook)
                    {
                        klPl = "rook";
                    }
                    else if (otherChessPiece.type == ChessPieceType.Bishop)
                    {
                        klPl = "bishop";
                    }

                    blackPlayersKilled.Add(otherChessPiece.GetType().ToString());

                }
            }

            //if (mvPl == "")
            //{
            //    mvPl = "none";
            //}

            //if (klPl == "")
            //{
            //    klPl = "none";
            //}

            //temp = temp + ":" + mvPl + ":" + klPl;
            /*chessPieces[x, y]*/
            boardFormation.ChessPieces[x, y] = cp;
            /*chessPieces[previousPosition.x, previousPosition.y]*/
            boardFormation.ChessPieces[previousPosition.x, previousPosition.y] = null;

            boardFormation.PlaceOnePiece(x, y);

            //if (z == 0)
            //{
            //    m1 = a.ToString();
            //    m2 = b.ToString();
            //    m3 = x.ToString();
            //    m4 = y.ToString();
            //    SendMove(Data.RoomTimeId, cRoom, a.ToString(), b.ToString(), x.ToString(), y.ToString(), PromotionController.instance.GetSelectedPromotionValue());
            //}


            //gameState.IsWhiteTurn = !gameState.IsWhiteTurn;
            
            if (gameState.CurrentGameMode == ChessGameMode.VsAI)
            {
                gameState.SwitchTurn();

                //Debug.Log(
                //    $"AFTER FLIP | " +
                //    $"Frame={Time.frameCount} | " +
                //    $"WhiteTurn={gameState.IsWhiteTurn}");
            }
            else
            {
                NetworkGameManager manager = FusionRoomManager.Instance.GetNetworkGameManager();

                if (manager.HasStateAuthority)
                {
                    //Debug.Log(
                    //    $"MoveTo BEFORE FLIP | " +
                    //    $"Local={manager.Runner.LocalPlayer} | " +
                    //    $"HasStateAuthority={manager.HasStateAuthority} | " +
                    //    $"WhiteTurn(before)={manager.IsWhiteTurn}");

                    manager.IsWhiteTurn = !manager.IsWhiteTurn;
                    //Debug.Log(
                    //    $"AFTER FLIP | " +
                    //    $"Frame={Time.frameCount} | " +
                    //    $"WhiteTurn={manager.IsWhiteTurn}");
                }
            }


            if (GameManager.Instance.SelectedMode == ChessGameMode.VsAI && !gameState.IsWhiteTurn)
            {
                StartCoroutine(AIMoveRoutine());
            }
            //MoveController.instance.SaveTurn(isWhiteTurn);//mlintegration

            moveList.Add(new Vector2Int[] { previousPosition, new Vector2Int(x, y) });
            gameState.AddMove(new ChessMove( previousPosition.x, previousPosition.y, x, y, specialMove));


            string sp_move = ProcessSpecialMove();
            //fromFillAllowedMoves = false;

            if (gameState.CurrentGameMode == ChessGameMode.VsAI)
            {
                TimeController.instance.SetTimer();
            }
            else if (gameState.CurrentGameMode == ChessGameMode.Multiplayer)
            {
                NetworkGameManager mgr = FusionRoomManager.Instance.GetNetworkGameManager();
                if (manager != null && manager.HasStateAuthority)
                {
                    TimeController.instance.SetTimer();
                }
            }



            //moves.Add(temp);


            //if (shouldRunCheckLogic)
            //{
            if (Checks.Instance.CheckForCheckmate())
                {
                    checkmateFlag = true;
                    GameOver.instance.UpdateGameOverFlag(true);
                    sp_move = (sp_move == "") ? "Checkmate" : (sp_move + ";Checkmate");
                    GameOver.instance.GameEnded(cp.team, "Normal Game:Checkmate", "c");
                }
            //bool shouldRunCheckLogic = true;

            //if (gameState.CurrentGameMode == ChessGameMode.Multiplayer)
            //{
            //    NetworkGameManager manager = FusionRoomManager.Instance.GetNetworkGameManager();

            //    // Local player without authority:
            //    // don't evaluate checks/turn animation yet.
            //    if (!isExecutingRemoteMove && !manager.HasStateAuthority)
            //    {
            //        shouldRunCheckLogic = false;
            //    }
            //}

            if (!checkmateFlag && Checks.Instance.CheckForStalemate())
                {
                    sp_move = sp_move == "" ? "Stalemate" : sp_move + ";Stalemate";
                    GameOver.instance.UpdateGameOverFlag(true);
                    GameOver.instance.GameEnded(2, "Draw:Stalemate", "d");
                }
            //}

            

            //if (sp_move == "")
            //{
            //    if (AnimHandler.instance.GetPlayerOneCheck() == true)
            //    {
            //        if (sp_move == "")
            //        {
            //            sp_move = "checkW";
            //        }
            //        else
            //        {
            //            sp_move = sp_move + ";checkW";
            //        }
            //    }
            //    if (AnimHandler.instance.GetPlayerTwoCheck() == true)
            //    {
            //        if (sp_move == "")
            //        {
            //            sp_move = ";checkB";
            //        }
            //        else
            //        {
            //            sp_move = sp_move + ";checkB";
            //        }

            //    }
            //    if (sp_move == "")
            //    {
            //        sp_move = "none";
            //    }
            //    temp = temp + ":" + sp_move;
            //}
            //else
            //{
            //    if (AnimHandler.instance.GetPlayerOneCheck() == true)
            //    {
            //        sp_move = sp_move + ";checkW";
            //    }
            //    if (AnimHandler.instance.GetPlayerTwoCheck() == true)
            //    {
            //        sp_move = sp_move + ";checkB";
            //    }
            //    temp = temp + ":" + sp_move;
            //}

            //int count = 0, kg = 0, rk = 0, kt = 0;
            //bool flagcount = false;
            //for (int i = 0; i < TILE_COUNT_X; i++)
            //{
            //    for (int j = 0; j < TILE_COUNT_Y; j++)
            //    {
            //        if (chessPieces[i, j] != null)
            //        {
            //            count++;

            //            if (chessPieces[i, j].type == ChessPieceType.King)
            //            {
            //                kg++;
            //            }
            //            else if (chessPieces[i, j].type == ChessPieceType.Knight)
            //            {
            //                rk++;
            //            }
            //            else if (chessPieces[i, j].type == ChessPieceType.Rook)
            //            {
            //                kt++;
            //            }

            //            if (count > 4)
            //            {
            //                flagcount = true;
            //                break;
            //            }
            //        }
            //    }
            //    if (flagcount == true)
            //    {
            //        break;
            //    }
            //}



            //if (count == 2)
            //{
            //    GameOver.instance.UpdateGameOverFlag(true);
            //    GameOver.instance.GameEnded(2, "Draw:Stalemate", "e");
            //}
            //else if (count == 3)
            //{
            //    if ((kg == 2 && kt == 1) || (kg == 2 && rk == 1))
            //    {
            //        GameOver.instance.UpdateGameOverFlag(true);                    
            //        GameOver.instance.GameEnded(2, "Draw:Stalemate", "f");
            //    }
            //}


            //temp = temp + ":" + Timer.timeRemaining + ":" + Timer.timeRemaining;
            //moves.RemoveAt(moves.Count - 1);
            //moves.Add(temp);
            //string tempBrdState = PlayerPrefs.GetString(LastBoardStateKey);
            //string tempWhiteKilled = PlayerPrefs.GetString("WhiteChessManKilled");
            //string tempBlackKilled = PlayerPrefs.GetString("BlackChessManKilled");
            //string tempwhiteTimeRem = GameTimer.timeRemaining.ToString();
            //string tempblackTimeRem = GameTimer.timeRemaining2.ToString();
            //string tempTurn = isWhiteTurn == true ? "black" : "white";
            //string tempMove = temp;
            //string combinedString = string.Join(",", moves);
            //MoveController.instance.CallDataSpec(Data.CurrentRoom, tempBrdState, tempWhiteKilled, tempBlackKilled, tempwhiteTimeRem, tempblackTimeRem, tempTurn, tempMove, combinedString);

            //if (GameOver.instance.isGameover() == true)
            //{
            //    MoveController.instance.LastSpecData(Data.CurrentRoom, tempBrdState, tempWhiteKilled, tempBlackKilled, tempwhiteTimeRem, tempblackTimeRem, tempTurn, tempMove, combinedString);
            //}
            //string[] movesArray = moves.ToArray();
            //string base64Tex4 = string.Join(", ", movesArray);
            //PlayerPrefs.DeleteKey("Moves");
            //PlayerPrefs.SetString("Moves", base64Tex4);
            //string uid = PlayerPrefs.GetString(Constants.Uid);
            //string rid = Data.RoomId;
            //time = Data.RoomTime;
            //string roomTimeId = Data.RoomTimeId;


            //if (z == 0)
            //{
            //    currBoard.GetCurrentBoardState(chessPieces, whitePlayersKilled, blackPlayersKilled, base64Tex4);
            //    string res = SaveTime(uid, rid, roomTimeId, moves.Count.ToString());
            //    string s = GameTimer.timeRemaining + " :: " + GameTimer.timeRemainingM + " :: " + GameTimer.timeRemaining2 + " :: " + GameTimer.timeRemainingM2;
            //}

            if (!isExecutingRemoteMove && GameManager.Instance.SelectedMode == ChessGameMode.Multiplayer)
            {
                NetworkGameManager manager = FusionRoomManager.Instance.GetNetworkGameManager();

                //Debug.Log(
                //    $"Before SendMove: " +
                //    $"isExecutingRemoteMove={isExecutingRemoteMove} " +
                //    $"Mode={GameManager.Instance.SelectedMode} " +
                //    $"Manager={(FusionRoomManager.Instance.GetNetworkGameManager() != null)}");

                if (manager != null)
                {
                    //manager.SendMove(previousPosition.x, previousPosition.y, x, y, 0);
                    byte promotion = 0;

                    if (specialMove == SpecialMove.Promotion)
                    {
                        promotion = (byte)PromotionController.instance.GetPromotionType();
                    }

                    //Debug.Log("Actually calling SendMove");
                    IsExecutingLocalNetworkMove = true;
                    manager.SendMove(
                        previousPosition.x,
                        previousPosition.y,
                        x,
                        y,
                        promotion);
                    IsExecutingLocalNetworkMove = false;
                }
            }
            

            if (!Utility.ContainsValidMove(ref availableMoves, new Vector2Int(x, y)))
            {
                //Debug.Log("Move rejected because destination is not in availableMoves");
                return false;
            }

            if (gameState.CurrentGameMode == ChessGameMode.Multiplayer)
            {
                manager.SyncTimers();
            }
        }
        return true;
    }


    public bool ExecuteAIMove(ChessPiece piece, int targetX, int targetY)
    {
        pieces = boardFormation.ChessPieces;

        availableMoves = piece.GetAvailableMoves(ref pieces, boardFormation.GetBoardWidth(), boardFormation.GetBoardHeight());

        specialMove = piece.GetSpecialMoves(ref pieces, ref moveList, ref availableMoves);

        return MoveTo(piece, targetX, targetY, 0);
    }

    public void ExecuteRemoteMove(int fromX, int fromY, int toX, int toY, int promotion)
    {
        //Debug.Log(
        //    $"ExecuteRemoteMove START | " +
        //    $"Frame={Time.frameCount} | " +
        //    $"WhiteTurn={Chessboard.Instance.IsWhiteTurn()}");

        networkPromotionType = (PromotionType)promotion;
        if (isExecutingRemoteMove)
            return;

        isExecutingRemoteMove = true;

        try
        {
            ChessPiece piece = boardFormation.ChessPieces[fromX, fromY];

            //Debug.Log(
            //    $"After MoveTo: " +
            //    $"From={(boardFormation.ChessPieces[fromX, fromY] == null ? "NULL" : "PIECE")} " +
            //    $"To={(boardFormation.ChessPieces[toX, toY] == null ? "NULL" : boardFormation.ChessPieces[toX, toY].type.ToString())}");

            if (piece == null)
            {
                Debug.LogError($"Remote piece not found ({fromX},{fromY})");

                return;
            }

            selectionX = toX;
            selectionY = toY;

            currentlyDragging = piece;

            pieces = boardFormation.ChessPieces;

            availableMoves = piece.GetAvailableMoves(ref pieces, boardFormation.GetBoardWidth(), boardFormation.GetBoardHeight());

            specialMove = piece.GetSpecialMoves(ref pieces, ref moveList, ref availableMoves);

            Checks.Instance.PreventCheck();

            //Debug.Log("Calling MoveTo...");

            bool success = MoveTo(piece, toX, toY, 0);

            //Debug.Log("MoveTo returned : " + success);

            if (!success)
            {
                Debug.LogError("Remote move rejected.");
            }

            currentlyDragging = null;
        }
        finally
        {
            isExecutingRemoteMove = false;
        }
    }

    public void SetupPiece()
    {        
        currentlyDragging = boardFormation.ChessPieces[selectionX, selectionY]/*chessPieces[selectionX, selectionY]*/;
        pieces =  boardFormation.ChessPieces;        

        availableMoves = currentlyDragging.GetAvailableMoves(ref pieces, boardFormation.GetBoardWidth(), boardFormation.GetBoardHeight());
        specialMove = currentlyDragging.GetSpecialMoves(ref pieces, ref moveList, ref availableMoves);        
        Checks.Instance.PreventCheck();        
        availableMoves.Add(new Vector2Int(selectionX, selectionY));
        handleHighlight.HighlightTiles();        
    }


    public void SetSelectX(int val)
    {
        selectionX = val;
    }

    //Stores selected y-index
    public void SetSelectY(int val)
    {
        selectionY = val;
    }

    public List<Vector2Int> GetAvailablemoves()
    {
        return availableMoves;
    }

    public List<GameObject> GetHighlightedspots()
    {
        return highlightedSpots;
    }

    public List<Vector2Int> GetPrevAvailablemoves()
    {
        return prevavailableMoves;
    }

    public List<GameObject> GetPrevHighlightedspots()
    {
        return prevhighlightedSpots;
    }

    //Frees up previous selection
    public void Clearlastmove()
    {
        prevselectionX = "";
    }

    public ChessPiece GetCurrentlyDragging()
    {
        return currentlyDragging;
    }

    public List<Vector2Int> GetAvailableMoves()
    {
        return availableMoves;
    }

    public List<Vector2Int[]> GetMoveList()
    {
        return moveList;
    }


    //Special Moves
    private string ProcessSpecialMove()
    {
        //print("ProcessSpecialMove, 1 " + specialMove);
        string special_move_string = "";
        if (specialMove == SpecialMove.EnPassant)
        {
            var newMove = moveList[moveList.Count - 1];
            ChessPiece myPawn = boardFormation.ChessPieces[newMove[1].x, newMove[1].y];
            var targetPawnPosition = moveList[moveList.Count - 2];
            ChessPiece enemyPawn = boardFormation.ChessPieces[targetPawnPosition[1].x, targetPawnPosition[1].y];
            string fr1, fr2, to1, to2, c1, c2;
            fr1 = xnom[targetPawnPosition[1].x];
            fr2 = ynom[targetPawnPosition[1].y];
            to1 = xnom[newMove[1].x];
            to2 = ynom[newMove[1].y];
            c1 = xnom[enemyPawn.currentX];
            c2 = ynom[enemyPawn.currentY];

            //print("Enpassant " + fr1 + fr2 + "_" + to1 + to2);
            if (myPawn.currentX == enemyPawn.currentX)
            {
                if (myPawn.currentY == enemyPawn.currentY - 1 || myPawn.currentY == enemyPawn.currentY + 1)
                {
                    if (enemyPawn.team == 0)
                    {
                        deadWhites.Add(enemyPawn);                     

                        enemyPawn.SetScale(Vector3.one * deathSize);

                        Vector3 deadPos;
                        if (gameState.CurrentGameMode == ChessGameMode.VsAI)
                        {                            
                            deadPos = new Vector3(whiteX * boardFormation.GetDistanceX(), -commonY * boardFormation.GetDistanceY(), 0) + Vector3.left * deathSpacing * (deadWhites.Count - 1);
                        }
                        else
                        {
                            if (manager.AmIWhite())
                            {
                                deadPos = new Vector3(whiteX * boardFormation.GetDistanceX(), -commonY * boardFormation.GetDistanceY(), 0) + Vector3.left * deathSpacing * (deadWhites.Count - 1);
                            }
                            else
                            {
                                deadPos = new Vector3(-blackX * boardFormation.GetDistanceX(), -commonY * boardFormation.GetDistanceY(), 0) + Vector3.right * deathSpacing * (deadWhites.Count - 1);
                            }
                        }
                        enemyPawn.SetPosition(deadPos);

                  
                        if (special_move_string == "")
                        {
                            special_move_string = "EnPassant_" + fr1 + fr2 + "_" + to1 + to2 + "_" + c1 + c2;
                        }
                        else
                        {
                            special_move_string = special_move_string + ";EnPassant_" + fr1 + fr2 + "_" + to1 + to2 + "_" + c1 + c2;
                        }

                        //enemyPawn.SetPosition(new Vector3(8 * tileSize, yOffset + 0.13f, -1 * tileSize)
                        //    - bounds
                        //    + new Vector3(tileSize / 2, 0, tileSize / 2)
                        //    + (Vector3.forward * deathSpacing) * deadWhites.Count);
                    }
                    else
                    {
                        deadBlacks.Add(enemyPawn);
                        //enemyPawn.SetScale(Vector3.one * deathSize);
                        //enemyPawn.SetPosition(new Vector3(7.9f * tileSize, yOffset + 0.05f, 7.9f * tileSize)
                        //    - bounds
                        //    + new Vector3(tileSize / 2, 0, tileSize / 2)
                        //    + (Vector3.left * deathSpacing) * deadBlacks.Count);
                        enemyPawn.SetScale(Vector3.one * deathSize);

                        Vector3 deadPos;
                        if (gameState.CurrentGameMode == ChessGameMode.VsAI)
                        {
                            deadPos = new Vector3(blackX * boardFormation.GetDistanceX(), commonY * boardFormation.GetDistanceY(), 0) + Vector3.left * deathSpacing * (deadBlacks.Count - 1);   
                        }
                        else
                        {
                            if (manager.AmIWhite())
                            {
                                deadPos = new Vector3(whiteX * boardFormation.GetDistanceX(), commonY * boardFormation.GetDistanceY(), 0) + Vector3.left * deathSpacing * (deadBlacks.Count - 1);

                            }
                            else
                            {
                                deadPos = new Vector3(-blackX * boardFormation.GetDistanceX(), commonY * boardFormation.GetDistanceY(), 0) + Vector3.right * deathSpacing * (deadBlacks.Count - 1);
                            }
                        }
                        enemyPawn.SetPosition(deadPos);                       
                        


                        if (special_move_string == "")
                        {
                            special_move_string = "EnPassant_" + fr1 + fr2 + "_" + to1 + to2 + "_" + c1 + c2;
                        }
                        else
                        {
                            special_move_string = special_move_string + ";EnPassant_" + fr1 + fr2 + "_" + to1 + to2 + "_" + c1 + c2;
                        }

                        //enemyPawn.SetPosition(new Vector3(-1 * tileSize, yOffset + 0.13f, 8 * tileSize)
                        //    - bounds
                        //    + new Vector3(tileSize / 2, 0, tileSize / 2)
                        //    + (Vector3.back * deathSpacing) * deadBlacks.Count);
                    }
                    boardFormation.ChessPieces[enemyPawn.currentX, enemyPawn.currentY] = null;
                }
            }

        }

        if (specialMove == SpecialMove.Promotion)
        {
            //newEmptyGameObject2.transform.position = Vector3.zero;

            float offsetLeft = (-boardFormation.GetBoardWidth() / 2f) * boardFormation.GetDistanceX() + boardFormation.GetDistanceX() / 2f;
            float offsetBottom = (-boardFormation.GetBoardHeight() / 2f) * boardFormation.GetDistanceY() + boardFormation.GetDistanceY() / 2f;
            //Debug.Log("Spawn " + offsetLeft + "::" + distanceX);
            Vector3 nextPosition = new Vector3(offsetLeft, offsetBottom, 1f);
            //print("ProcessSpecialMove, 2 " + moveList.Count+" :: "+chessPieces.Length);
            Vector2Int[] lastMove = moveList[moveList.Count - 1];
            ChessPiece targetPawn = boardFormation.ChessPieces[lastMove[1].x, lastMove[1].y];
            //print("ProcessSpecialMove, 3 " + targetPawn.team + " :: " + lastMove[1].y+" :: "+ lastMove[1].x);
            if (targetPawn.type == ChessPieceType.Pawn)
            {
                if (targetPawn.team == 0 && lastMove[1].y == 7)
                {
                    ChessPiece newQueen;
                    PromotionType promotionType = isExecutingRemoteMove ? networkPromotionType : PromotionController.instance.GetPromotionType();
                    //switch (PromotionController.instance.GetSelectedPromotionValue())
                    switch (promotionType)
                    {
                        case PromotionType.Queen:
                            //newQueen = SpawnSinglePiece(4, 0, ChessPieceType.Queen);
                            newQueen = boardFormation.SpawnOnePiece(4, nextPosition,  0, ChessPieceType.Queen);
                            special_move_string = special_move_string == "" ? "Promotion_QueenW" : special_move_string + ";Promotion_QueenW";
                            break;
                        case PromotionType.Rook:
                            //newQueen = SpawnSinglePiece(1, 0, ChessPieceType.Rook);
                            newQueen = boardFormation.SpawnOnePiece(1, nextPosition,  0, ChessPieceType.Rook);
                            special_move_string = special_move_string == "" ? "Promotion_RookW" : special_move_string + ";Promotion_RookW";
                            break;
                        case PromotionType.Knight:
                            //newQueen = SpawnSinglePiece(2, 0, ChessPieceType.Knight);
                            newQueen = boardFormation.SpawnOnePiece(2, nextPosition,  0, ChessPieceType.Knight);
                            special_move_string = special_move_string == "" ? "Promotion_KnightW" : special_move_string + ";Promotion_KnightW";
                            break;
                        case PromotionType.Bishop:
                            //newQueen = SpawnSinglePiece(3, 0, ChessPieceType.Bishop);
                            newQueen = boardFormation.SpawnOnePiece(3, nextPosition,  0, ChessPieceType.Bishop);
                            special_move_string = special_move_string == "" ? "Promotion_BishopW" : special_move_string + ";Promotion_BishopW";
                            break;
                        default:
                            //newQueen = SpawnSinglePiece(4, 0, ChessPieceType.Queen);
                            newQueen = boardFormation.SpawnOnePiece(4, nextPosition,  0, ChessPieceType.Queen);
                            special_move_string = special_move_string == "" ? "Promotion_QueenW" : special_move_string + ";Promotion_QueenW";
                            break;

                    }

                    //print("ProcessSpecialMove, 4 " + xnom.Length + " :: " + lastMove[1].y + " :: " + lastMove[1].x+" :: "+ (lastMove[1].x - 1));
                    //string[] ynom = { "1", "2", "3", "4", "5", "6", "7", "8" };
                    //string[] xnom = { "a", "b", "c", "d", "e", "f", "g", "h" };
                    string fr1, fr2, fr3;
                    fr1 = xnom[lastMove[1].x];
                    fr2 = ynom[lastMove[1].y];
                    //print("ProcessSpecialMove, 4.1 "+ xnom[lastMove[1].x]);
                    //print("ProcessSpecialMove, 4.2 " + ynom[lastMove[1].y]);
                    if ((lastMove[1].x - 1) > 0)
                    {
                        //print("ProcessSpecialMove, 4.3 " + xnom[lastMove[1].x - 1]);
                        fr3 = xnom[lastMove[1].x - 1];
                    }
                    else
                    {
                        //print("ProcessSpecialMove, 4.3.1 " + xnom[lastMove[1].x - 1]);
                        fr3 = xnom[lastMove[1].x];
                    }

                    //print("ProcessSpecialMove, 5 "+ special_move_string + " :: " + fr1 + " :: "+ fr2 + " :: " + fr3 + " :: "+ fr2);
                    special_move_string = special_move_string + "_" + fr1 + fr2 + "_" + fr3 + fr2;
                    //print("ProcessSpecialMove, 5.1 ");
                    //print("ProcessSpecialMove, 5.2 "+ chessPieces[lastMove[1].x, lastMove[1].y]);
                    //newQueen.transform.localPosition = chessPieces[lastMove[1].x, lastMove[1].y].transform.localPosition;
                    //print("ProcessSpecialMove, 6 ");
                    Destroy(boardFormation.ChessPieces[lastMove[1].x, lastMove[1].y].gameObject);
                    //if (player2 == RoomPhoton.room.GetID())
                    //{
                    //    newQueen.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                    //}
                    boardFormation.ChessPieces[lastMove[1].x, lastMove[1].y] = newQueen;
                    //print("ProcessSpecialMove, 7 ");
                    boardFormation.PlaceOnePiece(lastMove[1].x, lastMove[1].y, true);

                    //print("ProcessSpecialMove, 8 ");
                }
                //print("ProcessSpecialMove, 9 "+ targetPawn.team+" :: "+ lastMove[1].y);
                if (targetPawn.team == 1 && lastMove[1].y == 0)
                {
                    ChessPiece newQueen;
                    PromotionType promotionType = isExecutingRemoteMove ? networkPromotionType : PromotionController.instance.GetPromotionType();
                    //switch (PromotionController.instance.GetSelectedPromotionValue())
                    switch (promotionType)
                    {
                        case PromotionType.Queen:
                            //newQueen = SpawnSinglePiece(10, 1, ChessPieceType.Queen);
                            newQueen = boardFormation.SpawnOnePiece(10, nextPosition,  1, ChessPieceType.Queen);
                            special_move_string = special_move_string == "" ? "Promotion_QueenB" : special_move_string + ";Promotion_QueenB";
                            break;
                        case PromotionType.Rook:
                            //newQueen = SpawnSinglePiece(7, 1, ChessPieceType.Rook);
                            newQueen = boardFormation.SpawnOnePiece(7, nextPosition,  1, ChessPieceType.Rook);
                            special_move_string = special_move_string == "" ? "Promotion_RookB" : special_move_string + ";Promotion_RookB";
                            break;
                        case PromotionType.Knight:
                            //newQueen = SpawnSinglePiece(8, 1, ChessPieceType.Knight);
                            newQueen = boardFormation.SpawnOnePiece(8, nextPosition,  1, ChessPieceType.Knight);
                            special_move_string = special_move_string == "" ? "Promotion_KnightB" : special_move_string + ";Promotion_KnightB";
                            break;
                        case PromotionType.Bishop:
                            //newQueen = SpawnSinglePiece(9, 1, ChessPieceType.Bishop);
                            newQueen = boardFormation.SpawnOnePiece(9, nextPosition,  1, ChessPieceType.Bishop);
                            special_move_string = special_move_string == "" ? "Promotion_BishopB" : special_move_string + ";Promotion_BishopB";
                            break;
                        default:
                            //newQueen = SpawnSinglePiece(10, 1, ChessPieceType.Queen);
                            newQueen = boardFormation.SpawnOnePiece(10, nextPosition,  1, ChessPieceType.Queen);
                            special_move_string = special_move_string == "" ? "Promotion_QueenB" : special_move_string + ";Promotion_QueenB";
                            break;


                    }
                    //print("ProcessSpecialMove, 10 ");
                    string fr1, fr2, fr3;
                    fr1 = xnom[lastMove[1].x];
                    fr2 = ynom[lastMove[1].y];
                    if ((lastMove[1].x + 1 <= 8))
                    {
                        fr3 = xnom[lastMove[1].x + 1];
                    }
                    else
                    {
                        fr3 = xnom[lastMove[1].x];
                    }

                    //print("ProcessSpecialMove, 11 ");
                    special_move_string = special_move_string + "_" + fr1 + fr2 + "_" + fr3 + fr2;
                    //newQueen.transform.localPosition = chessPieces[lastMove[1].x, lastMove[1].y].transform.localPosition;
                    //print("ProcessSpecialMove, 12 ");
                    Destroy(boardFormation.ChessPieces[lastMove[1].x, lastMove[1].y].gameObject);
                    //print("ProcessSpecialMove, 13 ");
                    //if (player2 == RoomPhoton.room.GetID())
                    //{
                    //    newQueen.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                    //}
                    boardFormation.ChessPieces[lastMove[1].x, lastMove[1].y] = newQueen;
                    //print("ProcessSpecialMove, 14 ");
                    boardFormation.PlaceOnePiece(lastMove[1].x, lastMove[1].y, true);
                }
            }

        }

        if (specialMove == SpecialMove.Castling /*&& AnimHandler.instance.NotinCheck()*/)
        {
            Vector2Int[] lastMove = moveList[moveList.Count - 1];

            //Left Rook
            if (lastMove[1].x == 2)
            {
                if (lastMove[1].y == 0) //White side
                {
                    ChessPiece rook = boardFormation.ChessPieces[0, 0];
                    boardFormation.ChessPieces[3, 0] = rook;
                    boardFormation.PlaceOnePiece(3, 0);
                    boardFormation.ChessPieces[0, 0] = null;
                    if (special_move_string == "")
                    {
                        special_move_string = "Castling_rook_a1_d1_king_e1_c1";
                    }
                    else
                    {
                        special_move_string = special_move_string + ";Castling_rook_a1_d1_king_e1_c1";
                    }

                }
                else if (lastMove[1].y == 7)
                {
                    ChessPiece rook = boardFormation.ChessPieces[0, 7];
                    boardFormation.ChessPieces[3, 7] = rook;
                    boardFormation.PlaceOnePiece(3, 7);
                    boardFormation.ChessPieces[0, 7] = null;
                    if (special_move_string == "")
                    {
                        special_move_string = "Castling_rook_a8_d8_king_e8_c8";
                    }
                    else
                    {
                        special_move_string = special_move_string + ";Castling_rook_a8_d8_king_e8_c8";
                    }

                }
            }
            else if (lastMove[1].x == 6)
            {
                if (lastMove[1].y == 0) //White side
                {
                    ChessPiece rook = boardFormation.ChessPieces[7, 0];
                    boardFormation.ChessPieces[5, 0] = rook;
                    boardFormation.PlaceOnePiece(5, 0);
                    boardFormation.ChessPieces[7, 0] = null;
                    if (special_move_string == "")
                    {
                        special_move_string = "Castling_rook_h1_f1_king_e1_g1";
                    }
                    else
                    {
                        special_move_string = special_move_string + ";Castling_rook_h1_f1_king_e1_g1";
                    }

                }
                else if (lastMove[1].y == 7)
                {
                    ChessPiece rook = boardFormation.ChessPieces[7, 7];
                    boardFormation.ChessPieces[5, 7] = rook;
                    boardFormation.PlaceOnePiece(5, 7);
                    boardFormation.ChessPieces[7, 7] = null;
                    if (special_move_string == "")
                    {
                        special_move_string = "Castling_rook_h8_f8_king_e8_g8";
                    }
                    else
                    {
                        special_move_string = special_move_string + ";Castling_rook_h8_f8_king_e8_g8";
                    }
                }
            }


        }

        return special_move_string;
    }

    //Promotion
    public void ImplementPromtionMove()
    {
        //Debug.Log("ImplementPromotionMove called");
        Vector2Int previousPosition = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);
        //print("Here3.1");
        selectionX = hisPromX;
        selectionY = hisPromY;
        //print("Move to called from Inside ImplementPromtionMove 1 " + currentlyDragging + " :: " + selectionX + " :: " + selectionY);

        if (prevavailableMoves.Count != 0)
        {
            //print("Previous moves ends at " + prevavailableMoves[prevavailableMoves.Count - 1].x + " :: " + prevavailableMoves[prevavailableMoves.Count - 1].y);
            //print("Current moves start at  " + currentlyDragging.currentX + " :: " + currentlyDragging.currentY);
            if (prevavailableMoves[prevavailableMoves.Count - 1].x == currentlyDragging.currentX && prevavailableMoves[prevavailableMoves.Count - 1].y == currentlyDragging.currentY)
            {
                return;
            }
        }
        bool validMove = MoveTo(currentlyDragging, selectionX, selectionY, 0);

        if (!validMove)
        {
            ////print("Here2.1.2");
            currentlyDragging.SetPosition(boardFormation.GetTileCenter(previousPosition.x, previousPosition.y));
            currentlyDragging = null;
        }
        else
        {
            ////print("Here2.1.2.(");
            currentlyDragging = null;
        }

        handleHighlight.RemoveHighlightTiles();
        if (validMove)
        {
            //print("Before HighlightPrevTiles 0 " + validMove);
            handleHighlight.HighlightPrevTiles();
        }
        PromotionController.instance.SetSelectedPromotionValue("");
        PromotionController.instance.PromotionStripOff();
        hisPromX = -1;
        hisPromY = -1;
        selectionX = -1;
        selectionY = -1;
    }

    public bool IsWhiteTurn()
    {
        if (gameState.CurrentGameMode == ChessGameMode.Multiplayer)
        {
            if (manager == null)
                manager = FusionRoomManager.Instance.GetNetworkGameManager();

            return manager.IsWhiteTurn;
        }

        return gameState.IsWhiteTurn;
    }

    public ChessGameState GetGameState()
    {
        return gameState;
    }

    public ChessPiece[,] GetBoard()
    {
        return boardFormation.ChessPieces;
    }

    public UnityEngine.Color GetBlackColor()
    {
        return blackCol;
    }

    public int GetCountdownTime()
    {
        return countdownTime;
    }

    public void SetCountdownTime(int val)
    {
        countdownTime = val;
    }

    public bool GetIsExecutingRemoteMove()
    {
        return isExecutingRemoteMove;
    }

}


