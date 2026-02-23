using Json_Demo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace Json_Demo.Controllers
{
    [Route("api/rest")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public PostsController()
        {
            _httpClient = new();
            _httpClient.BaseAddress = new Uri("https://jsonplaceholder.typicode.com");
        }

        //GET: api/rest
        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            var response = await _httpClient.GetAsync("posts");
            var posts = await response.Content.ReadAsStringAsync();
            return Ok(posts);
        }

        [HttpGet("dos")]
        public async Task<IActionResult> GetPosts2()
        {
            var response = await _httpClient.GetAsync("posts");
            var posts = await response.Content.ReadAsStringAsync();
            return Ok(posts);
        }

        // GET: api/posts/5
        [HttpGet("buscar/{id}")]
        public async Task<IActionResult> GetPost(int id)
        {
            var response = await _httpClient.GetAsync($"posts/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound($"Post {id} no existe");

            var post = await response.Content.ReadAsStringAsync();
            return Ok(post);
        }

        // POST: api/posts
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] Post nuevoPost)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(nuevoPost);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("posts", content);
            var postCreado = await response.Content.ReadAsStringAsync();

            return Created($"api/posts/{nuevoPost.id}", postCreado);
        }

        // PUT: api/posts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(
            int id, [FromBody] Post postActualizado)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(postActualizado);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            await _httpClient.PutAsync($"posts/{id}", content);
            return NoContent();
        }

        // DELETE: api/posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            await _httpClient.DeleteAsync($"posts/{id}");
            return NoContent();
        }

    }
}
