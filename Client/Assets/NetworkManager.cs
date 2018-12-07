using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using UnityEngine.UI;

//소켓통신으로 서버와 정보전달을받는 스크립트
public class NetworkManager : MonoBehaviour {
   
    SocketIOComponent socket;

    public InputField messageInputField;
    public Text messageText;
    string nickname;

	// Use this for initialization
	void Start () {
        GameObject io = GameObject.Find("SocketIO");
        socket = io.GetComponent<SocketIOComponent>();
        nickname = "Goya";
        socket.On("chat", UpdateMessage);
	}
    void UpdateMessage(SocketIOEvent e)
    {
        string nick = e.data.GetField("nick").str;
        string msg = e.data.GetField("msg").str;

        messageText.text += string.Format("{0}:{1}\n", nick, msg);
    }

    public void Send()
    {
        //자신의 메세지를 화면에 표시
        string message = messageInputField.text;
        messageText.text += string.Format("{0}:{1}\n", nickname, message);

        //자신이 입력한 메세지를 서버에 전송
        JSONObject obj = new JSONObject();
        obj.AddField("nick",nickname);
        obj.AddField("msg", message);
        socket.Emit("message",obj);

        messageInputField.text = "";

    }
}
