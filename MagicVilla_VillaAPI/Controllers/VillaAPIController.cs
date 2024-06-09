using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<VillaDTO> GetVillas()
        {
            return VillaStore.villaList;
        }

        [HttpGet("{Id:int}", Name ="GetVilla")]
        [ProducesResponseType(200, Type = typeof(VillaDTO))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public ActionResult GetVilla(int Id)
        {
            if (Id == 0)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == Id);
            if (villa == null)
            {
                return NotFound();
            }
            return Ok(villa);
        }

        [HttpPost]
        public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villa)
        {
            if (VillaStore.villaList.FirstOrDefault(u => u.Name.ToLower() == villa.Name.ToLower())!=null)
            {
                ModelState.AddModelError("CustomError", "Villa already Exists!");
                return BadRequest(ModelState);
            }
            if (villa == null)
            {
                return BadRequest();
            }
            if (villa.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            villa.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
            VillaStore.villaList.Add(villa);
            return CreatedAtRoute("GetVilla",new {Id=villa.Id},villa);
        }

        [HttpDelete("{Id:int}",Name ="DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteVilla(int Id)
        {
            if(Id==0)
            {
                return BadRequest();
            }
            var villa=VillaStore.villaList.FirstOrDefault(u=>u.Id==Id);
            if (villa == null)
            {
                return NotFound();
            }
            VillaStore.villaList.Remove(villa);
            return NoContent();
        }

        [HttpPost("{Id:int}",Name ="UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult UpdateVilla(int Id, [FromBody] VillaDTO villaDTO)
        {
            if(villaDTO==null || villaDTO.Id != Id)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == Id);
            villa.Name = villaDTO.Name;
            villa.Occupancy = villaDTO.Occupancy;
            villa.Sqft = villaDTO.Sqft;

            return NoContent();
        }

        [HttpPatch("{Id:int}",Name ="UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult UpdatePartialVilla(int Id, JsonPatchDocument<VillaDTO> patchDTO)
        {
            if(Id==0 ||patchDTO == null)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == Id);
            if (villa == null)
            {
                return NotFound();
            }
            patchDTO.ApplyTo(villa, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();

        }
    }
}

