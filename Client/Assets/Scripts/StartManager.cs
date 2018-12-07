using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour {

    //계정 접속 확인 조회
    public void OnClickWho()
    {
        StartCoroutine(Who());
    }

    IEnumerator Who()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:3000/users/who"))
        {
            yield return www.Send();

            string sessionId = www.downloadHandler.text;
            PlayerPrefs.SetString("isWho", sessionId);
        }
    }
}
