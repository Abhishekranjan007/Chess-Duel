using UnityEngine;

public class AIMove
{
    public ChessPiece Piece;
    public Vector2Int Target;
    public int Score;

    public AIMove(ChessPiece piece, Vector2Int target)
    {
        Piece = piece;
        Target = target;
        Score = 0;
    }
}