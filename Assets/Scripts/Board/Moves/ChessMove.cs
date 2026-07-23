using UnityEngine;

[System.Serializable]
public struct ChessMove
{
    public int FromX;
    public int FromY;

    public int ToX;
    public int ToY;

    public SpecialMove SpecialMove;

    public ChessPieceType PromotionPiece;

    public ChessMove(
        int fromX,
        int fromY,
        int toX,
        int toY,
        SpecialMove specialMove = SpecialMove.None,
        ChessPieceType promotionPiece = ChessPieceType.None)
    {
        FromX = fromX;
        FromY = fromY;

        ToX = toX;
        ToY = toY;

        SpecialMove = specialMove;
        PromotionPiece = promotionPiece;
    }
}
