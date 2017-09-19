namespace Lushi.PaperProducts.Ui.WebSite.Model
{
    public interface IModelView
    {
    }

    public static class ModelViewExtension
    {
        public static string ErrorImage(this IModelView view)
        {
            return "onerror=this.src = '/image/common/noimage.jpg'";
        }
    }
}
