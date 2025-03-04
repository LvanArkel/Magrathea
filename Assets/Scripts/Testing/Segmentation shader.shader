Shader "Unlit/Segmentation shader"
{
    Properties
    {
        _SegmentedColor ("Segmented color", Color) = (0,0,0,1)
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader 
    {
        Tags {"RenderType"="Opaque"}
        LOD 100
        Pass
        {
            CGPROGRAM
            #pragma vertex segmentVert
            #pragma fragment frag

            #include "Segmentation.hlsl"
            
            float4 frag(v2f i): SV_TARGET
            {
                return segmentFrag(i, false);
            }
            ENDCG
        }
    }

    SubShader 
    {
        Tags {"RenderType"="TreeBark"}
        LOD 100
        Pass
        {
            CGPROGRAM
            #pragma vertex segmentVert
            #pragma fragment frag

            #include "Segmentation.hlsl"

            float4 frag(v2f i): SV_TARGET
            {
                return segmentFrag(i, false);
            }
            ENDCG
        }
    }
    
    SubShader
    {
        Tags { "RenderType" = "TreeLeaf" "IgnoreProjector"="True" "Queue"="Transparent" }
        ZWrite Off
        //Blend SrcAlpha OneMinusSrcAlpha
        //Blend SrcAlpha Zero
        AlphaToMask On
        //BlendOp Max
        LOD 100
        Pass
        {
            CGPROGRAM
            #pragma vertex segmentVert
            #pragma fragment frag

            #include "Segmentation.hlsl"

            float4 frag(v2f i): SV_TARGET 
            {
                return segmentFrag(i, true);
            }

            ENDCG
        }
    }
}
