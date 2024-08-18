#ifndef OS_CROSS_UPSAMPLE_INCLUDED
#define OS_CROSS_UPSAMPLE_INCLUDED

SamplerState linear_clamp_sampler;

half3 CrossSample(TEXTURE2D_X(Tex), half2 UV, half2 SourceScale, half Ratio)
{
	half2 invScale = 1.0 / SourceScale;
	invScale *= Ratio;
	half2 p[5];
	p[0] = UV;
	p[1] = half2(UV.x + invScale.x, UV.y);
	p[2] = half2(UV.x - invScale.x, UV.y);
	p[3] = half2(UV.x, UV.y + invScale.y);
	p[4] = half2(UV.x, UV.y - invScale.y);
	
	half3 r = 0;
	for (int a = 0; a < 5; a++)
	{
		r += SAMPLE_TEXTURE2D_X_LOD(Tex, linear_clamp_sampler, UnityStereoTransformScreenSpaceTex(p[a]), 0).rgb;
	}
	
	
	return r * 0.2;
}
#endif