using System.Numerics;
using ImGuiNET;

// Required for ImVec4 and ImVec2 equivalents

namespace SamLabs.Gfx.Viewer.Display.ImGuiBackends.ImGuiStyles;

public class DarkStyle
{
    public DarkStyle()
    {
        ImGuiStylePtr style = ImGui.GetStyle();

        // --- 📐 Style Variables (Rounding, Spacing, Padding) ---
        style.WindowPadding = new Vector2(8, 8);
        style.FramePadding = new Vector2(5, 4);
        style.ItemSpacing = new Vector2(6, 4);
        style.ItemInnerSpacing = new Vector2(6, 4);
        style.IndentSpacing = 20.0f;
        style.ScrollbarSize = 10.0f;
        style.GrabMinSize = 8.0f;
        
        // Rounding
        style.WindowRounding = 4.0f;
        style.ChildRounding = 4.0f;
        style.FrameRounding = 4.0f;
        style.PopupRounding = 4.0f;
        style.ScrollbarRounding = 9.0f; // Fully rounded scrollbar
        style.GrabRounding = 4.0f;
        style.TabRounding = 4.0f;

        // Borders
        style.WindowBorderSize = 0.0f;
        style.FrameBorderSize = 0.0f;
        style.PopupBorderSize = 0.0f;
        style.TabBorderSize = 0.0f;


        // --- 🎨 Colors ---
        // Define base colors (R, G, B, A in 0.0-1.0 range)
        Vector4 darkGray = new Vector4(0.13f, 0.13f, 0.13f, 1.00f);
        Vector4 mediumGray = new Vector4(0.20f, 0.20f, 0.20f, 1.00f);
        Vector4 lightGray = new Vector4(0.28f, 0.28f, 0.28f, 1.00f);
        Vector4 accentColor = new Vector4(0.0f, 0.44f, 0.88f, 1.00f); // A vibrant blue
        Vector4 accentHover = new Vector4(0.0f, 0.55f, 1.0f, 1.00f);
        Vector4 accentActive = new Vector4(0.0f, 0.33f, 0.77f, 1.00f);

        // Apply colors to the style array
        style.Colors[(int)ImGuiCol.Text] = new Vector4(0.90f, 0.90f, 0.90f, 1.00f);
        style.Colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.50f, 0.50f, 0.50f, 1.00f);
        style.Colors[(int)ImGuiCol.WindowBg] = darkGray;
        style.Colors[(int)ImGuiCol.ChildBg] = darkGray;
        style.Colors[(int)ImGuiCol.PopupBg] = new Vector4(0.10f, 0.10f, 0.10f, 0.94f); // Slightly darker, semi-transparent
        style.Colors[(int)ImGuiCol.Border] = new Vector4(0.40f, 0.40f, 0.40f, 0.40f);
        style.Colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f);

        // Frame Backgrounds (Input/Slider/Checkboxes)
        style.Colors[(int)ImGuiCol.FrameBg] = mediumGray;
        style.Colors[(int)ImGuiCol.FrameBgHovered] = lightGray;
        style.Colors[(int)ImGuiCol.FrameBgActive] = accentActive;

        // Title Bar
        style.Colors[(int)ImGuiCol.TitleBg] = darkGray;
        style.Colors[(int)ImGuiCol.TitleBgActive] = lightGray;
        style.Colors[(int)ImGuiCol.TitleBgCollapsed] = darkGray;

        // Menu Bar
        style.Colors[(int)ImGuiCol.MenuBarBg] = mediumGray;

        // Scrollbar
        style.Colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.10f, 0.10f, 0.10f, 0.53f);
        style.Colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.31f, 0.31f, 0.31f, 1.00f);
        style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.41f, 0.41f, 0.41f, 1.00f);
        style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.51f, 0.51f, 0.51f, 1.00f);

        // Checkmark and Slider Grab
        style.Colors[(int)ImGuiCol.CheckMark] = accentColor;
        style.Colors[(int)ImGuiCol.SliderGrab] = accentColor;
        style.Colors[(int)ImGuiCol.SliderGrabActive] = accentActive;

        // Buttons
        style.Colors[(int)ImGuiCol.Button] = lightGray;
        style.Colors[(int)ImGuiCol.ButtonHovered] = accentHover;
        style.Colors[(int)ImGuiCol.ButtonActive] = accentActive;

        // Headers (Tree nodes, Collapsing Headers)
        style.Colors[(int)ImGuiCol.Header] = mediumGray;
        style.Colors[(int)ImGuiCol.HeaderHovered] = accentHover;
        style.Colors[(int)ImGuiCol.HeaderActive] = accentActive;

        // Separators
        style.Colors[(int)ImGuiCol.Separator] = style.Colors[(int)ImGuiCol.Border];
        style.Colors[(int)ImGuiCol.SeparatorHovered] = accentHover;
        style.Colors[(int)ImGuiCol.SeparatorActive] = accentActive;

        // Resize Grip
        style.Colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.4f, 0.4f, 0.4f, 0.2f);
        style.Colors[(int)ImGuiCol.ResizeGripHovered] = accentHover;
        style.Colors[(int)ImGuiCol.ResizeGripActive] = accentActive;

        // Tabs
        style.Colors[(int)ImGuiCol.Tab] = mediumGray;
        style.Colors[(int)ImGuiCol.TabHovered] = accentHover;
        // style.Colors[(int)ImGuiCol.TabUnfocusedActive] = darkGray;
        // style.Colors[(int)ImGuiCol.TabUnfocusedActive] = accentColor * new Vector4(1.0f, 1.0f, 1.0f, 0.8f);

        // Plotting
        style.Colors[(int)ImGuiCol.PlotLines] = new Vector4(0.61f, 0.61f, 0.61f, 1.00f);
        style.Colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(1.00f, 0.43f, 0.35f, 1.00f);
        style.Colors[(int)ImGuiCol.PlotHistogram] = new Vector4(0.90f, 0.70f, 0.00f, 1.00f);
        style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(1.00f, 0.60f, 0.00f, 1.00f);

        // Text Selection and Drag/Drop
        style.Colors[(int)ImGuiCol.TextSelectedBg] = accentHover * new Vector4(1.0f, 1.0f, 1.0f, 0.4f);
        style.Colors[(int)ImGuiCol.DragDropTarget] = new Vector4(1.00f, 1.00f, 0.00f, 0.90f);
        
        // Navigation and Modal
        style.Colors[(int)ImGuiCol.NavWindowingHighlight] = new Vector4(1.00f, 1.00f, 1.00f, 0.70f);
        style.Colors[(int)ImGuiCol.NavWindowingDimBg] = new Vector4(0.80f, 0.80f, 0.80f, 0.20f);
        style.Colors[(int)ImGuiCol.ModalWindowDimBg] = new Vector4(0.20f, 0.20f, 0.20f, 0.35f);
    }
}