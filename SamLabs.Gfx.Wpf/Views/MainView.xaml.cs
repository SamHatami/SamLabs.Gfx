using System.Windows;
using System.Windows.Input;
using OpenTK.Mathematics;
using OpenTK.Wpf;
using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.Viewer.Framework;
using SamLabs.Gfx.Wpf.ViewModels; //

namespace SamLabs.Gfx.Wpf;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainView : Window
{

    private ISceneManager _sceneManager;

    private bool _isLeftDown = false;
    private bool _isRightDown = false;
    private Vector2 _lastPos = Vector2.Zero;

    public MainView()
    {
        InitializeComponent();

        Loaded += OnViewLoaded;

    }

    private void OnViewLoaded(object sender, RoutedEventArgs e)
    {
        if (this.DataContext is MainViewModel vm)
            _sceneManager = vm._sceneManager;

        var settings = new GLWpfControlSettings
        {
            MajorVersion = 4,
            MinorVersion = 6
        };
        OpenTkControl.Start(settings);
    }

    private void OpenTkControl_OnRender(TimeSpan delta)
    {
        // Since mainScene is not defined in this file,
        // we should call the renderer logic based on the current scene.
        // Assuming your rendering logic is inside a class with a Render method
        // you would call it here, passing the current scene.
        // For simplicity, I'll use a placeholder `mainScene` from your original file.
        // mainScene.Render();

        // This is where your rendering pipeline should execute.
        // If your Window.cs logic is what performs the draw, you'll need to
        // refactor that logic into the OpenTkControl_OnRender method or into
        // a dedicated Renderer object that this class can call.
    }

    // --- New Input Event Handlers ---

    private void OpenTkControl_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            _isLeftDown = true;
        }
        else if (e.ChangedButton == MouseButton.Right)
        {
            _isRightDown = true;
        }

        // Capture the mouse so we keep receiving events even if the cursor leaves the control
        OpenTkControl.CaptureMouse();

        // Set lastPos for tracking
        var currentPos = e.GetPosition(OpenTkControl);
        _lastPos = new Vector2((float)currentPos.X, (float)currentPos.Y);
    }

    private void OpenTkControl_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            _isLeftDown = false;
        }
        else if (e.ChangedButton == MouseButton.Right)
        {
            _isRightDown = false;
        }

        // Release mouse capture
        OpenTkControl.ReleaseMouseCapture();
    }

    private void OpenTkControl_MouseMove(object sender, MouseEventArgs e)
    {
        // Get the current scene and camera
        var currentScene = _sceneManager?.GetCurrentScene();
        if (currentScene == null) return;

        var currentPos = e.GetPosition(OpenTkControl);
        var pos = new Vector2((float)currentPos.X, (float)currentPos.Y);

        // Calculate the delta in screen coordinates
        var delta = pos - _lastPos;

        if (_isLeftDown)
        {
            // Orbit logic from Window.cs: _currentScene.Camera.Orbit(delta.X * 0.2f, delta.Y * 0.2f);
            currentScene.Camera.Orbit(delta.X * 0.2f, delta.Y * 0.2f);
        }
        else if (_isRightDown)
        {
            // Pan logic from Window.cs: _currentScene.Camera.Pan(new Vector3(-delta.X * 0.01f, delta.Y * 0.01f, 0));
            currentScene.Camera.Pan(new Vector3(-delta.X * 0.01f, delta.Y * 0.01f, 0));
        }

        _lastPos = pos;

        // Force a re-render to update the view immediately
        //OpenTkControl.InvalidateVisual();
    }

    private void OpenTkControl_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        var currentScene = _sceneManager?.GetCurrentScene();
        if (currentScene == null) return;

        // Zoom logic from Window.cs: _currentScene.Camera.Zoom(e.OffsetY * 0.5f);
        // Note: In WPF, Delta is a property of MouseWheelEventArgs, not OffsetY.
        // A typical mouse wheel click is 120, so normalize to -1 or 1.
        float scrollDelta = e.Delta / 120.0f;

        currentScene.Camera.Zoom(scrollDelta * 0.5f);

        // Force a re-render
        OpenTkControl.InvalidateVisual();
    }
}