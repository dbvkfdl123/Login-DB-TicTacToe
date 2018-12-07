using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEngine.Networking;

public struct DownloadData
{
    public string username;
    public string nickname;
    public double score;
}

public class UserInfo : MonoBehaviour {
    private string userName;

    public Text idText;
    public Text nickText;
    public Text scoreText;
    
    public GameObject statPanel;

    public Text ServerMessage;

    //스코어 추가
    public InputField scoreInputField;

    //스코어 가져오기
    public Text newscoreText;

    public void OnclickAddScore()
    {
        string scoreStr = scoreInputField.text;
        if (!string.IsNullOrEmpty(scoreStr))
        {
            StartCoroutine(AddScore(scoreStr));
        }
    }
    public void OnClickGetScore()
    {
        StartCoroutine(GetScore());
    }
    IEnumerator AddScore(string score)
    {
        
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:3000/users/addscore/"+score))
        {
            string sid = PlayerPrefs.GetString("sid");
            if (!string.IsNullOrEmpty(sid))
            {
                www.SetRequestHeader("Cookie", sid);
            }
            yield return www.Send();
            string resultStr = www.downloadHandler.text;
        }
    }
    IEnumerator GetScore()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:3000/users/score/"))
        {
            string sid = PlayerPrefs.GetString("sid");
            if (!string.IsNullOrEmpty(sid))
            {
                yield return www.Send();
                string resultStr = www.downloadHandler.text;

                if (!string.IsNullOrEmpty(resultStr))
                {
                    scoreText.text = resultStr;
                }
            }
        }
    }


    public void Start()
    {
        statPanel.SetActive(false);
        userName = PlayerPrefs.GetString("UserName");
    }

    public void StatusClick()
    {
        UserData ud = new UserData();
        ud.username = userName;
        if (!statPanel.activeSelf)
        {
            statPanel.SetActive(true);
        }
        else
        {
            statPanel.SetActive(false);
        }
        StartCoroutine(Serch(ud));
    }

    public void OnBtnClicked()
    {
        StartCoroutine(GetUserInfo());
    }

    IEnumerator GetUserInfo()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:3000/users/info"))
        {
            string username = PlayerPrefs.GetString("UserName");
            if (!string.IsNullOrEmpty(username))
            {
                www.SetRequestHeader("Cookie", "username=" + username);
            }
            yield return www.Send();

            ServerMessage.text = www.downloadHandler.text;
        }
    }

    IEnumerator Serch(UserData userdata)
    {
        //Debug.Log(username);
        string postData = JsonUtility.ToJson(userdata);
        byte[] sendData = Encoding.UTF8.GetBytes(postData);

        using (UnityWebRequest www = UnityWebRequest.Put("http://localhost:3000/users/serch", postData))
        {
            www.method = "POST";
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.Send();

            if (www.isHttpError || www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string dataStr = www.downloadHandler.text;
                var data = JsonUtility.FromJson<DownloadData>(dataStr);
               
                idText.text = data.username;
                nickText.text = data.nickname;
                scoreText.text = "" + data.score;
            }
        }
    }
}
