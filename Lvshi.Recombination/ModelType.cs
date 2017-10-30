using System;
using System.Data;
using System.Runtime.Serialization;
using Lvshi.PaperProducts.Model.Table;

namespace Lvshi.PaperProducts.Model.Recombination
{
    [Serializable]
    public class ModelType : ModelColumnType, ISerializable
    {
        private ModelType _mParent;

        public ModelType() { }

        protected ModelType(SerializationInfo info, StreamingContext context)
        {
            var propertyInfos = TypeData.GetPropertys<ModelType>();
            foreach (var propertyInfo in propertyInfos)
            {
                dynamic value;
                try
                {
                    value = info.GetValue(propertyInfo.Name, propertyInfo.PropertyType);
                }
                catch (Exception)
                {
                    value = null;
                }
                if (value == null)
                {
                    continue;
                }
                propertyInfo.SetValue(this, value);
            }
        }

        public ModelType Parent
        {
            get { return _mParent ?? new ModelType(); }
            set { _mParent = value; }
        }

        public new StructMenuIcon.Value Icon => (StructMenuIcon.Value)base.Icon;

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var propertyInfos = TypeData.GetPropertys<ModelType>();
            foreach (var propertyInfo in propertyInfos)
            {
                if (propertyInfo.Name == "Parent" && _mParent == null)
                {
                    continue;
                }
                info.AddValue(propertyInfo.Name, propertyInfo.GetValue(this), propertyInfo.PropertyType);
            }
        }
    }

    public static class ModelTypeExtension
    {
        public static string Url(this ModelType model)
        {
            if (model == null)
            {
                return string.Empty;
            }
            if (!string.IsNullOrWhiteSpace(model.Url))
            {
                return model.Url;
            }
            if (model.Level == EnumColumnTypeLevel.Value.One)
            {
                return $"/{model.EnName}";
            }
            if (model.Level == EnumColumnTypeLevel.Value.Two)
            {
                return $"/{model.Parent.EnName}/{model.EnName}";
            }
            return string.Empty;
        }
    }
}
