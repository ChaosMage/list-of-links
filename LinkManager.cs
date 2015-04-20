//
// Copyright (c) 2014 Alexander Volodkovich
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Global String -> GameObject links
/// Unity3D Resources replacement
/// </summary>
public class LinkManager : MonoBehaviour
{
    private static readonly HashSet<LinksList> AllLists = new HashSet<LinksList>(); // TODO LinksList instead?

    // TODO named Link managers?

    public LinksList[] Refs;

	void Awake ()
	{
	    foreach (var list in Refs.Where(list => list != null))
	    {
	        AllLists.Add(list);
            //Debug.LogError("+LIST");
	    }
        //Dump();
	}

    public static void Dump(string msg = "")
    {
        var s = new StringBuilder();
        //s.AppendFormat("{1}\nList 1 count: {0}\n", AllLists.First().Keys.Length, msg);
        foreach (var list in AllLists)
        {
            for (int i = 0; i < list.Keys.Length; i++)
            {
                s.AppendFormat("* '{0}' is '{1}'\n", list.Keys[i], list.Prefabs[i]);
            }
        }
        Debug.LogError(s.ToString());
        //Immortal.Warning(s.ToString());
    }

    void OnDestroy()
    {
        foreach (var list in Refs.Where(list => list != null))
        {
            AllLists.Remove(list);
        }

        Debug.Log(string.Format("[LinkManager] '{0}' removed", name));
    }

    public static Object Get(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("[LinkManager] empty key");
            return null;
        }

        var res = AllLists.Select(lst => lst.Get(key)).FirstOrDefault(r => r != null);
        if (!res)
        {
            Debug.LogError(string.Format("[LinkManager] Nothing linked as '{0}' ({1} list(s))", key, AllLists.Count));
            //Debug.LogError(string.Format("[LinkManager] First list item count: {0}", AllLists.First().Keys.Length));
            Dump(key);
        }
        return res;
    }

    /// <summary>
    /// Get prefab!
    /// </summary>
    /*
    public static GameObject GetPrefab(string key)
    {
        var prefab = Get(key) as GameObject;
        return prefab ?? ResCache.GetPrefab(key);
    }
    */

    public static Texture GetTex(string key)
    {
        return Get(key) as Texture;
    }

    public static GameObject GetInstance(string key)
    {
        return Instantiate(Get(key)) as GameObject;
    }

    public static GameObject GetInstance(string key, Vector3 pos, Quaternion q)
    {
        return Instantiate(Get(key), pos, q) as GameObject;
    }
}
