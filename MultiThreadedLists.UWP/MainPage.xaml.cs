using Windows.UI.Xaml.Controls;
using Xamarin.Forms.Platform.UWP;

namespace MultiThreadedLists.UWP
{
    public sealed partial class MainPage : WindowsPage
    {
        public MainPage()
        {
            InitializeComponent();

            LoadApplication(new MultiThreadedLists.App());
        }
    }
}
