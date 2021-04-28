using Newtonsoft.Json.Linq;

public interface IPersistent
{
    public JObject OnSaveData();
    public void OnLoadData(JObject jsonObj);
    public void OnPostLoadData(JObject jsonObj, Simulator.Context context);
}
