using System.Collections.Generic;
using Lvshi.PaperProducts.Model.Recombination;
using Lvshi.PaperProducts.Model.Table;

namespace Lvshi.PaperProducts.Ui.WebSite.Model.Home
{
    public class ModelViewIndex: IModelView
    {
        public Dictionary<ModelType, List<ModelArticle>> Column { get; set; }
    }
}