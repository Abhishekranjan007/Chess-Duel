using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;

public class ProfileImage : MonoBehaviour
{
    public static ProfileImage instance;
    
    //public Image whiteplImageOne, whiteplImageTwo, blackplImageOne, blackplImageTwo;

    public GameObject whiteImageplone, blackImageplone, whiteImagepltwo, blackImagepltwo, whiteplNameone, blackplNameone, whiteplNametwo, blackplNametwo;


    public GameObject timeHolderdown, timeHolderup, topprofilepanel, downprofilepanel;

    public TimeView timeView;
    NetworkGameManager manager;

    //Set up instance and devicetypes 
    void Awake()
    {        
        if (instance == null)
            instance = this;
    }
    

    public void ShowProfile()
    {        

        if (Chessboard.Instance.GetGameState().CurrentGameMode == ChessGameMode.VsAI)
        {
            whiteImageplone.GetComponent<CanvasGroup>().alpha = 1f;
            blackImageplone.GetComponent<CanvasGroup>().alpha = 1f;
            whiteplNameone.GetComponent<TMP_Text>().text = Constants.You;
            blackplNameone.GetComponent<TMP_Text>().text = "Player";
        }
        else
        {
            manager = FusionRoomManager.Instance.GetNetworkGameManager();

            if (manager.AmIWhite())
            {
                whiteImageplone.GetComponent<CanvasGroup>().alpha = 1f;
                blackImageplone.GetComponent<CanvasGroup>().alpha = 1f;
                whiteplNameone.GetComponent<TMP_Text>().text = Constants.You;
                blackplNameone.GetComponent<TMP_Text>().text = manager.GetOpponentName();
            }
            else
            {
                whiteImagepltwo.GetComponent<CanvasGroup>().alpha = 1f;
                blackImagepltwo.GetComponent<CanvasGroup>().alpha = 1f;
                whiteplNametwo.GetComponent<TMP_Text>().text = manager.GetOpponentName();
                blackplNametwo.GetComponent<TMP_Text>().text = Constants.You;
            }
        }
            
        
    }

    

    //Public endpoint for other classes to load Images
    //Uses Coroutines to load images
    public void LoadImages()
    {
        //Debug.Log("LoadImages1 ");
        //StartCoroutine(SetImage());
        ////Debug.Log("LoadImages5 ");
        //StartCoroutine(SetImagetwo());
    }

    //Coroutine to load first Image
    //private IEnumerator SetImage()
    //{
    //    string url = PlayerPrefs.GetString("whitephoto");
    //    //Debug.Log("LoadImages2 "+url);
    //    Texture2D tex;
    //    UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
    //    //Debug.Log("LoadImages3 ");
    //    yield return request.SendWebRequest();
    //    if (request.isNetworkError || request.isHttpError)
    //    {
    //        //Debug.Log(request.error);
    //    }
    //    else
    //    {
    //        tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
    //        //Debug.Log("LoadImages4 ");
    //        if (tex != null)
    //        {
    //            //Debug.Log("Size of texture " + tex.width + "::" + tex.height);
    //            Sprite sprite;
    //            sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100);
    //            //sp1one = sprite;
    //            if (Chessboard.Instance.GetPlayer1() == RoomPhoton.room.GetID())
    //            {
    //                if (devicetype == "mobile")
    //                {
    //                    whiteplImageOne.sprite = sprite;
    //                }
    //                else if (devicetype == "tab")
    //                {
    //                    whiteplImageOnetb.sprite = sprite;
    //                }
    //                else
    //                {
    //                    whiteplImageOnedt.sprite = sprite;
    //                }
    //            }
    //            else
    //            {
    //                if (devicetype == "mobile")
    //                {
    //                    whiteplImageTwo.sprite = sprite;
    //                }
    //                else if (devicetype == "tab")
    //                {
    //                    whiteplImageTwotb.sprite = sprite;
    //                }
    //                else
    //                {
    //                    whiteplImageTwodt.sprite = sprite;
    //                }
    //            }

    //        }
    //    }

    //    yield return null;

    //}

    ////Coroutine to load second Image
    //private IEnumerator SetImagetwo()
    //{
    //    string url = PlayerPrefs.GetString("blackphoto");
    //    //Debug.Log("LoadImages6 "+url);
    //    Texture2D tex;
    //    UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
    //    //Debug.Log("LoadImages7 ");
    //    yield return request.SendWebRequest();
    //    if (request.isNetworkError || request.isHttpError)
    //    {
    //        //Debug.Log(request.error);
    //    }
    //    else
    //    {
    //        tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
    //        //Debug.Log("LoadImages8 ");
    //        if (tex != null)
    //        {
    //            //Debug.Log("2 Size of texture " + tex.width + "::" + tex.height);
    //            Sprite sprite;
    //            sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100);
    //            //sp2one = sprite;
    //            if (Chessboard.Instance.GetPlayer1() == RoomPhoton.room.GetID())
    //            {
    //                if (devicetype == "mobile")
    //                {
    //                    blackplImageOne.sprite = sprite;
    //                }
    //                else if (devicetype == "tab")
    //                {
    //                    blackplImageOnetb.sprite = sprite;
    //                }
    //                else
    //                {
    //                    blackplImageOnedt.sprite = sprite;
    //                }
    //            }
    //            else
    //            {
    //                if (devicetype == "mobile")
    //                {
    //                    blackplImageTwo.sprite = sprite;
    //                }
    //                else if (devicetype == "tab")
    //                {
    //                    blackplImageTwotb.sprite = sprite;
    //                }
    //                else
    //                {
    //                    blackplImageTwodt.sprite = sprite;
    //                }
    //            }
    //        }
    //    }

    //    yield return null;
    //}

    //Sets up Profile UIs for both players on game load
    //public void DisplayProfiles(string player1, string player2)
    //{
    //    Data.IsHost = RoomPhoton.room.isHost();
    //    string uid = PlayerPrefs.GetString(Constants.Uid);
    //    string rid = Data.RoomId;
    //    string roomTimeId = Data.RoomTimeId;
    //    string playeronename, playertwoname;

    //    if (player1 == RoomPhoton.room.GetID())
    //    {
    //        string ownId = PlayerPrefs.GetString(Constants.Uid);
    //        string oppId = PlayerPrefs.GetString(Constants.OppUid);

    //        if (player1 == ownId)
    //        {
    //            Chessboard.Instance.SetPlayerNames(PlayerPrefs.GetString(Constants.OwnName), PlayerPrefs.GetString(Constants.OppName));
    //            playeronename = PlayerPrefs.GetString(Constants.OwnName);
    //            playertwoname = PlayerPrefs.GetString(Constants.OppName);
    //        }
    //        else //if (player1 == oppId)
    //        {
    //            Chessboard.Instance.SetPlayerNames(PlayerPrefs.GetString(Constants.OppName), PlayerPrefs.GetString(Constants.OwnName));
    //            playeronename = PlayerPrefs.GetString(Constants.OppName);
    //            playertwoname = PlayerPrefs.GetString(Constants.OwnName);
    //        }

    //        timeView.DisplayTimeWhite(600f, Chessboard.Instance.GetBlackColor());
    //        timeView.DisplayTimeBlack(600f, Chessboard.Instance.GetBlackColor());

    //        if (devicetype == Constants.Mobile)
    //        {
    //            whiteImageplone.SetActive(true);
    //            blackImageplone.SetActive(true);
    //            whiteplNameone.SetActive(true);
    //            whiteplNameone.GetComponent<Text>().text = playeronename.Length >= 10 ? playeronename.Substring(0, 8) + "..." : playeronename;
    //            blackplNameone.GetComponent<Text>().text = playertwoname.Length >= 10 ? playertwoname.Substring(0, 8) + "..." : playertwoname;
    //            blackplNameone.SetActive(true);
    //        }
    //        else if (devicetype == Constants.Tab)
    //        {
    //            whiteImageplonetb.SetActive(true);
    //            blackImageplonetb.SetActive(true);
    //            whiteplNameonetb.SetActive(true);
    //            whiteplNameonetb.GetComponent<Text>().text = playeronename.Length >= 10 ? playeronename.Substring(0, 8) + "..." : playeronename;
    //            blackplNameonetb.GetComponent<Text>().text = playertwoname.Length >= 10 ? playertwoname.Substring(0, 8) + "..." : playertwoname;
    //            blackplNameonetb.SetActive(true);
    //        }
    //        else
    //        {
    //            whiteImageplonedt.SetActive(true);
    //            blackImageplonedt.SetActive(true);
    //            whiteplNameonedt.SetActive(true);
    //            whiteplNameonedt.GetComponent<Text>().text = playeronename.Length >= 10 ? playeronename.Substring(0, 8) + "..." : playeronename;
    //            blackplNameonedt.GetComponent<Text>().text = playertwoname.Length >= 10 ? playertwoname.Substring(0, 8) + "..." : playertwoname;
    //            blackplNameonedt.SetActive(true);
    //        }
    //        timeView.DisplayTimerOne();

    //    }
    //    else if (player2 == RoomPhoton.room.GetID())
    //    {
    //        string ownId = PlayerPrefs.GetString(Constants.Uid);
    //        string oppId = PlayerPrefs.GetString(Constants.OppUid);

    //        if (player1 == ownId)
    //        {
    //            Chessboard.Instance.SetPlayerNames(PlayerPrefs.GetString(Constants.OwnName), PlayerPrefs.GetString(Constants.OppName));
    //            playeronename = PlayerPrefs.GetString(Constants.OwnName);
    //            playertwoname = PlayerPrefs.GetString(Constants.OppName);
    //        }
    //        else //if (player1 == oppId)
    //        {
    //            Chessboard.Instance.SetPlayerNames(PlayerPrefs.GetString(Constants.OppName), PlayerPrefs.GetString(Constants.OwnName));
    //            playeronename = PlayerPrefs.GetString(Constants.OppName);
    //            playertwoname = PlayerPrefs.GetString(Constants.OwnName);
    //        }

    //        timeView.DisplayTimeWhiteBl(600f, Chessboard.Instance.GetBlackColor());
    //        timeView.DisplayTimeBlackBl(600f, Chessboard.Instance.GetBlackColor());

    //        if (devicetype == Constants.Mobile)
    //        {
    //            whiteImagepltwo.SetActive(true);
    //            blackImagepltwo.SetActive(true);
    //            whiteplNametwo.SetActive(true);
    //            whiteplNametwo.GetComponent<Text>().text = playeronename.Length >= 10 ? playeronename.Substring(0, 8) + "..." : playeronename;
    //            blackplNametwo.GetComponent<Text>().text = playertwoname.Length >= 10 ? playertwoname.Substring(0, 8) + "..." : playertwoname;
    //            blackplNametwo.SetActive(true);
    //        }
    //        else if (devicetype == Constants.Tab)
    //        {
    //            whiteImagepltwotb.SetActive(true);
    //            blackImagepltwotb.SetActive(true);
    //            whiteplNametwotb.SetActive(true);
    //            whiteplNametwotb.GetComponent<Text>().text = playeronename.Length >= 10 ? playeronename.Substring(0, 8) + "..." : playeronename;
    //            blackplNametwotb.GetComponent<Text>().text = playertwoname.Length >= 10 ? playertwoname.Substring(0, 8) + "..." : playertwoname;
    //            blackplNametwotb.SetActive(true);
    //        }
    //        else
    //        {
    //            whiteImagepltwodt.SetActive(true);
    //            blackImagepltwodt.SetActive(true);
    //            whiteplNametwodt.SetActive(true);
    //            whiteplNametwodt.GetComponent<Text>().text = playeronename.Length >= 10 ? playeronename.Substring(0, 8) + "..." : playeronename;
    //            blackplNametwodt.GetComponent<Text>().text = playertwoname.Length >= 10 ? playertwoname.Substring(0, 8) + "..." : playertwoname;
    //            blackplNametwodt.SetActive(true);
    //        }
    //        timeView.DisplayTimerTwo();
    //        //print("PlayerOneName - PlayerTwoName" + playeronename + " :: " + playeronename.Length + " - " + playertwoname + " :: " + playertwoname.Length);            
    //    }
    //}

}
