// See https://aka.ms/new-console-template for more information

using CPURendering;
using Run;

Console.WriteLine("Hello, World!");

var display = new Display();
var running = true;

display.InitializeWindow(800, 600);

while (InputHandler.HandleInput())
{
    display.DrawGrid(0x3333333);
    display.DrawRect(100, 100, 100, 100, 0xFFFFFFFF);
    display.Render();
}