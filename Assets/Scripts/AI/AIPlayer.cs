using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    public static AIPlayer Instance;

    private bool thinking;

    private ChessPiece lastMovedPiece;

    private void Awake()
    {
        Instance = this;
    }

    public void TryMakeMove()
    {
        if (thinking)
            return;

        thinking = true;

        Chessboard board = Chessboard.Instance;

        ChessPiece[,] pieces = board.GetBoard();

        List<AIMove> legalMoves = new List<AIMove>();

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                ChessPiece piece = pieces[x, y];

                if (piece == null)
                    continue;

                if (piece.team != 1)
                    continue;

                //List<Vector2Int> moves =
                //    piece.GetAvailableMoves(
                //        ref pieces,
                //        8,
                //        8);
                List<Vector2Int> moves = Checks.Instance.GetLegalMovesForPiece(piece);

                foreach (Vector2Int move in moves)
                {
                    AIMove aiMove = new AIMove(piece, move);

                    // Check whether this move captures a white piece
                    ChessPiece targetPiece = pieces[move.x, move.y];

                    if (targetPiece != null && targetPiece.team == 0)
                    {
                        aiMove.Score = GetPieceValue(targetPiece.type);
                    }

                    // Discourage moving same piece again
                    if (piece == lastMovedPiece)
                    {
                        aiMove.Score -= 60;
                    }

                    legalMoves.Add(aiMove);
                    //legalMoves.Add(new AIMove(piece, move));
                }
            }
        }

        //Debug.Log("AI Legal Moves = " + legalMoves.Count);

        if (legalMoves.Count == 0)
        {
            //Debug.Log("AI HAS NO MOVES");
            thinking = false;
            return;
        }

        // Highest score first
        legalMoves.Sort((a, b) => b.Score.CompareTo(a.Score));

        //Debug.Log("Best Move Score = " +legalMoves[0].Score);

        int randomIndex = Random.Range(0, Mathf.Min(2, legalMoves.Count));

        AIMove selectedMove = legalMoves[randomIndex];

        //int topCount = Mathf.Min(4, legalMoves.Count);
        //AIMove selectedMove = legalMoves[Random.Range(0, topCount)];

        //AIMove selectedMove = legalMoves[0];
        //AIMove selectedMove = legalMoves[Random.Range(0, legalMoves.Count)];

        //Debug.Log("AI MOVES " + selectedMove.Piece.type + " -> " + selectedMove.Target);

        Chessboard.Instance.ExecuteAIMove(selectedMove.Piece, selectedMove.Target.x, selectedMove.Target.y);

        lastMovedPiece = selectedMove.Piece;

        thinking = false;
    }

    private int GetPieceValue(ChessPieceType type)
    {
        switch (type)
        {
            case ChessPieceType.Pawn:
                return 100;

            case ChessPieceType.Knight:
                return 300;

            case ChessPieceType.Bishop:
                return 300;

            case ChessPieceType.Rook:
                return 500;

            case ChessPieceType.Queen:
                return 900;

            case ChessPieceType.King:
                return 10000;

            default:
                return 0;
        }
    }
}