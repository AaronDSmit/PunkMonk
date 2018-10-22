// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1280277,fgcg:0.1953466,fgcb:0.2352941,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:6821,x:33256,y:32711,varname:node_6821,prsc:2|diff-3484-RGB,spec-8047-OUT,gloss-8047-OUT,normal-6956-OUT;n:type:ShaderForge.SFN_Tex2d,id:3484,x:32651,y:32406,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:_Diffuse,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:b66bceaf0cc0ace4e9bdc92f14bba709,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:8047,x:32481,y:32620,ptovrint:False,ptlb:Wetness,ptin:_Wetness,varname:_Wetness,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.7952354,max:1;n:type:ShaderForge.SFN_Vector4Property,id:4284,x:29093,y:32842,ptovrint:False,ptlb:Width Hight Speed,ptin:_WidthHightSpeed,varname:_WidthHightSpeed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:4,v2:4,v3:10,v4:0;n:type:ShaderForge.SFN_Multiply,id:5872,x:29518,y:33227,varname:node_5872,prsc:2|A-4284-Z,B-640-OUT;n:type:ShaderForge.SFN_Time,id:2969,x:29013,y:33284,varname:node_2969,prsc:2;n:type:ShaderForge.SFN_Multiply,id:6956,x:32823,y:32909,varname:node_6956,prsc:2|A-1506-RGB,B-3273-OUT;n:type:ShaderForge.SFN_TexCoord,id:912,x:27633,y:33745,varname:node_912,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:4635,x:31737,y:32738,varname:node_4635,prsc:2|A-6044-UVOUT,B-8246-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4287,x:27674,y:33920,ptovrint:False,ptlb:Tiling,ptin:_Tiling,varname:_Tiling,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_ComponentMask,id:7996,x:32142,y:32843,varname:node_7996,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-3005-RGB;n:type:ShaderForge.SFN_Slider,id:4297,x:31780,y:33091,ptovrint:False,ptlb:Normal Strength,ptin:_NormalStrength,varname:_NormalStrength,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:20,x:32340,y:32922,varname:node_20,prsc:2|A-7996-OUT,B-9630-OUT;n:type:ShaderForge.SFN_Vector1,id:5917,x:32340,y:33068,varname:node_5917,prsc:2,v1:1;n:type:ShaderForge.SFN_Append,id:3273,x:32546,y:32982,varname:node_3273,prsc:2|A-20-OUT,B-5917-OUT;n:type:ShaderForge.SFN_Append,id:9630,x:32142,y:33011,varname:node_9630,prsc:2|A-639-OUT,B-4297-OUT;n:type:ShaderForge.SFN_Vector1,id:639,x:31937,y:32991,varname:node_639,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Multiply,id:8395,x:27931,y:33781,varname:node_8395,prsc:2|A-912-UVOUT,B-4287-OUT;n:type:ShaderForge.SFN_Code,id:2851,x:28458,y:33803,varname:node_2851,prsc:2,code:ZgBsAG8AYQB0ADIAIABwACAAPQAgAGYAbABvAG8AcgAoAFUAVgApADsADQAKACAAIABmAGwAbwBhAHQAMgAgAGYAIAA9ACAAZgByAGEAYwAoAFUAVgApADsADQAKACAAIABmACAAPQAgAGYAIAAqACAAZgAgACoAIAAoADMALgAwACAALQAgADIALgAwACAAKgAgAGYAKQA7AA0ACgAgACAAZgBsAG8AYQB0ACAAbgAgAD0AIABwAC4AeAAgACsAIABwAC4AeQAgACoAIAA1ADcALgAwADsADQAKACAAIABmAGwAbwBhAHQANAAgAG4AbwBpAHMAZQAgAD0AIABmAGwAbwBhAHQANAAoAG4ALAAgAG4AIAArACAAMQAsACAAbgAgACsAIAA1ADcALgAwACwAIABuACAAKwAgADUAOAAuADAAKQA7AA0ACgAgACAAbgBvAGkAcwBlACAAPQAgAGYAcgBhAGMAKABzAGkAbgAoAG4AbwBpAHMAZQApACoANAAzADcALgA1ADgANQA0ADUAMwApADsADQAKACAAIAByAGUAdAB1AHIAbgAgAGwAZQByAHAAKABsAGUAcgBwACgAbgBvAGkAcwBlAC4AeAAsACAAbgBvAGkAcwBlAC4AeQAsACAAZgAuAHgAKQAsACAAbABlAHIAcAAoAG4AbwBpAHMAZQAuAHoALAAgAG4AbwBpAHMAZQAuAHcALAAgAGYALgB4ACkALAAgAGYALgB5ACkAOwAgAA==,output:0,fname:Noise,width:891,height:344,input:1,input_1_label:UV|A-7675-OUT;n:type:ShaderForge.SFN_Ceil,id:7675,x:28144,y:33793,varname:node_7675,prsc:2|IN-8395-OUT;n:type:ShaderForge.SFN_TexCoord,id:4701,x:30412,y:33051,varname:node_4701,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:8246,x:31357,y:32949,varname:node_8246,prsc:2|A-4701-UVOUT,B-6813-OUT;n:type:ShaderForge.SFN_Add,id:640,x:29309,y:33343,varname:node_640,prsc:2|A-2969-T,B-2851-OUT;n:type:ShaderForge.SFN_Relay,id:6813,x:28812,y:34364,varname:node_6813,prsc:2|IN-4287-OUT;n:type:ShaderForge.SFN_UVTile,id:6044,x:30688,y:32876,varname:node_6044,prsc:2|UVIN-4701-UVOUT,WDT-4284-X,HGT-4284-Y,TILE-6844-OUT;n:type:ShaderForge.SFN_Multiply,id:8987,x:29697,y:32999,varname:node_8987,prsc:2|A-4284-X,B-4284-Y;n:type:ShaderForge.SFN_Fmod,id:9065,x:29929,y:33156,varname:node_9065,prsc:2|A-5872-OUT,B-8987-OUT;n:type:ShaderForge.SFN_Round,id:6844,x:30129,y:33042,varname:node_6844,prsc:2|IN-9065-OUT;n:type:ShaderForge.SFN_Tex2d,id:3005,x:31937,y:32799,varname:node_3005,prsc:2,tex:aab176d225be0f940b0548435d57141c,ntxv:0,isnm:False|UVIN-4635-OUT,TEX-8198-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:8198,x:31737,y:32896,ptovrint:False,ptlb:Rain Animation,ptin:_RainAnimation,varname:node_8198,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:True,tagnrm:False,tex:aab176d225be0f940b0548435d57141c,ntxv:3,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:1506,x:32546,y:32770,ptovrint:False,ptlb:Normal Map,ptin:_NormalMap,varname:node_1506,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;proporder:3484-1506-8198-4284-4297-8047-4287;pass:END;sub:END;*/

Shader "Custom/RainShader" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "white" {}
        [NoScaleOffset]_RainAnimation ("Rain Animation", 2D) = "bump" {}
        _WidthHightSpeed ("Width Hight Speed", Vector) = (4,4,10,0)
        _NormalStrength ("Normal Strength", Range(0, 1)) = 1
        _Wetness ("Wetness", Range(0, 1)) = 0.7952354
        _Tiling ("Tiling", Float ) = 2
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
            uniform float _Tiling;
            uniform float _NormalStrength;
            float Noise( float2 UV ){
            float2 p = floor(UV);
              float2 f = frac(UV);
              f = f * f * (3.0 - 2.0 * f);
              float n = p.x + p.y * 57.0;
              float4 noise = float4(n, n + 1, n + 57.0, n + 58.0);
              noise = frac(sin(noise)*437.585453);
              return lerp(lerp(noise.x, noise.y, f.x), lerp(noise.z, noise.w, f.x), f.y); 
            }
            
            uniform sampler2D _RainAnimation;
            uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
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
                float node_5872 = (_WidthHightSpeed.b*(node_2969.g+Noise( ceil((i.uv0*_Tiling)) )));
                float node_8987 = (_WidthHightSpeed.r*_WidthHightSpeed.g);
                float node_6844 = round(fmod(node_5872,node_8987));
                float2 node_6044_tc_rcp = float2(1.0,1.0)/float2( _WidthHightSpeed.r, _WidthHightSpeed.g );
                float node_6044_ty = floor(node_6844 * node_6044_tc_rcp.x);
                float node_6044_tx = node_6844 - _WidthHightSpeed.r * node_6044_ty;
                float2 node_6044 = (i.uv0 + float2(node_6044_tx, node_6044_ty)) * node_6044_tc_rcp;
                float2 node_8246 = (i.uv0*_Tiling);
                float2 node_4635 = (node_6044+node_8246);
                float4 node_3005 = tex2D(_RainAnimation,node_4635);
                float3 normalLocal = (_NormalMap_var.rgb*float3((node_3005.rgb.rg*float2(0.1,_NormalStrength)),1.0));
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
            uniform float _Tiling;
            uniform float _NormalStrength;
            float Noise( float2 UV ){
            float2 p = floor(UV);
              float2 f = frac(UV);
              f = f * f * (3.0 - 2.0 * f);
              float n = p.x + p.y * 57.0;
              float4 noise = float4(n, n + 1, n + 57.0, n + 58.0);
              noise = frac(sin(noise)*437.585453);
              return lerp(lerp(noise.x, noise.y, f.x), lerp(noise.z, noise.w, f.x), f.y); 
            }
            
            uniform sampler2D _RainAnimation;
            uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
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
                float node_5872 = (_WidthHightSpeed.b*(node_2969.g+Noise( ceil((i.uv0*_Tiling)) )));
                float node_8987 = (_WidthHightSpeed.r*_WidthHightSpeed.g);
                float node_6844 = round(fmod(node_5872,node_8987));
                float2 node_6044_tc_rcp = float2(1.0,1.0)/float2( _WidthHightSpeed.r, _WidthHightSpeed.g );
                float node_6044_ty = floor(node_6844 * node_6044_tc_rcp.x);
                float node_6044_tx = node_6844 - _WidthHightSpeed.r * node_6044_ty;
                float2 node_6044 = (i.uv0 + float2(node_6044_tx, node_6044_ty)) * node_6044_tc_rcp;
                float2 node_8246 = (i.uv0*_Tiling);
                float2 node_4635 = (node_6044+node_8246);
                float4 node_3005 = tex2D(_RainAnimation,node_4635);
                float3 normalLocal = (_NormalMap_var.rgb*float3((node_3005.rgb.rg*float2(0.1,_NormalStrength)),1.0));
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
