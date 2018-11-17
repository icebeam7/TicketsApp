using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TicketsApp
{
    public partial class App : Application
    {
        public static string Cliente { get; set; }

        public App()
        {
            InitializeComponent();

            //MainPage = new NavigationPage(new Paginas.PaginaLogin());
            MainPage = new NavigationPage(new Paginas.PaginaLogin());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
