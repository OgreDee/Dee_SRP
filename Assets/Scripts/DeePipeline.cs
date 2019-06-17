using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using Conditional = System.Diagnostics.ConditionalAttribute;

public class DeePipeline : RenderPipeline
{
    public override void Render(ScriptableRenderContext renderContext, Camera[] cameras)
    {
        base.Render(renderContext, cameras);

        foreach (var cam in cameras)
        {
            Render(renderContext, cam);
        }
    }

    CommandBuffer clearBuffer = new CommandBuffer { name = "Render Camera" };
    CullResults cull;
    void Render(ScriptableRenderContext renderContext, Camera camera)
    {
        //填充当前相机信息
        renderContext.SetupCameraProperties(camera);

        //设置渲染指令
        CameraClearFlags clearFlag = camera.clearFlags;
        clearBuffer.ClearRenderTarget(
            (clearFlag & CameraClearFlags.Depth) != 0,
            (clearFlag & CameraClearFlags.Color) != 0,
            camera.backgroundColor);

        renderContext.ExecuteCommandBuffer(clearBuffer);
        clearBuffer.Clear();

        //剔除
        ScriptableCullingParameters cullingParameters;
        if (!CullResults.GetCullingParameters(camera, out cullingParameters))
        {
            return;
        }
#if UNITY_EDITOR
        if (camera.cameraType == CameraType.SceneView)
        {
            ScriptableRenderContext.EmitWorldGeometryForSceneView(camera); //保证UI在SceneView显示
        }
#endif

        CullResults.Cull(ref cullingParameters, renderContext, ref cull);

        //绘制不透明物体
        DrawRendererSettings drawSetting = new DrawRendererSettings(camera, new ShaderPassName("SRPDefaultUnlit"));
        drawSetting.sorting.flags = SortFlags.CommonOpaque; //告诉渲染器从前向后渲染(减少overdraw)
        FilterRenderersSettings filterSetting = new FilterRenderersSettings(true) {
            renderQueueRange = RenderQueueRange.opaque //过滤出不透明shape
        };
        renderContext.DrawRenderers(cull.visibleRenderers, ref drawSetting, filterSetting);

        //绘制天空盒
        renderContext.DrawSkybox(camera);

        //绘制透明物体
        drawSetting.sorting.flags = SortFlags.CommonTransparent; //告诉渲染器从后向前渲染(做颜色叠加)
        filterSetting.renderQueueRange = RenderQueueRange.transparent; //过滤出透明shape
        renderContext.DrawRenderers(cull.visibleRenderers, ref drawSetting, filterSetting);

        DrawDefaultPipline(renderContext, camera);

        //执行缓冲区里面的命令
        renderContext.Submit();
    }

    Material errorMaterial;
    [Conditional("DEE_DEV")]
    void DrawDefaultPipline(ScriptableRenderContext context, Camera camera)
    {
        if(errorMaterial == null)
        {
            Shader errorSh = Shader.Find("Hidden/InternalErrorShader");
            errorMaterial = new Material(errorSh)
            { hideFlags = HideFlags.HideAndDontSave};
        }
        DrawRendererSettings drawSetting = new DrawRendererSettings(camera, new ShaderPassName("ForwardBase"));
        drawSetting.SetOverrideMaterial(errorMaterial, 0);
        FilterRenderersSettings filterSetting = new FilterRenderersSettings(true);
        context.DrawRenderers(cull.visibleRenderers, ref drawSetting, filterSetting);
    }
}
