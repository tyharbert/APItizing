using APItizing.Models;
using APItizing.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace APItizing.Controllers
{
    /// <summary>
    /// A CRUD control for Snack objects
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class SnacksController : ControllerBase
    {
        private readonly SnacksRepository _snackRepository;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SnacksController()
        {
            _snackRepository = new SnacksRepository();
        }

        #region HttpGet

        /// <summary>
        /// READ
        /// </summary>
        /// <param name="snackFilters">From the query string parameters</param>
        /// <remarks>
        /// GET is an idempotent method because it is only a read operation and shouldn't cause any state change in the back-end.
        /// </remarks>
        [HttpGet]
        public ActionResult<IEnumerable<Snack>> Get([FromQuery] SnackFilters snackFilters)
        {
            return _snackRepository.Browse(snackFilters)
                .ToList();
        }

        /// <summary>
        /// READ
        /// </summary>
        /// <param name="id">From the route</param>
        /// <remarks>
        /// GET is an idempotent method because it is only a read operation and shouldn't cause any state change in the back-end.
        /// </remarks>
        [HttpGet("{id}")]
        public ActionResult<Snack> Get(int id)
        {
            var snack = _snackRepository.Read(id);

            if (snack == null)
            {
                return NotFound($"Snack '{id}' not found.");
            }

            return snack;
        }

        #endregion

        #region HttpPost

        /// <summary>
        /// CREATE
        /// </summary>
        /// <param name="snack">The <see cref="Snack"/> to Create</param>
        /// <remarks>
        /// POST is not an idempotent method since calling it multiple times may result in incorrect updates. Usually, POST APIs create new resources on the server. If a POST endpoint is called with an identical request body, you will create multiple records. To avoid this, you must have your own custom logic preventing duplicate records. 
        /// </remarks>
        [HttpPost]
        public ActionResult<Snack> Post(Snack snack)
        {
            return _snackRepository.Create(snack);
        }

        #endregion

        #region HttpPut

        /// <summary>
        /// UPDATE
        /// </summary>
        /// <param name="id">From the route</param>
        /// <param name="snack">The <see cref="Snack"/> to Update</param>
        /// <remarks>
        /// PUT is an idempotent method because it updates a record. If a PUT endpoint is called with an identical request, it will result in no state change other than the first request.  
        /// </remarks>
        [HttpPut("{id}")]
        public ActionResult<Snack> Put(int id, Snack snack)
        {
            if (id != snack.Id)
            {
                return BadRequest($"The Id '{id}' does not match the Snack Id '{snack.Id}'");
            }

            var dbSnack = _snackRepository.Read(id);

            if (dbSnack == null)
            {
                return NotFound($"Snack '{id}' not found.");
            }

            // Using Entity Framework you either have to update the existing model properties or bind the new model to the context and mark it as modified
            dbSnack.Name = snack.Name;
            dbSnack.Calories = snack.Calories;

            return _snackRepository.Update(id, dbSnack);
        }

        #endregion

        #region HttpPatch

        /// <summary>
        /// UPDATE
        /// </summary>
        /// <remarks>
        /// PATCH is not typically an idempotent method because if you perform a consecutive series of move operations on a JSON tree with the same payload like { “op”: “move”, “from”: “/tags/main”, “path”: “/tags/sub” }, the first one will cause the move operation to happen and the consecutive ones will cause errors. Unlike PUT which updates the entire record, PATCH can be used to update only certain fields in a record.  
        /// </remarks>
        [HttpPatch("{id}")]
        public ActionResult<Snack> Patch(int id, JsonPatchDocument<Snack> snackPatch)
        {
            var dbSnack = _snackRepository.Read(id);

            if (dbSnack == null)
            {
                return NotFound($"Snack '{id}' not found.");
            }

            snackPatch.ApplyTo(dbSnack);

            return _snackRepository.Update(id, dbSnack);
        }

        #endregion

        #region HttpDelete

        /// <summary>
        /// DELETE
        /// </summary>
        /// <param name="id">From the route</param>
        /// <remarks>
        /// DELETE is an idempotent method because consecutive similar requests wouldn't change the delete state. The first call of a DELETE may return a 200 (ok), but additional DELETE calls will likely return a 404 (Not Found). The response is different after the first request but there is no change of state. 
        /// </remarks>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var dbSnack = _snackRepository.Read(id);

            if (dbSnack == null)
            {
                return NotFound($"Snack '{id}' not found.");
            }

            _snackRepository.Delete(id);

            return NoContent();
        }

        #endregion
    }
}
