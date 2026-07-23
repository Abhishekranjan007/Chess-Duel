using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; set; }

    [SerializeField] TMP_Text victoryMessage;
    [SerializeField] GameObject gameOverPar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void ShowGameOverUI(int i, int j)
    {
        gameOverPar.SetActive(true);
        if (i == 0)
        {            
            if (j == 0)
                victoryMessage.text = "You Win!!!";
            else
                victoryMessage.text = "Stalemate: You Win By Time!!!";
        }
        else
        {
            if (j == 0)
                victoryMessage.text = "Opponent Wins...";
            else
                victoryMessage.text = "Stalemate: Opponent Wins By Time";
        }
            
    }

    public void Reset()
    {
        TimeController.instance.SetActiveTimer(0);
        GameTimer.gameTimerInstance.DeleteLocalPrefs();
        GameTimer.gameTimerInstance.ResetTimer();
        GameTimer.gameTimerInstance.ResetTimer2();
        if (Chessboard.Instance.GetGameState().CurrentGameMode == ChessGameMode.Multiplayer)
        {            
            FusionRoomManager.Instance.ReturnToLobby();
        }
        else
        {
            SceneManager.LoadScene("Lobby");
        }
       
    }

    public void ExitClicked()
    {
        Application.Quit();
    }

    public void CleanupCurrentGame()
    {
        TimeController.instance.SetActiveTimer(0);
        GameOver.instance.UpdateGameOverFlag(true);

        GameTimer.gameTimerInstance.StopTimer();
        GameTimer.gameTimerInstance.StopTimer2();

        GameTimer.gameTimerInstance.ResetTimer();
        GameTimer.gameTimerInstance.ResetTimer2();

        GameTimer.gameTimerInstance.DeleteLocalPrefs();
    }
}
