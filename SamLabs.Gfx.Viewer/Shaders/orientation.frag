#version 330 core
in vec3 fragWorldPosition;
out vec4 FragColor;
void main()
{
    float epsilon = 0.001;
    
    if(abs(fragWorldPosition.x) < epsilon)
        FragColor = vec4(1,0,0,0.85);
    else if(abs(fragWorldPosition.z) < epsilon)
        FragColor = vec4(0,0,1,1.0);
    else
        FragColor = vec4(0.5f, 0.5f, 0.5f, 1.0);
    
    
}
