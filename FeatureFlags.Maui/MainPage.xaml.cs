using FeatureFlags.Maui.ViewModels;
namespace FeatureFlags.Maui;

public partial class MainPage : ContentPage
{
    readonly MainViewModel _vm;

    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCommand.ExecuteAsync(null);
    }
}