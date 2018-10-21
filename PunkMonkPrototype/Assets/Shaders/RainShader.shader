// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1280277,fgcg:0.1953466,fgcb:0.2352941,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:6821,x:33256,y:32711,varname:node_6821,prsc:2|diff-3484-RGB,spec-8047-OUT,gloss-8047-OUT,normal-6956-OUT;n:type:ShaderForge.SFN_Tex2d,id:3484,x:32379,y:32486,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:node_3484,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:b66bceaf0cc0ace4e9bdc92f14bba709,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:8047,x:32308,y:32717,ptovrint:False,ptlb:Wetness,ptin:_Wetness,varname:node_8047,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.7952354,max:1;n:type:ShaderForge.SFN_Vector4Property,id:4284,x:30955,y:33761,ptovrint:False,ptlb:Width Hight Speed,ptin:_WidthHightSpeed,varname:node_4284,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4,v2:4,v3:0.2,v4:0;n:type:ShaderForge.SFN_Multiply,id:5872,x:31348,y:34113,varname:node_5872,prsc:2|A-4284-Z,B-2969-T;n:type:ShaderForge.SFN_Time,id:2969,x:31169,y:34137,varname:node_2969,prsc:2;n:type:ShaderForge.SFN_Frac,id:3012,x:31542,y:34126,varname:node_3012,prsc:2|IN-5872-OUT;n:type:ShaderForge.SFN_Multiply,id:4253,x:31744,y:34074,varname:node_4253,prsc:2|A-4578-OUT,B-3012-OUT;n:type:ShaderForge.SFN_Round,id:4763,x:31932,y:34031,varname:node_4763,prsc:2|IN-4253-OUT;n:type:ShaderForge.SFN_Divide,id:7029,x:32337,y:33630,varname:node_7029,prsc:2|A-2367-OUT,B-4284-X;n:type:ShaderForge.SFN_Divide,id:284,x:32323,y:33906,varname:node_284,prsc:2|A-7736-OUT,B-4284-Y;n:type:ShaderForge.SFN_Fmod,id:2367,x:32337,y:33766,varname:node_2367,prsc:2|A-4763-OUT,B-4284-X;n:type:ShaderForge.SFN_Floor,id:7736,x:32323,y:34045,varname:node_7736,prsc:2|IN-6683-OUT;n:type:ShaderForge.SFN_Divide,id:6683,x:32131,y:34045,varname:node_6683,prsc:2|A-4763-OUT,B-4284-X;n:type:ShaderForge.SFN_OneMinus,id:4126,x:32548,y:33906,varname:node_4126,prsc:2|IN-284-OUT;n:type:ShaderForge.SFN_Append,id:5452,x:32536,y:33555,varname:node_5452,prsc:2|A-4284-X,B-4284-Y;n:type:ShaderForge.SFN_Append,id:2673,x:32634,y:33712,varname:node_2673,prsc:2|A-7029-OUT,B-4126-OUT;n:type:ShaderForge.SFN_TexCoord,id:8508,x:32657,y:33347,varname:node_8508,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Divide,id:8604,x:32827,y:33644,varname:node_8604,prsc:2|A-3550-OUT,B-5452-OUT;n:type:ShaderForge.SFN_Add,id:654,x:32849,y:33814,varname:node_654,prsc:2|A-8604-OUT,B-2673-OUT;n:type:ShaderForge.SFN_Tex2d,id:2389,x:33377,y:33443,ptovrint:False,ptlb:Rain Normal,ptin:_RainNormal,varname:node_2389,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:True,tagnrm:False,tex:aab176d225be0f940b0548435d57141c,ntxv:3,isnm:True|UVIN-4635-OUT;n:type:ShaderForge.SFN_Multiply,id:4578,x:31542,y:33962,varname:node_4578,prsc:2|A-4284-X,B-4284-Y;n:type:ShaderForge.SFN_Multiply,id:6956,x:32826,y:33083,varname:node_6956,prsc:2|A-3703-RGB,B-3273-OUT;n:type:ShaderForge.SFN_Tex2d,id:3703,x:32595,y:32972,ptovrint:False,ptlb:Normal Map,ptin:_NormalMap,varname:node_3703,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_TexCoord,id:912,x:33092,y:33924,varname:node_912,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:2412,x:33329,y:33816,varname:node_2412,prsc:2|A-4287-OUT,B-912-UVOUT;n:type:ShaderForge.SFN_Add,id:4635,x:33177,y:33651,varname:node_4635,prsc:2|A-654-OUT,B-2412-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4287,x:33083,y:33838,ptovrint:False,ptlb:Tiling,ptin:_Tiling,varname:node_4287,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:5;n:type:ShaderForge.SFN_Frac,id:567,x:32875,y:33320,varname:node_567,prsc:2|IN-8508-U;n:type:ShaderForge.SFN_Frac,id:8597,x:32852,y:33442,varname:node_8597,prsc:2|IN-8508-V;n:type:ShaderForge.SFN_Append,id:3550,x:33072,y:33396,varname:node_3550,prsc:2|A-567-OUT,B-8597-OUT;n:type:ShaderForge.SFN_ComponentMask,id:7996,x:33548,y:33376,varname:node_7996,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-2389-RGB;n:type:ShaderForge.SFN_Slider,id:4297,x:33317,y:33693,ptovrint:False,ptlb:Normal Strength,ptin:_NormalStrength,varname:node_4297,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:20,x:33737,y:33410,varname:node_20,prsc:2|A-7996-OUT,B-9630-OUT;n:type:ShaderForge.SFN_Vector1,id:5917,x:33988,y:33784,varname:node_5917,prsc:2,v1:1;n:type:ShaderForge.SFN_Append,id:3273,x:33994,y:33425,varname:node_3273,prsc:2|A-20-OUT,B-5917-OUT;n:type:ShaderForge.SFN_Append,id:9630,x:33684,y:33797,varname:node_9630,prsc:2|A-639-OUT,B-4297-OUT;n:type:ShaderForge.SFN_Vector1,id:639,x:33458,y:33605,varname:node_639,prsc:2,v1:0.1;proporder:3484-3703-2389-4297-8047-4284-4287;pass:END;sub:END;*/

Shader "Custom/RainShader" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "white" {}
        [NoScaleOffset]_RainNormal ("Rain Normal", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Range(0, 1)) = 1
        _Wetness ("Wetness", Range(0, 1)) = 0.7952354
        _WidthHightSpeed ("Width Hight Speed", Vector) = (4,4,0.2,0)
        _Tiling ("Tiling", Float ) = 5
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        LOD 200
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _Wetness;
            uniform float4 _WidthHightSpeed;
            uniform sampler2D _RainNormal;
            uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
            uniform float _Tiling;
            uniform float _NormalStrength;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 _NormalMap_var = tex2D(_NormalMap,TRANSFORM_TEX(i.uv0, _NormalMap));
                float4 node_2969 = _Time;
                float node_4763 = round(((_WidthHightSpeed.r*_WidthHightSpeed.g)*frac((_WidthHightSpeed.b*node_2969.g))));
                float2 node_4635 = (((float2(frac(i.uv0.r),frac(i.uv0.g))/float2(_WidthHightSpeed.r,_WidthHightSpeed.g))+float2((fmod(node_4763,_WidthHightSpeed.r)/_WidthHightSpeed.r),(1.0 - (floor((node_4763/_WidthHightSpeed.r))/_WidthHightSpeed.g))))+(_Tiling*i.uv0));
                float3 _RainNormal_var = UnpackNormal(tex2D(_RainNormal,node_4635));
                float3 normalLocal = (_NormalMap_var.rgb*float3((_RainNormal_var.rgb.rg*float2(0.1,_NormalStrength)),1.0));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = _Wetness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_Wetness,_Wetness,_Wetness);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                float3 diffuseColor = _Diffuse_var.rgb;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _Wetness;
            uniform float4 _WidthHightSpeed;
            uniform sampler2D _RainNormal;
            uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
            uniform float _Tiling;
            uniform float _NormalStrength;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
                UNITY_FOG_COORDS(7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 _NormalMap_var = tex2D(_NormalMap,TRANSFORM_TEX(i.uv0, _NormalMap));
                float4 node_2969 = _Time;
                float node_4763 = round(((_WidthHightSpeed.r*_WidthHightSpeed.g)*frac((_WidthHightSpeed.b*node_2969.g))));
                float2 node_4635 = (((float2(frac(i.uv0.r),frac(i.uv0.g))/float2(_WidthHightSpeed.r,_WidthHightSpeed.g))+float2((fmod(node_4763,_WidthHightSpeed.r)/_WidthHightSpeed.r),(1.0 - (floor((node_4763/_WidthHightSpeed.r))/_WidthHightSpeed.g))))+(_Tiling*i.uv0));
                float3 _RainNormal_var = UnpackNormal(tex2D(_RainNormal,node_4635));
                float3 normalLocal = (_NormalMap_var.rgb*float3((_RainNormal_var.rgb.rg*float2(0.1,_NormalStrength)),1.0));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = _Wetness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float3 specularColor = float3(_Wetness,_Wetness,_Wetness);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                float3 diffuseColor = _Diffuse_var.rgb;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
