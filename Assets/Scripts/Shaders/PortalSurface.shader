Shader "Custom/PortalSurface"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MaskTex("Mask (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            #pragma target 3.0

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _Color;
            sampler2D _MaskTex;
            //int displayMask;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.screenPos.xy / i.screenPos.w;
                //fixed4 c = ;
                return tex2D(_MainTex, uv); //* text2D(_MaskTex, uv) * (1 - displayMask);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
