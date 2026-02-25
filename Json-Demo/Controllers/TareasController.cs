using Json_Demo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Json_Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        // Constructor: configuramos todo lo necesario
        public TareasController()
        {
            // Configuramos el HttpClient AQUÍ MISMO en el controlador
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://jsonplaceholder.typicode.com");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _httpClient.Timeout = TimeSpan.FromSeconds(30);

            // Configuración para JSON (ignorar mayúsculas/minúsculas)
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true // Para que el JSON se vea bonito
            };
        }



        // GET: api/tareas/usuario/5
        [HttpGet("usuario/{userId}")]
        public async Task<IActionResult> GetTareasPorUsuario(int userId)
        {
            try
            {
                // 1. Validar el parámetro
                if (userId <= 0 || userId > 10)
                {
                    return BadRequest(new
                    {
                        Error = "El userId debe estar entre 1 y 10",
                        Ejemplo = "api/tareas/usuario/1"
                    });
                }

                // 2. Llamar a JSONPlaceholder
                var response = await _httpClient.GetAsync($"/todos?userId={userId}");

                // 3. Verificar si la llamada fue exitosa
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(503, new
                    {
                        Error = "El servicio externo no está disponible",
                        Detalle = $"Código: {response.StatusCode}"
                    });
                }

                // 4. Leer el contenido
                string jsonString = await response.Content.ReadAsStringAsync();

                // 5. Convertir JSON a objetos C#
                var tareas = JsonSerializer.Deserialize<List<Todo>>(jsonString, _jsonOptions);

                // 6. Verificar si hay tareas
                if (tareas == null || tareas.Count == 0)
                {
                    return NotFound(new
                    {
                        Mensaje = $"No se encontraron tareas para el usuario {userId}",
                        Sugerencia = "Prueba con userId 1, 2, 3, etc."
                    });
                }

                // 7. Transformar las tareas a nuestro formato de respuesta
                var tareasResponse = tareas.Select(t => new TareaResponse
                {
                    Id = t.Id,
                    Titulo = t.Title,
                    UsuarioId = t.UserId,
                    Completada = t.Completed,
                    Estado = t.Completed ? "✅ Completada" : "⏳ Pendiente"
                }).ToList();

                // 8. Devolver respuesta exitosa
                return Ok(new
                {
                    UsuarioId = userId,
                    TotalTareas = tareasResponse.Count,
                    Tareas = tareasResponse
                });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(503, new
                {
                    Error = "No se pudo conectar con el servicio externo",
                    Detalle = ex.Message
                });
            }
            catch (JsonException ex)
            {
                return StatusCode(500, new
                {
                    Error = "Error al procesar los datos recibidos"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Ocurrió un error interno"
                });
            }
        }

        // ===== ENDPOINT 2: Obtener una tarea específica por ID =====
        // GET: api/tareas/3
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTareaPorId(int id)
        {
            try
            {
                // 1. Validar ID
                if (id <= 0 || id > 200)
                {
                    return BadRequest(new
                    {
                        Error = "El ID debe estar entre 1 y 200",
                        Ejemplo = "api/tareas/1"
                    });
                }

                // 2. Llamar a JSONPlaceholder
                var response = await _httpClient.GetAsync($"/todos/{id}");

                // 3. Manejar caso de no encontrado
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound(new
                    {
                        Mensaje = $"No existe la tarea con ID {id}",
                        Sugerencia = "Los IDs válidos son del 1 al 200"
                    });
                }

                // 4. Verificar otros errores
                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(503, "Error del servicio externo");
                }

                // 5. Leer y convertir
                string jsonString = await response.Content.ReadAsStringAsync();
                var tarea = JsonSerializer.Deserialize<Todo>(jsonString, _jsonOptions);

                if (tarea == null)
                {
                    return NotFound($"Tarea {id} no encontrada");
                }

                // 6. Transformar a respuesta personalizada
                var tareaResponse = new TareaResponse
                {
                    Id = tarea.Id,
                    Titulo = tarea.Title,
                    UsuarioId = tarea.UserId,
                    Completada = tarea.Completed,
                    Estado = tarea.Completed ? "✅ Completada" : "⏳ Pendiente"
                };

                return Ok(tareaResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }


        // ===== ENDPOINT 3: Resumen completo de un usuario =====
        // GET: api/tareas/usuario/1/resumen
        [HttpGet("usuario/{userId}/resumen")]
        public async Task<IActionResult> GetResumenUsuario(int userId)
        {
            try
            {
                // 1. Validar usuario
                if (userId <= 0 || userId > 10)
                {
                    return BadRequest("Usuario inválido (debe ser 1-10)");
                }

                // 2. Obtener todas las tareas del usuario
                var response = await _httpClient.GetAsync($"/todos?userId={userId}");
                response.EnsureSuccessStatusCode();

                string jsonString = await response.Content.ReadAsStringAsync();
                var tareas = JsonSerializer.Deserialize<List<Todo>>(jsonString, _jsonOptions);

                if (tareas == null || tareas.Count == 0)
                {
                    return NotFound($"El usuario {userId} no tiene tareas");
                }

                // 3. Calcular estadísticas
                var tareasCompletadas = tareas.Count(t => t.Completed);
                var tareasPendientes = tareas.Count - tareasCompletadas;
                var porcentaje = (double)tareasCompletadas / tareas.Count * 100;

                // 4. Crear lista de tareas con formato amigable
                var listaTareas = tareas.Select(t => new TareaResponse
                {
                    Id = t.Id,
                    Titulo = t.Title,
                    UsuarioId = t.UserId,
                    Completada = t.Completed,
                    Estado = t.Completed ? "✅ Completada" : "⏳ Pendiente"
                }).ToList();

                // 5. Crear resumen completo
                var resumen = new
                {
                    UsuarioId = userId,
                    Nombre = $"Usuario {userId}",
                    Estadisticas = new
                    {
                        TotalTareas = tareas.Count,
                        Completadas = tareasCompletadas,
                        Pendientes = tareasPendientes,
                        Progreso = $"{porcentaje:F1}%",
                        BarraProgreso = GenerarBarraProgreso(porcentaje)
                    },
                    Tareas = listaTareas,
                    ResumenTexto = $"{tareasCompletadas} de {tareas.Count} tareas completadas ({porcentaje:F1}%)"
                };

                return Ok(resumen);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al generar el resumen");
            }
        }

        // ===== ENDPOINT 4: Marcar tarea como completada (simulado) =====
        // PATCH: api/tareas/5/completar
        [HttpPatch("{id}/completar")]
        public async Task<IActionResult> MarcarComoCompletada(int id)
        {
            try
            {
                // 1. Primero verificamos que la tarea existe
                var responseGet = await _httpClient.GetAsync($"/todos/{id}");

                if (responseGet.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound(new
                    {
                        Mensaje = $"No se encontró la tarea con ID {id}",
                        Accion = "No se puede marcar como completada"
                    });
                }

                if (!responseGet.IsSuccessStatusCode)
                {
                    return StatusCode(503, "Error al verificar la tarea");
                }

                // 2. Leer la tarea original
                string jsonOriginal = await responseGet.Content.ReadAsStringAsync();
                var tareaOriginal = JsonSerializer.Deserialize<Todo>(jsonOriginal, _jsonOptions);

                if (tareaOriginal == null)
                {
                    return NotFound("Tarea no encontrada");
                }

                // 3. Si ya está completada, informar
                if (tareaOriginal.Completed)
                {
                    return Ok(new
                    {
                        Mensaje = "La tarea ya estaba completada",
                        Tarea = new TareaResponse
                        {
                            Id = tareaOriginal.Id,
                            Titulo = tareaOriginal.Title,
                            UsuarioId = tareaOriginal.UserId,
                            Completada = tareaOriginal.Completed,
                            Estado = "✅ Completada"
                        },
                        Nota = "No se realizaron cambios"
                    });
                }

                // 4. SIMULAMOS la actualización (JSONPlaceholder no persiste)
                // Creamos el objeto para enviar en el PATCH
                var datosActualizacion = new { completed = true };
                string jsonPatch = JsonSerializer.Serialize(datosActualizacion);
                var content = new StringContent(jsonPatch, System.Text.Encoding.UTF8, "application/json");

                // 5. Enviamos el PATCH
                var responsePatch = await _httpClient.PatchAsync($"/todos/{id}", content);

                if (!responsePatch.IsSuccessStatusCode)
                {
                    return StatusCode(503, "Error al actualizar la tarea");
                }

                // 6. Devolvemos respuesta exitosa (simulada)
                tareaOriginal.Completed = true;

                return Ok(new
                {
                    Mensaje = "✅ Tarea marcada como completada (SIMULADO - JSONPlaceholder no guarda cambios)",
                    Tarea = new TareaResponse
                    {
                        Id = tareaOriginal.Id,
                        Titulo = tareaOriginal.Title,
                        UsuarioId = tareaOriginal.UserId,
                        Completada = tareaOriginal.Completed,
                        Estado = "✅ Completada"
                    },
                    Nota = "En un ambiente real, este cambio se guardaría en la base de datos"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al procesar la solicitud");
            }
        }


        // ===== ENDPOINT 5: Obtener solo tareas pendientes =====
        // GET: api/tareas/usuario/1/pendientes
        [HttpGet("usuario/{userId}/pendientes")]
        public async Task<IActionResult> GetTareasPendientes(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/todos?userId={userId}&completed=false");
                response.EnsureSuccessStatusCode();

                string jsonString = await response.Content.ReadAsStringAsync();
                var tareas = JsonSerializer.Deserialize<List<Todo>>(jsonString, _jsonOptions);

                if (tareas == null || tareas.Count == 0)
                {
                    return Ok(new
                    {
                        UsuarioId = userId,
                        Mensaje = "¡Felicidades! No tienes tareas pendientes 🎉",
                        TareasPendientes = new List<TareaResponse>()
                    });
                }

                var tareasResponse = tareas.Select(t => new TareaResponse
                {
                    Id = t.Id,
                    Titulo = t.Title,
                    UsuarioId = t.UserId,
                    Completada = t.Completed,
                    Estado = "⏳ Pendiente"
                }).ToList();

                return Ok(new
                {
                    UsuarioId = userId,
                    TotalPendientes = tareasResponse.Count,
                    Tareas = tareasResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al obtener tareas");
            }
        }


        // ===== ENDPOINT 6: Health check =====
        // GET: api/tareas/health
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new
            {
                Status = "🟢 API funcionando correctamente",
                Timestamp = DateTime.Now,
                Servicio = "Diario de Tareas Simple",
                Version = "1.0",
                EndpointsDisponibles = new[]
                {
                "GET /api/tareas/usuario/{userId}",
                "GET /api/tareas/{id}",
                "GET /api/tareas/usuario/{userId}/resumen",
                "PATCH /api/tareas/{id}/completar",
                "GET /api/tareas/usuario/{userId}/pendientes",
                "GET /api/tareas/health"
            }
            });
        }

        // ===== Método auxiliar para generar barra de progreso visual =====
        private string GenerarBarraProgreso(double porcentaje, int longitud = 20)
        {
            int completados = (int)(porcentaje * longitud / 100);
            string barra = new string('█', completados) + new string('░', longitud - completados);
            return $"[{barra}] {porcentaje:F1}%";
        }

        // IMPORTANTE: Liberar recursos cuando el controlador se destruye


        //public void Dispose()
        //{
        //    throw new NotImplementedException();
        //}
    }
}