using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using TicketsApp.Modelos;
using TicketsApp.Servicios;

namespace TicketsApp.Paginas
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PaginaListaTickets : ContentPage
	{
		public PaginaListaTickets ()
		{
			InitializeComponent ();
		}

        void Loading(bool show)
        {
            indicator.IsEnabled = show;
            indicator.IsRunning = show;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            Loading(true);
            lsvTickets.ItemsSource = await ServicioTableStorage.ObtenerTickets(App.Cliente);
            Loading(false);
        }

        private async void lsvTickets_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                if (e.SelectedItem != null)
                {
                    var item = (Ticket)e.SelectedItem;
                    await Navigation.PushAsync(new PaginaTicket(item));
                    lsvTickets.SelectedItem = null;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async void btnSolicitarTicket_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PaginaTicket(new Ticket()));
        }
    }
}