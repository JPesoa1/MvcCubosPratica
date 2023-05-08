using Microsoft.AspNetCore.Mvc;
using MvcCubosPratica.Filters;
using MvcCubosPratica.Models;
using MvcCubosPratica.Services;
using System.Security.Claims;

namespace MvcCubosPratica.Controllers
{
    public class CubosController : Controller
    {
        private ServiceCubos service;
        private ServiceStorageBlobs serviceStorageBlobs;

        public CubosController(ServiceCubos service, ServiceStorageBlobs serviceStorageBlobs) 
        {
            this.service = service;
            this.serviceStorageBlobs = serviceStorageBlobs;
        }

       

        [AuthorizeCubos]
        public async Task<IActionResult> GetCubos()
        {
            List<Cubos> cubos = await this.service.GetCubosAsync();
            List<BlobModel> blobModels =
                await this.serviceStorageBlobs.GetBlobsAsync("cubos");
            
            ViewData["IMAGENES"] = blobModels;

            return View(cubos);
        }


        public IActionResult GetCubosMarca()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetCubosMarca(string marca)
        {
            List<Cubos> cubos = await this.service.GetCuboMarcaAsync(marca);
            return View(cubos);
        }


        public IActionResult CreateUsuario()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUsuario(Usuarios usuarios, IFormFile file)
        {
           
            string blobName = file.FileName;
            usuarios.Imagen = blobName;
            using (Stream stream = file.OpenReadStream())
            {
                await this.serviceStorageBlobs.UploadBlobAsync
                    ("usuarios", blobName, stream);
            }
            await this.service.InsertUsuarioAsync(usuarios.Nombre,usuarios.Email,usuarios.Pass,usuarios.Imagen);
            return View();
        }



        public IActionResult CreateCubo()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCubo(Cubos cubos, IFormFile file)
        {
            string blobName = file.FileName;
            cubos.Imagen = blobName;
            using (Stream stream = file.OpenReadStream())
            {
                await this.serviceStorageBlobs.UploadBlobAsync
                    ("cubos", blobName, stream);
            }
            await this.service.InsertCuboAsync(cubos.Nombre, cubos.Marca, cubos.Imagen, cubos.Precio);
            return RedirectToAction("GetCubos");
        }



        [AuthorizeCubos]
        public async Task<IActionResult> Perfil()
        {
            string token =
               HttpContext.Session.GetString("TOKEN");
            if (token == null)
            {
                

                
                ViewData["MENSAJE"] = "Debe realizar Login para visualizar datos";
                return View();
            }
            else
            {
                
                Usuarios usuarios
                    = await this.service.GetPerfilUsuarioAsync(token);

                string imagen = await this.serviceStorageBlobs.GetBlobUriAsync("usuarios",usuarios.Imagen);
                ViewData["FOTO"] = imagen;

                return View(usuarios);
            }
            
        }

        [AuthorizeCubos]
        public async Task<IActionResult> Pedidos()
        {
            string token =
               HttpContext.Session.GetString("TOKEN");
            if (token == null)
            {
                ViewData["MENSAJE"] = "Debe realizar Login para visualizar datos";
                return View();
            }
            else
            {
               List<Compra> compras 
                    = await this.service.GetPedidosAsync(token);
                return View(compras);
            }
           
        }

       
    
        [AuthorizeCubos]
        public async Task<IActionResult> Compra(int idcubo)
        {
            string token =
              HttpContext.Session.GetString("TOKEN");

            Usuarios usuarios= await
         this.service.GetPerfilUsuarioAsync(token);
           
            await this.service.InsertarPedidoAsync(idcubo, usuarios.IdUsuario, DateTime.Now, token);
            return RedirectToAction("Pedidos");
        }


    }
}
