Shader "Hidden/Klak/TestTools/ImageSource"
{
    CGINCLUDE

    #include "UnityCG.cginc"

    float2 _Resolution;

    // Hue to RGB convertion
    half3 HueToRGB(half h)
    {
        h = saturate(h);
        half r = abs(h * 6 - 3) - 1;
        half g = 2 - abs(h * 6 - 2);
        half b = 2 - abs(h * 6 - 4);
        half3 rgb = saturate(half3(r, g, b));
        return rgb;
    }

    void Vertex(float4 position : POSITION,
                float2 texCoord : TEXCOORD0,
                out float4 outPosition : SV_Position,
                out float2 outTexCoord : TEXCOORD0)
    {
        outPosition = UnityObjectToClipPos(position);
        outTexCoord = float2(texCoord.x, 1 - texCoord.y);
    }

    float4 FragmentCard(float4 position : SV_Position,
                        float2 texCoord : TEXCOORD0) : SV_Target
    {
        // Rotation (narrow screen) support
        bool wide = _Resolution.x >= _Resolution.y;
        float2 res = wide ? _Resolution.xy : _Resolution.yx;
        float2 uv = wide ? texCoord.xy : texCoord.yx;

        float scale = 27 / res.y;           // Grid scale
        float2 p0 = (uv - 0.5) * res.xy;    // Position (pixel)
        float2 p1 = p0 * scale;             // Position (half grid)
        float2 p2 = p1 / 2 - 0.5;           // Position (grid)

        // Size of inner area
        half2 area = half2(floor(6.5 * res.x / res.y) * 2 + 1, 13);

        // Crosshair and grid lines
        half2 ch = abs(p0);
        half2 grid = (1 - abs(frac(p2) - 0.5) * 2) / scale;
        half c1 = min(min(ch.x, ch.y), min(grid.x, grid.y)) < 1 ? 1 : 0.5;

        // Outer area checker
        half2 checker = frac(floor(p2) / 2) * 2;
        if (any(abs(p1) > area)) c1 = abs(checker.x - checker.y);

        half corner = sqrt(8) - length(abs(p1) - area + 4); // Corner circles
        half circle = 12 - length(p1);                      // Big center circle
        half mask = saturate(circle / scale);               // Center circls mask

        // Grayscale bars
        half bar1 = saturate(p1.y < 5 ? floor(p1.x / 4 + 3) / 5 : p1.x / 16 + 0.5);
        if (abs(5 - p1.y) < 4 * mask) c1 = bar1;

        // Basic color bars
        half3 bar2 = HueToRGB((p1.y > -5 ? floor(p1.x / 4) / 6 : p1.x / 16) + 0.5);
        float3 rgb = abs(-5 - p1.y) < 4 * mask ? bar2 : saturate(c1);

        // Circle lines
        rgb = lerp(rgb, 1, saturate(1.5 - abs(max(circle, corner)) / scale));

        #if !defined(UNITY_COLORSPACE_GAMMA)
        rgb = GammaToLinearSpace(rgb);
        #endif

        return half4(rgb, 1);
    }

    float4 FragmentGradient(float4 position : SV_Position,
                            float2 texCoord : TEXCOORD0) : SV_Target
    {
        float t = _Time.y;

        float3 x = texCoord.x;
        x *= sin(t * float3(1.71, 1.88, 2.23));
        x += t * float3(0.21, 0.34, 0.13);

        float3 y = texCoord.y;
        y *= sin(t * float3(1.12, 1.43, 1.73));
        y += t * float3(0.33, 0.27, 0.31);

        float3 rgb = sin((x + sin(y)) * 4.45) * 0.5 + 0.5;

        #if !defined(UNITY_COLORSPACE_GAMMA)
        rgb = GammaToLinearSpace(rgb);
        #endif

        return half4(rgb, 1);
    }

    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma multi_compile __ UNITY_COLORSPACE_GAMMA
            #pragma vertex Vertex
            #pragma fragment FragmentCard
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma multi_compile __ UNITY_COLORSPACE_GAMMA
            #pragma vertex Vertex
            #pragma fragment FragmentGradient
            ENDCG
        }
    }
}
