using DlmsWebApi.Business.AuthorBusiness;
using DlmsWebApi.Business.BookBusiness;
using DlmsWebApi.Extensions.StringHelper;
using DlmsWebApi.Helpers;
using DlmsWebApi.Repository.Models;
using DlmsWebApi.Shared.AuthorData;
using DlmsWebApi.Shared.BookData;
using Microsoft.AspNetCore.Mvc;

namespace DlmsWebApi.Controllers
{
    [ApiController]
    [Route("api/book")]
    public class BookController : ControllerBase
    {
        private readonly IBookBusiness _bookBusiness;
        private readonly IAuthorBusiness _authorBusiness;
        //private readonly ICategoryBusiness _categoryBusiness;
        //private readonly IPublicationBusiness _publicationBusiness;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookController(IBookBusiness bookBusiness, IAuthorBusiness authorBusiness, /*ICategoryBusiness categoryBusiness, IPublicationBusiness publicationBusiness,*/ IWebHostEnvironment webHostEnvironment)
        {
            _bookBusiness = bookBusiness;
            _authorBusiness = authorBusiness;
            //_categoryBusiness = categoryBusiness;
            //_publicationBusiness = publicationBusiness;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [Route("get-book-list")]
        public async Task<IActionResult> GetList()
        {
            var bookList = await _bookBusiness.GetBookList();
            return Ok(bookList);
        }

        [HttpPost]
        [Route("add-book")]
        public async Task<IActionResult> AddBook([FromBody] BookDetails book)
        {
            if (book.ImageFile != null)
            {
                var extension = Path.GetExtension(book.ImageFile.FileName).ToLowerInvariant();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest("Only .jpg, .jpeg, and .png images are allowed.");
                }
            }

            bool isAdded = await _bookBusiness.AddBook(book);
            if (isAdded)
            {
                return Created();
            }
            else
            {
                return BadRequest("Failed to add books");
            }
        }


        [HttpGet]
        [Route("get-book-details")]
        public async Task<IActionResult> EditBook([FromQuery] string id)
        {
            var bookId = Convert.ToInt32(EncryptionHelper.Decrypt(id));
            var bookDetails = await _bookBusiness.GetBookDetails(bookId);
            bookDetails.BookIdString = EncryptionHelper.Encrypt(bookDetails.BookId.ToString());
            return Ok(bookDetails);
        }



        [HttpPut]
        [Route("update-book-details")]
        public async Task<IActionResult> EditBook([FromBody] BookDetails book)
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

            if (book.ImageFile != null)
            {
                book.ImageUrl = await BookImageHelper.UploadImageAsync(book.ImageFile, _webHostEnvironment.WebRootPath);
            }
            book.BookId = Convert.ToInt32(EncryptionHelper.Decrypt(book.BookIdString));
            var details = await _bookBusiness.EditBooks(book);
            if (details)
            {
                return Created();
            }
            else
            {
                return BadRequest("Failed to update book details");
            }
        }



        [HttpPatch]
        [Route("update-book-status")]
        public async Task<IActionResult> UpdateStatus([FromQuery] string id)
        {
            var bookId = Convert.ToInt32(EncryptionHelper.Decrypt(id));
            var user = "admin";
            var isUpdated = await _bookBusiness.UpdateStatus(bookId, user);
            if (isUpdated)
            {
                return Created();
            }
            else
            {
                return BadRequest("Failed to update author status");
            }
        }

        //[HttpGet]
        //public IActionResult ViewSecureImage(string fileName, string signature)
        //{
        //    if (!BookImageHelper.VerifySignature(fileName, signature))
        //    {
        //        return Forbid(); // Return 403 Forbidden on invalid or missing proof
        //    }

        //    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
        //    var filePath = Path.Combine(uploadsFolder, fileName);

        //    // Double check existence and path traversal safety
        //    var fileInfo = new FileInfo(filePath);
        //    if (!fileInfo.Exists || !fileInfo.DirectoryName!.Equals(uploadsFolder, StringComparison.OrdinalIgnoreCase))
        //    {
        //        return BadRequest("Image not found.");
        //    }

        //    var extension = Path.GetExtension(fileName).ToLowerInvariant();
        //    var contentType = extension switch
        //    {
        //        ".jpg" or ".jpeg" => "image/jpeg",
        //        ".png" => "image/png",
        //        _ => "application/octet-stream"
        //    };

        //    return PhysicalFile(filePath, contentType);
        //}


    }
}

