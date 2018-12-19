using System.Linq;
using Xamarin.Forms;

namespace MultiThreadedLists
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            // create some data
            var dataItems = Enumerable.Range(1, 100)
                .Select(x => new DataItem(x))
                .ToList();

            listview.ItemsSource = dataItems;
        }
    }
}
