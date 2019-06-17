using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;


[CreateAssetMenu(menuName = "Rendering/Dee Pipline")]
public class DeePipelineAsset : RenderPipelineAsset
{
    protected override IRenderPipeline InternalCreatePipeline()
    {
        return new DeePipeline();
    }
}
