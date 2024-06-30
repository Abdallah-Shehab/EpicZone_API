using Microsoft.AspNetCore.Mvc;
using ProductsApi.Models;
using ProductsApi.UnitOFWork;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        UnitOfWorks unitOfWork;
        public ProductController(UnitOfWorks unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        //[HttpGet]

        //public List<Product> getAll()
        //{
        //    return unitOfWork.ProductRepo.getAll();
        //}

        [HttpGet]

        public List<Product> getAllByCategory(string category = null)
        {
            return unitOfWork.ProductRepo.getAllByCategory(category);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> getById(int id)
        {
            var pro = unitOfWork.ProductRepo.getByID(id);
            if (pro == null)
            {
                return NotFound();
            }
            return Ok(pro);
        }
        [HttpPost]
        public IActionResult AddProduct([FromBody] Product pro)
        {
            unitOfWork.ProductRepo.Add(pro);
            try
            {
                unitOfWork.UnitSave();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Created();
        }

        [HttpPut("{id}")]

        public IActionResult UpdateProduct(int id, [FromBody] Product pro)
        {

            //if (id != pro.Id)
            //{
            //    return BadRequest();
            //}
            if (pro == null || string.IsNullOrEmpty(pro.Name))
            {
                return BadRequest("Invalid product data.");
            }

            unitOfWork.ProductRepo.Update(pro);
            try
            {
                unitOfWork.UnitSave();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var pro = unitOfWork.ProductRepo.getByID(id);
            if (pro == null)
            {
                return BadRequest();
            }

            unitOfWork.ProductRepo.Delete(id);
            try
            {
                unitOfWork.UnitSave();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

    }
}
