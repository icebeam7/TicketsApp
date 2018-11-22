using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

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

            // You de-queue a message in two steps. 
            // Call GetMessage at which point the message becomes 
            // invisible to any other code reading messages 
            // from this queue for a default period of 30 seconds. 
            // To finish removing the message from the queue, you call DeleteMessage. 
            // This two-step process ensures that if your code fails to process a message 
            // due to hardware or software failure, another instance 
            // of your code can get the same message and try again. 

            var cloudQueueMessage = await queue.GetMessageAsync();
            var mensaje = cloudQueueMessage.AsString;
            await queue.DeleteMessageAsync(cloudQueueMessage);

            return JsonConvert.DeserializeObject<Ticket>(mensaje);
        }

        public static async Task<List<Ticket>> ObtenerTickets()
        {
            await queue.CreateIfNotExistsAsync();
            await queue.FetchAttributesAsync();

            var numeroMensajes = queue.ApproximateMessageCount;
            List<Ticket> tickets = new List<Ticket>();

            if (numeroMensajes.HasValue)
            {
                if (numeroMensajes.Value > 0)
                {
                    var mensajes = await queue.GetMessagesAsync(numeroMensajes.Value);

                    foreach (var mensaje in mensajes)
                    {
                        tickets.Add(JsonConvert.DeserializeObject<Ticket>(mensaje.AsString));
                        //await queue.DeleteMessageAsync(x);
                    }
                }
            }

            return tickets;
        }
    }
}
