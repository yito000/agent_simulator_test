using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class Simulator : MonoBehaviour
{
    public class Context
    {
        private PathPointManager pathPointManager = new PathPointManager();
        private AgentManager agentManager = new AgentManager();

        public PathPointManager PathPoints
        {
            get
            {
                return pathPointManager;
            }
        }

        public AgentManager Agents
        {
            get
            {
                return agentManager;
            }
        }

        public void ClearAll()
        {
            pathPointManager.ClearAllPathPoint();
            agentManager.ClearAllAgent();
        }
    }
    private Context context = new Context();

    public void SpawnAgent()
    {
        var newAgent = context.Agents.SpawnAgent();
        newAgent.NextPoint = context.PathPoints.PathPointDict[0]; // TODO: first id
    }

    public void DespawnAgent()
    {
        context.Agents.RemoveFirstAgent();
    }

    public void Save()
    {
        var dir = Directory.GetCurrentDirectory();

        var pathPointJson = context.PathPoints.OnSaveData();
        var agentJson = context.Agents.OnSaveData();
        var json = new JObject{
            ["pathPoint"] = pathPointJson,
            ["agent"] = agentJson
        };

        if (!Directory.Exists(dir + "/save"))
        {
            Directory.CreateDirectory(dir + "/save");
        }
        System.IO.File.WriteAllText(dir + "/save/save.json", json.ToString());
    }

    public void Load()
    {
        var saveFilePath = Directory.GetCurrentDirectory() + "/save/save.json";
        if (File.Exists(saveFilePath))
        {
            context.ClearAll();

            var jsonObj = JObject.Parse(File.ReadAllText(saveFilePath));
            bool pathPointKeyFound = false;
            bool agentKeyFound = false;

            if (jsonObj["pathPoint"] != null)
            {
                var v = jsonObj["pathPoint"];
                context.PathPoints.OnLoadData(v.Value<JObject>());
                pathPointKeyFound = true;
            }
            if (jsonObj["agent"] != null)
            {
                var v = jsonObj["agent"];
                context.Agents.OnLoadData(v.Value<JObject>());
                agentKeyFound = true;
            }

            if (pathPointKeyFound)
            {
                context.PathPoints.OnPostLoadData(jsonObj["pathPoint"].Value<JObject>(), context);
            }
            if (agentKeyFound)
            {
                context.Agents.OnPostLoadData(jsonObj["agent"].Value<JObject>(), context);
            }
        }
        else
        {
            Debug.Log("save file not found");
        }
    }
}
