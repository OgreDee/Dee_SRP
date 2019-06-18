using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class InstancedColor : MonoBehaviour {

    [SerializeField]
    Color color;

    static MaterialPropertyBlock s_materialPropertyBlock;
    static int s_colorShaderID = Shader.PropertyToID("_Color");
	// Use this for initialization
	void Start () {
        OnValidate();
    }

    private void OnValidate()
    {
        if (s_materialPropertyBlock == null)
        {
            s_materialPropertyBlock = new MaterialPropertyBlock();
        }

        s_materialPropertyBlock.SetColor(s_colorShaderID, color);
        GetComponent<Renderer>().SetPropertyBlock(s_materialPropertyBlock);
    }
}
