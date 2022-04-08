using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static MLProject.MLModelAnimal;

namespace MLProject.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment _webHost;
        public IndexModel(IWebHostEnvironment webHost)
        {
            _webHost = webHost;
        }

        [BindProperty]
        public IFormFile Picture { get; set; }

        public string ResultOutput { get; set; }
        public string PicturePath { get; set; }

        public void OnGet()
        {

        }

        public async Task OnPostAsync()
        {
            string file = Path.Combine(_webHost.WebRootPath, "Images", Picture.FileName);
            using (FileStream fileStream = new FileStream(file, FileMode.Create))
            {
                await Picture.CopyToAsync(fileStream);
            }

            //Load sample data
            ModelInput userInput = new()
            {
                ImageSource = file,
            };

            //Load model and predict output
            ModelOutput result = MLModelAnimal.Predict(userInput);

            PicturePath = Picture.FileName;
            //PicturePath = file;
            if (result.Prediction == "Cow")
            {
                MLModelCow.ModelInput refineResult = new()
                {
                    ImageSource = userInput.ImageSource
                };

                MLModelCow.ModelOutput cowResult = MLModelCow.Predict(refineResult);
                ResultOutput = cowResult.Prediction + ". Score: " + (cowResult.Score.OrderByDescending(x => x).First() * 100).ToString("n2") + "%";
            }
            else
            {
                ResultOutput = result.Prediction + ". Score: " + (result.Score.OrderByDescending(x => x).First() * 100).ToString("n2") + "%";
            }
        }
    }
}