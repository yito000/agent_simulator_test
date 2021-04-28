using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class PathPointManager : IPersistent
{
    private Dictionary<int, PathPoint> pathPointDict = new Dictionary<int, PathPoint>();
    private List<PathPoint> pathPointList = new List<PathPoint>();

    public Dictionary<int, PathPoint> PathPointDict
    {
        get
        {
            return pathPointDict;
        }
    }

    public List<PathPoint> PathPointList
    {
        get
        {
            return pathPointList;
        }
    }

    public void ClearAllPathPoint()
    {
        foreach (var pp in pathPointList)
        {
            pp.DestroySelf();
        }

        pathPointList.Clear();
        pathPointDict.Clear();
    }

    public JObject OnSaveData()
    {
        JObject resultJson = new JObject();
        resultJson.Add("pathPoints", new JArray());

        var arr = (JArray)resultJson["pathPoints"];
        foreach (var pp in pathPointList)
        {
            var nextPoint = -1;
            if (pp.next != null)
            {
                nextPoint = pp.next.pathId;
            }

            JObject jsonObj = new JObject{
                ["posX"] = pp.transform.position.x,
                ["posY"] = pp.transform.position.y,
                ["posZ"] = pp.transform.position.z,
                ["rotX"] = pp.transform.rotation.x,
                ["rotY"] = pp.transform.rotation.y,
                ["rotZ"] = pp.transform.rotation.z,
                ["rotW"] = pp.transform.rotation.w,
                ["pathId"] = pp.pathId,
                ["nextPoint"] = nextPoint,
            };

            arr.Add(jsonObj);
        }

        return resultJson;
    }

    public void OnLoadData(JObject jsonObj)
    {
        JArray arr = (JArray)jsonObj["pathPoints"];
        if (arr == null)
        {
            Debug.Log("key: pathPoints not found");
            return;
        }

        // set basic parameters
        foreach (var jo in arr)
        {
            var go = new GameObject();
            go.name = "path";
            var pp = go.AddComponent<PathPoint>();

            if (jo["pathId"] != null)
            {
                var v = jo["pathId"];
                pp.pathId = v.Value<int>();
            }

            pathPointDict[pp.pathId] = pp;
            pathPointList.Add(pp);

            var pos = new Vector3();
            var rot = new Quaternion();

            // position
            if (jo["posX"] != null)
            {
                var v = jo["posX"];
                pos.x = v.Value<float>();
            }
            if (jo["posY"] != null)
            {
                var v = jo["posY"];
                pos.y = v.Value<float>();
            }
            if (jo["posZ"] != null)
            {
                var v = jo["posZ"];
                pos.z = v.Value<float>();
            }
            pp.transform.position = pos;

            // rotation
            if (jo["rotX"] != null)
            {
                var v = jo["rotX"];
                rot.x = v.Value<float>();
            }
            if (jo["rotY"] != null)
            {
                var v = jo["rotY"];
                rot.y = v.Value<float>();
            }
            if (jo["rotZ"] != null)
            {
                var v = jo["rotZ"];
                rot.z = v.Value<float>();
            }
            if (jo["rotW"] != null)
            {
                var v = jo["rotW"];
                rot.w = v.Value<float>();
            }
            pp.transform.rotation = rot;
        }

        // link path points
        foreach (var jo in arr)
        {
            int pathId = -1;
            int nextPathId = -1;

            if (jo["pathId"] != null)
            {
                var v = jo["pathId"];
                pathId = v.Value<int>();
            }

            if (jo["nextPoint"] != null)
            {
                var v = jo["nextPoint"];
                nextPathId = v.Value<int>();
            }

            PathPoint pathPoint;
            PathPoint nextPoint;
            if (pathPointDict.TryGetValue(pathId, out pathPoint) && 
                pathPointDict.TryGetValue(nextPathId, out nextPoint))
            {
                pathPoint.next = nextPoint;
            }
        }
    }

    public void OnPostLoadData(JObject jsonObj, Simulator.Context context)
    {
        //
    }
}
