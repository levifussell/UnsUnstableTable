Shader "Unlit/Ring"
{
    Properties
    {
        _MainTex        ("Texture", 2D) = "white" {}
        _Color          ("Color", Color) = (1,0,0,1)

        _RingRadius     ("RingRadius", Range(0.0, 0.5)) = 0.5
        _RingWidth      ("RingWidth", Range(0.0, 1.0)) = 0.1
        _RingEdgeSmooth ("RingEdgeSmooth", Range(0.0, 0.1)) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 vertexLocal : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _Color;

            float _RingRadius;
            float _RingWidth;
            float _RingEdgeSmooth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertexLocal = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                // build ring.
				float vRadius = i.vertexLocal.x * i.vertexLocal.x + i.vertexLocal.y * i.vertexLocal.y;
                float ringLower = pow(_RingRadius - 0.5 * _RingWidth, 2);
                float ringHigher = pow(_RingRadius + 0.5 * _RingWidth, 2);
                float ringMask = smoothstep(ringLower - _RingEdgeSmooth, ringLower, vRadius) * (1.0 - smoothstep(ringHigher, ringHigher + _RingEdgeSmooth, vRadius));

                col = ringMask * _Color;

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
