using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeView : MonoBehaviour
{
    public GameObject homePanel;
    public Animator homePanelAnim;

    public void ShowHomePopUp()
    {
        homePanel.SetActive(true);
        homePanelAnim.SetTrigger("ZoomIN");
    }


    public void HideHomePopUp()
    {        
        homePanelAnim.SetTrigger("ZoomOut");
        homePanel.SetActive(false);
    }

    public async void HomePressed()
    {
        UIManager.Instance.CleanupCurrentGame();

        if (Chessboard.Instance.GetGameState().CurrentGameMode == ChessGameMode.Multiplayer)
        {
            await FusionRoomManager.Instance.LeaveMultiplayer();
        }

        SceneManager.LoadScene("Lobby");
    }


}
