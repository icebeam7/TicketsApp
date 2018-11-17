using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using TicketsApp.Helpers;

namespace TicketsApp.Paginas
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PaginaLogin : ContentPage
	{
		public PaginaLogin ()
		{
			InitializeComponent ();
		}

        private async void btnIniciarSesion_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtCliente.Text))
            {
                App.Cliente = txtCliente.Text;
                await DisplayAlert("¡Bienvenido!", $"Bienvenido {App.Cliente}", "OK");

                if (App.Cliente == Constantes.Admin)
                    await Navigation.PushAsync(new PaginaListaTicketsPendientes());
                else
                    await Navigation.PushAsync(new PaginaListaTickets());
            }
            else
            {
                await DisplayAlert("¡Error!", "Datos incorrectos", "OK");
            }
        }
    }
}