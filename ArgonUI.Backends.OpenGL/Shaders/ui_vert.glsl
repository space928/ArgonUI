#version 420 core

uniform vec2 uResolution;

layout(location = 0) in vec2 in_vert;
layout(location = 1) in vec4 in_color;
layout(location = 0) out vec4 v_color;
#ifdef SUPPORT_TEXT
// The uv coordinates into the font texture are stored in xy, and z stores the font weight.
layout(location = 2) in vec3 in_char;
layout(location = 1) out vec3 v_char;
#endif // SUPPORT_TEXT
#if defined(SUPPORT_TEXTURE) || defined(SUPPORT_ROUNDING)
layout(location = 3) in vec2 in_texcoord;
layout(location = 2) out vec2 v_texcoord;
#endif // defined(SUPPORT_TEXTURE) || defined(SUPPORT_ROUNDING)
#ifdef SUPPORT_ROUNDING
// To get the correct aspect ratio, in_size.xy stores the size of the rect to be rounded in pixels
// in_size.z stores the rounding radius in pixels
layout(location = 4) in vec3 in_size;
layout(location = 3) out flat vec3 v_size;
#endif // T_SUPPORT_ROUNDING

void main() {
    gl_Position = vec4((in_vert/uResolution.xy)*2.-1., -0.9, 1.0);
    gl_Position.y = -gl_Position.y;
    v_color = in_color;
    #ifdef SUPPORT_TEXT
    v_char = in_char;
    #endif // SUPPORT_TEXT
    #if defined(SUPPORT_TEXTURE) || defined(SUPPORT_ROUNDING)
    v_texcoord = in_texcoord;
    #endif // defined(SUPPORT_TEXTURE) || defined(SUPPORT_ROUNDING)
    #ifdef SUPPORT_ROUNDING
    v_size = in_size;
    #endif // SUPPORT_ROUNDING
}
