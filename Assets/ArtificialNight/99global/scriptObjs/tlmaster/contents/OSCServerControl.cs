using System;
using UnityEngine;
using UnityEngine.Playables;
using uOSC;

public class OSCServerControl : MonoBehaviour
{
    public PlayableDirector director;
    public GameObject currentObj;

    public GameObject[] objects;

    public string _pfx;
    public int _code;
    void Start()
    {
        var server = GetComponent<uOscServer>();
        server.onDataReceived.AddListener(OnDataReceived);
    }

    void OnDataReceived(Message message)
    {
        _code = 0;
        _pfx = "";

        foreach (var value in message.values)
        {
            if (value is int) _code = (int)value;
            else if (value is float) _code = (int)value;
            else if (value is string) _pfx = (string)value;
        }


        if (_pfx == "load")
        {
            try
            {
                currentObj = objects[_code - 1];
                currentObj.SetActive(true);
                director = currentObj.GetComponentInChildren<PlayableDirector>();
                director.RebuildGraph();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
        else if (_pfx == "play")
        {
            director.Play(director.playableAsset);
        }
        else if (_pfx == "pause")
        {
            director.Pause();
        }
        else if (_pfx == "time")
        {
            director.time = _code;
        }
        else if (_pfx == "frame")
        {
            director.time = _code / 60;
        }
        else if (_pfx == "stop")
        {
            director.Stop();
            currentObj.SetActive(false);
        }

        Debug.Log($"pfx: {_pfx}, code: {_code}");
    }
}
