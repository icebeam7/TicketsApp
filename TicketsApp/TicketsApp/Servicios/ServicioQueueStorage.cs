using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using TicketsApp.Helpers;
using TicketsApp.Modelos;

namespace TicketsApp.Servicios
{
    public static class ServicioQueueStorage
    {
        readonly static CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(Constantes.StorageAccountConnectionString);
        readonly static CloudQueueClient queueClient = cloudStorageAccount.CreateCloudQueueClient();
        readonly static CloudQueue queue = queueClient.GetQueueReference(Constantes.Queue_Tickets);

        public static async Task ColocarTicket(Ticket ticket)
        {
            await queue.CreateIfNotExistsAsync();

            var mensaje = JsonConvert.SerializeObject(ticket);
            var cloudQueueMessage = new CloudQueueMessage(mensaje);
            await queue.AddMessageAsync(cloudQueueMessage);
        }

        public static async Task<Ticket> ConsultarTicket()
        {
            await queue.CreateIfNotExistsAsync();

            var mensaje = await queue.PeekMessageAsync();
            //mensaje.ExpirationTime.Value.ToString();
            return JsonConvert.DeserializeObject<Ticket>(mensaje.AsString);
        }

        public static async Task<Ticket> ProcesarTicket()
        {
            await queue.CreateIfNotExistsAsync();

            var cloudQueueMessage = await queue.GetMessageAsync();
            var mensaje = cloudQueueMessage.AsString;
            await queue.DeleteMessageAsync(cloudQueueMessage);

            return JsonConvert.DeserializeObject<Ticket>(mensaje);
        }

        public static async Task<List<Ticket>> ObtenerTickets()
        {
            await queue.CreateIfNotExistsAsync();

            var numeroMensajes = queue.ApproximateMessageCount;
            List<Ticket> tickets = new List<Ticket>();

            if (numeroMensajes.HasValue)
            {
                var mensajes = await queue.GetMessagesAsync(numeroMensajes.Value);

                foreach (var mensaje in mensajes)
                {
                    tickets.Add(JsonConvert.DeserializeObject<Ticket>(mensaje.AsString));
                }
            }

            return tickets;
        }
    }
}
