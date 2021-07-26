using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BodyPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    List<List<Vector3>> vector3_motion;
    List<string> jointNames;
    GameObject[] debugObjects;
    int frameNumber;
    public static string StreamJSON;
    Mapper mapper;
    void Start()
    {
        string JSONPath = Path.Combine(Application.streamingAssetsPath, "stream.json");
        WWW JSONReader = new WWW(JSONPath);
        while (!JSONReader.isDone)
        {
        }
        string json = JSONReader.text;
        //StreamReader reader = new StreamReader(JSONPath);
        //string json = reader.ReadToEnd();
        //reader.Close();

        Mapper mapper = new Mapper();
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

        //frameNumber = 0;
        //StartCoroutine(PlayMotion());

    }

    IEnumerator PlayMotion()
    {
        while (true){
            List<Vector3> currentVector3Frame = vector3_motion[frameNumber];
            for (var i = 0; i < currentVector3Frame.Count; i++)
            {
                var r = new Quaternion(0f, 0f, 0f, 0f);
                var obj = debugObjects[i];
                obj.transform.SetPositionAndRotation(currentVector3Frame[i], r);
            }

            if (frameNumber >= vector3_motion.Count)
            {
                frameNumber = 0;
            }
            else
            {
                frameNumber += 1;
            }

            yield return new WaitForSeconds(0.032f);
        }
        
    }

}
