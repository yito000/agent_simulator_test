using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class AgentManager : IPersistent
{
    private Dictionary<int, Agent> agentDict = new Dictionary<int, Agent>();
    private List<Agent> agentList = new List<Agent>();
    private int currentId = 0;

    public Dictionary<int, Agent> AgentDict
    {
        get
        {
            return agentDict;
        }
    }

    public List<Agent> AgentList
    {
        get
        {
            return agentList;
        }
    }

    public Agent SpawnAgent()
    {
        var a = Object.Instantiate(Resources.Load<GameObject>("agent"));
        while (true)
        {
            if (!agentDict.ContainsKey(currentId))
            {
                var aa = a.GetComponent<Agent>();
                aa.AgentId = currentId;
                aa.Speed = UnityEngine.Random.Range(3.0f, 6.0f);
                agentDict[currentId] = aa;
                agentList.Add(aa);
                currentId++;
                return aa;
            }

            currentId++;
        }
    }

    public void RemoveFirstAgent()
    {
        if (agentList.Count > 0)
        {
            var first = agentList[0];
            agentList.Remove(first);
            agentDict.Remove(first.AgentId);
            first.DestroySelf();
        }
    }

    public void ClearAllAgent()
    {
        foreach (var aa in agentList)
        {
            aa.DestroySelf();
        }

        agentList.Clear();
        agentDict.Clear();
    }

    public JObject OnSaveData()
    {
        JObject resultJson = new JObject();
        resultJson.Add("agents", new JArray());

        var arr = (JArray)resultJson["agents"];
        foreach (var a in agentList)
        {
            var nextPoint = -1;
            if (a.NextPoint != null)
            {
                nextPoint = a.NextPoint.pathId;
            }

            JObject jsonObj = new JObject{
                ["posX"] = a.transform.position.x,
                ["posY"] = a.transform.position.y,
                ["posZ"] = a.transform.position.z,
                ["rotX"] = a.transform.rotation.x,
                ["rotY"] = a.transform.rotation.y,
                ["rotZ"] = a.transform.rotation.z,
                ["rotW"] = a.transform.rotation.w,
                ["agentId"] = a.AgentId,
                ["nextPoint"] = nextPoint,
                ["speed"] = a.Speed
            };

            arr.Add(jsonObj);
        }

        return resultJson;
    }

    public void OnLoadData(JObject jsonObj)
    {
        JArray arr = (JArray)jsonObj["agents"];
        if (arr == null)
        {
            Debug.Log("key: agents not found");
            return;
        }

        foreach (var jo in arr)
        {
            var a = Object.Instantiate(Resources.Load<GameObject>("agent"));
            a.name = "agent";
            var aa = a.GetComponent<Agent>();

            if (jo["agentId"] != null)
            {
                var v = jo["agentId"];
                aa.AgentId = v.Value<int>();
            }

            agentDict[aa.AgentId] = aa;
            agentList.Add(aa);

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
            a.transform.position = pos;

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
            a.transform.rotation = rot;
            
            // speed
            if (jo["speed"] != null)
            {
                var v = jo["speed"];
                aa.Speed = v.Value<float>();
            }
        }
    }

    public void OnPostLoadData(JObject jsonObj, Simulator.Context context)
    {
        JArray arr = (JArray)jsonObj["agents"];
        if (arr == null)
        {
            return;
        }

        foreach (var jo in arr)
        {
            if (jo["agentId"] != null && jo["nextPoint"] != null)
            {
                var v = jo["agentId"];
                var vv = jo["nextPoint"];
                var agentId = v.Value<int>();
                var nextPointId = vv.Value<int>();

                Agent agent;
                PathPoint nextPoint;
                if (context.Agents.AgentDict.TryGetValue(agentId, out agent) && 
                    context.PathPoints.PathPointDict.TryGetValue(nextPointId, out nextPoint))
                {
                    agent.NextPoint = nextPoint;
                }
            }
        }
    }
}
