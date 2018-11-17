using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace TicketsApp.Modelos
{
    public class Ticket : TableEntity
    {
        public string Comentarios { get; set; }
        public string Status { get; set; }
        public string ComprobanteURL { get; set; }
        public string TicketURL { get; set; }

        public Ticket()
        {
            this.RowKey = Guid.NewGuid().ToString();
            this.Status = "Sin comprobante";
        }
    }
}
