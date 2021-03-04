using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class LaunchFromStart : Editor {

 [MenuItem("Edit/Play-Stop, But From Prelaunch Scene %`")]
 	public static void PlayFromPrelaunchScene()
    {
    	if ( EditorApplication.isPlaying == true )
    	{
        	EditorApplication.isPlaying = false;
        	return;
        }
     
		EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
		EditorSceneManager.OpenScene("Assets/Scenes/Title/TitleScene.unity");
    	EditorApplication.isPlaying = true;
     }
}
