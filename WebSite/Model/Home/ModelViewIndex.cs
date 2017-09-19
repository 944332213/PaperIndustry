using System.Collections.Generic;
using Lushi.PaperProducts.Model.Recombination;
using Lushi.PaperProducts.Model.Table;

namespace Lushi.PaperProducts.Ui.WebSite.Model.Home
{
    public class ModelViewIndex: IModelView
    {
        public Dictionary<ModelType, List<ModelArticle>> Column { get; set; }
    }
}