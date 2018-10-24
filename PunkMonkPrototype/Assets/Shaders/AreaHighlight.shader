// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32719,y:32712,varname:node_3138,prsc:2|emission-7241-RGB,alpha-9987-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32325,y:32666,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.07843138,c2:0.3921569,c3:0.7843137,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:6977,x:31644,y:33340,ptovrint:False,ptlb:MaxAlpha,ptin:_MaxAlpha,varname:node_6977,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.8;n:type:ShaderForge.SFN_Tex2d,id:4495,x:31844,y:32964,ptovrint:False,ptlb:AlphaTex,ptin:_AlphaTex,varname:node_4495,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:0000000000000000f000000000000000,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Sin,id:8801,x:31644,y:33008,varname:node_8801,prsc:2|IN-6375-OUT;n:type:ShaderForge.SFN_Time,id:9412,x:31310,y:32958,varname:node_9412,prsc:2;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:516,x:31844,y:33128,varname:node_516,prsc:2|IN-8801-OUT,IMIN-8594-OUT,IMAX-656-OUT,OMIN-8095-OUT,OMAX-6977-OUT;n:type:ShaderForge.SFN_ValueProperty,id:8095,x:31644,y:33263,ptovrint:False,ptlb:MinAlpha,ptin:_MinAlpha,varname:_MaxAlpha_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.2;n:type:ShaderForge.SFN_Vector1,id:8594,x:31644,y:33140,varname:node_8594,prsc:2,v1:-1;n:type:ShaderForge.SFN_Vector1,id:656,x:31644,y:33191,varname:node_656,prsc:2,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:306,x:31310,y:33099,ptovrint:False,ptlb:SpeedMultiplier,ptin:_SpeedMultiplier,varname:node_306,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:6;n:type:ShaderForge.SFN_Multiply,id:6375,x:31484,y:33008,varname:node_6375,prsc:2|A-9412-T,B-306-OUT;n:type:ShaderForge.SFN_Multiply,id:8722,x:32102,y:33030,varname:node_8722,prsc:2|A-4495-R,B-516-OUT;n:type:ShaderForge.SFN_OneMinus,id:206,x:32281,y:33014,varname:node_206,prsc:2|IN-8722-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2360,x:32281,y:32943,ptovrint:False,ptlb:GlobalAlpha,ptin:_GlobalAlpha,varname:node_2360,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:9987,x:32489,y:32975,varname:node_9987,prsc:2|A-2360-OUT,B-206-OUT;proporder:7241-4495-6977-8095-306-2360;pass:END;sub:END;*/

Shader "Shader Forge/AreaHighlight" {
    Properties {
        _Color ("Color", Color) = (0.07843138,0.3921569,0.7843137,1)
        _AlphaTex ("AlphaTex", 2D) = "white" {}
        _MaxAlpha ("MaxAlpha", Float ) = 0.8
        _MinAlpha ("MinAlpha", Float ) = 0.2
        _SpeedMultiplier ("SpeedMultiplier", Float ) = 6
        _GlobalAlpha ("GlobalAlpha", Float ) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform float _MaxAlpha;
            uniform sampler2D _AlphaTex; uniform float4 _AlphaTex_ST;
            uniform float _MinAlpha;
            uniform float _SpeedMultiplier;
            uniform float _GlobalAlpha;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float3 emissive = _Color.rgb;
                float3 finalColor = emissive;
                float4 _AlphaTex_var = tex2D(_AlphaTex,TRANSFORM_TEX(i.uv0, _AlphaTex));
                float4 node_9412 = _Time;
                float node_8594 = (-1.0);
                return fixed4(finalColor,(_GlobalAlpha*(1.0 - (_AlphaTex_var.r*(_MinAlpha + ( (sin((node_9412.g*_SpeedMultiplier)) - node_8594) * (_MaxAlpha - _MinAlpha) ) / (1.0 - node_8594))))));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
