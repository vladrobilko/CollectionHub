using CollectionHub.Models.ViewModels;
using CollectionHub.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CollectionHub.Controllers
{
    public class CollectionController : Controller
    {
        private readonly IImageService _imageService;

        public CollectionController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult MyCollections()
        {
            var collections = new List<CollectionViewModel>
            {
                new CollectionViewModel
                {
                     Name = "Old Books",
                     Description = "The Old Books Collection takes you on a captivating journey through time, offering a glimpse into the literary treasures of bygone eras. Immerse yourself in the rich narratives, timeless wisdom, and classic tales that have withstood the test of time. This carefully curated collection features rare and antique books, each bearing the marks of history and the stories of countless readers. From dusty volumes with weathered leather bindings to faded pages filled with the ink of yesteryear, these books whisper the echoes of literary heritage. Explore the profound insights of past authors, embark on adventures of forgotten worlds, and witness the evolution of language and storytelling. The Old Books Collection invites you to embrace the enchanting allure of vintage literature and connect with the enduring magic held within each aged page.",
                     ImageLink = "https://collectionhubimagesdata.blob.core.windows.net/datavidsconteiner1/2b68c76c-4419-4325-8562-6cbd466d42dc.jpg"
                },
                new CollectionViewModel
                {
                     Name = "Coins",
                     Description = "Step into the fascinating world of numismatics with our Old Coins Collection, a treasury of historical currency that spans the ages. This carefully curated assortment brings together a diverse array of coins from different epochs, each telling a unique story of its time. From ancient civilizations to the modern era, these coins bear witness to the ebb and flow of history, capturing the artistry, culture, and political landscapes of bygone civilizations. Whether you're a seasoned collector or a curious enthusiast, our Old Coins Collection offers a glimpse into the tangible remnants of trade, commerce, and the evolution of currency. Discover the intricacies of coinage craftsmanship, marvel at the symbols and inscriptions that reflect the spirit of their era, and appreciate the enduring value of these numismatic treasures. Immerse yourself in the allure of our Old Coins Collection, where every coin is a portal to the past, waiting to share its tale with those who dare to explore.",
                     ImageLink = "https://collectionhubimagesdata.blob.core.windows.net/datavidsconteiner1/11486f7f-0e52-4aa3-8f55-183d247022f6.jpg"
                },
                new CollectionViewModel
                {
                     Name = "Watches",
                     Description = "Embark on a journey through time with our Vintage Watches Collection, a curated selection of horological masterpieces that transcend eras and redefine elegance. Each watch in this collection is a testament to the artistry and craftsmanship that has defined the world of timekeeping. From classic pocket watches that harken back to a bygone era to iconic wristwatches that have graced the wrists of discerning collectors, our Vintage Watches Collection invites you to explore the timeless allure of these meticulously crafted timepieces. Admire the intricate details of vintage dials, the precision of mechanical movements, and the enduring design elements that have stood the test of time. Whether you are a horology enthusiast or a connoisseur of timeless style, our collection offers a glimpse into the rich history and sophistication of vintage watchmaking. Indulge in the nostalgia of yesteryear and experience the legacy of precision and elegance that defines each watch in this extraordinary collection.",
                     ImageLink = "https://collectionhubimagesdata.blob.core.windows.net/datavidsconteiner1/37329166-8abe-4d12-89ad-b068d436cc6a.jpg"
                },
                new CollectionViewModel
                {
                     Name = "Model Cars",
                     Description = "Cars that I collected all my life",
                     ImageLink = "https://collectionhubimagesdata.blob.core.windows.net/datavidsconteiner1/4b9a9a36-284e-42af-8d18-3416e184bad2.jpg"
                },                new CollectionViewModel
                {
                     Name = "Model Airplains",
                     Description = "My new collection of airplaines",
                     ImageLink = "https://collectionhubimagesdata.blob.core.windows.net/datavidsconteiner1/705245d9-bc40-4a20-8294-1fdc2d23709c.jpg"
                }
            }; // get all user collections by mail
            return View(collections);
        }

        [Authorize]
        [HttpGet]
        public IActionResult CreateCollection()
        {
            var viewModel = new CollectionViewModel();
            viewModel.Themes = GetThemes();
            if (TempData.TryGetValue("ImagePath", out var imagePath)) viewModel.ImageLink = (string)imagePath;
            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateCollection(CollectionViewModel collectionViewModel)
        {
            if (ModelState.IsValid)
            {
                //save to db
                //when create collection create empty tables and entities for default
                throw new NotImplementedException();
            }
            collectionViewModel.Themes = GetThemes(); //get themes from db 
            return View(collectionViewModel);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetCollection(int id)
        {
            var collection = new CollectionViewModel
            {
                Id = id,
                Name = "Old Books",
                Description = "The Old Books Collection takes you on a captivating journey through time, offering a glimpse into the literary treasures of bygone eras. Immerse yourself in the rich narratives, timeless wisdom, and classic tales that have withstood the test of time. This carefully curated collection features rare and antique books, each bearing the marks of history and the stories of countless readers. From dusty volumes with weathered leather bindings to faded pages filled with the ink of yesteryear, these books whisper the echoes of literary heritage. Explore the profound insights of past authors, embark on adventures of forgotten worlds, and witness the evolution of language and storytelling. The Old Books Collection invites you to embrace the enchanting allure of vintage literature and connect with the enduring magic held within each aged page.",
                ItemsDataTypes = GetItemsDataTypes(),
                AllHeaders = new List<string>
                {
                    "Name",
                    "Tags"
                }
            };
            return View(collection);
        }

        [HttpPost]
        public IActionResult AddItemField(string collectionId, string type, string name)
        {
            if (string.IsNullOrEmpty(type))
            {
                ModelState.AddModelError("type", "Please select a type.");
            }

            if (string.IsNullOrEmpty(name))
            {
                ModelState.AddModelError("name", "Please enter a type name.");
            }

            var collection = new CollectionViewModel
            {
                Id = 0,
                Name = "Old Books",
                Description = "The Old Books Collection takes you on a captivating journey through time, offering a glimpse into the literary treasures of bygone eras. Immerse yourself in the rich narratives, timeless wisdom, and classic tales that have withstood the test of time. This carefully curated collection features rare and antique books, each bearing the marks of history and the stories of countless readers. From dusty volumes with weathered leather bindings to faded pages filled with the ink of yesteryear, these books whisper the echoes of literary heritage. Explore the profound insights of past authors, embark on adventures of forgotten worlds, and witness the evolution of language and storytelling. The Old Books Collection invites you to embrace the enchanting allure of vintage literature and connect with the enduring magic held within each aged page.",
                ItemsDataTypes = GetItemsDataTypes(),
                AllHeaders = new List<string> { "Name", "Tags", name }
            };

            if (ModelState.IsValid)
            {
                //save to db
                return View("GetCollection", collection);
            }

            // If there are validation errors, redisplay the form with error messages
            return View("GetCollection", collection);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImage(CollectionViewModel collectionViewModel)
        {
            var imagePath = await _imageService.UploadImageToAzureAndGiveImageLink(collectionViewModel.File);
            TempData["ImagePath"] = imagePath;
            return RedirectToAction("CreateCollection");
        }

        private List<SelectListItem> GetItemsDataTypes()
        {
            return new List<SelectListItem>
                {
                new SelectListItem
                {
                    Text = "integer"
                },
                new SelectListItem
                {
                    Text = "string"
                },
                new SelectListItem
                {
                    Text = "text"
                },
                new SelectListItem
                {
                    Text = "bool"
                },
                new SelectListItem
                {
                    Text = "date"
                }
            };
        }

        private List<SelectListItem> GetThemes()
        {
            return new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Books"
                },
                new SelectListItem
                {
                    Text = "Movies"
                },
                new SelectListItem
                {
                    Text = "Coins"
                },
                new SelectListItem
                {
                    Text = "Watches"
                },
                new SelectListItem
                {
                    Text = "Board games"
                },
                new SelectListItem
                {
                    Text = "Model cars"
                },
                new SelectListItem
                {
                    Text = "Model airplanes"
                }
            };
        }
    }
}
