Shader "SandalStudio/Snow"
{
	Properties
	{
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_Mask1("Mask 1", Float) = 0.74
		_Mask2("Mask 2", Float) = 2.73
		_Glow("Glow", Float) = 1.19
		_Tiling("Tiling", Float) = 14.79
		_Mask3("Mask 3", Float) = 0.9
		[HDR]_Color1("Color 1", Color) = (0,0.6579595,1,1)
		[HDR]_Color0("Color 0", Color) = (1,1,1,1)
		[ASEEnd]_Emissive("Emissive", Float) = 1

	}


	Category 
	{
		SubShader
		{
		LOD 0

			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend SrcAlpha One
			ColorMask RGB
			Cull Off
			Lighting Off 
			ZWrite Off
			ZTest LEqual
			
			Pass {
			
				CGPROGRAM
				
				#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
				#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
				#endif
				
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				#define ASE_NEEDS_FRAG_COLOR


				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD2;
					#endif
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
					
				};
				
				
				#if UNITY_VERSION >= 560
				UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
				#else
				uniform sampler2D_float _CameraDepthTexture;
				#endif

				//Don't delete this comment
				// uniform sampler2D_float _CameraDepthTexture;

				uniform sampler2D _MainTex;
				uniform fixed4 _TintColor;
				uniform float4 _MainTex_ST;
				uniform float _InvFade;
				uniform float _Tiling;
				uniform float _Glow;
				uniform float _Mask3;
				uniform float _Mask2;
				uniform float _Mask1;
				uniform float4 _Color0;
				uniform float4 _Color1;
				uniform float _Emissive;


				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					

					v.vertex.xyz +=  float3( 0, 0, 0 ) ;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					o.color = v.color;
					o.texcoord = v.texcoord;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag ( v2f i  ) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID( i );
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( i );

					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate (_InvFade * (sceneZ-partZ));
						i.color.a *= fade;
					#endif

					float2 CenteredUV15_g1 = ( i.texcoord.xy - float2( 0.5,0.5 ) );
					float2 break17_g1 = CenteredUV15_g1;
					float2 appendResult23_g1 = (float2(( length( CenteredUV15_g1 ) * 1.0 * 2.0 ) , ( atan2( break17_g1.x , break17_g1.y ) * ( 1.0 / 6.28318548202515 ) * 1.0 )));
					float2 temp_output_1_0 = appendResult23_g1;
					float temp_output_11_0 = (( temp_output_1_0 * 1.98 )).x;
					float temp_output_7_0 = cos( (( temp_output_1_0 * ( _Tiling * 3.0 ) )).y );
					float temp_output_15_0 = cos( temp_output_7_0 );
					float temp_output_16_0 = ( temp_output_11_0 * temp_output_15_0 );
					float4 lerpResult37 = lerp( _Color0 , _Color1 , (temp_output_1_0).x);
					

					fixed4 col = ( saturate( ( ( ( 1.0 - temp_output_16_0 ) + step( temp_output_16_0 , _Glow ) ) * step( temp_output_7_0 , _Mask3 ) * step( frac( ( ( temp_output_11_0 * ( 1.0 - temp_output_15_0 ) ) * _Mask2 ) ) , _Mask1 ) ) ) * ( lerpResult37 * _Emissive ) * i.color );
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
	
	
	
}
