using System.Collections.Generic;
using Lushi.PaperProducts.Model.Recombination;

namespace Lushi.PaperProducts.Ui.WebSite.Model.Master
{
    public class ModelViewHead: IModelView
    {
        public List<ModelType> Navigation { get; set; }
    }
}