//https://catlikecoding.com/unity/tutorials/scriptable-render-pipeline/custom-shaders/
#ifndef SRP_FIRSTSH_INCLUDE
#define SRP_FIRSTSH_INCLUDE

//假如Instance开启的话,会把M矩阵数组放在常量缓冲区,这个是在UnityInstancing中实现的,
//我们这样做的话,在Instance无效的情况下也能得到正确结果
#define UNITY_MATRIX_M unity_ObjectToWorld

#include "CoreRP/ShaderLibrary/Common.hlsl"
#include "CoreRP/ShaderLibrary/UnityInstancing.hlsl"

//不一定所有平台都支持Constant Buffers,所以使用CBUFFER_START宏代替cbuffer关键字
CBUFFER_START(UnityPerFrame) //放在每帧的缓冲区
    float4x4 unity_MatrixVP;
CBUFFER_END


CBUFFER_START(UnityPerDraw) //一次Draw缓冲区
    float4x4 unity_ObjectToWorld;
CBUFFER_END

UNITY_INSTANCING_BUFFER_START(PreInstance) //恒定缓冲区,只有切换材质的时候会更改
    UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
UNITY_INSTANCING_BUFFER_END(PreInstance)


struct VertexInput {
    float4 pos : POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct VertexOutput {
    float4 clipPos : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

VertexOutput UnlitPassVertex (VertexInput input) {
    VertexOutput output;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    float4 worldPos = mul(UNITY_MATRIX_M, float4(input.pos.xyz, 1.0));
    output.clipPos = mul(unity_MatrixVP, worldPos);
    return output;
}

float4 UnlitPassFragment (VertexOutput input) : SV_TARGET {
    UNITY_SETUP_INSTANCE_ID(input);
    return UNITY_ACCESS_INSTANCED_PROP(PreInstance, _Color);
    return 1;
}


#endif