using UnityEngine;
using System.Collections;

public class TrackEditorGrid : MonoBehaviour {

	Material lineMaterial;
	Texture2D cursor;
	public int placeHeight;

	void CreateLineMaterial()
	{
		
		if( !lineMaterial ) {
			lineMaterial = new Material( "Shader \"Lines/Colored Blended\" {" +
			                            "SubShader { Pass { " +
			                            " Blend SrcAlpha OneMinusSrcAlpha " +
			                            " ZWrite Off Cull Off Fog { Mode Off } " +
			                            " BindChannels {" +
			                            " Bind \"vertex\", vertex Bind \"color\", color }" +
			                            "} } }" );
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;}
	}
	void OnPostRender()
	{
		/* Crweate the Line Material */
		CreateLineMaterial();

		/* Set the Material */
		lineMaterial.SetPass(0);

		GL.Begin(GL.LINES);

		GL.Color(new Color(0.3f, 0.3f, 0.3f));
		for (int i = -3000; i < 3000; i+=92)
		{
			for (int j = -3000; j < 3000; j+=92)
			{
				GL.Vertex3(i, placeHeight, 0);
				GL.Vertex3(i,placeHeight, j);

				GL.Vertex3(0, placeHeight, i);
				GL.Vertex3(j,placeHeight, i);
			}
		}


		GL.End();
	}
}
