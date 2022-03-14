using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MvcAzureBlobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MvcAzureBlobs.Services
{
    public class ServiceStorageBlobs
    {
        private BlobServiceClient client;

        public ServiceStorageBlobs(BlobServiceClient client)
        {
            this.client = client;
        }

        //METODO PARA DEVOLVER TODOS LOS CONTENEDORES
        public async Task<List<string>> GetContainersAsync()
        {
            List<string> containers = new List<string>();
            await foreach (var container in this.client.GetBlobContainersAsync())
            {
                containers.Add(container.Name);
            }
            return containers;
        }

        //METODO PARA CREAR NUEVOS CONTENEDORES
        public async Task CreateContainerAsync(string nombre)
        {
            await this.client.CreateBlobContainerAsync(nombre.ToLower()
                , Azure.Storage.Blobs.Models.PublicAccessType.Blob);
        }

        //METODO PARA ELIMINAR CONTENEDORES
        public async Task DeleteContainerAsync(string nombre)
        {
            await this.client.DeleteBlobContainerAsync(nombre);
        }

        //METODO PARA MOSTRAR TODOS LOS BLOBS DE UN CONTAINER
        public async Task<List<BlobClass>> GetBlobsAsync(string containerName)
        {
            //CUALQUIER ACCION SOBRE UN BLOB DIRECTAMENTE
            //NECESITA UN CLIENTE DE BLOBCONTAINER
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);
            List<BlobClass> blobs = new List<BlobClass>();
            await foreach (BlobItem item in containerClient.GetBlobsAsync())
            {
                //PARA PODER ACCEDER A LA INFORMACION COMPLETA DE UN BLOB
                //NECESITAMOS UN BlobClient A PARTIR DEL NOMBRE DEL BLOB
                BlobClient blobClient = containerClient.GetBlobClient(item.Name);
                BlobClass blobClass = new BlobClass
                {
                    Nombre = item.Name,
                    Url = blobClient.Uri.AbsoluteUri
                };
                blobs.Add(blobClass);
            }
            return blobs;
        }

        //METODO PARA ELIMINAR UN BLOB
        public async Task DeleteBlobAsync(string containerName, string blobName)
        {
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);
            await containerClient.DeleteBlobAsync(blobName);
        }

        //METODO PARA SUBIR BLOB A AZURE
        public async Task UploadBlobAsync(string containerName, string blobName
            , Stream stream)
        {
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);
            await containerClient.UploadBlobAsync(blobName, stream);
        }

    }
}
