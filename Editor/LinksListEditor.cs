//
// Copyright (c) 2014 Alexander Volodkovich
//

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(LinksList))]
public class LinksListEditor : Editor
{
    private string _prefix;

    private bool _tex2spr;

    public override void OnInspectorGUI()
    {
        var obj = target as LinksList;

        if (Application.isPlaying)
        {
            EditorGUILayout.LabelField("WARNING! READ ONLY MODE IN RUNTIME");
            return;
        }

        var objCount = obj != null && obj.Keys != null ? obj.Keys.Length : 0;

        var st = (GUIStyle) "WhiteLabel";
        st.richText = true;

        if (GUILayout.Button("Import selected"))
        {
            /*
            var guids = AssetDatabase.FindAssets("none");
	        foreach (var guid in guids)
	        {
	            var lo = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof (Object));
                Debug.LogError(string.Format("ASSET {0} ({1}) {2} [{3}]", lo.name, lo.GetType(), AssetDatabase.IsMainAsset(lo), guid));
            }
            */

            foreach (var sel in Selection.objects)
            {
                if (AssetDatabase.Contains(sel))
                {
                    if (_tex2spr && sel is Texture2D)
                    {
                        var reps = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(sel));
                        foreach (var rep in reps)
                        {
                            //Debug.LogError(string.Format("SELECTED SUB ASSET {0} ({1}) {2}", rep.name, rep.GetType(), AssetDatabase.IsSubAsset(rep)));
                            AddAsset(rep);
                        }    
                    }
                    else
                    {
                        //Debug.LogError(string.Format("SELECTED ASSET {0} ({1}) {2}", sel.name, sel.GetType(), AssetDatabase.IsSubAsset(sel)));    
                        AddAsset(sel);
                    }
                }
            }
        }

        _tex2spr = GUILayout.Toggle(_tex2spr, "Get SPRITE sub-assets from TEXTURE");

        // ---

        EditorGUILayout.LabelField(string.Format("List of <b>{0}</b> Objects", objCount), st);

        EditorGUILayout.Space();

        var lst = DropZone("Drag objects here", Screen.width - 45, 50);
        if (lst != null)
        {
            foreach (var o in lst)
            {
                if (AssetDatabase.Contains(o) && AssetDatabase.IsMainAsset(o))
                {
                    //Debug.LogError("ASSET> " + o.name);
                    AddAsset(o);
                }
            }
        }

        EditorGUILayout.Space();

        _prefix = EditorGUILayout.TextField("Key Prefix for new", _prefix);

        EditorGUILayout.Space();

        var tool = GUILayout.Toolbar(-1, new[] { "Add Empty", "Clear All" });

        EditorGUILayout.Space();

        EditorGUIUtility.labelWidth = 80;

        EditorGUILayout.BeginHorizontal();

        search = EditorGUILayout.TextField("", search, "ToolbarSeachTextField");

        var searchLow = search.ToLower();

        if (GUILayout.Button("", "ToolbarSeachCancelButton", GUILayout.Width(20)))
        {
            search = "";
            GUIUtility.keyboardControl = 0;
        }

        EditorGUILayout.EndHorizontal();

        // если со списком, то тут отображать только пустые ключи

        var del = -1;

        if (obj != null && obj.Keys != null && obj.Keys.Length > 0)
            for (var i = 0; i < obj.Keys.Length; i++)
            {
                if (!string.IsNullOrEmpty(search))
                {
                    if (!obj.Keys[i].ToLower().Contains(searchLow)) continue;
                }

                EditorGUILayout.Space();

                EditorGUI.indentLevel = 0;

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("X", EditorStyles.toolbarButton, GUILayout.Width(20)))
                {
                    del = i;
                }

                if (IsConflict(i))
                {
                    //EditorGUILayout.PrefixLabel("Key Conflict", EditorStyles.label, new GUIStyle { normal = new GUIStyleState { textColor = UnityColor.Orange } });
                    EditorGUILayout.PrefixLabel("Key Conflict", EditorStyles.label, "CN StatusWarn");
                }
                else
                {
                    EditorGUILayout.PrefixLabel("Key");
                }

                obj.Keys[i] = EditorGUILayout.TextField(obj.Keys[i]);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("C", EditorStyles.toolbarButton, GUILayout.Width(20)))
                {
                    obj.Prefabs[i] = null;
                }

                obj.Prefabs[i] = EditorGUILayout.ObjectField("Object", obj.Prefabs[i], typeof(Object), false);

                if (string.IsNullOrEmpty(obj.Keys[i]) && obj.Prefabs[i] != null) obj.Keys[i] = obj.Prefabs[i].name;

                EditorGUILayout.EndHorizontal();
            }

        if (del != -1)
        {
            DelElement(del);
            del = -1;
        }

        switch (tool)
        {
            case 0:
                AddElement();
                break;
            case 1:
                ClearAll();
                break;
        }

        if (GUI.changed)
            EditorUtility.SetDirty(obj);
    }

    private void AddAsset(Object o)
    {
        var obj = target as LinksList;
        if (obj.Keys == null)
        {
            obj.Keys = new string[0];
            obj.Prefabs = new Object[0];
        }
        ArrayUtility.Add(ref obj.Keys, _prefix + o.name);
        ArrayUtility.Add(ref obj.Prefabs, o);    
        EditorUtility.SetDirty(obj);
    }

    private void AddElement()
    {
        var obj = target as LinksList;
        ArrayUtility.Insert(ref obj.Prefabs, 0, null);
        ArrayUtility.Insert(ref obj.Keys, 0, null);
        EditorUtility.SetDirty(obj);
    }

    private void DelElement(int i)
    {
        var obj = target as LinksList;
        ArrayUtility.RemoveAt(ref obj.Prefabs, i);
        ArrayUtility.RemoveAt(ref obj.Keys, i);
        EditorUtility.SetDirty(obj);
    }

    private void ClearAll()
    {
        //var obj = target as LinksList;
        //obj.Links.Clear();
    }

    private bool IsConflict(int i)
    {
        var obj = target as LinksList;
        var key = obj.Keys[i];

        var c = obj.Keys.Count(x => x == key);

        return c > 1;
    }

    private int ndx = -1;

    private string search = "";

    public void OnInspectorUpdate()
    {
        this.Repaint();
    }

    public static Object[] DropZone(string title, int w, int h)
    {
        GUILayout.Box(title, "U2D.createRect", GUILayout.ExpandWidth(true), GUILayout.Height(h));

        var boxRect = GUILayoutUtility.GetLastRect();

        EventType eventType = Event.current.type;
        bool isAccepted = false;

        if (!boxRect.Contains(Event.current.mousePosition)) return null;

        if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (eventType == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                isAccepted = true;
            }
            Event.current.Use();
        }

        return isAccepted ? DragAndDrop.objectReferences : null;
    }

    [MenuItem("Assets/Create/List of Links")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<LinksList>();
    }
}
