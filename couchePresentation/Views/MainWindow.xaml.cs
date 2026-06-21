using System.Windows;
using RefugeAnimaux.couchePresentation.ViewModels;

namespace RefugeAnimaux.couchePresentation.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
