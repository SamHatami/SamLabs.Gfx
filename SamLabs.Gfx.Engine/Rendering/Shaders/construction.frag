#version 330 core

out vec4 FragColor;

in vec2 TexCoord;
uniform int uIsHovered;
uniform int uIsSelected;

// More saturated orange for edge
uniform vec4 uEdgeColor = vec4(1.0, 0.6, 0.1, 1.0);
// Desaturated light orange for plane
uniform vec4 uPlaneColor = vec4(1.0, 0.85, 0.6, 0.5);
// Hovered color (matching flat.frag, assumed blue highlight)
uniform vec4 uHoverColor = vec4(0.25, 0.25, 1.0, 0.5);

uniform float uEdgePixelWidth = 2.0; // Edge width in pixels

void main()
{
    float distToEdge = min(min(TexCoord.x, 1.0 - TexCoord.x), min(TexCoord.y, 1.0 - TexCoord.y));
    float pixelWidth = fwidth(distToEdge);
    float edgeThreshold = uEdgePixelWidth * pixelWidth;

    bool isHover = (uIsHovered == 1 || uIsSelected == 1);

    if (distToEdge < edgeThreshold) {
        FragColor = isHover ? uHoverColor : uEdgeColor;
    }
    else if(isHover)
    {
        FragColor = uHoverColor;
    }
    else
    {
        FragColor = uPlaneColor;
    }

}