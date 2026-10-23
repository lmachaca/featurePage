using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FeatureFlags.Maui.Services;


namespace FeatureFlags.Maui.ViewModels;

public partial class MainViewModel(FlagService service) : ObservableObject
{
    [ObservableProperty] bool showNotificaciones;
    [ObservableProperty] bool showModoOscuro;
    [ObservableProperty] bool showUbicacion;
    [ObservableProperty] string estado = "Cargando…";

    // Extra: el switch de modo oscuro sí cambia el tema de la app
    [ObservableProperty] bool modoOscuroOn;

    // El toolkit llama este método cada vez que cambia ModoOscuroOn
    partial void OnModoOscuroOnChanged(bool value) =>
        Application.Current!.UserAppTheme = value ? AppTheme.Dark : AppTheme.Light;

    // [RelayCommand] genera "LoadCommand" para el botón "Recargar flags"
    [RelayCommand]
    async Task LoadAsync()
    {
        try
        {
            // 1) Pedir los flags al API
            var flags = await service.GetFlagsAsync();

            // 2) Función corta: ¿existe ese flag y está en true?
            bool On(string name) => flags.Any(f => f.Name == name && f.IsEnabled);

            // 3) Al cambiar estas propiedades, la vista se actualiza sola
            ShowNotificaciones = On("Notificaciones");
            ShowModoOscuro = On("ModoOscuro");
            ShowUbicacion = On("Ubicacion");

            // Si apagan el flag, regresamos al tema claro
            if (!ShowModoOscuro) ModoOscuroOn = false;

            Estado = $"Conectado · {flags.Count(f => f.IsEnabled)} de {flags.Count} activos";
        }
        catch (Exception ex)
        {
            // Falla segura: si el API no responde, todo queda oculto
            ShowNotificaciones = ShowModoOscuro = ShowUbicacion = false;
           
        }
    }
}