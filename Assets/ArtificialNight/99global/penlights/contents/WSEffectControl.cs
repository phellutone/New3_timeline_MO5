using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class WSEffectControl : MonoBehaviour
{
    [SerializeField]
    private string _serverAddress;

    private List<ANClientFx> _effects = new List<ANClientFx>();

    private WebSocket _webSocket = null;

    private List<Action> _actions = new List<Action>();

    private void Awake()
    {
        try
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                _effects.Add(GetClient(transform.GetChild(i)));
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private void Start()
    {
        _webSocket = new WebSocket(_serverAddress);
        Debug.Log(_webSocket);

        _webSocket.OnMessage += (sender, args) =>
        {
            try
            {
                var data = JsonUtility.FromJson<ANDataFormat>(args.Data);
                Debug.Log(args.Data);
                var client = _effects[data.id];
                if(data.state == 1)
                    _actions.Add(() => client.fx1.Play());
                else if(data.state == 2)
                    _actions.Add(() => client.fx2.Play());
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        };

        _webSocket.OnError += (sender, args) =>
        {
            //Debug.Log(args.Message);
        };

        _webSocket.Connect();
    }

    private ANClientFx GetClient(Transform o)
    {
        var client = new ANClientFx();
        client.fx1 = o.Find("fx1").GetComponent<ParticleSystem>();
        client.fx2 = o.Find("fx2").GetComponent<ParticleSystem>();

        var ps1 = client.fx1.main;
        var ps2 = client.fx2.main;

        ps1.playOnAwake = false;
        ps2.playOnAwake = false;
        
        return client;
    }

    private void Update()
    {
        if(_actions.Count != 0)
        {
            foreach (var action in _actions)
                action();
            _actions.Clear();
        }
    }

    private void OnApplicationQuit()
    {
        _webSocket.Close();
    }
}

class ANDataFormat
{
    public int id;
    public int color;
    public int state;
}

class ANClientFx
{
    public ParticleSystem fx1;
    public ParticleSystem fx2;
}
