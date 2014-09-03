using UnityEngine;
using System.Collections;

public class TrackEditorObjectCatalogueLists : MonoBehaviour {

	/* Decorations */
	public static GameObject[] TrackEditorObjectDecorationPrefab;
	public static Texture2D[] TrackEditorObjectDecorationTexture;
	public static string[] TrackEditorObjectDecorationName;
	public static string[] TrackEditorObjectDecorationDesc;

	public static void LoadObjects()
	{
		/* Increase these if needed! */
		System.Array.Resize(ref TrackEditorObjectDecorationPrefab, 1);
		System.Array.Resize(ref TrackEditorObjectDecorationTexture, 1);
		System.Array.Resize(ref TrackEditorObjectDecorationName, 1);
		System.Array.Resize(ref TrackEditorObjectDecorationDesc, 1);


		/* Load Decorations */
		LoadDecorations();
	}

	public static void LoadDecorations()
	{
		int currentIndex = 0;

		/* Decoration 01 */
		TrackEditorObjectDecorationPrefab[currentIndex] = Resources.Load("TrackEditorPrefabs/City/Decorative/CityDeco01/Prefab_CityDeco01") as GameObject;
		TrackEditorObjectDecorationTexture[currentIndex] = Resources.Load("TrackEditorPrefabs/City/Decorative/CityDeco01/CatalogueImage") as Texture2D;
		TrackEditorObjectDecorationName[currentIndex] = "Sky Spiral 2.5";
		TrackEditorObjectDecorationDesc[currentIndex] = "Now with more spirals!";

		currentIndex ++;
	}
}
