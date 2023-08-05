using GameTrackerMAUI.ViewModels;

namespace GameTrackerMAUI.Views;

public partial class FilterPage : ContentPage
{
    private readonly BaseViewModel _viewModel;

    public FilterPage(FilterViewModel vm)
	{
		InitializeComponent();
        BindingContext = _viewModel = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing();
    }

#if ANDROID
    // workaround for maui bug with secondary toolbar items
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        var secondayToolBarItems = ToolbarItems.Where(x => x.Order == ToolbarItemOrder.Secondary).ToList();

        foreach (var item in secondayToolBarItems)
        {
            ToolbarItems.Remove(item);
        }

        Application.Current.Dispatcher.Dispatch(() =>
        {
            foreach (var item in secondayToolBarItems)
            {
                ToolbarItems.Add(item);
            }
        });
    }
#endif
}