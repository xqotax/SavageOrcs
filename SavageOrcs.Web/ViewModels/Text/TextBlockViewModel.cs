using SavageOrcs.Web.ViewModels.Text.Blocks;

namespace SavageOrcs.Web.ViewModels.Text
{
    public class TextBlockViewModel
    {
        public TextBlockViewModel()
        {
            Headers = new List<HeaderBlockViewModel>();
            Images = new List<ImageBlockViewModel>();
            Videos = new List<ImageBlockViewModel>();
            //CheckBoxes = new List<CheckListBlockViewModel>();
            Listes = new List<ListBlockViewModel>();
            Paragraphs = new List<ParagraphBlockViewModel>();
            Raws = new List<RawBlockViewModel>();
        }

        public List<HeaderBlockViewModel> Headers { get; set; }

        public List<ImageBlockViewModel> Images { get; set; }

        //public List<CheckListBlockViewModel> CheckBoxes { get; set; }

        public List<ListBlockViewModel> Listes { get; set; }

        public List<ParagraphBlockViewModel> Paragraphs { get; set; }

        public List<RawBlockViewModel> Raws { get; set; }

        public List<ImageBlockViewModel> Videos { get;  set; }
    }
}
