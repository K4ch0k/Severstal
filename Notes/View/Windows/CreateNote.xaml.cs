using Notes.Model;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Notes.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для CreateNote.xaml
    /// </summary>
    public partial class CreateNote : Window
    {
        public CreateNote(string Path, ObservableCollection<NotesNames> AllNotes)
        {
            InitializeComponent();
            path = Path;
            allNotes = AllNotes;
        }

        public int returnvalue { get; set; } = 0;
        string path { get; set; }
        public ObservableCollection<NotesNames> allNotes { get; set; } = new ObservableCollection<NotesNames>();

        private void CreateNewNote()
        {
            try
            {
                if (FileName.Text.Contains(Path.GetInvalidPathChars().ToString()) || FileName.Text == "")
                {
                    MessageBox.Show("В названии заметки содержатся недопустимые символы!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                foreach (var item in allNotes)
                {
                    if (item.FileName == FileName.Text)
                    {
                        MessageBox.Show("Заметка с таким названием уже существует", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        return;
                    }
                }

                FileStream fs = new FileStream(path + "/" + FileName.Text + ".rtf", FileMode.Create);
                RichTextBox rtb = new RichTextBox();
                TextRange range = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                range.Save(fs, DataFormats.Rtf);
                fs.Close();
                returnvalue = 1;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Что-то пошло не так : \n" + ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            CreateNewNote();
        }

        private void FileName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                CreateNewNote();
            }
        }
    }
}