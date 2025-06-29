// See https://aka.ms/new-console-template for more information

using CPURendering;
using Run;
using SDL3;

Console.WriteLine("Hello, World!");

Display display = new Display();
bool running = true;

display.InitializeWindow(800, 600);

while (InputHandler.HandleInput())
{
    display.DrawGrid(0x3333333);
    display.DrawRect(100,100,100,100,0xFFFFFFFF);
    display.Render();
    
}


