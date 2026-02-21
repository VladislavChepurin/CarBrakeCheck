using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace WpfApp1.Controls
{
    public partial class EntityManager : UserControl
    {
        // DependencyProperty: Title
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(EntityManager), new PropertyMetadata(""));

        public static readonly DependencyProperty ButtonAddProperty =
            DependencyProperty.Register("ButtonAdd", typeof(string), typeof(EntityManager), new PropertyMetadata(""));

        public static readonly DependencyProperty ButtonDeleteProperty =
            DependencyProperty.Register("ButtonDelete", typeof(string), typeof(EntityManager), new PropertyMetadata(""));

        public static readonly DependencyProperty ButtonUpdateProperty =
            DependencyProperty.Register("ButtonUpdate", typeof(string), typeof(EntityManager), new PropertyMetadata(""));

        public static readonly DependencyProperty ButtonChooseProperty =
          DependencyProperty.Register("ButtonChoose", typeof(string), typeof(EntityManager), new PropertyMetadata(""));

        public static readonly DependencyProperty NameElementProperty =
            DependencyProperty.Register("NameElement", typeof(string), typeof(EntityManager), new PropertyMetadata(""));

        public static readonly DependencyProperty MessageWarningProperty =
          DependencyProperty.Register("MessageWarning", typeof(string), typeof(EntityManager), new PropertyMetadata(""));

        public static readonly DependencyProperty EnterNameProperty =
         DependencyProperty.Register("EnterName", typeof(string), typeof(EntityManager), new PropertyMetadata(""));

        public static readonly DependencyProperty SelectItemToDeleteProperty =
         DependencyProperty.Register("SelectItemToDelete", typeof(string), typeof(EntityManager), new PropertyMetadata(""));

        public static readonly DependencyProperty SelectItemToEditProperty =
         DependencyProperty.Register("SelectItemToEdit", typeof(string), typeof(EntityManager), new PropertyMetadata(""));

        public static readonly DependencyProperty DeleteSelectedItemProperty =
         DependencyProperty.Register("DeleteSelectedItem", typeof(string), typeof(EntityManager), new PropertyMetadata(""));
              
        // DependencyProperty: ItemsSource (коллекция элементов)
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(EntityManager), new PropertyMetadata(null));

        // DependencyProperty: SelectedItem (текущий выбранный элемент)
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(EntityManager), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        // DependencyProperty для сообщения о необходимости выбора элемента
        public static readonly DependencyProperty SelectItemToChooseProperty =
            DependencyProperty.Register("SelectItemToChoose", typeof(string), typeof(EntityManager), new PropertyMetadata(""));

        // DependencyProperty для кнопки Add
        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register("AddCommand", typeof(ICommand), typeof(EntityManager), new PropertyMetadata(null));

        // DependencyProperty для кнопки Edit
        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register("EditCommand", typeof(ICommand), typeof(EntityManager), new PropertyMetadata(null));

        // DependencyProperty для кнопки Delete
        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(EntityManager), new PropertyMetadata(null));

        // DependencyProperty для кнопки Choose
        public static readonly DependencyProperty ChooseCommandProperty =
            DependencyProperty.Register("ChooseCommand", typeof(ICommand), typeof(EntityManager), new PropertyMetadata(null));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string ButtonAdd
        {
            get => (string)GetValue(ButtonAddProperty);
            set => SetValue(ButtonAddProperty, value);
        }

        public string ButtonDelete
        {
            get => (string)GetValue(ButtonDeleteProperty);
            set => SetValue(ButtonDeleteProperty, value);
        }

        public string ButtonUpdate
        {
            get => (string)GetValue(ButtonUpdateProperty);
            set => SetValue(ButtonUpdateProperty, value);
        }
        
        public string ButtonChoose
        {
            get => (string)GetValue(ButtonChooseProperty);
            set => SetValue(ButtonChooseProperty, value);
        }

        public string NameElement
        {
            get => (string)GetValue(NameElementProperty);
            set => SetValue(NameElementProperty, value);
        }
        public string MessageWarning
        {
            get => (string)GetValue(MessageWarningProperty);
            set => SetValue(MessageWarningProperty, value);
        }

        public string EnterName
        {
            get => (string)GetValue(EnterNameProperty);
            set => SetValue(EnterNameProperty, value);
        }

        public string SelectItemToDelete
        {
            get => (string)GetValue(SelectItemToDeleteProperty);
            set => SetValue(SelectItemToDeleteProperty, value);
        }

        public string SelectItemToEdit
        {
            get => (string)GetValue(SelectItemToEditProperty);
            set => SetValue(SelectItemToEditProperty, value);
        }

        public string DeleteSelectedItem
        {
            get => (string)GetValue(DeleteSelectedItemProperty);
            set => SetValue(DeleteSelectedItemProperty, value);
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public string DisplayMemberPath
        {
            get => (string)GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }

        public string SelectItemToChoose
        {
            get => (string)GetValue(SelectItemToChooseProperty);
            set => SetValue(SelectItemToChooseProperty, value);
        }

        public ICommand AddCommand
        {
            get => (ICommand)GetValue(AddCommandProperty);
            set => SetValue(AddCommandProperty, value);
        }

        public ICommand EditCommand
        {
            get => (ICommand)GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }

        public ICommand DeleteCommand
        {
            get => (ICommand)GetValue(DeleteCommandProperty);
            set => SetValue(DeleteCommandProperty, value);
        }

        public ICommand ChooseCommand
        {
            get => (ICommand)GetValue(ChooseCommandProperty);
            set => SetValue(ChooseCommandProperty, value);
        }

        public EntityManager()
        {
            InitializeComponent();
            Loaded += EntityManager_Loaded;
        }

        private void EntityManager_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateDataGridColumn();
        }

        // Обновляем колонку при изменении DisplayMemberPath
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(EntityManager),
                new PropertyMetadata("", OnDisplayMemberPathChanged));

        private static void OnDisplayMemberPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EntityManager)d).UpdateDataGridColumn();
        }

        private void UpdateDataGridColumn()
        {
            DataGrid.Columns.Clear();

            if (string.IsNullOrEmpty(DisplayMemberPath))
                return;

            var column = new DataGridTextColumn
            {
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                Binding = new Binding(DisplayMemberPath) { Mode = BindingMode.OneWay } // IsReadOnly=true, поэтому OneWay
            };
            DataGrid.Columns.Add(column);
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedItem != null && !string.IsNullOrEmpty(DisplayMemberPath))
            {
                var prop = SelectedItem.GetType().GetProperty(DisplayMemberPath);
                if (prop != null)
                    InputTextBox.Text = prop.GetValue(SelectedItem)?.ToString() ?? "";
            }
            else
            {
                InputTextBox.Text = "";
            }
        }

        // Обработчик двойного клика на DataGrid
        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (SelectedItem != null)
            {
                if (ChooseCommand?.CanExecute(SelectedItem) == true)
                    ChooseCommand.Execute(SelectedItem);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string text = InputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show(EnterName, MessageWarning, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (AddCommand?.CanExecute(text) == true)
                AddCommand.Execute(text);
            InputTextBox.Clear();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null)
            {
                MessageBox.Show(SelectItemToEdit, MessageWarning, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string text = InputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show(EnterName, MessageWarning, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Передаём текущий элемент и новое имя как параметр
            var param = new EditCommandParameter { Item = SelectedItem, NewName = text };
            if (EditCommand?.CanExecute(param) == true)
                EditCommand.Execute(param);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null)
            {
                MessageBox.Show(SelectItemToDelete, MessageWarning, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var result = MessageBox.Show(DeleteSelectedItem, MessageWarning, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                if (DeleteCommand?.CanExecute(SelectedItem) == true)
                    DeleteCommand.Execute(SelectedItem);
                InputTextBox.Clear();
            }
        }

        private void ChooseButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null)
            {
                MessageBox.Show(SelectItemToChoose, MessageWarning, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (ChooseCommand?.CanExecute(SelectedItem) == true)
                ChooseCommand.Execute(SelectedItem);
        }
    }

    public class EditCommandParameter
    {
        public object Item { get; set; }
        public string NewName { get; set; }
    }

}