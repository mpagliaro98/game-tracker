using CommunityToolkit.Maui.Views;
using System.Runtime.CompilerServices;

namespace GameTrackerMAUI.Views;

public partial class PopupMain : Popup
{
    public enum EnumInputType { Ok, YesNo, OkCancel, OkCancelWithInput }
    public enum EnumOutputType { Ok, Yes, No, Cancel }
    public Tuple<EnumOutputType, string> ReturnValue;

    public PopupMain(string title, string message, EnumInputType inputType)
	{
		InitializeComponent();

        Size = new Size(300, 150); // default size

        LabelTitle.Text = title;
        LabelMessage.Text = message;

        EntryInput.IsVisible = (inputType == EnumInputType.OkCancelWithInput);
        ButtonNo.IsVisible = (inputType == EnumInputType.YesNo);
        ButtonYes.IsVisible = (inputType == EnumInputType.YesNo);
        ButtonCancel.IsVisible = (inputType == EnumInputType.OkCancel) || (inputType == EnumInputType.OkCancelWithInput);
        ButtonOk.IsVisible = (inputType == EnumInputType.Ok) || (inputType == EnumInputType.OkCancel) || (inputType == EnumInputType.OkCancelWithInput);
	}

    private void ButtonNo_Clicked(object sender, EventArgs e)
    {
        ClosePopup(EnumOutputType.No, EntryInput.Text);
    }

    private void ButtonYes_Clicked(object sender, EventArgs e)
    {
        ClosePopup(EnumOutputType.Yes, EntryInput.Text);
    }

    private void ButtonCancel_Clicked(object sender, EventArgs e)
    {
        ClosePopup(EnumOutputType.Cancel, EntryInput.Text);
    }

    private void ButtonOk_Clicked(object sender, EventArgs e)
    {
        ClosePopup(EnumOutputType.Ok, EntryInput.Text);
    }

    private void ClosePopup(EnumOutputType outputType, string inputValue)
    {
        Close(new Tuple<EnumOutputType, string>(outputType, inputValue));
    }
}