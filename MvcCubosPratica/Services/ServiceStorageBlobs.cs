﻿using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using MvcCubosPratica.Models;

namespace MvcCubosPratica.Services
{
    public class ServiceStorageBlobs
    {
        private BlobServiceClient client;

        public ServiceStorageBlobs(BlobServiceClient client)
        {
            this.client = client;
        }

       
       

        //METODO PARA ELIMINAR CONTENEDORES
        public async Task DeleteContainerAsync(string containerName)
        {
            await this.client.DeleteBlobContainerAsync(containerName);
        }


        public async Task<string> GetBlobUriAsync(string container, string blobName)
        {
            BlobContainerClient containerClient = client.GetBlobContainerClient(container);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);



            var response = await containerClient.GetPropertiesAsync();
            var properties = response.Value;



            // Will be private if it's None
            if (properties.PublicAccess == Azure.Storage.Blobs.Models.PublicAccessType.None)
            {
                Uri imageUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddSeconds(3600));
                return imageUri.ToString();
            }



            return blobClient.Uri.AbsoluteUri.ToString();
        }
     


        //METODO PARA RECUPERAR TODOS LOS BLOBS
        public async Task<List<BlobModel>> GetBlobsAsync
            (string containerName)
        {
            //RECUPERAMOS UN CLIENT DEL CONTAINER
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);
            List<BlobModel> blobModels = new List<BlobModel>();
            await foreach (BlobItem item in containerClient.GetBlobsAsync())
            {
                //NECESITAMOS UN BLOB CLIENT PARA VISUALIZAR MAS 
                //CARACTERISTICAS DEL OBJETO
                BlobClient blobClient =
                    containerClient.GetBlobClient(item.Name);
                BlobModel model = new BlobModel();
                model.Nombre = item.Name;
                model.Contenedor = containerName;
                model.Url = blobClient.Uri.AbsoluteUri;
                blobModels.Add(model);
            }
            return blobModels;
        }

        

        //METODO PARA SUBIR UN BLOB
        public async Task UploadBlobAsync
            (string containerName, string blobName, Stream stream)
        {
            BlobContainerClient containerClient =
                this.client.GetBlobContainerClient(containerName);
            await containerClient.UploadBlobAsync(blobName, stream);
        }
    }
}
