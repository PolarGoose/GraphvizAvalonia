using Avalonia.Controls;
using Avalonia.Interactivity;
using Example.GraphsSamples;
using GraphvizAvalonia;
namespace Example;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Opened += (_, _) => RedrawGraph(null, null);
    }

    private void RedrawGraph(object? sender, SelectionChangedEventArgs? e)
    {
        if (GraphCanvas is null || GraphSamplesComboBox is null || LayoutEngineComboBox is null)
        {
            return;
        }

        var graphSample = (GraphSampleBase)GraphSamplesComboBox.SelectedItem!;
        var layoutEngine = (GraphvizLayoutEngine)LayoutEngineComboBox.SelectedItem!;

        GraphCanvas.DrawGraph(graphSample.Description, layoutEngine);
    }

    private void ResetZoom(object? sender, RoutedEventArgs e)
    {
        ZoomBorder.Uniform();
    }
}
