using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using TicketsApp.Modelos;
using TicketsApp.Servicios;

namespace TicketsApp.Paginas
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PaginaListaTicketsPendientes : ContentPage
	{
		public PaginaListaTicketsPendientes ()
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
            lsvTickets.ItemsSource = await ServicioQueueStorage.ObtenerTickets();
            Loading(false);
        }

        private async void lsvTickets_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                if (e.SelectedItem != null)
                {
                    var item = (Ticket)e.SelectedItem;
                    await Navigation.PushAsync(new PaginaTicketPendiente(item));
                    lsvTickets.SelectedItem = null;
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}