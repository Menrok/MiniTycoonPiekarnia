namespace MiniTycoonPiekarnia.Models;

public class ErrorPopupController
{
    public string? Message { get; private set; }
    public bool Visible { get; private set; }
    public bool FadeOut { get; private set; }

    public event Action? OnChange;

    public async Task ShowError(string message)
    {
        Message = message;
        Visible = true;
        FadeOut = false;
        OnChange?.Invoke();

        await Task.Delay(700);
        FadeOut = true;
        OnChange?.Invoke();

        await Task.Delay(500);
        Visible = false;
        OnChange?.Invoke();
    }
}
