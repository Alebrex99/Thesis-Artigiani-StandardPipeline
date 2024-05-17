Shader "Unlit/StereoImg"
{
    Properties
    {
        [NoScaleOffset] _MainTex("Left Eye Texture", 2D) = "white" {}
    [NoScaleOffset] _MainTex2("Right Eye Texture", 2D) = "white" {}
    }
        SubShader
    {
        Pass
        {
            // indicate that our pass is the "base" pass in forward
            // rendering pipeline. It gets ambient and main directional
            // light data set up; light direction in _WorldSpaceLightPos0
            // and color in _LightColor0
            Tags {"LightMode" = "ForwardBase"}
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc" // for UnityObjectToWorldNormal
            #include "UnityLightingCommon.cginc" // for _LightColor0
 
            struct v2f
            {
                float2 uv : TEXCOORD0;
                //float2 uv2 : TEXCOORD0;
                fixed4 diff : COLOR0; // diffuse lighting color
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };
 
            v2f vert(appdata_base v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
 
                // get vertex normal in world space
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                // dot product between normal and light direction for
                // standard diffuse (Lambert) lighting
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                // factor in the light color
                o.diff = nl * _LightColor0;
                o.diff.rgb += ShadeSH9(half4(worldNormal, 1));
                return o;
            }
 
 
 
            sampler2D _MainTex;
            sampler2D _MainTex2;
 
            fixed4 frag(v2f i) : SV_Target
            {
 
 
 
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
 
                    //half4 tex;
                    //half3 c;
 
                    if (unity_StereoEyeIndex == 0) {
                        // Left Eye
                        fixed4 col = tex2D(_MainTex, i.uv);
                        //c = DecodeHDR(tex, _TexRight_HDR);
                        col *= i.diff;
                        return col;
                    }
         else {
                        // Right Eye
                        fixed4 col = tex2D(_MainTex2, i.uv);
                        //tex = half4(1,0,0,1);
                        //c = DecodeHDR(tex, _TexRight_HDR);
                        col *= i.diff;
                        return col;
                    }
 
 
 
                // sample texture
                //fixed4 col = tex2D(_MainTex2, i.uv);
            // multiply by lighting
 
            //  col *= i.diff;
            //return col;
        }
        ENDCG
    }
    }
}
