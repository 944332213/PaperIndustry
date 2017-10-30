using System.Collections.Generic;
using System.Linq;
using Lvshi.PaperProducts.DataAccessLayer.DataBase;
using Lvshi.PaperProducts.Model.Recombination;
using Lvshi.PaperProducts.Model.Table;

namespace Lvshi.PaperProducts.BusinessLogicLayer.Implement
{
    /// <summary>
    /// 文章业务逻辑
    /// </summary>
    public class ArticleLogic
    {
        protected ArticleDao Dao => new ArticleDao();

        public Dictionary<ModelType, List<ModelArticle>> GetListTypeArticle(ICollection<ModelType> types)
        {
            if (types == null)
            {
                return null;
            }
            var list = Dao.SelectListByType(types.Select(item => item.Id).ToArray());
            if (list == null || list.Count <= 0)
            {
                return null;
            }
            return types.ToDictionary(item => item, item => list.Where(m => m.TypeId == item.Id).ToList());
        }

        public Dictionary<ModelType, List<ModelArticle>> GetListTypeArticle(params ModelType[] types)
        {
            return GetListTypeArticle((ICollection<ModelType>)types);
        }

        public List<ModelArticle> GetListTypeArticle(ModelType type)
        {
            return GetListTypeArticle(new[] { type })?[type];
        }

    }
}
