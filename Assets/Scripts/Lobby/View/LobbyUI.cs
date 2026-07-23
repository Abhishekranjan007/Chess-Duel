using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.LowLevelPhysics2D.PhysicsLayers;

public class LobbyUI : MonoBehaviour
{
    public GameObject playerProfile, welcomeProfile;
    public Button save;
    public TMP_InputField name;
    public TMP_Text welcomeText;


    void Awake()
    {
        string playerName = PlayerPrefs.GetString(Constants.PlayerName, "");

        if (!string.IsNullOrWhiteSpace(playerName))
        {
            playerProfile.SetActive(false);
            welcomeProfile.SetActive(true);
            welcomeText.text = $"Hi, {playerName}";
        }
        else
        {
            playerProfile.SetActive(true);
            welcomeProfile.SetActive(false);
        }
    }

    public void PlaySinglePlayer()
    {
        EnsurePlayerName();

        GameManager.Instance.SelectedMode = ChessGameMode.VsAI;
        SceneManager.LoadScene("GameScene");
    }

    public void PlayMultiplayer()
    {
        EnsurePlayerName();

        GameManager.Instance.SelectedMode = ChessGameMode.Multiplayer;
        MatchmakingManager.Instance.StartMatchmaking();
        //SceneManager.LoadScene("GameScene");
    }

    private void EnsurePlayerName()
    {
        string playerName = PlayerPrefs.GetString(Constants.PlayerName, "");

        if (string.IsNullOrWhiteSpace(playerName))
        {
            PlayerPrefs.SetString(Constants.PlayerName, Constants.GuestPlayer);
            PlayerPrefs.Save();
        }
    }

    public void SaveName()
    {
        string playerName = name.text.Trim();

        if (string.IsNullOrEmpty(playerName))
            return;

        PlayerPrefs.SetString(Constants.PlayerName, playerName);
        PlayerPrefs.Save();

        playerProfile.SetActive(false);
        welcomeProfile.SetActive(true);
        welcomeProfile.GetComponent<TMP_Text>().text = "Hi, " + playerName;
        
    }

    public void OnNameChanged(string value)
    {
        save.interactable = !string.IsNullOrWhiteSpace(value);
    }

    public void ShowPlayerProfile()
    {
        playerProfile.SetActive(true);
    }
}