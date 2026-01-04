#version 330 core

out vec4 FragColor;
uniform int uIsHovered;
uniform int uIsSelected;
uniform int uGizmoAxis;

void main()
{
    if(uIsHovered == 1 || uIsSelected == 1)
    {
        FragColor = vec4(0.95, 0.95, 0.5, 1.0);
    }
    else
    { 
        switch(uGizmoAxis)
        {
            case 0:
                FragColor = vec4(0.8, 0.1, 0.1, 1.0);
                break;
            case 1:
                FragColor = vec4(0.1, 1, 0.1, 1.0);
                break;
            case 2:
                FragColor = vec4(0.1, 0.1, 1, 1.0);
                break;
            
        }

    }
    

}
