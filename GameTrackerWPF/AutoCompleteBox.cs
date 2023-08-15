using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameTrackerWPF
{
    public class AutoCompleteBox : Control
    {
        TextBox inputText;
        ListBox suggestionBox;
        ICollectionView suggestionView;

        public event TextChangedEventHandler TextChanged;

        static AutoCompleteBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoCompleteBox), new FrameworkPropertyMetadata(typeof(AutoCompleteBox)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            inputText = (TextBox)GetTemplateChild("inputText");
            suggestionBox = (ListBox)GetTemplateChild("suggestionBox");
            suggestionView = CollectionViewSource.GetDefaultView(ItemsSource);
            suggestionView.Filter = filterSuggestion;
            inputText.TextChanged += updateSuggestion;
            inputText.KeyUp += onKeyUp;
            inputText.LostFocus += InputText_LostFocus;
            var style = new Style { TargetType = typeof(ListBoxItem) };
            var eventSetter = new EventSetter(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(LeftMouseButtonDown));
            style.Setters.Add(eventSetter);
            suggestionBox.Resources.Add(typeof(ListBoxItem), style);
            SelectItem = new Command(selectItem, (o) => IsSuggestionVisible);
        }

        private void InputText_LostFocus(object sender, RoutedEventArgs e)
        {
            ListBoxItem lbi = suggestionBox.ItemContainerGenerator.ContainerFromIndex(suggestionBox.SelectedIndex) as ListBoxItem;
            IsSuggestionVisible = lbi.IsKeyboardFocused;
        }

        void onKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                if (IsSuggestionVisible)
                {
                    suggestionView.MoveCurrentToFirst();
                    ((ListBoxItem)suggestionBox.ItemContainerGenerator.ContainerFromItem(suggestionView.CurrentItem)).Focus();
                }
            }
        }

        bool filterSuggestion(object o) => (((string)o).ToLower()).Contains(inputText.Text.ToLower());
        void updateSuggestion(object sender, TextChangedEventArgs e)
        {
            suggestionView.Refresh();
            IsSuggestionVisible = !string.IsNullOrWhiteSpace(inputText.Text);
            Text = inputText.Text;
            TextChanged?.Invoke(sender, e);
        }
        void selectItem(object o)
        {
            var text = (string)o;
            inputText.Text = text;
            inputText.CaretIndex = text.Length + 1;
            inputText.Focus();
            IsSuggestionVisible = false;
        }

        void LeftMouseButtonDown(object sender, MouseButtonEventArgs args)
        {
            var lbi = (ListBoxItem)sender;
            selectItem(lbi.Content);
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...  
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(AutoCompleteBox), new PropertyMetadata(null));


        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...  
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(AutoCompleteBox), new PropertyMetadata(null));

        public bool IsSuggestionVisible
        {
            get { return (bool)GetValue(IsSuggestionVisibleProperty); }
            set { SetValue(IsSuggestionVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSuggestionVisible.  This enables animation, styling, binding, etc...  
        public static readonly DependencyProperty IsSuggestionVisibleProperty =
            DependencyProperty.Register("IsSuggestionVisible", typeof(bool), typeof(AutoCompleteBox), new PropertyMetadata(false));


        public ICommand SelectItem
        {
            get { return (ICommand)GetValue(SelectItemProperty); }
            set { SetValue(SelectItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectItem.  This enables animation, styling, binding, etc...  
        public static readonly DependencyProperty SelectItemProperty =
            DependencyProperty.Register("SelectItem", typeof(ICommand), typeof(AutoCompleteBox), new PropertyMetadata(null));
    }

    public class Command : ICommand
    {
        private readonly Action<object> execute;
        private readonly Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public Command(Action<object> execute, Func<object, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }
    }
}