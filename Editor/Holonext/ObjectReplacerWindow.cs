﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class ObjectReplacerWindow : EditorWindow
{
    bool replaceAll = true;

    GameObject newObjectPrefab;
    public List<GameObject> OldObjects;
    float replacementPercentage = 100f;


    [MenuItem("Holonext/ObjectReplacer")]
    static void Init()
    {
        ObjectReplacerWindow window = (ObjectReplacerWindow)EditorWindow.GetWindow(typeof(ObjectReplacerWindow));
        window.minSize = new Vector2(335f, 200f);
        window.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.Space(); 
        if (!replaceAll)
        {
            replaceAll = GUILayout.Toggle(replaceAll, "Replace All", "button");

            EditorGUILayout.Space();
            replacementPercentage = EditorGUILayout.Slider("Replacement Percentage",replacementPercentage, 1, 100);
        }
        else
        {
            replaceAll = GUILayout.Toggle(replaceAll, "Replace By Percentage", "button");
            replacementPercentage = 100;
        }
        EditorGUILayout.Space();


        newObjectPrefab = (GameObject)EditorGUILayout.ObjectField("New Object Prefab",newObjectPrefab, typeof(GameObject), true);
        EditorGUILayout.Space();
        GUILayout.BeginHorizontal("box");
        CreateOldObjectField();
        EditorGUILayout.LabelField(OldObjects.Count.ToString(), GUILayout.MaxWidth(40));
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Clear List", GUILayout.MaxWidth(70)))
        {
            OldObjects.Clear();
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        if (GUILayout.Button("Replace"))
        {
            if(newObjectPrefab!=null)
            ReplaceObjects();
        }
    }

    void CreateOldObjectField()
    {
        ScriptableObject target = this;
        SerializedObject so = new SerializedObject(target);
        SerializedProperty oldObjects = so.FindProperty("OldObjects");

        EditorGUILayout.PropertyField(oldObjects, true);
        so.ApplyModifiedProperties();
    }

    void ReplaceObjects()
    {
        float nthElement = 100/ replacementPercentage;

        var replaced = 0;

        for (int i = 0; i < OldObjects.Count; i++)
        {
            var value = i % (int)nthElement ;
            Debug.Log((int)nthElement);

            Debug.Log(value);
            if (value != 0)
                continue;

            var oldObject = OldObjects[i];

            GameObject placedObject = (GameObject)PrefabUtility.InstantiatePrefab(newObjectPrefab,oldObject.transform.parent);

            placedObject.transform.position = oldObject.transform.position;
            placedObject.transform.rotation = oldObject.transform.rotation;
            replaced++;
           // Undo.RegisterCompleteObjectUndo(placedObject, "Replacement");
        }

        for (int i = 0; i < OldObjects.Count; i++)
        {
            if (i % (int)nthElement != 0)
                continue;

            var oldObject = OldObjects[i];
            DestroyImmediate(oldObject);
        }

        OldObjects.Clear();
        Debug.Log(replaced + " object is replaced");
    }

}
