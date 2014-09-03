using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(EditorInputObject))]
public class EditorInputManager : Editor {

	public static Event AGREditorEvent;
	public static KeyCode AGREditorKeyCode;
	public static Ray AGREditorRay;

	private bool AGREditorKeyDown;

	public static GameObject AGREditorInputObject;

	public static void CreateInputManager()
	{
		/* Check/Create Tools Parent Object */
		if (!GameObject.Find("SceneTools"))
		{
			GameObject sceneTools = new GameObject("SceneTools");
		}

		/* Create Input Object */
		if (!GameObject.Find("EditorInput"))
		{
			AGREditorInputObject = new GameObject("EditorInput");
			AGREditorInputObject.transform.parent = GameObject.Find("SceneTools").transform;
			AGREditorInputObject.AddComponent<EditorInputObject>();
		}
	}

	public static void DestroyInputManager()
	{
		/* Remove EditorInput */
		Destroy(GameObject.Find("EditorInput"));
	}

	void OnSceneGUI()
	{
		/* Create the editor raycast ray from the scene view GUI */
		AGREditorRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

		/* Reset the current key that is being pressed */
		AGREditorKeyCode = KeyCode.None;
		if (!AGREditorKeyDown)
		{
			AGREditorKeyCode = KeyCode.None;
		}

		/* Key press event */
		if (Event.current.type == EventType.keyDown && !AGREditorKeyDown)
		{
			AGREditorKeyCode = Event.current.keyCode;
			AGREditorKeyDown = true;
		}

		/* Key release event */
		if (Event.current.type == EventType.keyUp)
		{
			AGREditorKeyDown = false;
		}

	}
}
