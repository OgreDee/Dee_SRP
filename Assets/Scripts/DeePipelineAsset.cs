using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;


[CreateAssetMenu(menuName = "Rendering/Dee Pipline")]
public class DeePipelineAsset : RenderPipelineAsset
{
    [SerializeField]
    bool dynamicBatching;
    [SerializeField]
    bool instancing;

    protected override IRenderPipeline InternalCreatePipeline()
    {
        return new DeePipeline(dynamicBatching, instancing);
    }
}
