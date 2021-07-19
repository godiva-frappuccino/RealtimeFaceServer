using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using MiniJSON;
using System.Linq;
using System.Collections;

public class RealtimeFaceServer : MonoBehaviour
{
    // Eye Object
    //目のメッシュ設定
    public MeshRenderer eyeObjL;
    public MeshRenderer eyeObjR;
    private Vector2 eyeOffset;


    UdpClient udpClient;
    IPEndPoint endPoint;
    private int listenPort = 50007;

    void Start()
    {
        eyeObjL = GameObject.Find("eye_L_old").GetComponent<MeshRenderer>();
        eyeObjR = GameObject.Find("eye_R_old").GetComponent<MeshRenderer>();
        eyeOffset = Vector2.zero;

        Debug.Log("UnityStart");
        endPoint = new IPEndPoint(IPAddress.Any, listenPort);
        udpClient = new UdpClient(endPoint);
    }

    void Update()
    {
        while (udpClient.Available > 0)
        {
            var data = udpClient.Receive(ref endPoint);
            OnMessage(Encoding.ASCII.GetString(data));
        }
    }

    void OnMessage(string message)
    {
        //ここでメッセージの処理
        Debug.Log("I Receive " + message);
        string[] parsedMessage = message.Split(':');
        //眼球のxyの値を取得
        if (parsedMessage[0].Equals("look"))
        {
            float targetX = float.Parse(parsedMessage[1]);
            float targetY = float.Parse(parsedMessage[2]);
            Debug.Log("look " + targetX + "," + targetY);
            int duration = int.Parse(parsedMessage[3]);
            look(targetX, targetY, duration);
        }

        //UpdatePose(eyeRotationX, eyeRotationY);
    }

    // message
    public void look(float targetX, float targetY, int duration)
    {
        for(int i = 0; i < duration; i++)
        {
            StartCoroutine(DelayCoroutine(0.2f * i, () =>
            {
                float speed = 1.0f / duration;
                float diff_x = targetX - eyeOffset.x;
                float diff_y = targetY - eyeOffset.y;
                eyeOffset = new Vector2((eyeOffset.x + (diff_x * speed)) * 0.2f, (eyeOffset.y + (diff_y * speed)) * 0.2f);
                Debug.Log("Look:" + eyeOffset.x + "," + eyeOffset.y);
                eyeObjL.material.SetTextureOffset("_MainTex", eyeOffset);
                eyeObjR.material.SetTextureOffset("_MainTex", eyeOffset);
            }));

        }
    }

    // util
    private IEnumerator DelayCoroutine(float seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action?.Invoke();
    }

    // old
    public void UpdatePose(float eyeRotationX, float eyeRotationY)
    {
        Debug.Log("Value:" + eyeRotationX + "," + eyeRotationY);
        eyeOffset = new Vector2(eyeRotationX * 0.2f, eyeRotationY * 0.2f);
        Debug.Log("Change:" + eyeOffset);
        eyeObjL.material.SetTextureOffset("_MainTex", eyeOffset);
        eyeObjR.material.SetTextureOffset("_MainTex", eyeOffset);
        Debug.Log("Done.");
    }
}
