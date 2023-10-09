using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VillaAPIController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<VillaDto>> GetVillas()
    {
        return Ok(VillaStore.villaList);
    }

    //[HttpGet("{id:int}")]
    [HttpGet("{id:int}",Name ="GetVilla")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //[ProducesResponseType(200, Type = typeof(VillaDto))] // remove <VillaDto> form ActionResult for this

    public ActionResult<VillaDto> GetVilla(int id)
    {
        if(id == 0)
        {
            return BadRequest();
        }
        var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
        if(villa == null)
        {
            return NotFound();
        }
        return Ok(villa);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<VillaDto> CreateVilla(VillaDto villaDto) 
    { 
        //if(!ModelState.IsValid) // this is to check is the required fields are present or not
        //{
        //    return BadRequest(ModelState);
        //}
        if(VillaStore.villaList.FirstOrDefault(u=>u.Name.ToLower() == villaDto.Name.ToLower()) != null)
        {
            ModelState.AddModelError("CustomError", "Villa name already exist!");
        }
        if(villaDto == null)
        {
            return BadRequest(villaDto);
        }
        if(villaDto.Id > 0) // this is not a create request
        {
            return StatusCode(StatusCodes.Status500InternalServerError);// to return custom error message if they are not within the default error message
        }
        villaDto.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id+1;
        VillaStore.villaList.Add(villaDto);
        //return Ok(villaDto);
        return CreatedAtRoute("GetVilla", new {id = villaDto.Id }, villaDto);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{id:int}", Name ="DeleteVilla")]
    public IActionResult DeleteVilla(int id) // Return response with no content
    {
        if (id == 0)
        {
            return BadRequest();
        }
        var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
        if (villa == null)
        {
            return NotFound();
        }
        VillaStore.villaList.Remove(villa);
        return NoContent(); // we can also return Ok()
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("{id:int}", Name = "UpdateVilla")]
    public IActionResult UpdateVilla(int id, [FromBody]VillaDto villaDto)
    {
        if(villaDto == null || id != villaDto.Id)
        {
            return BadRequest();
        }
        var villa = VillaStore.villaList.FirstOrDefault(u=>u.Id == id);
        villa.Name = villaDto.Name;
        villa.Sqft = villaDto.Sqft;
        villa.Occupancy = villaDto.Occupancy;
        return NoContent() ;
    }


    [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDto )// Include patch document
    {
        if(patchDto == null || id == 0)
        {
            return BadRequest();
        }
        var villa = VillaStore.villaList.FirstOrDefault(u=> u.Id == id);
        if(villa == null)
        {
            return BadRequest();
        }
        patchDto.ApplyTo(villa, ModelState); // if there are some error, store them to modelstate
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        return NoContent();
    }
}
