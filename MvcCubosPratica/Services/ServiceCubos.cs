using MvcCubosPratica.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace MvcCubosPratica.Services
{
    public class ServiceCubos
    {
        private MediaTypeWithQualityHeaderValue Header;
        private string UrlApiCubos;

        public ServiceCubos(IConfiguration configuration)
        {
            this.UrlApiCubos =
          configuration.GetValue<string>("ApiUrls:ApiCubos");
            this.Header =
                new MediaTypeWithQualityHeaderValue("application/json");
        }


        public async Task<string> GetTokenAsync
        (string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/auth/login";
                client.BaseAddress = new Uri(this.UrlApiCubos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                LoginModel model = new LoginModel
                {
                    UserName = username,
                    Password = password
                };
                string jsonModel = JsonConvert.SerializeObject(model);
                StringContent content =
                    new StringContent(jsonModel, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string data =
                        await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(data);
                    string token =
                        jsonObject.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiCubos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }


        private async Task<T> CallApiAsync<T>
           (string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiCubos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add
                    ("Authorization", "bearer " + token);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }


        public async Task<List<Cubos>> GetCubosAsync()
        {
            string request = "/api/Cubos/GetCubos";
            List<Cubos> cubos =
                await this.CallApiAsync<List<Cubos>>(request);
            return cubos;
        }

        public async Task<List<Cubos>> GetCuboMarcaAsync(string marca)
        {
            string request = "/api/Cubos/GetCuboMarca/"+marca;
            List<Cubos> cubos =
                await this.CallApiAsync<List<Cubos>>(request);
            return cubos;
        }


        public async Task InsertUsuarioAsync
           (string nombre, string email, string pass, string imagen)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Cubos/InsertarUsuario";
                client.BaseAddress = new Uri(this.UrlApiCubos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                Usuarios usuarios = new Usuarios();
                usuarios.IdUsuario = 0;
                usuarios.Nombre = nombre;
                usuarios.Email = email;
                usuarios.Pass = pass;
                usuarios.Imagen = imagen;

                string json = JsonConvert.SerializeObject(usuarios);

                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
            }
        }


        public async Task InsertCuboAsync
           (string nombre, string marca, string? imagen, int precio)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Cubos/InsertarCubo";
                client.BaseAddress = new Uri(this.UrlApiCubos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                Cubos cubos = new Cubos();
                cubos.IdCubo = 0;
                cubos.Nombre = nombre;
                cubos.Marca = marca;
                cubos.Imagen = imagen;
                cubos.Precio = precio;

                string json = JsonConvert.SerializeObject(cubos);

                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
            }
        }


        public async Task<Usuarios> GetPerfilUsuarioAsync(string token)
        {
            string request = "/api/Cubos/PerfilUsuario";
            Usuarios usuarios =
                await this.CallApiAsync<Usuarios>(request,token);
            return usuarios;
        }


        public async Task<List<Compra>> GetPedidosAsync(string token)
        {
            string request = "/api/Cubos/Pedidos";
            List<Compra> compras=
                await this.CallApiAsync<List<Compra>>(request, token);
            return compras;
        }


        public async Task InsertarPedidoAsync
            (int idcubo, int idusuario, DateTime fecha,string token)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Cubos/InsertarPedido";
                client.BaseAddress = new Uri(this.UrlApiCubos);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer "+token);

                Compra compra = new Compra();
                compra.Idpedido = 0;
                compra.IdCubo = idcubo;
                compra.IdUsuario = idusuario;
                compra.Fecha = fecha;


                string json = JsonConvert.SerializeObject(compra);

                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
            }
        }






    }
}
