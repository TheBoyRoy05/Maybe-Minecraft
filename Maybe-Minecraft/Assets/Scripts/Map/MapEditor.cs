using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor {

    MapGenerator map;
    Editor mapEditor;

	public override void OnInspectorGUI(){
        using (var check = new EditorGUI.ChangeCheckScope()){
            base.OnInspectorGUI();
            if (check.changed){
                map.GenerateMap();
            }
        }

        if (GUILayout.Button("Generate Map")){
            map.GenerateMap();
        }

        DrawSettingsEditor(map.settings, map.OnMapSettingsUpdated, ref map.settingsFoldout, ref mapEditor);
	}

    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor){
        if (settings != null){
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
            using (var check = new EditorGUI.ChangeCheckScope()){
                if (foldout){
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();
                    if (check.changed){
                        if (onSettingsUpdated != null){
                            onSettingsUpdated();
                        }
                    }
                }
            }
        }
    }

	private void OnEnable(){
        map = (MapGenerator)target;
	}
}