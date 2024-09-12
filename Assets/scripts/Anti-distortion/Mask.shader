// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Mask"
{
    Properties
    {
//        _Thick("ThicknessBuffer weights", float) = 0
//        _Depth("DepthBuffer weights", float) = 0
//        _Bright("Brightness", float) = 0
//        _Color("ColorBuffer weights", float) = 0
//        _Alpha("AlphaBuffer weights", float) = 0
        _MainTex ("Texture", 2D) = "white" {}
        
        _ScreenSize("Screen Size", float) = 1.0
        _EyeOffsetX("Eye Offset X", float) = 0.0
        _EyeOffsetY("Eye Offset Y", float) = 0.0
        _ScreenOffsetX("Screen Offset X", float) = 0.0
        _ScreenOffsetY("Screen Offset Y", float) = 0.0

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Func.cginc"
            // #pragma multi_compile_fog
            // #pragma multi_compile_fwdbase
            // #pragma shader_feature 
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 viewVec : TEXCOORD01;
            };

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            sampler2D colorBuffer;
            sampler2D depthBuffer;
            sampler2D thicknessBuffer;
            sampler2D _GlobalTransparencyTexture;
            sampler2D _CurveTex;

            UNITY_DECLARE_SHADOWMAP(_ShadowMapTexture);
            
            float _Thick;
            float _Depth;
            float _Bright;
            float _Color;
            float _Alpha;
            float _Shadow;
            float _Transparency;
            
            float4 _MainTex_ST;
            float2 _MainTex_TexelSize;
            float _ScreenSize;
            float _EyeOffsetX;
            float _EyeOffsetY;
            float _ScreenOffsetX;
            float _ScreenOffsetY;
            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                float3 ndcPos = float3(o.uv.xy * 2.0 - 1.0, 1);
				float far = _ProjectionParams.z;
				float3 clipVec = float3(ndcPos.x, ndcPos.y, ndcPos.z * -1) * far;
				o.viewVec = mul(unity_CameraInvProjection, clipVec.xyzz).xyz;

                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 flippedUVs = i.uv;
                // flippedUVs.y = i.uv.y - _EyeOffsetY/_ScreenSize ;
                // flippedUVs.x = _EyeOffsetX/_ScreenSize - i.uv.x;

                float thick = tex2D(thicknessBuffer, flippedUVs).r * _Bright;
                float depth = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,i.uv));
                float3 viewPos = i.viewVec * depth;
				float3 worldPos = mul(unity_CameraToWorld, float4(viewPos,1)).xyz;

                float color = tex2D(_MainTex, flippedUVs).r + tex2D(_MainTex, flippedUVs).b + tex2D(_MainTex, flippedUVs).g;
                float alpha = tex2D(_MainTex, flippedUVs).a;

                // float transparency = 1-tex2D(_CurveTex, float2(tex2D(_GlobalTransparencyTexture, i.uv).r, 0)).r;
                float transparency = tex2D(_CurveTex, float2(tex2D(_GlobalTransparencyTexture, i.uv).r, 0)).r;

                // float shadow = 0;
                float4 shadowCoord = mul(unity_WorldToShadow[0], float4(worldPos, 1));
                float shadow = UNITY_SAMPLE_SHADOW(_ShadowMapTexture, shadowCoord);
                // float shadow = tex2Dproj(_ShadowMapTexture, shadowCoord).r;
                if (depth > 0.99) shadow = 1;
                
                // float shadow = tex2D(_ShadowMap,flippedUVs);
                // float shadow = tex2D(_ShadowMapTexture,flippedUVs);
                // if(alpha>0.9 && color<0.1) shadow = 1;
                // if(alpha>0.9 && color<0.1) shadow = 1;

                float mix = thick * _Thick + depth * _Depth + color * _Color + alpha * _Alpha
                                    + transparency * _Transparency + shadow * _Shadow;;
                // mix = 1-mix;
                return fixed4(mix, mix, mix, 1);
            }
            
            
            ENDCG
        }
    }
}
