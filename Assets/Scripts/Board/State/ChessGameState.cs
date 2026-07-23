using System.Collections.Generic;
using UnityEngine;

public enum ChessGameMode
{    
    VsAI,
    Multiplayer
}

public enum PlayerType
{
    Human,
    AI,
    RemotePlayer
}

public class ChessGameState
{
    public bool IsWhiteTurn = true;

    public ChessGameMode CurrentGameMode = ChessGameMode.VsAI;

    public readonly List<ChessMove> MoveHistory = new List<ChessMove>();

    public PlayerType WhitePlayer = PlayerType.Human;

    public PlayerType BlackPlayer = PlayerType.Human;

    public void AddMove(ChessMove move)
    {
        MoveHistory.Add(move);
    }

    public void SwitchTurn()
    {
        IsWhiteTurn = !IsWhiteTurn;
    }

    public ChessMove GetLastMove()
    {
        if (MoveHistory.Count == 0)
            return default;

        return MoveHistory[MoveHistory.Count - 1];
    }

    public int MoveCount()
    {
        return MoveHistory.Count;
    }

    
}