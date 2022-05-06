using Notes.Model;
using Notes.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Notes.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AllNotesCollection = GetAllNotes.GetAllNotesCollection(PathNotes);
            AllFonts = Fonts.SystemFontFamilies.OrderBy(item => item.Source).ToList();
            FontSizeComboBox.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            this.DataContext = this;
        }

        public ObservableCollection<NotesNames> AllNotesCollection { get; set; } = new ObservableCollection<NotesNames>();
        public List<FontFamily> AllFonts { get; set; } = new List<FontFamily>();
        public NotesNames CurrentNote { get; set; }
        public List<NotesNames> CurrentNoteList { get; set; } = new List<NotesNames>();

        private string PathNotes = "../../Model/Notes";

        //  События при выборе заметки
        private void AllNotesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AllNotesListView.SelectedItems.Count == 1)
            {
                var item = AllNotesListView.SelectedItem as NotesNames;
                TextRange tr = new TextRange(MainRchTxtBox.Document.ContentStart, MainRchTxtBox.Document.ContentEnd);
                if (item != CurrentNote)
                {
                    SaveNote();
                }

                CurrentNote = item;
                CurrentNoteList = new List<NotesNames>();
                CurrentNoteList.Add(item);
                SaveNoteBtn.Visibility = Visibility.Visible;
                DeleteNoteBtn.Visibility = Visibility.Visible;

                using (FileStream fs = File.Open(item.Path, FileMode.Open))
                {
                    tr.Load(fs, DataFormats.Rtf);
                }
            }
            else if (AllNotesListView.SelectedItems.Count > 1)
            {
                CurrentNote = null;
                CurrentNoteList = new List<NotesNames>();
                foreach (var item in AllNotesListView.SelectedItems)
                {
                    CurrentNoteList.Add(item as NotesNames);
                }
            }
        }

        //  Сохранение заметки при закрытии приложения
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveNote();
        }

        //  Смена состояний кнопок из шапки приложения под стили текста
        private void MainRchTxtBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object temp = MainRchTxtBox.Selection.GetPropertyValue(Inline.FontWeightProperty);
            
            BoldBtn.IsChecked = (temp != DependencyProperty.UnsetValue) && temp.Equals(FontWeights.Bold);
            temp = MainRchTxtBox.Selection.GetPropertyValue(Inline.FontStyleProperty);
            ItalicBtn.IsChecked = (temp != DependencyProperty.UnsetValue) && temp.Equals(FontStyles.Italic);


            temp = MainRchTxtBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            UnderlineBtn.IsChecked = (temp != DependencyProperty.UnsetValue) && temp.Equals(TextDecorations.Underline);


            temp = MainRchTxtBox.Selection.GetPropertyValue(Paragraph.TextAlignmentProperty);
            AlignLeftBtn.IsChecked = (temp != DependencyProperty.UnsetValue) && temp.Equals(TextAlignment.Left);
            AlignCenterBtn.IsChecked = (temp != DependencyProperty.UnsetValue) && temp.Equals(TextAlignment.Center);
            AlignRightBtn.IsChecked = (temp != DependencyProperty.UnsetValue) && temp.Equals(TextAlignment.Right);
            AlignJustifyBtn.IsChecked = (temp != DependencyProperty.UnsetValue) && temp.Equals(TextAlignment.Justify);

            temp = MainRchTxtBox.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            FontFamilyComboBox.SelectedItem = temp;
            if (FontFamilyComboBox.SelectedItem == DependencyProperty.UnsetValue)
            {
                FontFamilyComboBox.SelectedItem = null;
            }
            temp = MainRchTxtBox.Selection.GetPropertyValue(Inline.FontSizeProperty);
            FontSizeComboBox.Text = temp.ToString();
        }


        #region Стили Из шапки приложения

        //  Изменение стиля шрифта
        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (FontFamilyComboBox.SelectedItem != null)
                    MainRchTxtBox.Selection.ApplyPropertyValue(Paragraph.FontFamilyProperty, FontFamilyComboBox.SelectedItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Что-то пошло не так: \n" + ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //  Изменение Размера шрифта
        private void FontSizeComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (Convert.ToDouble(FontSizeComboBox.Text) <= 1000)
                    MainRchTxtBox.Selection.ApplyPropertyValue(Paragraph.FontSizeProperty, FontSizeComboBox.Text);
                FontSizeComboBox.Text = Math.Round(Convert.ToDouble(FontSizeComboBox.Text), 2).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Что-то пошло не так: \n" + ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackStepBtn_Click(object sender, RoutedEventArgs e)
        {
            MainRchTxtBox.Undo();
        }

        private void BoldBtn_Click(object sender, RoutedEventArgs e)
        {
            if (BoldBtn.IsChecked == false)
                MainRchTxtBox.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
            else
                MainRchTxtBox.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
        }

        private void ItalicBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ItalicBtn.IsChecked == false)
                MainRchTxtBox.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
            else
                MainRchTxtBox.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
        }

        private void UnderlineBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UnderlineBtn.IsChecked == false)
                MainRchTxtBox.Selection.ApplyPropertyValue(Underline.TextDecorationsProperty, null);
            else
                MainRchTxtBox.Selection.ApplyPropertyValue(Underline.TextDecorationsProperty, TextDecorations.Underline);
        }

        private void AlignLeftBtn_Click(object sender, RoutedEventArgs e)
        {
            AlignCenterBtn.IsChecked = false;
            AlignRightBtn.IsChecked = false;
            AlignJustifyBtn.IsChecked = false;
            if (AlignLeftBtn.IsChecked == false)
            {
                MainRchTxtBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);
                AlignLeftBtn.IsChecked = true;
            }
            else
                MainRchTxtBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);

        }

        private void AlignCenterBtn_Click(object sender, RoutedEventArgs e)
        {
            AlignLeftBtn.IsChecked = false;
            AlignRightBtn.IsChecked = false;

            AlignJustifyBtn.IsChecked = false;
            if (AlignCenterBtn.IsChecked == false)
            {
                MainRchTxtBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);
                AlignLeftBtn.IsChecked = true;
            }
            else
                MainRchTxtBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Center);
        }

        private void AlignRightBtn_Click(object sender, RoutedEventArgs e)
        {
            AlignLeftBtn.IsChecked = false;
            AlignCenterBtn.IsChecked = false;
            AlignJustifyBtn.IsChecked = false;
            if (AlignRightBtn.IsChecked == false)
            {
                MainRchTxtBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);
                AlignLeftBtn.IsChecked = true;
            }
            else
                MainRchTxtBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Right);
        }

        private void AlignJustifyBtn_Click(object sender, RoutedEventArgs e)
        {
            AlignCenterBtn.IsChecked = false;
            AlignRightBtn.IsChecked = false;
            AlignLeftBtn.IsChecked = false;
            if (AlignJustifyBtn.IsChecked == false)
            {
                MainRchTxtBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);
                AlignLeftBtn.IsChecked = true;
            }
            else
                MainRchTxtBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Justify);
        }
        #endregion

        #region Три кнопки действий под Списком с заметками
        //  Создание новой заметки
        private void CreateNoteBtn_Click(object sender, RoutedEventArgs e)
        {
            CreateNote CreateNoteDialog = new CreateNote(PathNotes, AllNotesCollection);
            Nullable<bool> dialogResult = CreateNoteDialog.ShowDialog();

            if (dialogResult.HasValue == true)
            {
                if (CreateNoteDialog.returnvalue == 1)
                {
                    //AllNotesCollection.Clear();
                    AllNotesCollection = GetAllNotes.GetAllNotesCollection(PathNotes);
                    AllNotesListView.ItemsSource = AllNotesCollection;
                }
            }
        }

        //  Удаление одной либо нескольких заметок
        private void DeleteNoteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult result = MessageBox.Show($"Вы действительно хотите удалить {CurrentNoteList.Count()} заметку(ок)?", "Удаление заметок", MessageBoxButton.YesNo, MessageBoxImage.Question);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        foreach (var item in CurrentNoteList)
                        {
                            File.Delete(item.Path);
                        }
                        CurrentNote = null;
                        AllNotesCollection.Clear();
                        AllNotesCollection = GetAllNotes.GetAllNotesCollection(PathNotes);
                        AllNotesListView.ItemsSource = AllNotesCollection;
                        DeleteNoteBtn.Visibility = Visibility.Hidden;
                        SaveNoteBtn.Visibility = Visibility.Hidden;
                        MessageBox.Show($"Успешно удалено {CurrentNoteList.Count()} заметку(ок)", "Удаление заметок", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Что-то пошло не так: \n" + ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //  Сохранение выбранной заметки
        private void SaveNoteBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveNote();
        }
        #endregion

        private void MainRchTxtBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (CurrentNote == null)
            {
                e.Handled = true;
            }
        }

        private void SaveNote()
        {
            if (CurrentNote != null)
            {
                TextRange tr = new TextRange(MainRchTxtBox.Document.ContentStart, MainRchTxtBox.Document.ContentEnd);
                using (FileStream fs = File.Create(CurrentNote.Path))
                {
                    if (Path.GetExtension(CurrentNote.Path).ToLower() == ".rtf")
                    {
                        tr.Save(fs, DataFormats.Rtf);
                    }
                    else
                    {
                        MessageBox.Show($"Файл {CurrentNote.FileName} не может быть сохранен", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            }
        }

        private void MainRchTxtBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.S)
                SaveNote();
        }

        private void MainRchTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
