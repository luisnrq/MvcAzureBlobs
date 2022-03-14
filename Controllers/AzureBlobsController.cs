using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcAzureBlobs.Models;
using MvcAzureBlobs.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MvcAzureBlobs.Controllers
{
    public class AzureBlobsController : Controller
    {
        private ServiceStorageBlobs service;

        public AzureBlobsController(ServiceStorageBlobs service)
        {
            this.service = service;
        }

        public async Task<IActionResult> Index()
        {
            List<string> containers =
                await this.service.GetContainersAsync();
            return View(containers);
        }

        public async Task<IActionResult> ListBlobs(string containerName)
        {
            List<BlobClass> blobs =
                await this.service.GetBlobsAsync(containerName);
            ViewData["CONTAINERNAME"] = containerName;
            return View(blobs);
        }

        public IActionResult CreateContainer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateContainer(string containerName)
        {
            await this.service.CreateContainerAsync(containerName);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteContainer(string containerName)
        {
            await this.service.DeleteContainerAsync(containerName);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteBlob
            (string containerName, string blobName)
        {
            await this.service.DeleteBlobAsync(containerName, blobName);
            return RedirectToAction("ListBlobs",
                new { containerName = containerName });
        }

        public IActionResult UploadBlob(string containerName)
        {
            ViewData["CONTAINERNAME"] = containerName;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadBlob
            (IFormFile file, string containerName)
        {
            string blobName = file.FileName;
            using (Stream stream = file.OpenReadStream())
            {
                await this.service.UploadBlobAsync
                    (containerName, blobName, stream);
            }
            return RedirectToAction("ListBlobs",
                new { containerName = containerName });
        }


    }
}
