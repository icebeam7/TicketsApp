using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

using TicketsApp.Helpers;

namespace TicketsApp.Servicios
{
    public static class ServicioBlobStorage
    {
        readonly static CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(Constantes.StorageAccountConnectionString);
        readonly static CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
        readonly static CloudBlobContainer comprobantesContainer = blobClient.GetContainerReference(Constantes.Container_Comprobantes);
        readonly static CloudBlobContainer ticketsContainer = blobClient.GetContainerReference(Constantes.Container_Tickets);

        public static async Task<string> SubirBlob(string tipo, Stream archivo, string extension, string nombre = "")
        {
            var contenedor =
                (tipo == Constantes.Container_Comprobantes) ? comprobantesContainer :
                (tipo == Constantes.Container_Tickets) ? ticketsContainer : null;

            await contenedor.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Container, null, null);

            if (contenedor != null)
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    nombre = $"{Guid.NewGuid().ToString()}{extension}";

                var blob = contenedor.GetBlockBlobReference(nombre);
                await blob.UploadFromStreamAsync(archivo);

                return blob.Uri.AbsoluteUri;
            }

            return string.Empty;
        }

        public static async Task<bool> BorrarBlob(string tipo, string archivo)
        {
            var contenedor =
                (tipo == Constantes.Container_Comprobantes) ? comprobantesContainer :
                (tipo == Constantes.Container_Tickets) ? ticketsContainer : null;

            await contenedor.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Container, null, null);

            if (contenedor != null)
            {
                var nombre = Path.GetFileName(archivo);
                var blob = contenedor.GetBlockBlobReference(nombre);
                return await blob.DeleteIfExistsAsync();
            }

            return true;
        }
    }
}
