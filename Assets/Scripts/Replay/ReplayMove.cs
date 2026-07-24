using UnityEngine;

[System.Serializable]
public class ReplayMove
{
    // Move

    public Vector2Int From;
    public Vector2Int To;

    // Moving piece

    public ChessPieceType MovingPieceType;
    public int MovingPieceTeam;

    // Capture

    public bool WasCapture;
    public ChessPieceType CapturedPieceType;
    public int CapturedPieceTeam;

    // Promotion

    public bool WasPromotion;
    public ChessPieceType PromotionPiece;

    // Castling / En Passant / Promotion

    public SpecialMove SpecialMove;

    // Timer

    public float WhiteRemainingTime;
    public float BlackRemainingTime;

    // Turn

    public bool WhiteTurnAfterMove;
}
