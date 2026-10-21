Shader "Unlit/Triplanar"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tiling ("Tiling", Float) = 1.0
        _OcclusionMap("Occlusion", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        // ---------- Основной проход: directional + ambient + тени ----------
        Pass
        {
            Tags { "LightMode"="ForwardBase" }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            #include "AutoLight.cginc"

            struct v2f
            {
                half3 objNormal : TEXCOORD0;
                float3 coords : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float4 pos : SV_POSITION;
                fixed4 diff : COLOR0;
                SHADOW_COORDS(3)
            };

            float _Tiling;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.coords = worldPos * _Tiling;
                o.objNormal = v.normal;
                o.uv = v.texcoord;

                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                fixed4 diff = nl * _LightColor0;
                diff.rgb += ShadeSH9(half4(worldNormal, 1));
                o.diff = diff;

                TRANSFER_SHADOW(o);
                return o;
            }

            sampler2D _MainTex;
            sampler2D _OcclusionMap;

            fixed4 frag (v2f i) : SV_Target
            {
                half3 blend = abs(i.objNormal);
                blend /= dot(blend, 1.0);
                fixed4 cx = tex2D(_MainTex, i.coords.yz);
                fixed4 cy = tex2D(_MainTex, i.coords.xz);
                fixed4 cz = tex2D(_MainTex, i.coords.xy);
                fixed4 c = cx * blend.x + cy * blend.y + cz * blend.z;
                c *= tex2D(_OcclusionMap, i.uv);
                fixed shadow = SHADOW_ATTENUATION(i);
                c.rgb *= i.diff.rgb * shadow;
                return c;
            }
            ENDCG
        }

        // ---------- Дополнительный проход: Point / Spot lights ----------
        Pass
        {
            Tags { "LightMode"="ForwardAdd" }
            Blend One One
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd_fullshadows
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            #include "AutoLight.cginc"

            struct v2f
            {
                half3 objNormal : TEXCOORD0;
                float3 coords : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD3;
                SHADOW_COORDS(4)
            };

            float _Tiling;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldPos = worldPos;
                o.coords = worldPos * _Tiling;
                o.objNormal = v.normal;
                o.uv = v.texcoord;

                TRANSFER_SHADOW(o);
                return o;
            }

            sampler2D _MainTex;
            sampler2D _OcclusionMap;

            fixed4 frag (v2f i) : SV_Target
            {
                half3 blend = abs(i.objNormal);
                blend /= dot(blend, 1.0);

                fixed4 cx = tex2D(_MainTex, i.coords.yz);
                fixed4 cy = tex2D(_MainTex, i.coords.xz);
                fixed4 cz = tex2D(_MainTex, i.coords.xy);
                fixed4 c = cx * blend.x + cy * blend.y + cz * blend.z;
                c *= tex2D(_OcclusionMap, i.uv);

                half3 lightDir = normalize(_WorldSpaceLightPos0.xyz - i.worldPos);

                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);

                half nl = max(0, dot(normalize(i.objNormal), lightDir));
                c.rgb *= nl * _LightColor0.rgb * atten;

                return c;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}