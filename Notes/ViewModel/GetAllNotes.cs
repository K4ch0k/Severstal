using Notes.Model;
using System.Collections.ObjectModel;
using System.IO;

namespace Notes.ViewModel
{
    public static class GetAllNotes
    {
        public static ObservableCollection<NotesNames> GetAllNotesCollection(string targetDirectory = "../Model/Notes")
        {
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
            AllFiles = Directory.GetFiles(targetDirectory);
            AllNotesCollection = new ObservableCollection<NotesNames>();
            foreach (string item in AllFiles)
            {
                AllNotesCollection.Add(new NotesNames
                {
                    FileName = Path.GetFileNameWithoutExtension(item),
                    Path = Path.GetFullPath(item), 
                    DateCreate = Directory.GetCreationTime(item)
                });
            }
            return AllNotesCollection;
        }
        static string[] AllFiles;

        static ObservableCollection<NotesNames> AllNotesCollection { get; set; }
    }
}
