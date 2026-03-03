using System.Windows.Input;

namespace TechSto.WPF.ViewModels.Behavior
{
    //Заложил для фикса поведения DataGrid при нажатии клавиши Enter
    public class EnterKeyArgs
    {
        public object SelectedItem { get; }
        public KeyEventArgs KeyArgs { get; }

        public EnterKeyArgs(object item, KeyEventArgs args)
        {
            SelectedItem = item;
            KeyArgs = args;
        }
    }
}

