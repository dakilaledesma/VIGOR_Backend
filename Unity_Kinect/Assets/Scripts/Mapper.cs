using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class Mapper : MonoBehaviour
{
    // Start is called before the first frame update
    Model model;

    void Start()
    {

    }

    public void SetJSONModel(string json)
    {
        model = JsonMapper.ToObject<Model>(json);
    }

    public List<List<Vector3>> GetModelVector3s()
    {
        List<ModelData> modelDataList = model.ModelDataList;
        List<List<Vector3>> frames = new List<List<Vector3>>();
        foreach (ModelData modelData in modelDataList)
        {
            List<Vector3> jointLocations = new List<Vector3>();
            foreach (JointData jointData in modelData.JointLocations)
            {
                jointLocations.Add(jointData.location);
            }
            frames.Add(jointLocations);
        }

        return frames;
    }

    public List<string> GetModelJointNames()
    {
        List<ModelData> modelDataList = model.ModelDataList;
        List<JointData> jointDataList = modelDataList[0].JointLocations;
        List<string> jointNames = new List<string>();
        foreach (JointData jointData in jointDataList)
        {
            jointNames.Add(jointData.JointName);
        }

        return jointNames;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
