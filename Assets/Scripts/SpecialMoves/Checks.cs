using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class Checks : MonoBehaviour
{
    public static Checks Instance {  get; private set; }

    private List<Vector2Int> availableMovesTemp;
    private ChessPiece[,] pieces;
    private BoardFormation board;
    private Chessboard csboard;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (Instance == null)
            Instance = this;        
    }

    void Start()
    {
        board = BoardFormation.Instance;
        csboard = Chessboard.Instance;//.GetMoveList()
    }

    //Prevent moves that will result to a check in the next move
    public void PreventCheck()
    {
        ChessPiece targetKing = null;
        for (int x = 0; x < board.GetBoardWidth(); x++)
        {
            for (int y = 0; y < board.GetBoardHeight(); y++)
            {
                if (/*chessPieces[x, y]*/board.ChessPieces[x, y] != null)
                {
                    if (/*chessPieces[x, y]*/board.ChessPieces[x, y].type == ChessPieceType.King)
                    {
                        if (/*chessPieces[x, y]*/board.ChessPieces[x, y].team == Chessboard.Instance.GetCurrentlyDragging().team)
                            targetKing = board.ChessPieces[x, y]/*chessPieces[x, y]*/;
                    }
                }

            }
        }

        availableMovesTemp = Chessboard.Instance.GetAvailableMoves();
        if (targetKing == null)
        {
            Debug.LogError("King not found");
            return;
        }

        //Since we're sending ref availableMoves, we will be deleting moves that are putting us in check
        SimulateMoveForSinglePiece(Chessboard.Instance.GetCurrentlyDragging(), ref availableMovesTemp, targetKing);
    }

    //Calculate future moves by simulating a movement
    private void SimulateMoveForSinglePiece(ChessPiece cp, ref List<Vector2Int> moves, ChessPiece targetKing)
    {
        //Save the current values, to reset after the function call
        int actualX = cp.currentX;
        int actualY = cp.currentY;
        List<Vector2Int> movesToRemove = new List<Vector2Int>();

        //Going through all the moves, simulate them and check if we're in check
        for (int i = 0; i < moves.Count; i++)
        {
            int simX = moves[i].x;
            int simY = moves[i].y;

            Vector2Int kingPositionThisSim = new Vector2Int(targetKing.currentX, targetKing.currentY);
            //Did we simulate the king's move
            if (cp.type == ChessPieceType.King)
                kingPositionThisSim = new Vector2Int(simX, simY);

            //Copy the [,] and not a reference
            ChessPiece[,] simulation = new ChessPiece[board.GetBoardWidth(), board.GetBoardHeight()];
            List<ChessPiece> simAttackingPieces = new List<ChessPiece>();
            for (int x = 0; x < board.GetBoardWidth(); x++)
            {
                for (int y = 0; y < board.GetBoardHeight(); y++)
                {
                    if (/*chessPieces[x, y]*/board.ChessPieces[x, y] != null)
                    {
                        simulation[x, y] = board.ChessPieces[x, y]/*chessPieces[x, y]*/;
                        if (simulation[x, y].team != cp.team)
                            simAttackingPieces.Add(simulation[x, y]);
                    }
                }
            }


            //Simulate that move
            simulation[actualX, actualY] = null;
            cp.currentX = simX;
            cp.currentY = simY;
            simulation[simX, simY] = cp;

            //Did one of the piece got taken down during our simulation
            var deadPiece = simAttackingPieces.Find(c => c.currentX == simX && c.currentY == simY);
            if (deadPiece != null)
                simAttackingPieces.Remove(deadPiece);

            //Get all the simulated attacking pieces moves
            List<Vector2Int> simMoves = new List<Vector2Int>();
            for (int a = 0; a < simAttackingPieces.Count; a++)
            {
                var pieceMoves = simAttackingPieces[a].GetAvailableMoves(ref simulation, board.GetBoardWidth(), board.GetBoardHeight());
                for (int b = 0; b < pieceMoves.Count; b++)
                {
                    simMoves.Add(pieceMoves[b]);
                }
            }

            //if the king is in trouble, remove the move
            if (Utility.ContainsValidMove(ref simMoves, kingPositionThisSim))
            {
                ////print("removed move"+moves[i]);
                movesToRemove.Add(moves[i]);
            }

            //Restore cp data
            cp.currentX = actualX;
            cp.currentY = actualY;

        }


        //Remove from the current available move list
        for (int i = 0; i < movesToRemove.Count; i++)
        {
            moves.Remove(movesToRemove[i]);
        }

    }

    private void GetPiecesForTeam(int targetTeam, List<ChessPiece> attackers, List<ChessPiece> defenders, out ChessPiece king)
    {
        king = null;

        for (int x = 0; x < board.GetBoardWidth(); x++)
        {
            for (int y = 0; y < board.GetBoardHeight(); y++)
            {
                ChessPiece piece =
                    board.ChessPieces[x, y];

                if (piece == null)
                    continue;

                if (piece.team == targetTeam)
                {
                    defenders.Add(piece);

                    if (piece.type == ChessPieceType.King)
                        king = piece;
                }
                else
                {
                    attackers.Add(piece);
                }
            }
        }
    }

    private bool IsKingInCheck(ChessPiece king, List<ChessPiece> attackers)
    {
        List<Vector2Int> moves =
            new List<Vector2Int>();

        foreach (var attacker in attackers)
        {
            ChessPiece[,] boards =
                BoardFormation.Instance.ChessPieces;

            var pieceMoves =
                attacker.GetAvailableMoves(
                    ref boards,
                    BoardFormation.Instance.GetBoardWidth(),
                    BoardFormation.Instance.GetBoardHeight());

            moves.AddRange(pieceMoves);
        }

        return Utility.ContainsValidMove(
            ref moves,
            new Vector2Int(
                king.currentX,
                king.currentY));
    }

    private bool CanAnyPieceSaveKing(List<ChessPiece> defenders, ChessPiece king)
    {
        foreach (var piece in defenders)
        {
            ChessPiece[,] boards =
                BoardFormation.Instance.ChessPieces;

            List<Vector2Int> moves =
                piece.GetAvailableMoves(
                    ref boards,
                    board.GetBoardWidth(),
                    board.GetBoardHeight());

            SimulateMoveForSinglePiece(
                piece,
                ref moves,
                king);

            if (moves.Count > 0)
                return true;
        }

        return false;
    }

    public bool CheckForStalemate()
    {
        //if (csboard.GetMoveList().Count == 0)
        //    return false;

        Vector2Int[] lastMove =
            csboard.GetMoveList()[csboard.GetMoveList().Count - 1];

        ChessPiece lastMovedPiece =
            board.ChessPieces[lastMove[1].x, lastMove[1].y];

        if (lastMovedPiece == null)
            return false;

        int targetTeam =
            (lastMovedPiece.team == 0) ? 1 : 0;

        List<ChessPiece> attackers =
            new List<ChessPiece>();

        List<ChessPiece> defenders =
            new List<ChessPiece>();

        GetPiecesForTeam(
            targetTeam,
            attackers,
            defenders,
            out ChessPiece king);

        if (king == null)
            return false;

        // Stalemate cannot happen if king is in check
        if (IsKingInCheck(king, attackers))
            return false;

        // If any legal move exists, not stalemate
        if (CanAnyPieceSaveKing(defenders, king))
            return false;

        return true;
    }  

    public bool CheckForCheckmate()
    {
        //Debug.Log("CheckForCheckmate sees turn = " + Chessboard.Instance.IsWhiteTurn());

        if (csboard == null)
            csboard = Chessboard.Instance;

        if (board == null)
            board = BoardFormation.Instance;

        if (csboard.GetMoveList().Count == 0)
            return false;

        Vector2Int[] lastMove =
            csboard.GetMoveList()[csboard.GetMoveList().Count - 1];

        ChessPiece lastMovedPiece =
            board.ChessPieces[lastMove[1].x, lastMove[1].y];

        if (lastMovedPiece == null)
            return false;

        int targetTeam =
            (lastMovedPiece.team == 0) ? 1 : 0;

        List<ChessPiece> attackers =
            new List<ChessPiece>();

        List<ChessPiece> defenders =
            new List<ChessPiece>();

        GetPiecesForTeam(
            targetTeam,
            attackers,
            defenders,
            out ChessPiece king);

        if (king == null)
            return false;

        if (!IsKingInCheck(king, attackers))
        {
            //Debug.Log("Calling TurnAnim from CheckForCheckMate()");

            AnimHandler.instance.ClearCheckAnimation();

            bool shouldAnimate = true;

            if (Chessboard.Instance.GetGameState().CurrentGameMode == ChessGameMode.Multiplayer)
            {
                // In multiplayer, skip TurnAnim() when the local player just
                // submitted their own move. IsWhiteTurn hasn't replicated back
                // yet from the host, so the turn state here is stale.
                // OnMoveChanged will trigger TurnAnim() after the correct
                // turn state has arrived.
                shouldAnimate = Chessboard.Instance.GetIsExecutingRemoteMove();
            }

            if (shouldAnimate)
                AnimHandler.instance.TurnAnim();
            return false;
        }

        AnimHandler.instance.HandleCheckAnimation(king);

        if (CanAnyPieceSaveKing(defenders, king))
            return false;

        return true;
    }

    public List<Vector2Int> GetLegalMovesForPiece(ChessPiece piece)
    {
        ChessPiece king = null;

        for (int x = 0; x < board.GetBoardWidth(); x++)
        {
            for (int y = 0; y < board.GetBoardHeight(); y++)
            {
                ChessPiece p = board.ChessPieces[x, y];

                if (p == null)
                    continue;

                if (p.team == piece.team && p.type == ChessPieceType.King)
                {
                    king = p;
                }
            }
        }

        ChessPiece[,] pieces = board.ChessPieces;

        List<Vector2Int> moves = piece.GetAvailableMoves(ref pieces, board.GetBoardWidth(), board.GetBoardHeight());

        SimulateMoveForSinglePiece(piece,ref moves, king);

        return moves;
    }

    //Check if stalemate has happened
    //private bool CheckForStalemate()
    //{
    //    if (csboard.GetMoveList().Count == 0)
    //        return false;

    //    bool returnValue = true;
    //    var lastMove = csboard.GetMoveList()[csboard.GetMoveList().Count - 1];
    //    int targetTeam = (board.ChessPieces[lastMove[1].x, lastMove[1].y].team == 0) ? 1 : 0;
    //    ////print("Inside Checkforstalemate "+ lastMove+" :: "+ targetTeam);
    //    List<ChessPiece> attackingPieces = new List<ChessPiece>();
    //    List<ChessPiece> defendingPieces = new List<ChessPiece>();
    //    ChessPiece targetKing = null;
    //    for (int x = 0; x < board.GetBoardWidth(); x++)
    //    {
    //        for (int y = 0; y < board.GetBoardHeight(); y++)
    //        {
    //            if (board.ChessPieces[x, y] != null)
    //            {
    //                if (board.ChessPieces[x, y].team == targetTeam)
    //                {
    //                    defendingPieces.Add(board.ChessPieces[x, y]);
    //                    if (board.ChessPieces[x, y].type == ChessPieceType.King)
    //                        targetKing = board.ChessPieces[x, y];
    //                }
    //                else
    //                {
    //                    attackingPieces.Add(board.ChessPieces[x, y]);
    //                }

    //            }

    //        }
    //    }

    //    //Is the King attacked Right now?
    //    List<Vector2Int> currentAvailableMoves = new List<Vector2Int>();
    //    for (int i = 0; i < attackingPieces.Count; i++)
    //    {
    //        pieces = board.ChessPieces;
    //        var pieceMoves = attackingPieces[i].GetAvailableMoves(ref pieces, board.GetBoardWidth(), board.GetBoardHeight());
    //        for (int b = 0; b < pieceMoves.Count; b++)
    //        {
    //            currentAvailableMoves.Add(pieceMoves[b]);
    //        }
    //    }


    //    //Are we in Check right now?
    //    if (Utility.ContainsValidMove(ref currentAvailableMoves, new Vector2Int(targetKing.currentX, targetKing.currentY)))
    //    {
    //        returnValue = false;
    //        return returnValue;
    //    }

    //    List<Vector2Int> avmoves = new List<Vector2Int>();
    //    for (int i = 0; i < defendingPieces.Count; i++)
    //    {
    //        pieces = board.ChessPieces;

    //        List<Vector2Int> pieceMoves = defendingPieces[i].GetAvailableMoves(ref pieces, board.GetBoardWidth(), board.GetBoardHeight());

    //        if (pieceMoves.Count != 0)
    //        {
    //            for (int s = 0; s < pieceMoves.Count; s++)
    //            {
    //                avmoves.Add(pieceMoves[s]);
    //            }

    //        }

    //        SimulateMoveForSinglePiece(defendingPieces[i], ref avmoves, targetKing);

    //        if (avmoves.Count != 0)
    //        {
    //            returnValue = false;
    //            ////print("Insidecheckstalemate != 0 " + returnValue + ":" + avmoves.Count + "::" + avmoves[0]);
    //            return returnValue;
    //        }
    //    }

    //    if (avmoves.Count == 0)
    //    {
    //        ////print("Insidecheckstalemate == 0 " + returnValue + ":" + avmoves.Count);
    //    }

    //    ////print("Insidecheckstalemate"+returnValue);
    //    return returnValue;
    //}

    //Check if Checkmate has happened
    //private bool CheckForCheckmate()
    //{
    //    //print("Inside CheckForCheckmate 1 ");
    //    var lastMove = csboard.GetMoveList()[csboard.GetMoveList().Count - 1];
    //    //print("Inside CheckForCheckmate 2");
    //    int targetTeam = (board.ChessPieces[lastMove[1].x, lastMove[1].y].team == 0) ? 1 : 0;
    //    //print("Inside CheckForCheckmate 3");
    //    string tmp = moves[moves.Count - 1];

    //    List<ChessPiece> attackingPieces = new List<ChessPiece>();
    //    List<ChessPiece> defendingPieces = new List<ChessPiece>();
    //    ChessPiece targetKing = null;
    //    for (int x = 0; x < board.GetBoardWidth(); x++)
    //    {
    //        for (int y = 0; y < board.GetBoardHeight(); y++)
    //        {
    //            if (board.ChessPieces[x, y] != null)
    //            {
    //                if (board.ChessPieces[x, y].team == targetTeam)
    //                {
    //                    defendingPieces.Add(board.ChessPieces[x, y]);
    //                    if (board.ChessPieces[x, y].type == ChessPieceType.King)
    //                        targetKing = board.ChessPieces[x, y];
    //                }
    //                else
    //                {
    //                    attackingPieces.Add(board.ChessPieces[x, y]);
    //                }

    //            }

    //        }
    //    }


    //    //Is the King attacked Right now?
    //    List<Vector2Int> currentAvailableMoves = new List<Vector2Int>();
    //    for (int i = 0; i < attackingPieces.Count; i++)
    //    {
    //        pieces = board.ChessPieces;
    //        var pieceMoves = attackingPieces[i].GetAvailableMoves(ref pieces, board.GetBoardWidth(), board.GetBoardHeight());
    //        for (int b = 0; b < pieceMoves.Count; b++)
    //        {
    //            currentAvailableMoves.Add(pieceMoves[b]);
    //        }
    //    }
    //    //print("Inside CheckForCheckmate 5");

    //    //Are we in Check right now?
    //    if (Utility.ContainsValidMove(ref currentAvailableMoves, new Vector2Int(targetKing.currentX, targetKing.currentY)))
    //    {
    //        List<Vector2Int> defendingMovesT = new List<Vector2Int>();

    //        //King is under attack, can we move something to help him?
    //        for (int i = 0; i < defendingPieces.Count; i++)
    //        {
    //            pieces = board.ChessPieces;
    //            List<Vector2Int> defendingMoves = defendingPieces[i].GetAvailableMoves(ref pieces, board.GetBoardWidth(), board.GetBoardHeight());
    //            SimulateMoveForSinglePiece(defendingPieces[i], ref defendingMoves, targetKing);
    //            defendingMovesT.AddRange(defendingMoves);
    //        }

    //        //AnimHandler.instance.HandleCanvasGroupParent();
    //        //print("Before calling defendingmoves.Count " + defendingMovesT.Count);
    //        if (defendingMovesT.Count != 0)
    //        {
    //            //AnimHandler.instance.HandleCheckAnimation(targetKing);
    //            ////print("check"+targetKing.team);

    //            //tmp = tmp + ":" + AnimHandler.instance.GetPlayerOneCheck() + ":" + AnimHandler.instance.GetPlayerTwoCheck();
    //            //moves[moves.Count - 1] = tmp;


    //            return false;
    //        }
    //        else
    //        {
    //            //tmp = tmp + ":" + AnimHandler.instance.GetPlayerOneCheck() + ":" + AnimHandler.instance.GetPlayerTwoCheck();
    //            //moves[moves.Count - 1] = tmp;

    //        }

    //        return true; //Checkmate exit
    //    }
    //    else
    //    {
    //        //AnimHandler.instance.HandleCanvasGroupParent();
    //        //Console.WriteLine("Check AND TurnAnim");
    //        //AnimHandler.instance.TurnAnim();


    //        //tmp = tmp + ":" + AnimHandler.instance.GetPlayerOneCheck() + ":" + AnimHandler.instance.GetPlayerTwoCheck();
    //        //moves[moves.Count - 1] = tmp;


    //    }
    //    return false;
    //}

}
