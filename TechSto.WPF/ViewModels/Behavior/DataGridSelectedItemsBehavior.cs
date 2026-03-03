using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace TechSto.WPF.ViewModels.Behavior
{
    //Заложил для работы с DataGrid
    public class DataGridSelectedItemsBehavior : Behavior<DataGrid>
    {
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(
                "SelectedItems",
                typeof(IList),
                typeof(DataGridSelectedItemsBehavior),
                new PropertyMetadata(null));

        public IList SelectedItems
        {
            get => (IList)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += DataGrid_SelectionChanged;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SelectionChanged -= DataGrid_SelectionChanged;
            base.OnDetaching();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedItems == null) return;

            // Очищаем коллекцию и добавляем элементы с проверкой типа
            SelectedItems.Clear();
            foreach (var item in AssociatedObject.SelectedItems)
            {
                // Важно: проверяем тип перед добавлением!
                //if (item is Resource resource)
                //{
                //    SelectedItems.Add(resource);
                //}
            }
        }
    }
}
