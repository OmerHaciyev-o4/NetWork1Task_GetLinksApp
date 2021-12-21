using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetWork1Task_GetLinksApp.MVVM.ViewModels
{ 
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    } 
} 