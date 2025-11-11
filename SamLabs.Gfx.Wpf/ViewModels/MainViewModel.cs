using SamLabs.Gfx.Core.Framework.Display;
using SamLabs.Gfx.Viewer.Framework;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SamLabs.Gfx.Wpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        
        public readonly ISceneManager _sceneManager;
        public readonly IScene _scene;
        private string _title = "SamLabs Gfx Viewer";
        public string Title
        {
            get => _title;
            set => NotifyPropertyChanged(ref _title, value);
        }


        public MainViewModel(ISceneManager sceneManager)
        {
            _sceneManager = sceneManager;
            _scene = _sceneManager.GetCurrentScene();
        }

    }
}
