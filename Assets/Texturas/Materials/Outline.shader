Shader "Custom/OutlineGlow"
{
    Properties
    {
        [HDR] _OutlineColor ("Outline Color", Color) = (1, 0.9, 0.1, 1)
        _OutlineWidth ("Outline Width (world units)", Float) = 0.03
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+1" }

        Pass
        {
            Cull Front
            ZWrite On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _OutlineColor;
            float _OutlineWidth;

            struct appdata { float4 vertex : POSITION; float3 normal : NORMAL; };
            struct v2f { float4 pos : SV_POSITION; };

            v2f vert (appdata v)
            {
                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
                worldPos += worldNormal * _OutlineWidth;
                o.pos = UnityWorldToClipPos(worldPos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target { return _OutlineColor; }
            ENDCG
        }
    }
}