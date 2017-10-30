using System.Collections.Generic;
using Lvshi.PaperProducts.Model.Recombination;

namespace Lvshi.PaperProducts.Ui.WebSite.Model.Master
{
    public class ModelViewHead: IModelView
    {
        public List<ModelType> Navigation { get; set; }
    }
}