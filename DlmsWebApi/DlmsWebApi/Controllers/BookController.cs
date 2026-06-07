using DlmsWebApi.Business.AuthorBusiness;
using DlmsWebApi.Business.BookBusiness;
using DlmsWebApi.Helpers;
using DlmsWebApi.Repository.Models;
using DlmsWebApi.Shared.BookData;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;


namespace DlmsWebApi.Controllers
{
    [ApiController]
    [Route("api/book")]
    //[Authorize(Roles = "SuperAdmin")]
    public class BookController : Controller
    {
        private readonly IBookBusiness _bookBusiness;
        //private readonly IAuthorBusiness _authorBusiness;
        //private readonly ICategoryBusiness _categoryBusiness;
        //private readonly IPublicationBusiness _publicationBusiness;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookController(IBookBusiness bookBusiness, IWebHostEnvironment webHostEnvironment)// IAuthorBusiness authorBusiness, ICategoryBusiness categoryBusiness, IPublicationBusiness publicationBusiness, IWebHostEnvironment webHostEnvironment)
        {
            _bookBusiness = bookBusiness;
            //_authorBusiness = authorBusiness;
            //_categoryBusiness = categoryBusiness;
            //_publicationBusiness = publicationBusiness;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [Route("get-book-list")]
        public async Task<IActionResult> GetList()
        {
            var bookList = await _bookBusiness.GetList();
            return Ok(bookList);
        }

        //public async Task<IActionResult> AddBook()
        //{
        //    BookDetails bookDetails = new BookDetails();
        //    await PopulateDropdowns(bookDetails);
        //    return Ok(bookDetails);
        //}

        [HttpPost]
        [Route("add-book")]

        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBook([FromBody] BookDetails book)
        {
            if (book.ImageFile != null)
            {
                var extension = Path.GetExtension(book.ImageFile.FileName).ToLowerInvariant();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ImageFile", "Only .jpg, .jpeg, and .png images are allowed.");
                }
            }

            if (ModelState.IsValid)
            {
                if (book.ImageFile != null)
                {
                    book.ImageUrl = await BookImageHelper.UploadImageAsync(book.ImageFile, _webHostEnvironment.WebRootPath);
                }

                book.User = "admin";
                bool isAdded = await _bookBusiness.AddBook(book);
                if (isAdded)
                {
                    return Created();
                }
                else
                {
                    return BadRequest("Failed to add author");
                }
            }
            else
            {
                // repopulate dropdowns before returning the view
                await PopulateDropdowns(book);
                return Ok(book);
            }
        }


        [HttpGet]
        [Route("get-book-details")]
        public async Task<IActionResult> EditBook(string id)
        {
            var bookId = Convert.ToInt32(EncryptionHelper.Decrypt(id));

            var bookDetails = await _bookBusiness.GetBookDetails(bookId);
            
            await PopulateDropdowns(bookDetails);
            return Ok(bookDetails);
        }


        [HttpPut]
        [Route("update-book-details")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBook([FromBody]BookDetails book)
        {
            if (book.ImageFile != null)
            {
                var extension = Path.GetExtension(book.ImageFile.FileName).ToLowerInvariant();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ImageFile", "Only .jpg, .jpeg, and .png images are allowed.");
                }
            }

            if (ModelState.IsValid)
            {
                if (book.ImageFile != null)
                {
                    book.ImageUrl = await BookImageHelper.UploadImageAsync(book.ImageFile, _webHostEnvironment.WebRootPath);
                }

                book.User = "admin";
                book.BookId = Convert.ToInt32(EncryptionHelper.Decrypt(book.BookIdString));

                var details = await _bookBusiness.EditBook(book);
                if (details)
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "Book updated successfully"
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Failed to update book details"
                    });
                }
            }
            else
            {
                // repopulate dropdowns before returning the view
                await PopulateDropdowns(book);
                return Ok(book);
            }
        }


        [HttpPatch]
        [Route("update-book-status")]
        public async Task<IActionResult> UpdateStatus(string id)
        {
            var bookId = Convert.ToInt32(EncryptionHelper.Decrypt(id));
            var user = "admin";
            var isUpdated = await _bookBusiness.UpdateStatus(bookId,user);
            if (isUpdated)
            {
                return Created();
            }
            else
            {
                return BadRequest("Failed to update author status");
            }
        }


       

        [HttpGet]
        public IActionResult ViewSecureImage(string fileName, string signature)
        {
            if (!BookImageHelper.VerifySignature(fileName, signature))
            {
                return Forbid(); // Return 403 Forbidden on invalid or missing proof
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Double check existence and path traversal safety
            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists || !fileInfo.DirectoryName!.Equals(uploadsFolder, StringComparison.OrdinalIgnoreCase))
            {
                return NotFound("Image not found.");
            }

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };

            return PhysicalFile(filePath, contentType);
        }

        private async Task PopulateDropdowns(BookDetails book)
        {
            //var authorList = await _authorBusiness.GetList();
            //book.AuthorList = authorList.Select(a => new SelectListItem
            //{
            //    Value = a.AuthorId.ToString(),
            //    Text = a.FirstName + " " + a.LastName,
            //    Selected = a.AuthorId.ToString() == book.Author
            //}).ToList();

            //var categoryList = await _categoryBusiness.GetList();
            //book.CategoryList = categoryList.Select(c => new SelectListItem
            //{
            //    Value = c.CategoryId.ToString(),
            //    Text = c.Name,
            //    Selected = c.CategoryId.ToString() == book.Category
            //}).ToList();

            //var publicationList = await _publicationBusiness.GetList();
            //book.PublicationList = publicationList.Select(p => new SelectListItem
            //{
            //    Value = p.PublicationId.ToString(),
            //    Text = p.PublicationName,
            //    Selected = p.PublicationId.ToString() == book.Publication
            //}).ToList();
        }
    }
}


