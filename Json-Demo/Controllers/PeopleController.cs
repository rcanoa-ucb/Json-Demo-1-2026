using Json_Demo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Json_Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private static List<Person> listPeople = new()
        {
            new Person { Id = 1, FullName = "Juan Perez", Age = 10 },
            new Person { Id = 2, FullName = "Luis Rodriguez", Age = 20 }
        };

        public PeopleController() { }

        //Get: /api/people
        [HttpGet]
        public ActionResult<IEnumerable<Person>> GetAll()
        {


            return Ok(JsonHelper.ToJson(listPeople));
        }

        //Get: /api/people/{id}
        [HttpGet("{id}")]
        public ActionResult<Person> GetById(int id)
        {
            var person = listPeople.FirstOrDefault(p => p.Id == id);
            return person is null ? NotFound() : Ok(JsonHelper.ToJson(person)); 

            //if (person == null)
            //    return NotFound();
            //else 
            //    return Ok(person);

            //return Ok(JsonHelper.ToJson(listPeople));
        }

        //POST: /api/people
        [HttpPost]
        public ActionResult<Person> Create([FromBody] Person newPerson)
        {
            newPerson.Id = listPeople.Any() ? listPeople.Max(p => p.Id) + 1 : 1;

            listPeople.Add(newPerson);

            return CreatedAtAction(nameof(GetById), new { id = newPerson.Id },
                newPerson);


            //if (person == null)
            //    return NotFound();
            //else 
            //    return Ok(person);

            //return Ok(JsonHelper.ToJson(listPeople));
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, Person actualizada)
        {
            var persona = listPeople.FirstOrDefault(p => p.Id == id);
            if (persona is null) return NotFound();

            persona.FullName = actualizada.FullName;
            persona.Age = actualizada.Age;
            return Ok(persona);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var persona = listPeople.FirstOrDefault(p => p.Id == id);
            if (persona is null) return NotFound();

            listPeople.Remove(persona);
            return NoContent();
        }
    }
}
