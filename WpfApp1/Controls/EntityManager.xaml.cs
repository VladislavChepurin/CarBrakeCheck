using System.Collections;
using System.Windows;
using System.Windows.Controls;

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

        public static readonly DependencyProperty NameElementProperty =
            DependencyProperty.Register("NameElement", typeof(string), typeof(EntityManager), new PropertyMetadata(""));

        // DependencyProperty: ItemsSource (коллекция элементов)
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(EntityManager), new PropertyMetadata(null));

        // DependencyProperty: SelectedItem (текущий выбранный элемент)
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(EntityManager), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        // DependencyProperty: DisplayMemberPath (имя свойства для отображения)
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(EntityManager), new PropertyMetadata(""));

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

        public string NameElement
        {
            get => (string)GetValue(NameElementProperty);
            set => SetValue(NameElementProperty, value);
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

        // События для операций
        public event EventHandler<AddEventArgs> AddRequested;
        public event EventHandler<EditEventArgs> EditRequested;
        public event EventHandler<DeleteEventArgs> DeleteRequested;

        public EntityManager()
        {
            InitializeComponent();
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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string text = InputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Введите название.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            AddRequested?.Invoke(this, new AddEventArgs(text));
            InputTextBox.Clear();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("Выберите элемент для редактирования.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string text = InputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Введите название.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            EditRequested?.Invoke(this, new EditEventArgs(SelectedItem, text));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null)
            {
                MessageBox.Show("Выберите элемент для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var result = MessageBox.Show("Удалить выбранный элемент?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                DeleteRequested?.Invoke(this, new DeleteEventArgs(SelectedItem));
                InputTextBox.Clear();
            }
        }
    }

    // Классы аргументов событий
    public class AddEventArgs : EventArgs
    {
        public string Name { get; }
        public AddEventArgs(string name) => Name = name;
    }

    public class EditEventArgs : EventArgs
    {
        public object Item { get; }
        public string NewName { get; }
        public EditEventArgs(object item, string newName) { Item = item; NewName = newName; }
    }

    public class DeleteEventArgs : EventArgs
    {
        public object Item { get; }
        public DeleteEventArgs(object item) => Item = item;
    }
}