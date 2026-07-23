using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchmakingManager : MonoBehaviour
{
    public static MatchmakingManager Instance;

    private bool searching;

    private void Awake()
    {
        Instance = this;
    }

    public void StartMatchmaking()
    {
        if (searching)
            return;

        searching = true;

        StartCoroutine(MatchmakingRoutine());
    }

    private IEnumerator MatchmakingRoutine()
    {
        //Debug.Log("Connecting to Fusion...");

        FusionRoomManager.Instance.ResetConnectionState();

        FusionRoomManager.Instance.Connect("ChessRoom");

        yield return new WaitUntil(() =>
            FusionRoomManager.Instance.ConnectedSuccessfully);

        //Debug.Log("Fusion Connected.");

        float timer = 15f;

        while (timer > 0)
        {
            if (FusionRoomManager.Instance.NumberOfPlayers() >= 2)
            {
                //Debug.Log("Opponent Found!");

                GameManager.Instance.SelectedMode = ChessGameMode.Multiplayer;

                if (FusionRoomManager.Instance.IsHost())
                {
                    FusionRoomManager.Instance.LoadGameScene();
                }

                yield break;
            }

            timer -= Time.deltaTime;

            yield return null;
        }

        //Debug.Log("No opponent found.");

        GameManager.Instance.SelectedMode = ChessGameMode.VsAI;

        if (FusionRoomManager.Instance.IsHost())
        {
            FusionRoomManager.Instance.LoadGameScene();
        }
    }
}
