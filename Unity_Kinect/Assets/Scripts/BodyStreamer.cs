using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.IO;

public class BodyStreamer : MonoBehaviour
{
    #region private members 	
    private TcpClient socketConnection;
    private Thread clientReceiveThread;
    List<List<Vector3>> vector3_motion;
    List<string> jointNames;
    GameObject[] debugObjects;
    Mapper mapper;
    public static string json;
    public static bool json_valid;
    #endregion
    // Use this for initialization 	
    void Start()
    {
        ConnectToTcpServer();

        string JSONPath = Path.Combine(Application.streamingAssetsPath, "stream.json");
        WWW JSONReader = new WWW(JSONPath);
        while (!JSONReader.isDone)
        {
        }
        string json = JSONReader.text;

        mapper = new Mapper();
        mapper.SetJSONModel(json);
        vector3_motion = mapper.GetModelVector3s();
        jointNames = mapper.GetModelJointNames();
        debugObjects = new GameObject[jointNames.Count];
        GameObject pointCube = GameObject.Find("Point Cube");
        for (var i = 0; i < jointNames.Count; i++)
        {
            var cube = Instantiate(pointCube);
            cube.name = jointNames[i];
            cube.transform.localScale = Vector3.one * 0.4f;
            debugObjects[i] = cube;
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (vector3_motion != null)
        {
            List<Vector3> currentVector3Frame = vector3_motion[0];
            for (var i = 0; i < currentVector3Frame.Count; i++)
            {
                var r = new Quaternion(0f, 0f, 0f, 0f);
                var obj = debugObjects[i];
                obj.transform.SetPositionAndRotation(currentVector3Frame[i], r);
            }
        }
        
    }
    /// <summary> 	
    /// Setup socket connection. 	
    /// </summary> 	
    private void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }
    /// <summary> 	
    /// Runs in background clientReceiveThread; Listens for incomming data. 	
    /// </summary>     
    void ListenForData()
    {
        try
        {
            socketConnection = new TcpClient("10.147.17.252", 40001);
            //socketConnection = new TcpClient("localhost", 8052);
            Byte[] bytes = new Byte[16532];
            while (true)
            {
                // Get a stream object for reading 				
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;

                    // Read incomming stream into byte arrary. 					
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message. 						
                        string serverMessage = Encoding.ASCII.GetString(incommingData);
                        json = serverMessage;
                        //Debug.Log("server message received as: " + serverMessage);

                        mapper.SetJSONModel(json);
                        vector3_motion = mapper.GetModelVector3s();
                    }
                }
                
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
    /// <summary> 	
    /// Send message to server using socket connection. 	
    /// </summary> 	
    public void SendMessage()
    {
        if (socketConnection == null)
        {
            return;
        }
        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                string clientMessage = "This is a message from one of your clients.";
                // Convert string message to byte array.                 
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
                // Write byte array to socketConnection stream.                 
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                Debug.Log("Client sent his message - should be received by server");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

}
