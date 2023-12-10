#if IOS
using Microsoft.Maui.Platform;

// partial fix to MAUI bug where date picker focused and unfocused events don't fire
// https://github.com/dotnet/maui/issues/12899
// still an issue where changing DatePicker.Date programmatically does not reflect new date on the opened calendar popup
public partial class DatePickerWithFocusHandler
{
    public static void MapIsFocused(DatePickerWithFocusHandler handler, IDatePicker datePicker)
    {
        if (handler.PlatformView.Focused == datePicker.IsFocused) return;

        if (datePicker.IsFocused)
        {
            handler.PlatformView.BecomeFirstResponder();
        }
        else
        {
            handler.PlatformView.ResignFirstResponder();
        }
    }

    protected override void ConnectHandler(MauiDatePicker platformView)
    {
        base.ConnectHandler(platformView);
        platformView.EditingDidBegin += OnEditingDidBegin;
        platformView.EditingDidEnd += OnEditingDidEnd;
    }

    private void OnEditingDidBegin(object sender, EventArgs e)
    {
        this.VirtualView.IsFocused = true;
    }

    private void OnEditingDidEnd(object sender, EventArgs e)
    {
        this.VirtualView.IsFocused = false;
    }
}
#endif