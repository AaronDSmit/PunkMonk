// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1280277,fgcg:0.1953466,fgcb:0.2352941,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:4013,x:32719,y:32712,varname:node_4013,prsc:2|diff-4528-OUT,emission-4528-OUT,alpha-526-OUT;n:type:ShaderForge.SFN_TexCoord,id:8850,x:31270,y:32590,varname:node_8850,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:4895,x:31476,y:32653,varname:node_4895,prsc:2,spu:0,spv:0|UVIN-8850-UVOUT;n:type:ShaderForge.SFN_Tex2dAsset,id:3735,x:31626,y:32495,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:node_4929,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:43e6b4cb50ec2b946b0b0c5b3e0f08c1,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:457,x:31943,y:32960,ptovrint:False,ptlb:Colour,ptin:_Colour,varname:node_8452,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.152141,c2:0.9852941,c3:0.9163435,c4:1;n:type:ShaderForge.SFN_Slider,id:9339,x:31886,y:33277,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_952,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Tex2d,id:945,x:31943,y:32751,varname:node_3936,prsc:2,tex:43e6b4cb50ec2b946b0b0c5b3e0f08c1,ntxv:0,isnm:False|UVIN-4422-OUT,TEX-3735-TEX;n:type:ShaderForge.SFN_Tex2d,id:3028,x:31837,y:32495,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_4414,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:b1d51a6ab97aacd40b537f9495340f14,ntxv:3,isnm:False;n:type:ShaderForge.SFN_ComponentMask,id:9979,x:32028,y:32495,varname:node_9979,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-3028-RGB;n:type:ShaderForge.SFN_Multiply,id:4528,x:32277,y:32662,varname:node_4528,prsc:2|A-945-R,B-457-RGB;n:type:ShaderForge.SFN_Multiply,id:526,x:32274,y:32934,varname:node_526,prsc:2|A-945-G,B-9339-OUT,C-9979-OUT;n:type:ShaderForge.SFN_Add,id:4422,x:31712,y:32751,varname:node_4422,prsc:2|A-4895-UVOUT,B-5981-OUT;n:type:ShaderForge.SFN_Slider,id:2072,x:30846,y:32925,ptovrint:False,ptlb:U_Speed,ptin:_U_Speed,varname:node_2072,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Slider,id:4752,x:30846,y:33077,ptovrint:False,ptlb:V_Speed,ptin:_V_Speed,varname:node_4752,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Append,id:4311,x:31209,y:32945,varname:node_4311,prsc:2|A-2072-OUT,B-4752-OUT;n:type:ShaderForge.SFN_Multiply,id:5981,x:31517,y:32943,varname:node_5981,prsc:2|A-4311-OUT,B-780-T;n:type:ShaderForge.SFN_Time,id:780,x:31287,y:33122,varname:node_780,prsc:2;proporder:3735-457-9339-3028-2072-4752;pass:END;sub:END;*/

Shader "Shader Forge/Scrolling_UI" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _Colour ("Colour", Color) = (0.152141,0.9852941,0.9163435,1)
        _Opacity ("Opacity", Range(0, 1)) = 1
        _Mask ("Mask", 2D) = "bump" {}
        _U_Speed ("U_Speed", Range(-1, 1)) = 0
        _V_Speed ("V_Speed", Range(-1, 1)) = 0
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
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _Colour;
            uniform float _Opacity;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _U_Speed;
            uniform float _V_Speed;
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
                float4 node_2618 = _Time;
                float4 node_780 = _Time;
                float2 node_4422 = ((i.uv0+node_2618.g*float2(0,0))+(float2(_U_Speed,_V_Speed)*node_780.g));
                float4 node_3936 = tex2D(_MainTex,TRANSFORM_TEX(node_4422, _MainTex));
                float3 node_4528 = (node_3936.r*_Colour.rgb);
                float3 emissive = node_4528;
                float3 finalColor = emissive;
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                return fixed4(finalColor,(node_3936.g*_Opacity*_Mask_var.rgb.r));
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
