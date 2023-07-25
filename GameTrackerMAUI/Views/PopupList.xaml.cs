using CommunityToolkit.Maui.Views;
using GameTrackerMAUI.Model;

namespace GameTrackerMAUI.Views;

public partial class PopupList : Popup
{
    public enum EnumOutputType { Cancel, Selection }

    public Command<PopupListOption> ItemTapped { get; }
    public object SelectedValue { get; set; }

    public PopupList(string title, IEnumerable<PopupListOption> options, object selectedValue)
    {
        InitializeComponent();

        Size = new Size(300, 500); // default size

        SelectedValue = selectedValue;
        LabelTitle.Text = title;
        ItemList.ItemsSource = options;
        ItemList.SelectedItem = selectedValue;

        ItemTapped = new Command<PopupListOption>(OnItemSelected);
    }

    private void CancelButton_Clicked(object sender, EventArgs e)
    {
        ClosePopup(EnumOutputType.Cancel, null);
    }

    void OnItemSelected(PopupListOption item)
    {
        ClosePopup(EnumOutputType.Selection, item.Value);
    }

    private void ClosePopup(EnumOutputType outputType, object selectedValue)
    {
        Close(new Tuple<EnumOutputType, object>(outputType, selectedValue));
    }
}