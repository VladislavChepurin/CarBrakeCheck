using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TechSto.WPF.Controls
{
    public partial class ToggleSwitch : UserControl
    {
        private bool _isDragging;
        private bool _moved;
        private Point _dragStart;
        private double _startOffset;
        private double MaxOffset => 160 - 80 - 4; // зависит от размеров в XAML

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(
                nameof(IsChecked),
                typeof(bool),
                typeof(ToggleSwitch),
                new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnIsCheckedChanged));

        public ToggleSwitch()
        {
            InitializeComponent();
        }

        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ToggleSwitch)d;
            control.UpdateSlider();
        }

        private void UpdateSlider()
        {
            // Устанавливаем положение ползунка в соответствии с IsChecked
            SliderTransform.X = IsChecked ? MaxOffset : 0;
        }

        private void ModeSwitch_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _moved = false;
            _dragStart = e.GetPosition(ModeSwitch);
            _startOffset = SliderTransform.X;
            ModeSwitch.CaptureMouse();
        }

        private void ModeSwitch_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging) return;

            var pos = e.GetPosition(ModeSwitch);
            double delta = pos.X - _dragStart.X;

            if (Math.Abs(delta) > 2)
                _moved = true;

            double newX = _startOffset + delta;
            newX = Math.Max(0, Math.Min(MaxOffset, newX));
            SliderTransform.X = newX;
        }

        private void ModeSwitch_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isDragging) return;

            _isDragging = false;
            ModeSwitch.ReleaseMouseCapture();

            // Если мышь не двигалась — это клик
            if (!_moved)
            {
                Toggle();
                return;
            }

            // Определяем, в какую сторону отпустили ползунок
            double middle = MaxOffset / 2;
            bool newCheckedState = SliderTransform.X > middle;

            // Устанавливаем финальную позицию (принудительно в край)
            SliderTransform.X = newCheckedState ? MaxOffset : 0;
            IsChecked = newCheckedState; // Обновляем свойство
        }

        private void Toggle()
        {
            // Меняем состояние на противоположное
            IsChecked = !IsChecked;
            // UpdateSlider вызовется автоматически через OnIsCheckedChanged
        }
    }
}