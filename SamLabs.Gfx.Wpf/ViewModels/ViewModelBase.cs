using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SamLabs.Gfx.Wpf.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool NotifyPropertyChanged<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false; // Value hasn't changed
            }

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
