#version 420 core

#ifdef SUPPORT_OUTLINE
#if !defined(SUPPORT_ROUNDING)
#define SUPPORT_ROUNDING
#endif
#endif

#ifdef SUPPORT_TEXT
uniform sampler2D uFontTex;
#endif // SUPPORT_TEXT

#ifdef SUPPORT_TEXTURE
uniform sampler2D uMainTex;
#endif // SUPPORT_TEXTURE

out vec4 fragColor;

layout(location = 0) in vec4 v_color;

#ifdef SUPPORT_TEXT
layout(location = 1) in vec3 v_char;
#endif // SUPPORT_TEXT

#if defined(SUPPORT_TEXTURE) || defined(SUPPORT_ROUNDING) || defined(SUPPORT_BLUR)
layout(location = 2) in vec2 v_texcoord;
#endif // defined(SUPPORT_TEXTURE) || defined(SUPPORT_ROUNDING)

#if defined(SUPPORT_ROUNDING) || defined(SUPPORT_BLUR)
layout(location = 3) in flat vec4 v_size;
#endif // SUPPORT_ROUNDING



#ifdef SUPPORT_TEXT
float median(vec3 x) {
    return max(min(x.r, x.g), min(max(x.r, x.g), x.b));
}

float contrast(float x, float k)
{
    k = 1.-k;
    x = .5-x;
    float s = sign(x);
    x = 2.*abs(x);
    return 0.5 + 0.5 * s * x / (x * (k - 1.0) - k);
}

float hint(float mask, float sdf) {
    // This function uses a heuristic to try to hint the font by forcing
    vec2 deriv = normalize(vec2(abs(dFdx(sdf)), abs(dFdy(sdf))));
    float hint = abs(deriv.x-deriv.y)*2.-1.;
    //hint *= hint;
    hint = abs(hint); // Hint horizontal, vertical, and 45deg segments
    //hint = max(hint, 0.); // Only hint horizontal and vertical segments
    //hint = max(-hint, 0.); // Only hint 45deg segments
    mask = contrast(mask, hint);
    return mask;
}
#endif // SUPPORT_TEXT

#ifdef SUPPORT_BLUR
float smootherstep(float edge0, float edge1, float x) {
  x = clamp((x - edge0) / (edge1 - edge0), 0., 1.);

  return x * x * x * (x * (6. * x - 15.) + 10.);
}
#endif // SUPPORT_BLUR

void main() {
    fragColor = v_color;

    #ifdef SUPPORT_TEXT
        // Multi-channel distance field text rendering derived from:
        // https://github.com/Chlumsky/msdfgen
        // See also: https://cdn.cloudflare.steamstatic.com/apps/valve/2007/SIGGRAPH2007_AlphaTestedMagnification.pdf
        float smoothing = abs(dFdy(v_char.y))*40.;
        float sdf = median(texture(uFontTex, v_char.xy).rgb);
        float mask = smoothstep(max(v_char.z - smoothing, 0.05), min(v_char.z + smoothing, 0.95), sdf);
        // Heuristic based hinting works, but has artifacts...
        //mask = hint(mask, sdf);
        fragColor.a *= mask;
        #ifdef SUPPORT_TEXT_SHADOW
            float shadow_sdf = median(texture(uFontTex, v_char.xy-0.01).rgb);
            float shadow = smoothstep(max(v_char.z - smoothing*2., 0.05), min(v_char.z + smoothing*2., 0.95), shadow_sdf);
            float shadow_exp = smoothstep(max(v_char.z-.2 - smoothing, 0.05), min(v_char.z-.2 + smoothing, 0.95), shadow_sdf);
            float shadow_alpha = max(shadow_exp-fragColor.a, 0.);
            fragColor.rgb = (fragColor.rgb*0.2)*shadow_alpha + fragColor.rgb*(1.-shadow_alpha);
            fragColor.a = min(fragColor.a+shadow*0.75, 1.);
        #endif // SUPPORT_TEXT_SHADOW
    #endif // SUPPORT_TEXT

    #ifdef SUPPORT_TEXTURE
        fragColor *= texture(uMainTex, v_texcoord);
    #endif // SUPPORT_TEXTURE

    #ifdef SUPPORT_ROUNDING
        #ifdef SUPPORT_BLUR
        float blur = v_size.w;
        #else
        float blur = 0.;
        #endif
        float radius = v_size.z;
        radius = min(radius, min(v_size.x, v_size.y)/4.);
        vec2 uv = v_texcoord * 2. - 1.;
        vec2 r = abs(uv*v_size.xy/4.) - v_size.xy/4. + radius + blur;
        float mask = length(max(r, 0.)) + min(max(r.x, r.y), 0.0) - radius;
        
        // Apply either the blur or the rounding
        #ifdef SUPPORT_BLUR
        mask = max(mask/blur, 0.);
        fragColor.a *= exp(-(mask*mask*2.)) * (1.-mask*mask*mask);
        #else
        fragColor.a *= smoothstep(0.5, -.25, mask);
        #endif // SUPPORT_BLUR

        #ifdef SUPPORT_OUTLINE
        float outlineThick = v_size.w;
        fragColor.a = smoothstep(outlineThick, outlineThick-1., abs(mask+outlineThick));
        #endif // SUPPORT_OUTLINE
    #endif // SUPPORT_ROUNDING

    #ifndef SUPPORT_ALPHA
        fragColor.a = 1.0;
    #endif // SUPPORT_ALPHA
}
