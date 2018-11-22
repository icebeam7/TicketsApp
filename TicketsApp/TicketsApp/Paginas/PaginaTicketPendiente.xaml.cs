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
	public partial class PaginaTicketPendiente : ContentPage
	{
		public PaginaTicketPendiente (Ticket ticket)
		{
			InitializeComponent ();

            this.BindingContext = ticket;
            this.ticket = ticket;
        }

        MediaFile foto;
        Ticket ticket;

        void Loading(bool mostrar)
        {
            indicator.IsEnabled = mostrar;
            indicator.IsRunning = mostrar;
        }

        private async void btnGenerarTicket_Clicked(object sender, EventArgs e)
        {
            try
            {
                Loading(true);

                foto = await ServicioImagen.TomarFoto();

                if (foto != null)
                {
                    imagenComprobante.Source = ImageSource.FromStream(foto.GetStream);

                    var extension = Path.GetExtension(foto.Path);

                    var uri = await ServicioBlobStorage.SubirBlob(Constantes.Container_Tickets, foto.GetStream(), extension);

                    if (uri != string.Empty)
                    {
                        ticket.TicketURL = uri;
                        ticket.Status = txtStatus.Text;

                        var registro = await ServicioTableStorage.GuardarTicket(ticket);

                        if (registro)
                        {
                            await ServicioQueueStorage.ProcesarTicket();

                            await DisplayAlert("¡Muy bien!", "Ticket generado y procesado exitosamente", "OK");
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

        private async void btnGuardarObservaciones_Clicked(object sender, EventArgs e)
        {
            try
            {
                Loading(true);

                ticket.Status = txtStatus.Text;

                var registro = await ServicioTableStorage.GuardarTicket(ticket);

                if (registro)
                {
                    await ServicioQueueStorage.ProcesarTicket();
                    await DisplayAlert("¡Muy bien!", "Ticket procesado, pero no generado.", "OK");
                    await Navigation.PopAsync();
                }
                else
                    await DisplayAlert("Error", "Excepción: Hubo un error al registrar la información en la tabla", "OK");
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
    }
}