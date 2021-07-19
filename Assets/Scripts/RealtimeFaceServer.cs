using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using MiniJSON;
using System.Linq;

public class RealtimeFaceServer : MonoBehaviour
{
    // Eye Object
    //目のメッシュ設定
    public MeshRenderer eyeObjL;
    public MeshRenderer eyeObjR;

    private float dirVal = 0;
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
        JsonNode jsonNode = JsonNode.Parse(message);
        //眼球のxyの値を取得
        float eyeRotationX = (float)(jsonNode["EyeX"].Get<double>());
        float eyeRotationY = (float)(jsonNode["EyeY"].Get<double>());
        UpdatePose(eyeRotationX, eyeRotationY);
    }


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
