//
// Copyright (c) 2014 Alexander Volodkovich
//

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Access Unity objects by string key
/// </summary>
public class LinksList : ScriptableObject
{
    public string[] Keys;
    public Object[] Prefabs;

    public Dictionary<string, Object> Links = new Dictionary<string, Object>();

    public Object Get(string key)
    {
        return Links.ContainsKey(key) ? Links[key] : null;
    }

    public void Init()
    {
        Links.Clear();

        for (var i = 0; i < Keys.Length; i++)
        {
            if (!Links.ContainsKey(Keys[i]))
            {
                Links.Add(Keys[i], Prefabs[i]);
            }
            else
            {
                Debug.LogWarning(string.Format("[LinkManager] Duplicate key '{0}' ignored", Keys[i]));
            }
        }
    }

    void OnEnable()
    {
        //Debug.LogError("OnEnable " + name);
        Init();
    }

    void OnDisable()
    {
        //Debug.LogError("OnDisable " + name);
    }

    void OnDestroy()
    {
        //Debug.LogError("OnDestroy " + name);
        Links = null;
    }
}
