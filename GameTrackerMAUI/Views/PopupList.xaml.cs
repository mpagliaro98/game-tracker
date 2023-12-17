using CommunityToolkit.Maui.Views;
using GameTrackerMAUI.Model;

namespace GameTrackerMAUI.Views;

public partial class PopupList : Popup
{
    public enum EnumOutputType { Cancel, Selection }

    private Action<PopupListOption, int> doubleTap;

    public Command<PopupListOption> ItemTapped { get; }
    public Command<PopupListOption> ItemDoubleTapped { get; }
    public object SelectedValue { get; set; }

    public PopupList(string title, IEnumerable<PopupListOption> options, object selectedValue, Action<PopupListOption, int> doubleTap = null)
    {
        InitializeComponent();
        this.doubleTap = doubleTap;

        Size = new Size(300, 500); // default size

        SelectedValue = selectedValue;
        LabelTitle.Text = title;
        ItemList.ItemsSource = options;
        ItemList.SelectedItem = selectedValue;

        ItemTapped = new Command<PopupListOption>(OnItemSelected);
        ItemDoubleTapped = new Command<PopupListOption>(OnItemDoubleTapped);
    }

    private void CancelButton_Clicked(object sender, EventArgs e)
    {
        ClosePopup(EnumOutputType.Cancel, null);
    }

    void OnItemSelected(PopupListOption item)
    {
        ClosePopup(EnumOutputType.Selection, item.Value);
    }

    void OnItemDoubleTapped(PopupListOption item)
    {
        int index = 0;
        foreach (PopupListOption listItem in (ItemList.ItemsSource as IEnumerable<PopupListOption>))
        {
            if (listItem.Equals(item))
                break;
            index++;
        }
        if (doubleTap != null)
        {
            doubleTap?.Invoke(item, index);
            ClosePopup(EnumOutputType.Cancel, null);
        }
    }

    private void ClosePopup(EnumOutputType outputType, object selectedValue)
    {
        Close(new Tuple<EnumOutputType, object>(outputType, selectedValue));
    }
}