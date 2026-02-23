using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Dynamic;

namespace Json_Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JsonDemoController : ControllerBase
    {
        public JsonDemoController()
        {
            
        }

        #region Lista
        [HttpGet("generica")]
        public IActionResult ListaGenerica()
        {
            var personas = new List<Persona>
            {
                new Persona { Id = 1, Nombre = "Juan Perez", Edad = 20 },
                new Persona { Id = 2 }
            };

            return Ok(JsonHelper.ToJson(personas));
        }
        #endregion

        #region Lista Generica
        [HttpGet("diccionario")]
        public IActionResult Diccionarios()
        {
            var diccionario = new Dictionary<string, string>
        {
            { "clave1", "valor1" },
            { "clave2", "valor2" }
        };

            return Ok(JsonHelper.ToJson(diccionario));
        }
        #endregion

        #region dynamic (ExpandoObject)
        [HttpGet("dinamico")]
        public IActionResult ObjetoDinamico()
        {
            dynamic objetoDinamico = new ExpandoObject();
            objetoDinamico.Nombre = "Carlos";
            objetoDinamico.Edad = 40;
            objetoDinamico.Activo = true;

            return Ok(JsonHelper.ToJson(objetoDinamico));
        }
        #endregion

        #region IEnumerable
        [HttpGet("ienumerable")]
        public IActionResult IEnumerableEjemplo()
        {
            IEnumerable<string> numeros = new List<string>
            {
                "Uno",
                "Dos",
                "Tres"
            };

            return Ok(JsonHelper.ToJson(numeros));
        }
        #endregion

        #region Hashtable 
        [HttpGet("hashtable")]
        public IActionResult HashtableEjemplo()
        {
            var hashtable = new Hashtable
            {
                { "uno", 1 },
                { "dos", 2 }
            };
            // Convertir Hashtable a Dictionary para serializar
            var dictFromHashtable = hashtable
                .Cast<DictionaryEntry>()
                .ToDictionary(k => k.Key.ToString(), v => v.Value);

            return Ok(JsonHelper.ToJson(dictFromHashtable));
        }
        #endregion

        #region Queue 
        [HttpGet("cola")]
        public IActionResult QueueEjemplo()
        {
            var cola = new Queue<string>();
            cola.Enqueue("Primero");
            cola.Enqueue("Segundo");
            cola.Enqueue("Tercero");

            return Ok(JsonHelper.ToJson(cola));
        }
        #endregion

        #region Stack 
        [HttpGet("pila")]
        public IActionResult StackEjemplo()
        {
            var pila = new Stack<int>();
            pila.Push(100);
            pila.Push(200);
            pila.Push(300);

            return Ok(JsonHelper.ToJson(pila));
        }
        #endregion

        #region HashSet 
        [HttpGet("hash")]
        public IActionResult HashEjemplo()
        {
            var conjunto = new HashSet<string> { "uno", "dos", "tres", "uno", "dos" };

            return Ok(JsonHelper.ToJson(conjunto));
        }
        #endregion

        #region Compleja 
        [HttpGet("anidada")]
        public IActionResult AnidadaEjemplo()
        {
            // Estructura anidada
            var personasGrupo1 = new List<Persona>
            {
                new Persona { Id = 1, Nombre = "Ana", Edad = 28 },
                new Persona { Id = 2, Nombre = "Luis", Edad = 35 }
            };

            var personasGrupo2 = new List<Persona>
            {
                new Persona { Id = 3, Nombre = "María", Edad = 22 },
                new Persona { Id = 4, Nombre = "Juan", Edad = 40 }
            };

            var estructuraCompleja = new List<Dictionary<string, List<Persona>>>
            {
                new Dictionary<string, List<Persona>> { { "GrupoA", personasGrupo1 } },
                new Dictionary<string, List<Persona>> { { "GrupoB", personasGrupo2 } }
            };

            return Ok(JsonHelper.ToJson(estructuraCompleja));
        }
        #endregion

    }

    public class Persona
    { 
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int? Edad { get; set; }
    }



}
