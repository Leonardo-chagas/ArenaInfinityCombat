using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviour
{
    float time = 180;
    int score = 0;
    int maxScore = 10;
    public bool startTimer = true;
    bool comparingScores = false;
    public static GameManager instance;
    public Transform[] scorePositions;
    [HideInInspector] public PhotonView view;
    string[] cores = {"Azul", "Vermelho", "Verde", "Roxo"};
    string[] textColors = {"blue", "red", "green", "purple"};

    public TMP_Text timeText;
    public TMP_Text winnerText;
    TMP_Text[] scores;
    TMP_Text scoreText;
    public Transform canvas;
    Player winnerPlayer;
    int winnerScore;
    Color[] scoreColors = {Color.blue, Color.red, Color.green, Color.magenta};

    void Start()
    {
        instance = this;
        view = GetComponent<PhotonView>();
        if(PhotonNetwork.IsMasterClient){
            int cont = 0;
            foreach(Player pl in PhotonNetwork.PlayerList){
                view.RPC("InstantiateScore", pl, cont, pl);
                cont += 1;
            }
        }
        Hashtable hash = new Hashtable();
        hash.Add("Score", score);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        winnerText.gameObject.SetActive(false);
    }

    
    void Update()
    {
        if(PhotonNetwork.IsMasterClient){
            if(time > 0 && startTimer){
                time -= Time.deltaTime;
            }
            else if(time <= 0 && !comparingScores){
                time = 0;
                CompareScores();
            }
            view.RPC("DisplayTime", RpcTarget.All, time);
        }
    }

    void CompareScores(){
        comparingScores = true;
        int cont = 0;
        int winnerIndex = 0;
        foreach(Player pl in PhotonNetwork.PlayerList){
            if(cont == 0){
                winnerPlayer = pl;
                winnerScore = (int)pl.CustomProperties["Score"];
            }
            else{
                if((int)pl.CustomProperties["Score"] > winnerScore){
                    winnerPlayer = pl;
                    winnerScore = (int)pl.CustomProperties["Score"];
                    winnerIndex = cont;
                }
            }
            cont += 1;
        }
        view.RPC("ChangeWinnerText", RpcTarget.All, winnerIndex);
        StartCoroutine("BackToLobby");
    }

    [PunRPC]
    void DisplayTime(float timeToDisplay){
        time = timeToDisplay;
        if(timeToDisplay < 0){
            timeToDisplay = 0;
        }
        else if(timeToDisplay > 0){
            timeToDisplay += 1;
        }

        float minutes = Mathf.FloorToInt(timeToDisplay/60);
        float seconds = Mathf.FloorToInt(timeToDisplay%60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    [PunRPC]
    void InstantiateScore(int index, Player pl){
        GameObject text = PhotonNetwork.Instantiate("Score", Vector3.zero, Quaternion.identity);
        //RectTransform rectTransform = text.GetComponent<RectTransform>();
        scoreText = text.GetComponent<TextMeshProUGUI>();
        view.RPC("ChangeScoreColor", RpcTarget.All, text.GetPhotonView().ViewID, index);
        view.RPC("ScorePosition", RpcTarget.All, index, scoreText.gameObject.GetPhotonView().ViewID);
    }

    [PunRPC]
    void ChangeScoreColor(int scoreView, int index){
        PhotonView.Find(scoreView).GetComponent<TextMeshProUGUI>().color = scoreColors[index];
    }

    [PunRPC]
    void ScorePosition(int index, int pointView){
        GameObject text = PhotonView.Find(pointView).gameObject;
        text.transform.SetParent(canvas, false);
        text.transform.localPosition = scorePositions[index].localPosition;
    }

    public void ScorePoint(){
        score += 1;
        Hashtable hash = new Hashtable();
        hash.Add("Score", score);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        view.RPC("DisplayPoint", RpcTarget.All, score, scoreText.gameObject.GetPhotonView().ViewID);
        if(score >= maxScore){
            EndGame();
        }
    }

    [PunRPC]
    void DisplayPoint(int point, int pointView){
        PhotonView.Find(pointView).GetComponent<TextMeshProUGUI>().text = string.Format("{0:00}", point);
    }

    void EndGame(){
        int index = 0;
        foreach(Player pl in PhotonNetwork.PlayerList){
            if(pl.IsLocal){
                break;
            }
            index += 1;
        }
        view.RPC("ChangeWinnerText", RpcTarget.All, index);
        StartCoroutine("BackToLobby");
    }

    [PunRPC]
    void ChangeWinnerText(int index){
        winnerText.gameObject.SetActive(true);
        winnerText.text = "<color=" + textColors[index] + ">" + cores[index] + "</color> venceu esta partida";
    }

    IEnumerator BackToLobby(){
        yield return new WaitForSeconds(5.0f);
        PhotonNetwork.LoadLevel("lobby");
    }
}
