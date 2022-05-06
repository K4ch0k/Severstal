using Notes.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notes.ViewModel
{
    public static class Core
    {
        public static ObservableCollection<NotesNames> AllNotes { get; set; } = new ObservableCollection<NotesNames>();
    }
}
