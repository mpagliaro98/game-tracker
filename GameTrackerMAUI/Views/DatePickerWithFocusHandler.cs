using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

// partial fix to MAUI bug where date picker focused and unfocused events don't fire
// https://github.com/dotnet/maui/issues/12899
// still an issue where changing DatePicker.Date programmatically does not reflect new date on the opened calendar popup
public partial class DatePickerWithFocusHandler : DatePickerHandler
{
    public static PropertyMapper<IDatePicker, DatePickerWithFocusHandler> Mapper = new(DatePickerHandler.Mapper)
    {
        [nameof(IDatePicker.IsFocused)] = MapIsFocused
    };

    public DatePickerWithFocusHandler() : base(Mapper)
    {
    }
}