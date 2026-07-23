using UnityEngine;
using System.Collections;

public class GameBootstrap : MonoBehaviour
{
    private void Start()
    {
        switch (GameManager.Instance.SelectedMode)
        {
            case ChessGameMode.VsAI:
                StartSinglePlayer();
                break;

            case ChessGameMode.Multiplayer:
                StartMultiplayer();
                break;
        }
    }

    private void StartSinglePlayer()
    {
        //Debug.Log("Starting Single Player");
    }

    private void StartMultiplayer()
    {
        //Debug.Log("Starting Multiplayer");

        //FusionRoomManager.Instance.ResetConnectionState();

        //FusionRoomManager.Instance.Connect();

        //StartCoroutine(MatchmakingRoutine());
    }

    private IEnumerator MatchmakingRoutine()
    {
        //Debug.Log("Waiting for Fusion connection...");

        yield return new WaitUntil(
            () => FusionRoomManager.Instance.ConnectedSuccessfully);

        //Debug.Log("Fusion Connected. Searching for opponent...");

        yield return new WaitForSeconds(5f);

        //Debug.Log("Matchmaking finished");

        int players = FusionRoomManager.Instance.NumberOfPlayers();

        //Debug.Log("Players Found = " + players);

        if (players >= 2)
        {
            //Debug.Log("START MULTIPLAYER MATCH");

            GameManager.Instance.SelectedMode = ChessGameMode.Multiplayer;
        }
        else
        {
            //Debug.Log("NO PLAYER FOUND -> START AI MATCH");

            GameManager.Instance.SelectedMode = ChessGameMode.VsAI;
        }

        //Debug.Log("Final Mode = " + GameManager.Instance.SelectedMode);
    }
}
