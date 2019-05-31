Shader "Hidden/EchoAmigo"
{
	Properties
	{
		_Color("Main Color", Color) = (0.1,0.1,0.1,0)
		_ShadowTex("Cookie", 2D) = "" { TexGen ObjectLinear }
		_FalloffTex("FallOff", 2D) = "" { TexGen ObjectLinear }
	}

		Subshader
	{
		Pass
		{
			ZTest Always // Turn off z test so things show through each other
			ZWrite Off
			Color[_Color]
			Blend One One
			Offset - 1, -1

			SetTexture[_ShadowTex]
			{
				combine texture * primary, ONE - texture
				Matrix[_Projector]
			}

			SetTexture[_FalloffTex]
			{
				constantColor(0,0,0,0)
				combine previous lerp(texture) constant
				Matrix[_ProjectorClip]
			}
		}
	}
}
