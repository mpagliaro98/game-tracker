#if ANDROID
using Android.App;
using Microsoft.Maui.Platform;

// partial fix to MAUI bug where date picker focused and unfocused events don't fire
// https://github.com/dotnet/maui/issues/12899
// still an issue where changing DatePicker.Date programmatically does not reflect new date on the opened calendar popup
public partial class DatePickerWithFocusHandler
{
    public static void MapIsFocused(DatePickerWithFocusHandler handler, IDatePicker datePicker)
    {
        if (handler.PlatformView.IsFocused == datePicker.IsFocused) return;

        if (datePicker.IsFocused)
        {
            handler.PlatformView.RequestFocus();
        }
        else
        {
            handler.PlatformView.ClearFocus();
        }
    }

    private DatePickerDialog? _dialog;

    protected override DatePickerDialog CreateDatePickerDialog(int year, int month, int day)
    {
        _dialog = base.CreateDatePickerDialog(year, month, day);
        return _dialog;
    }

    protected override void ConnectHandler(MauiDatePicker platformView)
    {
        base.ConnectHandler(platformView);
        if (_dialog != null)
        {
            _dialog.ShowEvent += OnDialogShown;
            _dialog.DismissEvent += OnDialogDismissed;
        }
    }

    protected override void DisconnectHandler(MauiDatePicker platformView)
    {
        if (_dialog != null)
        {
            _dialog.ShowEvent -= OnDialogShown;
            _dialog.DismissEvent -= OnDialogDismissed;
        }
        base.DisconnectHandler(platformView);

        _dialog = null;
    }

    private void OnDialogShown(object sender, EventArgs e)
    {
        this.VirtualView.IsFocused = true;
    }

    private void OnDialogDismissed(object sender, EventArgs e)
    {
        this.VirtualView.IsFocused = false;
    }
}
#endif