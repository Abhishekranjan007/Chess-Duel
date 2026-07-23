using System.Collections.Generic;
using UnityEngine;

public enum ChessPieceType
{
    None = 0,
    Pawn = 1,
    Rook = 2,
    Knight = 3,
    Bishop = 4,
    Queen = 5,
    King = 6
}

public class ChessPiece : MonoBehaviour
{
    public int team;
    public int currentX;
    public int currentY;
    public ChessPieceType type;

    private Vector2 desiredPosition;
    private Vector3 desiredScale = Vector2.one;

    //Sets the position and scale of the Chessmans
    private void Update()
    {
        transform.localPosition = Vector2.Lerp(transform.localPosition, desiredPosition, Time.deltaTime * 10);
        transform.localScale = Vector2.Lerp(transform.localScale, desiredScale, Time.deltaTime * 10);
    }

    //Virtual function to get available moves of the given chessman
    public virtual List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        return r;
    }

    //Virtual function to get special moves of the given chessman
    public virtual SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        return SpecialMove.None;
    }

    public virtual void SetPosition(Vector2 position, bool force = false)
    {
        transform.localPosition = position;
        desiredPosition = position;
        if (force)
            transform.localPosition = desiredPosition;
    }

    //Virtual function to set the scale of the given chessman
    public virtual void SetScale(Vector2 scale, bool force = false)
    {
        desiredScale = scale;
        if (force)
            transform.localScale = desiredScale;
    }
}
