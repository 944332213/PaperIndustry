using System;
using System.Data;
using System.Runtime.Serialization;
using Lushi.PaperProducts.Model.Table;

namespace Lushi.PaperProducts.Model.Recombination
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
}
