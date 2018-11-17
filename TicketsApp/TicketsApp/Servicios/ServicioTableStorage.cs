using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

using TicketsApp.Helpers;
using TicketsApp.Modelos;

namespace TicketsApp.Servicios
{
    public class ServicioTableStorage
    {
        readonly static CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(Constantes.StorageAccountConnectionString);
        readonly static CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
        readonly static CloudTable ticketsTable = tableClient.GetTableReference(Constantes.Table_Tickets);

        public static async Task<List<Ticket>> ObtenerTickets(string partitionKey)
        {
            TableQuery<Ticket> query = new TableQuery<Ticket>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", 
                QueryComparisons.Equal, partitionKey));

            TableContinuationToken continuationToken = null;
            List<Ticket> tickets = new List<Ticket>();

            try
            {
                do
                {
                    var resultado = await ticketsTable.ExecuteQuerySegmentedAsync(query, continuationToken);
                    continuationToken = resultado.ContinuationToken;

                    if (resultado.Results.Count > 0)
                        tickets.AddRange(resultado.Results);
                } while (continuationToken != null);
            }
            catch(Exception ex)
            {

            }

            return tickets;
        }

        public static async Task<Ticket> ObtenerTicket(string partitionKey, string rowKey)
        {
            var operacion = TableOperation.Retrieve<Ticket>(partitionKey, rowKey);
            var query = await ticketsTable.ExecuteAsync(operacion);
            return query.Result as Ticket;
        }

        public static async Task<bool> GuardarTicket(Ticket ticket)
        {
            var operacion = TableOperation.InsertOrMerge(ticket);
            var upsert = await ticketsTable.ExecuteAsync(operacion);
            var code = upsert.HttpStatusCode;

            return (code == 204);
            //return (code >= 200 && code < 300);
        }

        public static async Task<bool> BorrarTicket(Ticket ticket)
        {
            var operacion = TableOperation.Delete(ticket);
            var upsert = await ticketsTable.ExecuteAsync(operacion);
            var code = upsert.HttpStatusCode;

            return (code == 204);
            //return (code >= 200 && code < 300);
        }
    }
}