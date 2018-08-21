// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1280277,fgcg:0.1953466,fgcb:0.2352941,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33203,y:32718,varname:node_3138,prsc:2|emission-6104-OUT,alpha-211-OUT,clip-9607-OUT;n:type:ShaderForge.SFN_Tex2d,id:321,x:31508,y:32913,ptovrint:False,ptlb:Grid,ptin:_Grid,varname:node_321,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:2573,x:31742,y:32989,varname:node_2573,prsc:2|A-321-R,B-321-G,C-321-B;n:type:ShaderForge.SFN_If,id:9607,x:31997,y:33145,varname:node_9607,prsc:2|A-2573-OUT,B-3829-OUT,GT-6436-OUT,EQ-4453-OUT,LT-4453-OUT;n:type:ShaderForge.SFN_Vector1,id:3829,x:31554,y:33297,varname:node_3829,prsc:2,v1:2.5;n:type:ShaderForge.SFN_Vector1,id:6436,x:31700,y:33414,varname:node_6436,prsc:2,v1:0;n:type:ShaderForge.SFN_Distance,id:4453,x:31742,y:33145,varname:node_4453,prsc:2|A-2573-OUT,B-3829-OUT;n:type:ShaderForge.SFN_Blend,id:6104,x:32539,y:32694,varname:node_6104,prsc:2,blmd:10,clmp:True|SRC-55-OUT,DST-321-RGB;n:type:ShaderForge.SFN_Blend,id:55,x:32315,y:32399,varname:node_55,prsc:2,blmd:10,clmp:True|SRC-5978-OUT,DST-2452-RGB;n:type:ShaderForge.SFN_Tex2d,id:2452,x:31883,y:32494,ptovrint:False,ptlb:Line,ptin:_Line,varname:node_2452,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-6557-OUT,MIP-9202-OUT;n:type:ShaderForge.SFN_Noise,id:9202,x:31492,y:32557,varname:node_9202,prsc:2|XY-6557-OUT;n:type:ShaderForge.SFN_Add,id:6557,x:31195,y:32491,varname:node_6557,prsc:2|A-6739-UVOUT,B-3466-OUT;n:type:ShaderForge.SFN_TexCoord,id:6739,x:30907,y:32386,varname:node_6739,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Slider,id:4582,x:30425,y:32552,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_4582,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1014898,max:0.2;n:type:ShaderForge.SFN_Multiply,id:3466,x:30980,y:32606,varname:node_3466,prsc:2|A-4582-OUT,B-1102-T;n:type:ShaderForge.SFN_Time,id:1102,x:30503,y:32671,varname:node_1102,prsc:2;n:type:ShaderForge.SFN_Color,id:9918,x:32488,y:31650,ptovrint:False,ptlb:Colour,ptin:_Colour,varname:_Colour_copy_copy_copy_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.1,c3:0,c4:1;n:type:ShaderForge.SFN_Lerp,id:5978,x:32793,y:31821,varname:node_5978,prsc:2|A-9918-RGB,B-6594-RGB,T-7307-OUT;n:type:ShaderForge.SFN_Color,id:6594,x:32502,y:31924,ptovrint:False,ptlb:ColourTwo,ptin:_ColourTwo,varname:_ColourTwo_copy_copy_copy_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_FragmentPosition,id:5379,x:31384,y:31544,varname:node_5379,prsc:2;n:type:ShaderForge.SFN_Distance,id:9227,x:31708,y:31646,varname:node_9227,prsc:2|A-5379-XYZ,B-9851-XYZ;n:type:ShaderForge.SFN_Divide,id:8513,x:31948,y:31646,varname:node_8513,prsc:2|A-9227-OUT,B-4449-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4449,x:31698,y:31800,ptovrint:False,ptlb:transitionDistance,ptin:_transitionDistance,varname:_transitionDistance_copy_copy_copy_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4.5;n:type:ShaderForge.SFN_Power,id:9743,x:32272,y:31907,varname:node_9743,prsc:2|VAL-8513-OUT,EXP-2091-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2091,x:32094,y:32136,ptovrint:False,ptlb:Falloff,ptin:_Falloff,varname:_Falloff_copy_copy_copy_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:500;n:type:ShaderForge.SFN_Clamp01,id:7307,x:32529,y:32121,varname:node_7307,prsc:2|IN-9743-OUT;n:type:ShaderForge.SFN_Vector4Property,id:9851,x:31384,y:31704,ptovrint:False,ptlb:PlayerPos,ptin:_PlayerPos,varname:_PlayerPos_copy_copy_copy_copy_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_ValueProperty,id:211,x:32749,y:32964,ptovrint:False,ptlb:Alpha,ptin:_Alpha,varname:node_211,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;proporder:321-2452-4582-9918-6594-4449-2091-9851-211;pass:END;sub:END;*/

Shader "Shader Forge/Grid" {
    Properties {
        _Grid ("Grid", 2D) = "white" {}
        _Line ("Line", 2D) = "white" {}
        _Speed ("Speed", Range(0, 0.2)) = 0.1014898
        _Colour ("Colour", Color) = (0,0.1,0,1)
        _ColourTwo ("ColourTwo", Color) = (0.5,0.5,0.5,1)
        _transitionDistance ("transitionDistance", Float ) = 4.5
        _Falloff ("Falloff", Float ) = 500
        _PlayerPos ("PlayerPos", Vector) = (0,0,0,0)
        _Alpha ("Alpha", Float ) = 0
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
            uniform sampler2D _Grid; uniform float4 _Grid_ST;
            uniform sampler2D _Line; uniform float4 _Line_ST;
            uniform float _Speed;
            uniform float4 _Colour;
            uniform float4 _ColourTwo;
            uniform float _transitionDistance;
            uniform float _Falloff;
            uniform float4 _PlayerPos;
            uniform float _Alpha;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 _Grid_var = tex2D(_Grid,TRANSFORM_TEX(i.uv0, _Grid));
                float node_2573 = (_Grid_var.r+_Grid_var.g+_Grid_var.b);
                float node_3829 = 2.5;
                float node_9607_if_leA = step(node_2573,node_3829);
                float node_9607_if_leB = step(node_3829,node_2573);
                float node_4453 = distance(node_2573,node_3829);
                clip(lerp((node_9607_if_leA*node_4453)+(node_9607_if_leB*0.0),node_4453,node_9607_if_leA*node_9607_if_leB) - 0.5);
////// Lighting:
////// Emissive:
                float4 node_1102 = _Time;
                float2 node_6557 = (i.uv0+(_Speed*node_1102.g));
                float2 node_9202_skew = node_6557 + 0.2127+node_6557.x*0.3713*node_6557.y;
                float2 node_9202_rnd = 4.789*sin(489.123*(node_9202_skew));
                float node_9202 = frac(node_9202_rnd.x*node_9202_rnd.y*(1+node_9202_skew.x));
                float4 _Line_var = tex2Dlod(_Line,float4(TRANSFORM_TEX(node_6557, _Line),0.0,node_9202));
                float3 emissive = saturate(( _Grid_var.rgb > 0.5 ? (1.0-(1.0-2.0*(_Grid_var.rgb-0.5))*(1.0-saturate(( _Line_var.rgb > 0.5 ? (1.0-(1.0-2.0*(_Line_var.rgb-0.5))*(1.0-lerp(_Colour.rgb,_ColourTwo.rgb,saturate(pow((distance(i.posWorld.rgb,_PlayerPos.rgb)/_transitionDistance),_Falloff))))) : (2.0*_Line_var.rgb*lerp(_Colour.rgb,_ColourTwo.rgb,saturate(pow((distance(i.posWorld.rgb,_PlayerPos.rgb)/_transitionDistance),_Falloff)))) )))) : (2.0*_Grid_var.rgb*saturate(( _Line_var.rgb > 0.5 ? (1.0-(1.0-2.0*(_Line_var.rgb-0.5))*(1.0-lerp(_Colour.rgb,_ColourTwo.rgb,saturate(pow((distance(i.posWorld.rgb,_PlayerPos.rgb)/_transitionDistance),_Falloff))))) : (2.0*_Line_var.rgb*lerp(_Colour.rgb,_ColourTwo.rgb,saturate(pow((distance(i.posWorld.rgb,_PlayerPos.rgb)/_transitionDistance),_Falloff)))) ))) ));
                float3 finalColor = emissive;
                return fixed4(finalColor,_Alpha);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _Grid; uniform float4 _Grid_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 _Grid_var = tex2D(_Grid,TRANSFORM_TEX(i.uv0, _Grid));
                float node_2573 = (_Grid_var.r+_Grid_var.g+_Grid_var.b);
                float node_3829 = 2.5;
                float node_9607_if_leA = step(node_2573,node_3829);
                float node_9607_if_leB = step(node_3829,node_2573);
                float node_4453 = distance(node_2573,node_3829);
                clip(lerp((node_9607_if_leA*node_4453)+(node_9607_if_leB*0.0),node_4453,node_9607_if_leA*node_9607_if_leB) - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
