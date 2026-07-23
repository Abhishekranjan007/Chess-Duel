using Fusion;
using UnityEngine;

public struct NetworkMove : INetworkStruct
{
    public byte FromX;
    public byte FromY;

    public byte ToX;
    public byte ToY;

    public byte Promotion;

    public byte Sequence;

    public PlayerRef Sender;
}

public class NetworkGameManager : NetworkBehaviour
{
    [Networked]
    public PlayerRef WhitePlayer { get; set; }

    [Networked]
    public PlayerRef BlackPlayer { get; set; }

    [Networked]
    public NetworkString<_32> WhitePlayerName { get; set; }

    [Networked]
    public NetworkString<_32> BlackPlayerName { get; set; }

    private bool printed;

    [Networked]
    public NetworkBool IsWhiteTurn { get; set; }

    [Networked, OnChangedRender(nameof(OnMoveChanged))]
    public NetworkMove LastMove { get; set; }

    [Networked]
    public float WhiteRemainingTime { get; set; }

    [Networked]
    public float BlackRemainingTime { get; set; }

    [Networked]
    public int ActiveTimer { get; set; }

    private float syncInterval = 1f;
    private float syncTimer = 0f;

    private byte lastExecutedSequence;

    public void AssignPlayer(PlayerRef player, string playerName)
    {
        if (!HasStateAuthority)
            return;

        if (WhitePlayer == default)
        {
            WhitePlayer = player;
            WhitePlayerName = playerName;
            IsWhiteTurn = true;
            //Debug.Log($"[Fusion] {player} assigned WHITE");
        }
        else if (BlackPlayer == default)
        {
            BlackPlayer = player;
            BlackPlayerName = playerName;
            IsWhiteTurn = true;
            //Debug.Log($"[Fusion] {player} assigned BLACK");
        }
    }

    // Render() runs every frame on ALL clients including proxies,
    // unlike FixedUpdateNetwork() which only runs on the simulating client.
    // We use it to detect when both players have been replicated and print once.
    public override void Render()
    {
        // Temporary debug
        //Debug.Log(
        //    $"Render | Local={Runner.LocalPlayer} | " +
        //    $"Authority={HasStateAuthority} | " +
        //    $"MoveSeq={LastMove.Sequence}");

        if (printed)
            return;

        if (WhitePlayer == default || BlackPlayer == default)
            return;

        FusionRoomManager.Instance.SetPlayerS();

        printed = true;

        //Debug.Log("--------------------------------");
        //Debug.Log("WHITE : " + WhitePlayer);
        //Debug.Log("BLACK : " + BlackPlayer);
        //Debug.Log("LOCAL : " + Runner.LocalPlayer);
        //Debug.Log("Am I White : " + AmIWhite());
        //Debug.Log("Am I Black : " + AmIBlack());
        //Debug.Log("--------------------------------");
    }

    public override void Spawned()
    {
        FusionRoomManager.Instance.SetNetworkGameManager(this);

        //Debug.Log(
            //$"NetworkGameManager Spawned | " +
            //$"HasStateAuthority={HasStateAuthority} | " +
            //$"HasInputAuthority={HasInputAuthority} | " +
            //$"IsProxy={IsProxy}");

        // NOTE: WhitePlayer and BlackPlayer are not assigned yet at this point.
        // Render() polls every frame until both values have replicated.
    }

    //public void SendMove(int fromX, int fromY, int toX, int toY, int promotion = 0)
    //{
    //    Debug.Log(
    //$"[Network] SendMove " +
    //$"Local={Runner.LocalPlayer} " +
    //$"Authority={HasStateAuthority} " +
    //$"Move={fromX},{fromY}->{toX},{toY}");

    //    NetworkMove move = LastMove;

    //    move.Sequence++;

    //    move.FromX = (byte)fromX;
    //    move.FromY = (byte)fromY;

    //    move.ToX = (byte)toX;
    //    move.ToY = (byte)toY;

    //    move.Promotion = (byte)promotion;

    //    LastMove = move;
    //}

    public void SendMove(int fromX, int fromY, int toX, int toY, int promotion = 0, PlayerRef? sender = null)
    {
        //Debug.Log(
        //    $"[Network] SendMove " +
        //    $"Authority={HasStateAuthority} " +
        //    $"{fromX},{fromY}->{toX},{toY}");

        if (!HasStateAuthority)
        {
            RPC_SendMoveToAuthority(
                (byte)fromX,
                (byte)fromY,
                (byte)toX,
                (byte)toY,
                (byte)promotion,
                Runner.LocalPlayer);

            return;
        }

        NetworkMove move = LastMove;

        move.Sequence++;
        

        move.FromX = (byte)fromX;
        move.FromY = (byte)fromY;
        move.ToX = (byte)toX;
        move.ToY = (byte)toY;
        move.Promotion = (byte)promotion;

        move.Sender = sender ?? Runner.LocalPlayer;

        //Debug.Log($"Promotion Value = {promotion}");

        LastMove = move;
    }

    private void OnMoveChanged()
    {
        //Debug.Log(
        //    $"OnMoveChanged | " +
        //    $"Frame={Time.frameCount} | " +
        //    $"WhiteTurn={IsWhiteTurn} | " +
        //    $"Sender={LastMove.Sender}");

        //Debug.Log(
        //    $"[Network] OnMoveChanged " +
        //    $"{LastMove.FromX},{LastMove.FromY} -> {LastMove.ToX},{LastMove.ToY}");

        // Don't replay our own move
        if (LastMove.Sender == Runner.LocalPlayer)
        {
            AnimHandler.instance.TurnAnim();
            return;
        }
            

        Chessboard board = Chessboard.Instance;

        if (board == null)
        {
            Debug.LogError("Chessboard.Instance is NULL");
            return;
        }

        board.ExecuteRemoteMove(
            LastMove.FromX,
            LastMove.FromY,
            LastMove.ToX,
            LastMove.ToY,
            LastMove.Promotion);

        
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_SendMoveToAuthority(
    byte fromX,
    byte fromY,
    byte toX,
    byte toY,
    byte promotion,
    PlayerRef sender)
    {
        SendMove(fromX, fromY, toX, toY, promotion, sender);
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        syncTimer += Runner.DeltaTime;
        //Debug.Log($"[FUN] syncTimer={syncTimer} White={GameTimer.timeRemaining} Black={GameTimer.timeRemaining2}");

        if (syncTimer < syncInterval)
            return;

        syncTimer = 0f;

        WhiteRemainingTime = GameTimer.timeRemaining;
        BlackRemainingTime = GameTimer.timeRemaining2;
        ActiveTimer = TimeController.instance.GetActiveTimer();
        //Debug.Log($"[FUN] Synced White={WhiteRemainingTime} Black={BlackRemainingTime}");
    }

    //public override void FixedUpdateNetwork()
    //{       

    //    if (LastMove.Sequence == lastExecutedSequence)
    //        return;

    //    lastExecutedSequence = LastMove.Sequence;

    //    Debug.Log($"[Network] Received Move: {LastMove.FromX},{LastMove.FromY} -> {LastMove.ToX},{LastMove.ToY}  Seq={LastMove.Sequence}");

    //    if (HasStateAuthority)
    //        return;

    //    Chessboard board = Chessboard.Instance;

    //    if (board == null)
    //        return;

    //    board.ExecuteRemoteMove(LastMove.FromX, LastMove.FromY, LastMove.ToX, LastMove.ToY, LastMove.Promotion);
    //}

    public bool IsWhite(PlayerRef player)
    {
        return player == WhitePlayer;
    }

    public bool IsBlack(PlayerRef player)
    {
        return player == BlackPlayer;
    }

    public string GetWhiteName()
    {
        return WhitePlayerName.ToString();
    }

    public string GetBlackName()
    {
        return BlackPlayerName.ToString();
    }

    public bool AmIWhite()
    {
        return Runner.LocalPlayer == WhitePlayer;
    }

    public bool AmIBlack()
    {
        return Runner.LocalPlayer == BlackPlayer;
    }

    public bool IsMyTurn()
    {
        if (AmIWhite())
            return IsWhiteTurn;

        if (AmIBlack())
            return !IsWhiteTurn;

        return false;
    }

    public void SyncTimers()
    {
        if (!HasStateAuthority)
            return;

        WhiteRemainingTime = GameTimer.timeRemaining;
        BlackRemainingTime = GameTimer.timeRemaining2;
    }

    public string GetMyName()
    {
        if (AmIWhite())
            return WhitePlayerName.ToString();

        if (AmIBlack())
            return BlackPlayerName.ToString();

        return "";
    }

    public string GetOpponentName()
    {
        if (AmIWhite())
            return BlackPlayerName.ToString();

        if (AmIBlack())
            return WhitePlayerName.ToString();

        return "";
    }

}
