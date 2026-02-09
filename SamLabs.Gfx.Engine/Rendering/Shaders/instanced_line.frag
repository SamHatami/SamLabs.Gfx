#version 330 core

in VS_OUT {
    vec4 color;
    vec2 uv;
    flat int entityId;
    flat uint styleFlags;
    float dashLength;
    float gapLength;
    float distanceAlongLine;
} fs_in;

uniform int uIsPickingPass;

out vec4 FragColor;

// Style flag constants
const uint STYLE_DASHED = 1u;
const uint STYLE_DOTTED = 2u;
const uint STYLE_HIDDEN = 4u;

void main()
{
    // Check if line is hidden
    if ((fs_in.styleFlags & STYLE_HIDDEN) != 0u)
        discard;

    if (uIsPickingPass == 1)
    {
        // Output entity ID as color for picking
        int id = fs_in.entityId;
        FragColor = vec4(
            float((id >> 0) & 0xFF) / 255.0,
            float((id >> 8) & 0xFF) / 255.0,
            float((id >> 16) & 0xFF) / 255.0,
            1.0
        );
    }
    else
    {
        // Handle dashed/dotted patterns
        if ((fs_in.styleFlags & STYLE_DASHED) != 0u)
        {
            float dashCycle = fs_in.dashLength + fs_in.gapLength;
            float dashPosition = mod(fs_in.distanceAlongLine, dashCycle);
            if (dashPosition > fs_in.dashLength)
                discard;
        }
        else if ((fs_in.styleFlags & STYLE_DOTTED) != 0u)
        {
            float dotCycle = fs_in.dashLength + fs_in.gapLength;
            float dotPosition = mod(fs_in.distanceAlongLine, dotCycle);
            if (dotPosition > fs_in.dashLength * 0.5)
                discard;
        }
        
        // Anti-aliasing based on distance from line center
        float dist = abs(fs_in.uv.y);
        float alpha = smoothstep(0.6, 0.4, dist);
        
        FragColor = vec4(fs_in.color.rgb, fs_in.color.a * alpha);
    }
}

