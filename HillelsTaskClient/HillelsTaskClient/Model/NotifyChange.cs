using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillelsTaskClient.Model
{
    public class NotifyChange : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string propName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
