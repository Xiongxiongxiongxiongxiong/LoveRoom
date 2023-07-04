using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jagat.DressAssetBundle;
using UnityEngine.Networking;

public class TestABLoad : MonoBehaviour
{
    private AssetBundleLoadCtrl m_abCtrl;

    // Start is called before the first frame update
    void Start()
    {
        //var url = @"C:\Users\a1063\AppData\LocalLow\UTown\Jagat\HotResDir\LoveRoom";
        var url = $"{System.Environment.CurrentDirectory}/AssetBundles/Output";
        if (!System.IO.Directory.Exists(url))
        {
            Debug.LogError("assetbundle not generate:" + url);
            return;
        }
        m_abCtrl = new AssetBundleLoadCtrl(DownloandFunc,DownloadTextFunc,Application.persistentDataPath);
        var operation = m_abCtrl.LoadCatlogAsync(url);
        operation.RegistComplete(OnLoadCatlogFinish);
        GameObject.DontDestroyOnLoad(gameObject);
    }

    private void OnLoadCatlogFinish(AsyncCatlogOperation op)
    {
        Debug.Log("init complate");
        //m_abCtrl.LoadSceneAsync("MainRoomScene");
        m_abCtrl.StartPreload(1);
    }

    private void OnGUI()
    {
        if (m_abCtrl == null)
            return;

        if(GUILayout.Button("Load MainRoomScene"))
        {
            m_abCtrl.LoadSceneAsync("MainRoomScene");
        }
        if (GUILayout.Button("Load LoveRoom_Final"))
        {
            m_abCtrl.LoadSceneAsync("LoveRoom_Final");
        }
        if (GUILayout.Button("Load merry_goround_final"))
        {
            m_abCtrl.LoadSceneAsync("merry_goround_final");
        }
    }

    private void DownloadTextFunc(string url, System.Action<string, object> onDownloadFinish, object content)
    {
        onDownloadFinish?.Invoke(System.IO.File.ReadAllText(url),content);
    }

    private void DownloandFunc(string url, string localPath, System.Action<string, object> onDownloadFinish, System.Action<float> onProgress, object content)
    {
        onDownloadFinish?.Invoke(url,content);
        System.IO.File.Copy(url, localPath,true);
    }
}
