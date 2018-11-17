using System;
using System.IO;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Media.Abstractions;

using TicketsApp.Helpers;
using TicketsApp.Modelos;
using TicketsApp.Servicios;

namespace TicketsApp.Paginas
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PaginaTicket : ContentPage
	{
        public PaginaTicket (Ticket ticket)
		{
			InitializeComponent ();

            this.BindingContext = ticket;
            this.ticket = ticket;

            if (ticket.PartitionKey == string.Empty)
            {
                ToolbarItems.RemoveAt(1);
                ToolbarItems.RemoveAt(1);
                ToolbarItems.RemoveAt(1);
            }
            else
            {
                if (ticket.TicketURL == string.Empty)
                {
                    ToolbarItems.RemoveAt(2);
                }

                if (ticket.ComprobanteURL == string.Empty)
                {
                    ToolbarItems.RemoveAt(1);
                }
            }
        }

        MediaFile foto;
        Ticket ticket;

        void Loading(bool mostrar)
        {
            indicator.IsEnabled = mostrar;
            indicator.IsRunning = mostrar;
        }

        private async void btnSubirComprobante_Clicked(object sender, EventArgs e)
        {
            try
            {
                Loading(true);

                foto = await ServicioImagen.TomarFoto();

                if (foto != null)
                {
                    imagenComprobante.Source = ImageSource.FromStream(foto.GetStream);

                    var extension = Path.GetExtension(foto.Path);

                    var uri = await ServicioBlobStorage.SubirBlob(Constantes.Container_Comprobantes, foto.GetStream(), extension);

                    if (uri != string.Empty)
                    {
                        ticket.PartitionKey = App.Cliente;
                        ticket.ComprobanteURL = uri;
                        ticket.Status = "Verificando comprobante...";

                        var registro = await ServicioTableStorage.GuardarTicket(ticket);

                        if (registro)
                        {
                            await DisplayAlert("¡Muy bien!", "Comprobante almacenado exitosamente", "OK");
                            await Navigation.PopAsync();
                        }
                        else
                            await DisplayAlert("Error", "Excepción: Hubo un error al registrar la información en la tabla", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", "Excepción: Hubo un error al registrar la información en el blob", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Excepción: " + ex.Message, "OK");
            }
            finally
            {
                Loading(false);
            }
        }

        async void btnBorrarComprobante_Clicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("¡Atención!", "¿Realmente quieres eliminar este item?", "Si", "No"))
            {
                Loading(true);

                var borrarComprobante = await ServicioBlobStorage.BorrarBlob(Constantes.Container_Comprobantes, ticket.ComprobanteURL);
                var borrarTicket = await ServicioBlobStorage.BorrarBlob(Constantes.Container_Tickets, ticket.TicketURL);
                var borrarRegistro = await ServicioTableStorage.BorrarTicket(ticket);

                Loading(false);

                if (borrarComprobante && borrarTicket && borrarRegistro)
                {
                    await DisplayAlert("¡Ticket borrado!", "¡El ticket no existe más!", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("¡Error!", "¡El ticket no fue eliminado!", "OK");
                }
            }
        }

        private void btnVerComprobante_Clicked(object sender, EventArgs e)
        {
            if (ticket.ComprobanteURL != string.Empty)
                imagenComprobante.Source = ImageSource.FromUri(new Uri(ticket.ComprobanteURL));
        }

        private void btnVerTicket_Clicked(object sender, EventArgs e)
        {
            if (ticket.TicketURL != string.Empty)
                imagenTicket.Source = ImageSource.FromUri(new Uri(ticket.TicketURL));
        }
    }
}